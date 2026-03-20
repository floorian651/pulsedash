from pydantic import BaseModel


class GenerateRequest(BaseModel):
    track_id: str


class GenerateResponse(BaseModel):
    job_id: str
    state: str
