from __future__ import annotations

import json
import logging
from pathlib import Path
from typing import Dict, List, Tuple

import openai

from app.services.config import settings

logger = logging.getLogger(__name__)


def transcribe_audio(audio_path: Path) -> Tuple[str, Path]:
    """Transcribe audio using OpenAI Whisper API.

    Returns the transcript text and path to JSON transcript file.
    """
    client = openai.OpenAI(api_key=settings.openai_api_key)
    logger.info("Starting transcription for %s", audio_path)
    with audio_path.open("rb") as audio_file:
        transcription = client.audio.transcriptions.create(model="whisper-1", file=audio_file)

    transcript_text = transcription.text
    transcript_json_path = audio_path.with_suffix(".json")
    transcript_json_path.write_text(json.dumps(transcription, default=str, indent=2), encoding="utf-8")
    logger.info("Transcription finished for %s", audio_path)
    return transcript_text, transcript_json_path


def chunk_transcript(transcript: str, chunk_size: int = 1200) -> List[str]:
    words = transcript.split()
    chunks: List[str] = []
    for i in range(0, len(words), chunk_size):
        chunk = " ".join(words[i : i + chunk_size])
        chunks.append(chunk)
    return chunks
