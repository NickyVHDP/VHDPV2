from __future__ import annotations

import logging
import subprocess
from pathlib import Path
from typing import Optional

logger = logging.getLogger(__name__)


FFMPEG_BASE = ["ffmpeg", "-y"]


def extract_audio(video_path: Path, output_dir: Path) -> Path:
    output_dir.mkdir(parents=True, exist_ok=True)
    audio_path = output_dir / f"{video_path.stem}.mp3"
    cmd = FFMPEG_BASE + ["-i", str(video_path), "-vn", "-acodec", "mp3", str(audio_path)]
    _run(cmd, "extract audio")
    return audio_path


def render_clip(
    video_path: Path,
    start: float,
    end: float,
    output_dir: Path,
    subtitles_path: Optional[Path] = None,
) -> Path:
    output_dir.mkdir(parents=True, exist_ok=True)
    output_clip = output_dir / f"clip_{int(start)}_{int(end)}.mp4"
    duration = end - start
    filter_chain = "crop=in_h*9/16:in_h,scale=1080:1920"
    vf_args = ["-vf", filter_chain]
    if subtitles_path:
        vf_args = ["-vf", f"{filter_chain},subtitles='{subtitles_path}'"]
    cmd = FFMPEG_BASE + ["-ss", str(start), "-t", str(duration), "-i", str(video_path), *vf_args, str(output_clip)]
    _run(cmd, "render clip")
    return output_clip


def _run(cmd: list[str], description: str) -> None:
    logger.info("Running ffmpeg for %s", description)
    try:
        subprocess.run(cmd, check=True, capture_output=True)
    except subprocess.CalledProcessError as exc:
        logger.error("ffmpeg %s failed: %s", description, exc.stderr)
        raise
