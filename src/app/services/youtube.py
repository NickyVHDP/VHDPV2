from __future__ import annotations

import json
import logging
from pathlib import Path
from typing import Optional

from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from googleapiclient.http import MediaFileUpload

from app.services.config import settings

logger = logging.getLogger(__name__)
SCOPES = ["https://www.googleapis.com/auth/youtube.upload"]


def get_credentials() -> Optional[Credentials]:
    token_path = settings.youtube_token_path
    creds = None
    if token_path.exists():
        creds = Credentials.from_authorized_user_file(str(token_path), SCOPES)
    if creds and creds.expired and creds.refresh_token:
        creds.refresh(Request())
        token_path.write_text(creds.to_json())
    return creds


def start_oauth_flow() -> str:
    flow = InstalledAppFlow.from_client_config(
        {
            "installed": {
                "client_id": settings.youtube_client_id,
                "client_secret": settings.youtube_client_secret,
                "redirect_uris": [settings.youtube_redirect_uri],
                "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                "token_uri": "https://oauth2.googleapis.com/token",
            }
        },
        scopes=SCOPES,
    )
    auth_url, _ = flow.authorization_url(access_type="offline", prompt="consent", include_granted_scopes="true")
    return auth_url


def exchange_code_for_tokens(code: str) -> None:
    flow = InstalledAppFlow.from_client_config(
        {
            "installed": {
                "client_id": settings.youtube_client_id,
                "client_secret": settings.youtube_client_secret,
                "redirect_uris": [settings.youtube_redirect_uri],
                "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                "token_uri": "https://oauth2.googleapis.com/token",
            }
        },
        scopes=SCOPES,
    )
    flow.fetch_token(code=code)
    settings.youtube_token_path.write_text(flow.credentials.to_json())


def upload_video(file_path: Path, title: str, description: str, tags: Optional[list[str]] = None) -> Optional[str]:
    creds = get_credentials()
    if not creds:
        logger.error("Missing YouTube credentials. Authenticate first.")
        return None

    youtube = build("youtube", "v3", credentials=creds)
    body = {
        "snippet": {
            "title": title,
            "description": description,
            "tags": tags or [],
            "categoryId": "20",
        },
        "status": {
            "privacyStatus": "public",
            "selfDeclaredMadeForKids": False,
        },
    }
    try:
        media = MediaFileUpload(str(file_path), mimetype="video/mp4")
        request = youtube.videos().insert(part="snippet,status", body=body, media_body=media)
        response = request.execute()
        video_id = response.get("id")
        logger.info("YouTube upload complete: %s", video_id)
        return video_id
    except HttpError as exc:
        logger.error("YouTube upload failed: %s", exc)
        return None
