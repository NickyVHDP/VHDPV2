from __future__ import annotations

import logging
from typing import Dict

import openai

from app.services.config import settings

logger = logging.getLogger(__name__)

TITLE_PROMPT = """
You are creating metadata for a short viral gaming clip. Write a YouTube Shorts title (max 70 chars) and a description with 1-2 sentences plus up to 10 hashtags.
Return JSON with keys title and description. Context: {description}
"""


def generate_titles(description: str) -> Dict[str, str]:
    client = openai.OpenAI(api_key=settings.openai_api_key)
    prompt = TITLE_PROMPT.format(description=description)
    logger.info("Generating title/description for clip")
    completion = client.responses.create(
        model="gpt-4.1",
        input=[{"role": "user", "content": [{"type": "text", "text": prompt}]}],
        max_output_tokens=400,
        response_format={"type": "json_object"},
    )
    content = completion.output[0].content[0].text  # type: ignore[index]
    try:
        data = completion.output[0].content[0].text
    except Exception as exc:  # pragma: no cover - defensive
        logger.error("Failed to parse title response: %s", exc)
        return {"title": description[:70], "description": description}

    import json

    try:
        parsed = json.loads(data)
    except json.JSONDecodeError:
        return {"title": description[:70], "description": description}

    title = parsed.get("title") or description[:70]
    desc = parsed.get("description") or description
    return {"title": title, "description": desc}
