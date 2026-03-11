import uuid

from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session

from src.api.db.session import get_session
from src.api.db.repositories import job_repo
from src.api.schemas.generate import GenerateRequest, GenerateResponse
from src.api.services.tasks import generate_level_task

router = APIRouter()


@router.post("/generate", response_model=GenerateResponse)
async def generate_level(body: GenerateRequest, db: Session = Depends(get_session)):
    job_id = str(uuid.uuid4())
    job_repo.create_job(db, job_id=job_id)
    generate_level_task.delay(job_id, body.track_id)
    return GenerateResponse(job_id=job_id, state="pending")
