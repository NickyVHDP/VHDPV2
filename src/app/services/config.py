from __future__ import annotations

import os
from dataclasses import dataclass
from pathlib import Path


@dataclass
class Settings:
    openai_api_key: str
    youtube_client_id: str
    youtube_client_secret: str
    youtube_redirect_uri: str
    youtube_token_path: Path
    data_dir: Path

    @classmethod
    def load(cls) -> "Settings":
        data_dir = Path(os.environ.get("DATA_DIR", "./data"))
        data_dir.mkdir(parents=True, exist_ok=True)
        token_path = Path(os.environ.get("YOUTUBE_TOKEN_STORAGE_PATH", data_dir / "youtube_tokens.json"))
        return cls(
            openai_api_key=os.environ.get("OPENAI_API_KEY", ""),
            youtube_client_id=os.environ.get("YOUTUBE_CLIENT_ID", ""),
            youtube_client_secret=os.environ.get("YOUTUBE_CLIENT_SECRET", ""),
            youtube_redirect_uri=os.environ.get("YOUTUBE_REDIRECT_URI", "http://localhost:8000/auth/youtube/callback"),
            youtube_token_path=token_path,
            data_dir=data_dir,
        )


settings = Settings.load()
