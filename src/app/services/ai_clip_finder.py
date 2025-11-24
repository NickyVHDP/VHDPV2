from __future__ import annotations

import json
import logging
from typing import List

import openai

from app.services.config import settings

logger = logging.getLogger(__name__)

PROMPT_TEMPLATE = """
You are an expert gaming content strategist. Analyze the following transcript chunk from a long-form stream. 
Return 5-15 high-energy, engaging short-form clip suggestions as JSON list objects.
Each object must contain start_time_seconds, end_time_seconds, brief_description, and virality_score (1-10).
Keep clip durations between 15 and 60 seconds where possible. Avoid duplicates or overlapping segments.
Transcript chunk:
{chunk}
"""


def suggest_clips_from_chunk(chunk: str) -> List[dict]:
    client = openai.OpenAI(api_key=settings.openai_api_key)
    message = PROMPT_TEMPLATE.format(chunk=chunk)
    logger.info("Requesting clip suggestions from OpenAI")
    completion = client.responses.create(
        model="gpt-4.1",
        input=[{"role": "user", "content": [{"type": "text", "text": message}]}],
        max_output_tokens=800,
        response_format={"type": "json_object"},
    )
    content = completion.output[0].content[0].text  # type: ignore[index]
    try:
        data = json.loads(content)
    except json.JSONDecodeError:
        logger.warning("Failed to parse clip suggestions; returning empty list")
        return []

    if isinstance(data, dict):
        # Allow either list or dict with key
        for value in data.values():
            if isinstance(value, list):
                return value
        return []
    if isinstance(data, list):
        return data
    return []
