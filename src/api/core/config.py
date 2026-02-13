from pydantic import Field
from pydantic_settings import BaseSettings
from functools import lru_cache


class Settings(BaseSettings):
    # -----------------------------
    # Application
    # -----------------------------
    APP_NAME: str = "Wavr API"
    API_V1_PREFIX: str = "/api/v1"
    DEBUG: bool = True

    # -----------------------------
    # Database (PostgreSQL)
    # -----------------------------
    POSTGRES_HOST: str = "postgres"
    POSTGRES_PORT: int = 5432
    POSTGRES_USER: str = "wavr"
    POSTGRES_PASSWORD: str = "wavr"
    POSTGRES_DB: str = "wavr"

    @property
    def DATABASE_URL(self) -> str:
        return (
            f"postgresql+psycopg2://{self.POSTGRES_USER}:"
            f"{self.POSTGRES_PASSWORD}@{self.POSTGRES_HOST}:"
            f"{self.POSTGRES_PORT}/{self.POSTGRES_DB}"
        )

    # -----------------------------
    # Minio (S3 compatible)
    # -----------------------------
    MINIO_ENDPOINT: str = "minio:9000"
    MINIO_ACCESS_KEY: str = "minio"
    MINIO_SECRET_KEY: str = "minio123"
    MINIO_SECURE: bool = False

    MINIO_AUDIO_BUCKET: str = "audio"
    MINIO_LEVEL_BUCKET: str = "levels"

    # -----------------------------
    # Celery / Redis
    # -----------------------------
    REDIS_HOST: str = "redis"
    REDIS_PORT: int = 6379
    REDIS_DB: int = 0

    @property
    def CELERY_BROKER_URL(self) -> str:
        return f"redis://{self.REDIS_HOST}:{self.REDIS_PORT}/{self.REDIS_DB}"

    @property
    def CELERY_RESULT_BACKEND(self) -> str:
        return f"redis://{self.REDIS_HOST}:{self.REDIS_PORT}/{self.REDIS_DB}"

    # -----------------------------
    # Jamendo API
    # -----------------------------
    JAMENDO_CLIENT_ID: str = Field(..., description="Jamendo API client ID")

    # -----------------------------
    # Security
    # -----------------------------
    JWT_SECRET_KEY: str = "super-secret-key"
    JWT_ALGORITHM: str = "HS256"
    JWT_EXPIRE_MINUTES: int = 60

    # -----------------------------
    # CORS
    # -----------------------------
    CORS_ORIGINS: list[str] = ["*"]

    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"


@lru_cache
def get_settings() -> Settings:
    return Settings()
