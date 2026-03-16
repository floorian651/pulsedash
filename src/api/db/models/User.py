from sqlalchemy import Column, String, Boolean

from .base import Base


class User(Base):
    __tablename__ = "users"  # Assure-toi que c'est bien "users" ici aussi

    id = Column(String, primary_key=True)
    username = Column(String, unique=True, nullable=True)
    email = Column(String, unique=True, nullable=True)
    password = Column(String, nullable=True)
    is_active = Column(Boolean, default=True)
