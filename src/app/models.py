from __future__ import annotations

import enum
from datetime import datetime
from typing import Optional

from sqlalchemy import Column, DateTime, Enum, Float, ForeignKey, Integer, String, Text
from sqlalchemy.orm import declarative_base, relationship

Base = declarative_base()


class SourceType(str, enum.Enum):
    local = "local"
    youtube_url = "youtube_url"


class SourceStatus(str, enum.Enum):
    uploaded = "uploaded"
    processing = "processing"
    processed = "processed"
    error = "error"


class ClipStatus(str, enum.Enum):
    pending_review = "pending_review"
    approved = "approved"
    rejected = "rejected"
    uploaded = "uploaded"


class Creator(Base):
    __tablename__ = "creators"

    id: int = Column(Integer, primary_key=True, index=True)
    name: str = Column(String, nullable=False)
    notes: Optional[str] = Column(Text)
    default_handle: Optional[str] = Column(String)

    source_videos = relationship("SourceVideo", back_populates="creator")


class SourceVideo(Base):
    __tablename__ = "source_videos"

    id: int = Column(Integer, primary_key=True, index=True)
    creator_id: Optional[int] = Column(Integer, ForeignKey("creators.id"), nullable=True)
    source_type: SourceType = Column(Enum(SourceType), nullable=False)
    original_url: Optional[str] = Column(String)
    local_path: Optional[str] = Column(String)
    title: Optional[str] = Column(String)
    status: SourceStatus = Column(Enum(SourceStatus), default=SourceStatus.uploaded, nullable=False)
    duration_seconds: Optional[int] = Column(Integer)
    transcript_path: Optional[str] = Column(String)
    transcript_language: Optional[str] = Column(String)
    created_at: datetime = Column(DateTime, default=datetime.utcnow)
    updated_at: datetime = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    creator = relationship("Creator", back_populates="source_videos")
    clips = relationship("ClipCandidate", back_populates="source_video", cascade="all, delete-orphan")


class ClipCandidate(Base):
    __tablename__ = "clip_candidates"

    id: int = Column(Integer, primary_key=True, index=True)
    source_video_id: int = Column(Integer, ForeignKey("source_videos.id"), nullable=False)
    start_time_seconds: float = Column(Float, nullable=False)
    end_time_seconds: float = Column(Float, nullable=False)
    duration_seconds: float = Column(Float, nullable=False)
    ai_description: Optional[str] = Column(Text)
    virality_score: Optional[int] = Column(Integer)
    status: ClipStatus = Column(Enum(ClipStatus), default=ClipStatus.pending_review, nullable=False)
    preview_video_path: Optional[str] = Column(String)
    subtitles_srt_path: Optional[str] = Column(String)
    platform_title: Optional[str] = Column(String)
    platform_description: Optional[str] = Column(Text)
    youtube_video_id: Optional[str] = Column(String)
    error_message: Optional[str] = Column(Text)

    source_video = relationship("SourceVideo", back_populates="clips")

    @property
    def duration(self) -> float:
        return self.end_time_seconds - self.start_time_seconds
