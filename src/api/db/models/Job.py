from sqlalchemy import Column, String, Integer, DateTime, Enum
from sqlalchemy.ext.declarative import declarative_base
from datetime import datetime
from enum import Enum as PyEnum

Base = declarative_base()


class JobState(str, PyEnum):
    PENDING = "pending"
    RUNNING = "running"
    COMPLETED = "completed"
    FAILED = "failed"


class Job(Base):
    __tablename__ = "jobs"

    id = Column(String, primary_key=True)
    user_id = Column(String, nullable=True)
    state = Column(Enum(JobState), default=JobState.PENDING)
    progress = Column(Integer, default=0)
    result_path = Column(String, nullable=True)
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    def __repr__(self):
        return f"<Job(id={self.id}, state={self.state}, progress={self.progress})>"
