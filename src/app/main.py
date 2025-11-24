from __future__ import annotations

import logging
from pathlib import Path
from typing import Optional

from fastapi import BackgroundTasks, Depends, FastAPI, File, Form, HTTPException, Request, UploadFile
from fastapi.responses import HTMLResponse, RedirectResponse
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from sqlalchemy.orm import Session

from app import models
from app.database import DATA_DIR, engine, get_db
from app.models import ClipCandidate, ClipStatus, SourceStatus, SourceType, SourceVideo
from app.services import pipeline, youtube
from app.services.config import settings

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

models.Base.metadata.create_all(bind=engine)

app = FastAPI(title="Stream Clip Automator")

static_path = Path(__file__).parent / "static"
templates = Jinja2Templates(directory=str(Path(__file__).parent / "templates"))
app.mount("/static", StaticFiles(directory=static_path), name="static")
app.mount("/data", StaticFiles(directory=settings.data_dir), name="data")


@app.get("/", response_class=HTMLResponse)
def home(request: Request, db: Session = Depends(get_db)):
    videos = db.query(SourceVideo).order_by(SourceVideo.created_at.desc()).all()
    return templates.TemplateResponse("videos.html", {"request": request, "videos": videos})


@app.post("/videos/upload")
async def upload_video(
    request: Request,
    background_tasks: BackgroundTasks,
    file: UploadFile = File(...),
    title: str = Form(...),
    db: Session = Depends(get_db),
):
    storage_dir = DATA_DIR / "source_videos"
    storage_dir.mkdir(parents=True, exist_ok=True)
    saved_path = storage_dir / file.filename
    with saved_path.open("wb") as buffer:
        buffer.write(await file.read())

    video = SourceVideo(title=title, source_type=SourceType.local, local_path=str(saved_path))
    db.add(video)
    db.commit()
    db.refresh(video)

    background_tasks.add_task(pipeline.process_source_video, video.id)

    return RedirectResponse(url="/", status_code=303)


@app.post("/videos/from-youtube")
def register_youtube(
    request: Request,
    background_tasks: BackgroundTasks,
    url: str = Form(...),
    title: str = Form(""),
    db: Session = Depends(get_db),
):
    # For v1 assume manual download outside; store URL
    video = SourceVideo(title=title or url, source_type=SourceType.youtube_url, original_url=url, local_path=url)
    db.add(video)
    db.commit()
    db.refresh(video)
    background_tasks.add_task(pipeline.process_source_video, video.id)
    return RedirectResponse(url="/", status_code=303)


@app.get("/videos/{video_id}", response_class=HTMLResponse)
def view_video(video_id: int, request: Request, db: Session = Depends(get_db)):
    video = db.query(SourceVideo).filter(SourceVideo.id == video_id).first()
    if not video:
        raise HTTPException(status_code=404, detail="Video not found")
    return templates.TemplateResponse("video_detail.html", {"request": request, "video": video})


@app.post("/clips/{clip_id}/approve")
def approve_clip(
    clip_id: int,
    request: Request,
    title: str = Form(""),
    description: str = Form(""),
    db: Session = Depends(get_db),
):
    clip = db.query(ClipCandidate).filter(ClipCandidate.id == clip_id).first()
    if not clip:
        raise HTTPException(status_code=404, detail="Clip not found")
    clip.platform_title = title or clip.platform_title
    clip.platform_description = description or clip.platform_description
    clip.status = ClipStatus.approved
    db.commit()
    return RedirectResponse(url=f"/videos/{clip.source_video_id}", status_code=303)


@app.post("/clips/{clip_id}/reject")
def reject_clip(clip_id: int, request: Request, db: Session = Depends(get_db)):
    clip = db.query(ClipCandidate).filter(ClipCandidate.id == clip_id).first()
    if not clip:
        raise HTTPException(status_code=404, detail="Clip not found")
    clip.status = ClipStatus.rejected
    db.commit()
    return RedirectResponse(url=f"/videos/{clip.source_video_id}", status_code=303)


@app.post("/clips/upload")
def upload_clips(request: Request, db: Session = Depends(get_db)):
    pipeline.upload_pending_clips(db)
    return RedirectResponse(url="/", status_code=303)


@app.get("/auth/youtube")
def start_youtube_auth():
    url = youtube.start_oauth_flow()
    return RedirectResponse(url=url)


@app.get("/auth/youtube/callback")
def youtube_callback(code: str):
    youtube.exchange_code_for_tokens(code)
    return RedirectResponse(url="/", status_code=303)
