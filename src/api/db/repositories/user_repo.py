from sqlalchemy.orm import Session

from src.api.db.models import User


def create_user(db: Session, email: str, password: str) -> User:
    user = User(email=email, password=password)
    db.add(user)
    db.commit()
    return user


def get_user_by_email(db: Session, email: str) -> User:
    return db.query(User).filter(User.email == email).first()
