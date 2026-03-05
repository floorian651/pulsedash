# jobs.py

#     endpoint GET /jobs/{job_id}

#     endpoint GET /jobs/{job_id}/download

from fastapi import APIRouter

router = APIRouter()


@router.get("/jobs/{job_id}")
async def get_job(job_id: str):
    return {"job_id": job_id, "state": "pending"}
