# Stream Clip Automator

A minimal FastAPI application for turning authorized long-form gaming streams into short-form clips ready for YouTube Shorts (and TikTok export prep).

## Features
- Upload or register source videos you own or have explicit permission to use.
- Transcribe audio via OpenAI Whisper.
- Use GPT to propose viral clip candidates.
- Render 9:16 previews with ffmpeg and generate subtitles/title/description suggestions.
- Review, approve, reject clips in a simple dashboard.
- Upload approved clips to YouTube Shorts using the YouTube Data API v3.
- Export MP4 + metadata for manual TikTok upload.

## Requirements
- Python 3.11+
- ffmpeg installed and available on PATH
- SQLite (bundled) or alternative DB configured via `DATABASE_URL`
- OpenAI API key
- YouTube API credentials (client id/secret) and OAuth redirect URI

## Quickstart
1. Create and activate a virtual environment.
2. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```
3. Copy `.env.example` to `.env` and fill in your keys. Export the values or use a tool like `direnv`.
4. Start the server:
   ```bash
   uvicorn app.main:app --reload
   ```
   The dashboard will be available at `http://localhost:8000`.
5. Upload a local VOD or register a YouTube VOD URL (only if you own/have permission). Processing runs in background tasks.
6. Connect your YouTube account via the "Connect YouTube Account" button to enable uploads.
7. Approve clips and trigger uploads. Download MP4 + captions for TikTok as needed.

## Environment Variables
- `OPENAI_API_KEY`
- `YOUTUBE_CLIENT_ID`
- `YOUTUBE_CLIENT_SECRET`
- `YOUTUBE_REDIRECT_URI` (defaults to `http://localhost:8000/auth/youtube/callback`)
- `YOUTUBE_TOKEN_STORAGE_PATH` (defaults to `./data/youtube_tokens.json`)
- `DATA_DIR` (base directory for data storage; defaults to `./data`)
- `DATABASE_URL` (defaults to SQLite in `DATA_DIR`)

See `.env.example` for placeholders.

## Data Layout
```
data/
  source_videos/
  transcripts/
  clips/
  youtube_tokens.json
```

## Notes
- The pipeline assumes you have rights to any media processed. Do not ingest content without permissions.
- YouTube uploads require a configured OAuth2 client. The stored refresh token is saved in `YOUTUBE_TOKEN_STORAGE_PATH`.
- ffmpeg must be installed; subtitle burning uses its `subtitles` filter.
- For YouTube VODs, this v1 assumes you download the file separately and point `local_path` to it after registration if automation is not configured.

## Development
- Database models live in `app/models.py`.
- Pipeline orchestration lives in `app/services/pipeline.py`.
- Templates are in `app/templates/` and static assets in `app/static/`.

## Running background uploads
Use the "Upload Approved Clips" button on the dashboard to attempt uploads for all approved clips without a YouTube video ID.

## Security
- Secrets are loaded from environment variables; keep `.env` files out of source control.
- OAuth tokens are stored locally; secure the data directory when deploying.
