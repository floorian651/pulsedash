# generate.py

#     endpoint POST /generate

#     crée un job

#     envoie une tâche Celery

from fastapi import APIRouter

router = APIRouter()


@router.post("/generate")
async def generate_level():
    return {"message": "generation started"}
