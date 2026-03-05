from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.models import Job
from src.api.db.session import get_session

router = APIRouter()


@router.get("/jobs/{job_id}")
async def get_job(job_id: str, db: Session = Depends(get_session)):
    # Chercher le job dans la BDD par son ID
    job = db.query(Job).filter(Job.id == job_id).first()

    # Si le job n'existe pas, on renvoie une erreur 404 à Unity
    if not job:
        raise HTTPException(status_code=404, detail="Job non trouvé")

    # On renvoie les vraies infos de la BDD
    return {
        "job_id": job.id,
        "state": job.state,
        "progress": job.progress,
        "created_at": job.created_at,
    }
