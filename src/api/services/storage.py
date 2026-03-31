from datetime import timedelta
from minio import Minio
from ..core.config import get_settings


def ensure_buckets_exist():
    """Crée les buckets MinIO s'ils n'existent pas"""
    settings = get_settings()

    endpoint = settings.MINIO_ENDPOINT
    if endpoint.startswith("http://"):
        endpoint = endpoint[len("http://") :]
    elif endpoint.startswith("https://"):
        endpoint = endpoint[len("https://") :]

    client = Minio(
        endpoint,
        access_key=settings.MINIO_ACCESS_KEY,
        secret_key=settings.MINIO_SECRET_KEY,
        secure=settings.MINIO_SECURE,
    )

    buckets = [
        settings.MINIO_BUCKET_MUSIC,
        settings.MINIO_BUCKET_LEVELS,
    ]

    for bucket_name in buckets:
        try:
            if not client.bucket_exists(bucket_name):
                client.make_bucket(bucket_name)
        except Exception as e:
            pass


class StorageService:
    def __init__(self, bucket_type: str = "music"):
        # On récupère les settings
        settings = get_settings()

        # Initialisation du client MinIO
        endpoint = settings.MINIO_ENDPOINT
        if endpoint.startswith("http://"):
            endpoint = endpoint[len("http://") :]
        elif endpoint.startswith("https://"):
            endpoint = endpoint[len("https://") :]

        self.client = Minio(
            endpoint,
            access_key=settings.MINIO_ACCESS_KEY,
            secret_key=settings.MINIO_SECRET_KEY,
            secure=settings.MINIO_SECURE,
        )

        # Sélection du bucket selon le besoin (music ou levels)
        if bucket_type == "music":
            self.bucket_name = settings.MINIO_BUCKET_MUSIC
        else:
            self.bucket_name = settings.MINIO_BUCKET_LEVELS

    def get_download_url(self, object_name: str, expires_minutes: int = 60):
        """Génère une URL présignée pour Unity"""
        return self.client.presigned_get_object(
            self.bucket_name, object_name, expires=timedelta(minutes=expires_minutes)
        )

    def upload_file(self, object_name: str, file_path: str):
        """Upload vers le bucket sélectionné"""
        self.client.fput_object(self.bucket_name, object_name, file_path)
        return object_name

    def download_file(self, object_name: str, local_destination: str):
        """Téléchargement pour analyse locale au worker"""
        self.client.fget_object(self.bucket_name, object_name, local_destination)
