import requests
from loguru import logger

from src.api.core.config import get_settings

JAMENDO_TRACKS_URL = "https://api.jamendo.com/v3.0/tracks"


def download_track(track_id: str, dest_path: str) -> str:
    """Télécharge un MP3 depuis Jamendo et l'écrit dans dest_path."""
    settings = get_settings()

    resp = requests.get(
        JAMENDO_TRACKS_URL,
        params={
            "client_id": settings.JAMENDO_CLIENT_ID,
            "id": track_id,
            "format": "json",
            "audioformat": "mp32",
        },
        timeout=30,
    )
    resp.raise_for_status()
    data = resp.json()

    results = data.get("results", [])
    if not results:
        raise ValueError(f"Track {track_id} not found on Jamendo")

    audio_url = results[0].get("audiodownload") or results[0].get("audio")
    if not audio_url:
        raise ValueError(f"No audio URL for track {track_id}")

    logger.info(f"Downloading track {track_id} from {audio_url}")
    audio_resp = requests.get(audio_url, stream=True, timeout=120)
    audio_resp.raise_for_status()

    with open(dest_path, "wb") as f:
        for chunk in audio_resp.iter_content(chunk_size=8192):
            f.write(chunk)

    logger.info(f"Track {track_id} saved to {dest_path}")
    return dest_path
