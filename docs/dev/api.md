# API Backend

La documentation complète de l'API REST est maintenue dans le dépôt backend :

[floorian651/pulsedash_backend](https://github.com/floorian651/pulsedash_backend)

Une fois le backend démarré localement, la documentation Swagger interactive est disponible sur :

```
http://127.0.0.1:8000/docs
```

## Flux d'appels depuis Unity

```mermaid
flowchart TD
    Unity["Unity Client"]
    FastAPI["FastAPI /api/v1"]
    Celery["Celery Worker"]
    Minio["MinIO (levels)"]
    Postgres["PostgreSQL (jobs)"]
    Jamendo["Jamendo API"]

    Unity -->|"POST /generate\n{ track_id }"| FastAPI
    FastAPI -->|"INSERT job (état=pending)"| Postgres
    FastAPI -->|"enqueue tâche"| Celery

    Celery -->|"GET MP3"| Jamendo
    Celery -->|"run pipeline audio"| Celery
    Celery -->|"upload level.json"| Minio
    Celery -->|"UPDATE job (état=done)"| Postgres

    Unity -->|"GET /jobs/{id}"| FastAPI
    FastAPI -->|"SELECT job"| Postgres
    FastAPI -->|"presigned URL"| Minio
    Unity -->|"download level.json"| Minio
```

## Endpoints consommés par Unity

| Méthode | Route | Description |
|---|---|---|
| `POST` | `/api/v1/generate` | Lance la génération d'un niveau |
| `GET` | `/api/v1/jobs/{job_id}` | Consulte l'état d'un job |

Pour le détail des schémas de requête/réponse, voir [Schéma des données](data-schema.md).
