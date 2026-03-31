import os

from celery import Celery


def _build_broker_url() -> str:
    user = os.getenv("REDIS_USER", "")
    password = os.getenv("REDIS_PASSWORD", "")
    host = os.getenv("REDIS_HOST", "localhost")
    port = os.getenv("REDIS_PORT", "6379")
    db = os.getenv("REDIS_DB", "0")

    auth = ""
    if user and password:
        auth = f"{user}:{password}@"
    elif password:
        auth = f":{password}@"

    return f"redis://{auth}{host}:{port}/{db}"


broker_url = _build_broker_url()
result_backend = broker_url

app = Celery("wavr", broker=broker_url, backend=result_backend)
app.conf.update(
    task_serializer="json",
    accept_content=["json"],
    result_serializer="json",
    timezone=os.getenv("TZ", "UTC"),
    enable_utc=True,
    include=["src.api.services.tasks"],
)
