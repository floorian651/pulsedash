from functools import lru_cache

from pydantic import Field
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    # ---------------------------------------------------------
    # Application
    # ---------------------------------------------------------
    APP_NAME: str = Field(default="PulseDash API")
    API_V1_PREFIX: str = "/api/v1"
    DEBUG: bool = Field(default=False)

    # ---------------------------------------------------------
    # Database (PostgreSQL)
    # ---------------------------------------------------------
    POSTGRES_HOST: str
    POSTGRES_PORT: int = 5432
    POSTGRES_USER: str
    POSTGRES_PASSWORD: str
    POSTGRES_DB: str

    @property
    def DATABASE_URL(self) -> str:
        return (
            f"postgresql+psycopg2://{self.POSTGRES_USER}:"
            f"{self.POSTGRES_PASSWORD}@{self.POSTGRES_HOST}:"
            f"{self.POSTGRES_PORT}/{self.POSTGRES_DB}"
        )

    # ---------------------------------------------------------
    # Minio
    # ---------------------------------------------------------
    MINIO_ENDPOINT: str
    MINIO_ACCESS_KEY: str
    MINIO_SECRET_KEY: str
    MINIO_SECURE: bool = False

    MINIO_BUCKET_MUSIC: str = "music"
    MINIO_BUCKET_LEVELS: str = "levels"

    # ---------------------------------------------------------
    # Redis / Celery
    # ---------------------------------------------------------
    REDIS_HOST: str
    REDIS_PORT: int = 6379
    REDIS_DB: int = 0

    @property
    def CELERY_BROKER_URL(self) -> str:
        return f"redis://{self.REDIS_HOST}:{self.REDIS_PORT}/{self.REDIS_DB}"

    @property
    def CELERY_RESULT_BACKEND(self) -> str:
        return f"redis://{self.REDIS_HOST}:{self.REDIS_PORT}/{self.REDIS_DB}"

    # ---------------------------------------------------------
    # Jamendo API
    # ---------------------------------------------------------
    JAMENDO_CLIENT_ID: str

    # ---------------------------------------------------------
    # Security
    # ---------------------------------------------------------
    JWT_SECRET_KEY: str
    JWT_ALGORITHM: str = "HS256"
    JWT_EXPIRE_MINUTES: int = 60

    # ---------------------------------------------------------
    # CORS
    # ---------------------------------------------------------
    CORS_ORIGINS: str = "*"

    # ---------------------------------------------------------
    # Pydantic Settings
    # ---------------------------------------------------------
    model_config = SettingsConfigDict(
        env_file=".env", env_file_encoding="utf-8", extra="ignore"
    )


@lru_cache
def get_settings() -> Settings:
    return Settings()
