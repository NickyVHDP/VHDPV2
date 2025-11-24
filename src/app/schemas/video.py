from __future__ import annotations

from typing import Optional

from pydantic import BaseModel, HttpUrl

from app.models import ClipStatus, SourceStatus, SourceType


class SourceVideoCreate(BaseModel):
    title: str
    source_type: SourceType
    original_url: Optional[HttpUrl] = None


class SourceVideoRead(BaseModel):
    id: int
    title: Optional[str]
    status: SourceStatus
    source_type: SourceType

    class Config:
        from_attributes = True


class ClipCandidateRead(BaseModel):
    id: int
    ai_description: Optional[str]
    virality_score: Optional[int]
    status: ClipStatus
    preview_video_path: Optional[str]
    platform_title: Optional[str]
    platform_description: Optional[str]

    class Config:
        from_attributes = True
