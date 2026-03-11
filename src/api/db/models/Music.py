from sqlalchemy import Column, String, Float
from .base import Base


class Music(Base):
    __tablename__ = "music"

    title = Column(String, primary_key=True)
    artist = Column(String, nullable=True)
    bpm = Column(Float, nullable=True)
    duration = Column(Float, nullable=True)

    # Stockage MinIO
    bucket_name = Column(String, default="musics")  # Nom du bucket MinIO
    file_path = Column(String, nullable=False)  # ex: "user_1/analysis_result.mp3"

    def __repr__(self):
        return f"<Music(title={self.title}, bucket={self.bucket_name})>"
