from sqlalchemy import Column, String, DateTime
from sqlalchemy.orm import relationship
from datetime import datetime
from .base import Base


class Playlist(Base):
    __tablename__ = "playlists"

    name = Column(String, primary_key=True)
    description = Column(String, nullable=True)
    created_at = Column(DateTime, default=datetime.utcnow)

    # On utilise la chaîne "Track" pour éviter l'import circulaire
    tracks = relationship(
        "Track", back_populates="playlist", cascade="all, delete-orphan"
    )

    def __repr__(self):
        return f"<Playlist(name={self.name})>"
