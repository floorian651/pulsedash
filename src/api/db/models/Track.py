from sqlalchemy import Column, String, Integer, ForeignKey
from sqlalchemy.orm import relationship
from .base import Base


class Track(Base):
    __tablename__ = "tracks"

    id = Column(Integer, primary_key=True, autoincrement=True)
    playlist_name = Column(String, ForeignKey("playlists.name"), nullable=False)
    music_title = Column(String, ForeignKey("music.title"), nullable=False)
    position = Column(Integer, nullable=True)  # Pour l'ordre dans le jeu

    # On lie les objets entre eux
    playlist = relationship("Playlist", back_populates="tracks")
    music = relationship("Music")

    def __repr__(self):
        return f"<Track(playlist={self.playlist_name}, music={self.music_title}, pos={self.position})>"
