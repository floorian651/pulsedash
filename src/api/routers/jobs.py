from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from ..db.models import Job
from ..db.session import get_session
from ..services.storage import StorageService

router = APIRouter(prefix="/jobs", tags=["jobs"])


@router.get("/{job_id}")
async def get_job_status(job_id: str, db: Session = Depends(get_session)):
    # 1. Chercher le job
    job = db.query(Job).filter(Job.id == job_id).first()

    if not job:
        raise HTTPException(status_code=404, detail="Job non trouvé")

    # 2. Préparation de la réponse de base
    response = {
        "job_id": job.id,
        "state": job.state,
        "progress": job.progress,
        "result_url": None,
    }

    # 3. Si le job est terminé, on génère une URL de téléchargement MinIO
    if job.state == "completed" and job.result_path:
        storage = StorageService(bucket_type="levels")
        response["result_url"] = storage.get_download_url(job.result_path)

    return response
