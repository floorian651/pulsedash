# Le point d’entrée de l’API.

#     crée l’application FastAPI

#     configure CORS

#     initialise la DB

#     monte les routers

#     expose WebSocket /ws/jobs/{job_id}

# C’est le fichier à lancer avec Uvicorn.

# from fastapi import FastAPI, File, UploadFile

# from src.pipeline.main import main as run_pipeline

from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.middleware.cors import CORSMiddleware

from src.api.core.config import get_settings
from src.api.db.session import init_engine
from src.api.routers import generate, jobs, music
from src.api.services.storage import ensure_buckets_exist
from src.api.utils.websocket_manager import WebSocketManager

settings = get_settings()
ws_manager = WebSocketManager()


def create_app() -> FastAPI:
    app = FastAPI(title=settings.APP_NAME, debug=settings.DEBUG)

    # ---------------------------------------------------------
    # CORS
    # ---------------------------------------------------------
    app.add_middleware(
        CORSMiddleware,
        allow_origins=[o.strip() for o in settings.CORS_ORIGINS.split(",")],
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )

    # ---------------------------------------------------------
    # Database init
    # ---------------------------------------------------------
    init_engine(settings.DATABASE_URL)

    # ---------------------------------------------------------
    # MinIO setup
    # ---------------------------------------------------------
    ensure_buckets_exist()

    # ---------------------------------------------------------
    # Routers
    # ---------------------------------------------------------
    app.include_router(
        generate.router, prefix=settings.API_V1_PREFIX, tags=["generate"]
    )

    app.include_router(jobs.router, prefix=settings.API_V1_PREFIX, tags=["jobs"])

    app.include_router(music.router, prefix=settings.API_V1_PREFIX, tags=["music"])

    # ---------------------------------------------------------
    # Healthcheck
    # ---------------------------------------------------------
    @app.get("/")
    async def healthcheck():
        return {"status": "ok", "app": settings.APP_NAME}

    # ---------------------------------------------------------
    # WebSocket: suivi des jobs
    # ---------------------------------------------------------
    @app.websocket("/ws/jobs/{job_id}")
    async def job_ws(websocket: WebSocket, job_id: str):
        await ws_manager.connect(job_id, websocket)
        try:
            while True:
                await websocket.receive_text()  # on garde la connexion ouverte
        except WebSocketDisconnect:
            ws_manager.disconnect(job_id, websocket)

    return app


app = create_app()
