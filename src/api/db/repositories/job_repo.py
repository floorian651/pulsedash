from sqlalchemy.orm import Session
from src.api.db.models import Job
from datetime import datetime
from enum import Enum


class JobState(str, Enum):
    PENDING = "pending"
    RUNNING = "running"
    COMPLETED = "completed"
    FAILED = "failed"


def create_job(db: Session, job_id: str, user_id: str = None) -> Job:
    """Crée un nouveau job"""
    job = Job(
        id=job_id,
        user_id=user_id,
        state=JobState.PENDING,
        progress=0,
        created_at=datetime.utcnow(),
    )
    db.add(job)
    db.commit()
    db.refresh(job)
    return job


def get_job(db: Session, job_id: str) -> Job:
    """Récupère un job par ID"""
    return db.query(Job).filter(Job.id == job_id).first()


def update_job_state(db: Session, job_id: str, state: JobState) -> Job:
    """Met à jour l'état d'un job"""
    job = get_job(db, job_id)
    if job:
        job.state = state
        job.updated_at = datetime.utcnow()
        db.commit()
        db.refresh(job)
    return job


def update_job_progress(db: Session, job_id: str, progress: int) -> Job:
    """Met à jour la progression d'un job (0-100)"""
    job = get_job(db, job_id)
    if job:
        job.progress = min(progress, 100)
        job.updated_at = datetime.utcnow()
        db.commit()
        db.refresh(job)
    return job


def set_result_path(db: Session, job_id: str, result_path: str) -> Job:
    """Définit le chemin du résultat du job"""
    job = get_job(db, job_id)
    if job:
        job.result_path = result_path
        job.updated_at = datetime.utcnow()
        db.commit()
        db.refresh(job)
    return job
