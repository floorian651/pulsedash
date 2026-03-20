import tempfile
from datetime import datetime

from loguru import logger

from src.api.core.celery_app import app
from src.api.db.session import get_session
from src.api.db.repositories import job_repo
from src.api.db.repositories.job_repo import JobState


@app.task(name="generate_level")
def generate_level_task(job_id: str, track_id: str):
    """Tâche Celery : télécharge un MP3, lance le pipeline, stocke le résultat."""
    db = next(get_session())
    try:
        # 1. Marquer le job comme en cours
        job_repo.update_job_state(db, job_id, JobState.RUNNING)
        logger.info(f"Job {job_id}: processing track {track_id}")

        # 2. Télécharger le MP3 via Jamendo
        from src.api.services.jamendo import download_track

        with tempfile.NamedTemporaryFile(suffix=".mp3", delete=False) as tmp:
            audio_path = tmp.name
        download_track(track_id, audio_path)

        # 3. Stocker l'audio dans MinIO
        from src.api.services.storage import StorageService

        storage_audio = StorageService(bucket_type="audio")
        audio_object = f"{job_id}/{track_id}.mp3"
        storage_audio.upload_file(audio_object, audio_path)
        logger.info(f"Job {job_id}: audio uploaded to MinIO")

        # 4. Lancer le pipeline d'analyse
        from src.pipeline.main import main as run_pipeline

        level_data = run_pipeline(audio_path)

        # 5. Écrire le level.json et l'uploader dans MinIO
        import json

        with tempfile.NamedTemporaryFile(suffix=".json", mode="w", delete=False) as tmp:
            json.dump(level_data, tmp)
            level_path = tmp.name

        storage_levels = StorageService(bucket_type="levels")
        level_object = f"{job_id}/level.json"
        storage_levels.upload_file(level_object, level_path)

        # 6. Marquer le job comme terminé
        job_repo.set_result_path(db, job_id, level_object)
        job_repo.update_job_state(db, job_id, JobState.COMPLETED)
        logger.info(f"Job {job_id}: completed")

    except Exception as exc:
        logger.error(f"Job {job_id} failed: {exc}")
        job_repo.update_job_state(db, job_id, JobState.FAILED)
        raise
    finally:
        db.close()
