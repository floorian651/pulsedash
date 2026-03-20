import psycopg2
from datetime import timedelta
from minio import Minio
from ..core.config import get_settings


class StorageService:
    def __init__(self, bucket_type: str = "audio"):
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

        # Sélection du bucket selon le besoin (audio ou levels)
        if bucket_type == "audio":
            self.bucket_name = settings.MINIO_AUDIO_BUCKET
        else:
            self.bucket_name = settings.MINIO_LEVEL_BUCKET
        
        self.conn = psycopg2.connect(
            user=settings.POSTGRES_USER,
            password=settings.POSTGRES_PASSWORD,
            dbname=settings.POSTGRES_DB,
            host=settings.POSTGRES_HOST,
            port=settings.POSTGRES_PORT,
        )
        self.cur = self.conn.cursor()

    def get_download_url(self, object_name: str, expires_minutes: int = 60):
        """Génère une URL présignée pour Unity"""
        return self.client.presigned_get_object(
            self.bucket_name, object_name, expires=timedelta(minutes=expires_minutes)
        )

    def upload_file(self, object_name: str, file_path: str):
        """Upload vers le bucket sélectionné, ainsi que vers la base de données PostgreSQL"""
        self.client.fput_object(self.bucket_name, object_name, file_path)
        self.cur.execute("INSERT INTO public.music (title) VALUES (%s);", (object_name,))
        self.conn.commit()
        return object_name

    def download_file(self, object_name: str, local_destination: str):
        """Téléchargement pour analyse locale au worker"""
        self.client.fget_object(self.bucket_name, object_name, local_destination)
    
    def close(self):
        """Ferme les connexions à PostgreSQL"""
        self.cur.close()
        self.client.close()