from sqlalchemy.ext.declarative import declarative_base

# Crée la base déclarative SQLAlchemy
Base = declarative_base()

# Importe tous les modèles pour que SQLAlchemy les connaisse
from src.api.db.models.Job import Job

__all__ = ["Base", "Job"]
