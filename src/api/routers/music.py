from fastapi import APIRouter, HTTPException
from ..services.jamendo import download_track
from ..services.storage import StorageService
import tempfile
import os

router = APIRouter(prefix="/music", tags=["music"])


@router.post("/import-jamendo/{track_id}")
async def import_jamendo_track(track_id: str):
    """
    Télécharge une musique depuis Jamendo et l'enregistre dans MinIO.

    Retourne l'URL présignée pour télécharger le fichier depuis MinIO.
    """
    try:
        # 1. Télécharger depuis Jamendo vers un fichier temporaire
        with tempfile.NamedTemporaryFile(suffix=".mp3", delete=False) as tmp:
            temp_path = tmp.name

        download_track(track_id, temp_path)
        file_size = os.path.getsize(temp_path)

        # 2. Upload vers MinIO
        storage = StorageService(bucket_type="music")
        object_name = f"jamendo_{track_id}.mp3"
        storage.upload_file(object_name, temp_path)

        # 3. Générer URL de téléchargement pour 24h
        download_url = storage.get_download_url(object_name, expires_minutes=1440)

        # 4. Nettoyer le fichier temporaire
        os.unlink(temp_path)

        return {
            "status": "success",
            "track_id": track_id,
            "object_name": object_name,
            "file_size": file_size,
            "download_url": download_url,
        }

    except ValueError as e:
        # Track non trouvé sur Jamendo
        raise HTTPException(status_code=404, detail=str(e))
    except FileNotFoundError as e:
        raise HTTPException(status_code=404, detail=f"Fichier non trouvé: {str(e)}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
