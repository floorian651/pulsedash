from typing import Optional

from pydantic import BaseModel


class JobResponse(BaseModel):
    job_id: str
    state: str
    progress: int
    result_url: Optional[str] = None
