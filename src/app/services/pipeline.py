from __future__ import annotations

import json
import logging
from pathlib import Path
from typing import Iterable

from sqlalchemy.orm import Session

from app.database import SessionLocal
from app.models import ClipCandidate, ClipStatus, SourceStatus, SourceType, SourceVideo
from app.services import ai_clip_finder, ai_text_generation, ai_transcription, video_processing, youtube
from app.services.config import settings

logger = logging.getLogger(__name__)


DATA_DIR = settings.data_dir
SOURCE_VIDEO_DIR = DATA_DIR / "source_videos"
CLIPS_DIR = DATA_DIR / "clips"
TRANSCRIPTS_DIR = DATA_DIR / "transcripts"

for path in [SOURCE_VIDEO_DIR, CLIPS_DIR, TRANSCRIPTS_DIR]:
    path.mkdir(parents=True, exist_ok=True)


def process_source_video(video_id: int) -> None:
    db = SessionLocal()
    source_video = db.query(SourceVideo).filter(SourceVideo.id == video_id).first()
    if not source_video:
        logger.error("Source video %s not found", video_id)
        db.close()
        return
    try:
        source_video.status = SourceStatus.processing
        db.commit()
        db.refresh(source_video)

        video_path = _get_video_path(source_video)
        audio_path = video_processing.extract_audio(video_path, TRANSCRIPTS_DIR)
        transcript_text, transcript_path = ai_transcription.transcribe_audio(audio_path)
        source_video.transcript_path = str(transcript_path)
        source_video.status = SourceStatus.processed
        db.commit()

        chunks = ai_transcription.chunk_transcript(transcript_text)
        suggestions = []
        for chunk in chunks:
            suggestions.extend(ai_clip_finder.suggest_clips_from_chunk(chunk))

        unique_clips = _filter_and_merge_clips(suggestions)
        for clip in unique_clips:
            start = float(clip.get("start_time_seconds"))
            end = float(clip.get("end_time_seconds"))
            duration = max(0, end - start)
            if duration < 15 or duration > 70:
                continue
            candidate = ClipCandidate(
                source_video_id=source_video.id,
                start_time_seconds=start,
                end_time_seconds=end,
                duration_seconds=duration,
                ai_description=clip.get("brief_description"),
                virality_score=int(clip.get("virality_score", 0)),
            )
            db.add(candidate)
        db.commit()

        _render_previews_and_metadata(db, source_video)
    except Exception as exc:  # pragma: no cover - runtime protection
        logger.exception("Pipeline failed: %s", exc)
        source_video.status = SourceStatus.error
        db.commit()
    finally:
        db.close()


def _render_previews_and_metadata(db: Session, source_video: SourceVideo) -> None:
    transcript_data = None
    if source_video.transcript_path:
        try:
            transcript_data = json.loads(Path(source_video.transcript_path).read_text())
        except Exception:
            transcript_data = None

    for clip in source_video.clips:
        try:
            video_path = _get_video_path(source_video)
            subtitles_path = None
            clip_output_dir = CLIPS_DIR / str(source_video.id)
            preview_path = video_processing.render_clip(video_path, clip.start_time_seconds, clip.end_time_seconds, clip_output_dir, subtitles_path)
            web_path = f"/data/{preview_path.relative_to(settings.data_dir)}"
            clip.preview_video_path = web_path

            metadata = ai_text_generation.generate_titles(clip.ai_description or "Gaming highlight")
            clip.platform_title = metadata["title"]
            clip.platform_description = metadata["description"]
            clip.status = ClipStatus.pending_review
            db.add(clip)
            db.commit()
        except Exception as exc:  # pragma: no cover - runtime protection
            logger.exception("Failed to render clip %s: %s", clip.id, exc)
            clip.error_message = str(exc)
            db.commit()


def _get_video_path(source_video: SourceVideo) -> Path:
    if source_video.source_type == SourceType.local and source_video.local_path:
        return Path(source_video.local_path)
    if source_video.source_type == SourceType.youtube_url and source_video.local_path:
        return Path(source_video.local_path)
    raise FileNotFoundError("Video path missing")


def _filter_and_merge_clips(raw: Iterable[dict]) -> list[dict]:
    seen: list[dict] = []
    for clip in raw:
        try:
            start = float(clip.get("start_time_seconds"))
            end = float(clip.get("end_time_seconds"))
        except (TypeError, ValueError):
            continue
        if any(abs(start - existing["start_time_seconds"]) < 1 and abs(end - existing["end_time_seconds"]) < 1 for existing in seen):
            continue
        seen.append(
            {
                "start_time_seconds": start,
                "end_time_seconds": end,
                "brief_description": clip.get("brief_description", ""),
                "virality_score": clip.get("virality_score", 0),
            }
        )
    return seen


def upload_pending_clips(db: Session) -> None:
    pending = db.query(ClipCandidate).filter(
        ClipCandidate.status == ClipStatus.approved, ClipCandidate.youtube_video_id.is_(None)
    )
    for clip in pending:
        tags = [tag.strip("#") for tag in (clip.platform_description or "").split() if tag.startswith("#")]
        video_id = youtube.upload_video(Path(clip.preview_video_path), clip.platform_title or "", clip.platform_description or "", tags)
        if video_id:
            clip.youtube_video_id = video_id
            clip.status = ClipStatus.uploaded
            db.add(clip)
            db.commit()
        else:
            clip.error_message = "Upload failed - check logs"
            db.commit()
