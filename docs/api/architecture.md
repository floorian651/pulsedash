# PulseDash — Architecture Backend
## Architecture

Ce document décrit l'architecture et le schéma de déploiement du projet PulseDash. Le projet est organisé en deux dépôts indépendants : le présent dépôt héberge le **client Unity**, le second héberge le **backend** (API, workers, base de données, stockage).

- Dépôt backend : [floorian651/pulsedash_backend](https://github.com/floorian651/pulsedash_backend)
## Vue d'ensemble

PulseDash API suit une architecture **en couche** déployée sur un conteneur Linux. Elle combine un serveur HTTP asynchrone (FastAPI/Uvicorn), un broker de tâches asynchrones (Celery/Redis), un stockage objet S3-compatible (MinIO) et une base de données relationnelle (PostgreSQL). L'exposition publique est assurée via un tunnel Cloudflare.

## Architecture de déploiement

- Le backend est entièrement **conteneurisé** et orchestré via **Docker Compose**.
---

## Organisation du code source

```
src/api/
├── core/
│   ├── config.py          # Settings Pydantic (env vars)
│   ├── celery_app.py      # Instance Celery
│   └── limiter.py         # SlowAPI rate limiter
├── db/
│   ├── models/            # Modèles SQLAlchemy
│   ├── repositories/      # Pattern Repository (accès données)
│   ├── migrations/        # Migrations Alembic
│   └── session.py         # Engine + Session SQLAlchemy
├── routers/               # Endpoints FastAPI (10 modules)
├── schemas/               # Modèles Pydantic I/O
├── services/
│   ├── auth.py            # Logique JWT, bcrypt
│   ├── storage.py         # Interface MinIO
│   ├── tasks.py           # Tâches Celery
│   ├── jamendo.py         # Client API Jamendo
│   └── token_blacklist.py # Blacklist Redis JTI
├── dependencies.py        # Injections (get_current_user, get_admin_user)
└── main.py                # Factory app + WebSocket /ws/jobs/{id}
```

### Réseaux et ports

| Service | Port | Rôle |
|---|---|---|
| API (Uvicorn) | 8000 | Point d'entrée HTTP |
| Redis | 6379 | Broker Celery |
| PostgreSQL | 5432 | Métadonnées |
| MinIO API | 9000 | Stockage objet |
| MinIO Console | 9001 | Interface web d'administration |

```mermaid
%%{init: {'themeVariables': { 'fontFamily': 'Fira Code, Consolas, Monaco, monospace', 'fontSize': '13px'}, 'flowchart': { 'curve': 'basis' } } }%%
flowchart TD
  subgraph Host [ Machine Hôte ]
    DockerEngine["Docker Engine"]

    subgraph ComposeStack [ Stack Docker Compose ]
        direction TB

        subgraph Compute [ Compute ]
            app["app (uvicorn :8000)"]
            worker{{"celery_worker"}}
        end

        subgraph DataServices [ Data Services ]
            redis[("redis :6379")]
            db[("postgres :5432")]
            minio[("minio :9000/:9001")]
        end
    end

    Volumes[("Volumes Persistants")]
  end

  DockerEngine -.->|"gère"| ComposeStack

  app -->|"TCP :6379"| redis
  app -->|"TCP :5432"| db
  app -->|"TCP :9000"| minio

  worker -->|"TCP :6379"| redis
  worker -->|"TCP :9000"| minio

  db ===|"mount"| Volumes
  minio ===|"mount"| Volumes
```


### Rôles des composants

- **Client Unity** : envoie les requêtes (génération de niveau, récupération de résultats) et consomme le fichier `level.json` pour piloter le gameplay.
- **API FastAPI** : point d'entrée HTTP. Valide les requêtes, crée les jobs, déclenche le traitement asynchrone et renvoie les résultats.
- **Redis (broker)** : file d'attente pour les tâches Celery, assurant le découplage entre l'API et les workers.
- **Workers Celery** : exécutent le pipeline de  (téléchargement audio, analyse, génération du niveau JSON).
- **PostgreSQL** : stocke l'état des jobs (`pending` → `running` → `done` / `failed`) et les métadonnées.
- **MinIO** : stockage objet des fichiers d'entrée/sortie (audio MP3, niveaux JSON).
- **Jamendo API** : source externe de morceaux audio libres de droits.

```mermaid
flowchart TB
    subgraph Clients["Clients"]
        UNITY["Unity"]
    end

    subgraph Edge["Edge"]
        CF["Tunnel Cloudflare"]
    end

    subgraph API["FastAPI  ·  :9050"]
        direction TB
        REST["REST API\n/api/v1/..."]
        WS["WebSocket\n/ws/jobs/{id}"]

        subgraph Routers["Routers"]
            R_AUTH["auth"]
            R_MUSIC["music · playlists · tracks"]
            R_JAM["jamendo"]
            R_GEN["generate"]
            R_JOBS["jobs"]
            R_SCORES["scores · game_sessions"]
            R_PROFILE["profile"]
        end
    end

    subgraph Async["Traitement asynchrone"]
        REDIS["🗄️ Redis\nBroker + Pub/Sub"]
        CELERY["⚙️ Celery Worker\ngenerate_level"]
        PIPELINE["🎵 Pipeline Librosa\nBPM · Key · Hits · Sections"]
    end

    subgraph Storage["Stockage"]
        PG[("🐘 PostgreSQL\nUsers · Tracks · Playlists\nScores · GameSessions · Jobs")]
        MINIO[("MinIO\nbucket: music\nbucket: levels")]
    end

    subgraph External["Externe"]
        JAMENDO["🎶 Jamendo API"]
    end

    %% Flux principaux
    Clients -->|"HTTPS"| CF
    CF -->|"HTTP"| API

    REST --> Routers
    WS <-->|"Pub/Sub"| REDIS

    Routers --> PG
    Routers -->|"Enqueue task"| REDIS
    Routers <-->|"Presigned URL"| MINIO
    R_JAM <-->|"Search / Download"| JAMENDO

    REDIS -->|"Task queue"| CELERY
    CELERY --> PIPELINE
    CELERY <-->|"Download / Upload"| MINIO
    CELERY -->|"Publish progress"| REDIS
    CELERY --> PG

    PIPELINE -->|"level.json"| CELERY

    style Clients fill:#1e3a5f,color:#fff,stroke:#4a9eff
    style Edge fill:#2d1b4e,color:#fff,stroke:#9b59b6
    style API fill:#1a3a2a,color:#fff,stroke:#2ecc71
    style Async fill:#3a2000,color:#fff,stroke:#f39c12
    style Storage fill:#2a1a1a,color:#fff,stroke:#e74c3c
    style External fill:#1a2a3a,color:#fff,stroke:#3498db
```

---
## Génération d'un niveau

La génération de niveau est le coeur du système. L'API répond avec `202 Accepted` et délègue le traitement audio à un worker Celery. Le client a un retour de la progression via WebSocket.

1. Téléchargement du fichier MP3 depuis l'API Jamendo.
2. Stockage de l'audio dans MinIO (bucket `audio`).
3. Analyse audio via **librosa** : extraction du tempo (BPM), détection des beats, analyse spectrale.
4. Génération du fichier `level.json` structuré.
5. Stockage du niveau dans MinIO (bucket `levels`).
6. Mise à jour de l'état du job dans PostgreSQL (`état=completed`, `result_path`).

| Bibliothèque | Rôle |
|---|---|
| librosa | Analyse audio (tempo, beats, spectral features) |
| numpy | Calcul numérique |
| scipy | Traitement du signal |

```mermaid
sequenceDiagram
    participant C as 🎮 Client
    participant API as ⚡ FastAPI
    participant RD as 🗄️ Redis
    participant CL as ⚙️ Celery
    participant JAM as 🎶 Jamendo
    participant MN as 🪣 MinIO
    participant PL as 🎵 Pipeline

    C->>API: POST /api/v1/generate\n{music_title}
    API->>RD: Enqueue generate_level
    API-->>C: 202 {job_id, state: "pending"}

    C->>API: WS /ws/jobs/{job_id}?token=...
    API->>RD: SUBSCRIBE job:{job_id}
    API-->>C: snapshot initial {state, progress}

    RD->>CL: Dispatch task
    CL->>MN: Download audio (bucket: music)
    Note over CL: progress 40%
    CL->>RD: PUBLISH {state: running, progress: 40}
    RD-->>API: événement Redis
    API-->>C: {state: running, progress: 40}

    CL->>PL: Analyse audio
    Note over PL: BPM · Key · Hits · Sections
    Note over CL: progress 85%
    CL->>RD: PUBLISH {state: running, progress: 85}
    API-->>C: {state: running, progress: 85}

    CL->>MN: Upload level.json (bucket: levels)
    CL->>RD: PUBLISH {state: completed, progress: 100}
    API-->>C: {state: completed, progress: 100}

    C->>API: GET /api/v1/music/{title}/level
    API->>MN: get_download_response(level_path)
    API-->>C: StreamingResponse (level.json)
```
---

## Authentification et gestion des tokens

```mermaid
sequenceDiagram
    participant C as Client
    participant API as FastAPI
    participant DB as PostgreSQL
    participant R as Redis (Blacklist)

    C->>API: POST /api/v1/auth/login {email, password}
    API->>DB: Vérifie email + bcrypt hash
    API-->>C: {access_token (60min), refresh_token (30j)}

    Note over C,API: Requêtes authentifiées
    C->>API: GET /api/v1/auth/me\nAuthorization: Bearer <access_token>
    API->>R: Vérifie JTI non blacklisté
    API->>DB: Charge l'utilisateur
    API-->>C: UserProfile

    Note over C,API: Renouvellement du token (rotation)
    C->>API: POST /api/v1/auth/refresh {refresh_token}
    API->>R: Vérifie JTI non blacklisté
    API->>R: Blackliste l'ancien refresh_token
    API-->>C: Nouveaux {access_token, refresh_token}

    Note over C,API: Déconnexion propre
    C->>API: POST /api/v1/auth/logout\n+ Bearer access_token + {refresh_token}
    API->>R: Blackliste access_token (JTI)
    API->>R: Blackliste refresh_token (JTI)
    API-->>C: 200 {message: "Déconnecté avec succès"}
```

---

## Modèle de données

```mermaid
erDiagram
    USERS {
        string id PK
        string username UK
        string email UK
        string password
        boolean is_active
        boolean is_admin
        datetime created_at
    }
    MUSIC {
        string title PK
        string artist
        float bpm
        float duration
        string bucket_name
        string file_path
        string level_path
    }
    JOBS {
        string id PK
        string user_id FK
        string state
        int progress
        string result_path
        string error_message
    }
    PLAYLISTS {
        string name PK
        string description
        datetime created_at
    }
    TRACKS {
        int id PK
        string playlist_name FK
        string music_title FK
        int position
    }
    GAME_SESSIONS {
        string id PK
        string user_id FK
        string music_title FK
        string status
        datetime started_at
        datetime ended_at
        int final_score
        float accuracy
    }
    SCORES {
        int id PK
        string user_id FK
        string session_id FK
        string music_title FK
        int points
        float accuracy
        datetime created_at
    }

    USERS ||--o{ JOBS : "lance"
    USERS ||--o{ GAME_SESSIONS : "joue"
    USERS ||--o{ SCORES : "obtient"
    MUSIC ||--o{ TRACKS : "appartient à"
    MUSIC ||--o{ GAME_SESSIONS : "utilisée dans"
    MUSIC ||--o{ SCORES : "liée à"
    PLAYLISTS ||--o{ TRACKS : "contient"
    GAME_SESSIONS ||--o| SCORES : "génère"
```

### Décisions architecturales

**Repository Pattern** — La couche `repositories/` découple la logique métier de SQLAlchemy.

**202 Accepted + WebSocket** — La génération de niveaux étant longue, l'API répond immédiatement et donne les mises à jour de progression via WebSocket/Redis.

**Rotation du Refresh Token** — À chaque appel `/auth/refresh`, l'ancien token est blacklisté et un nouveau est émis. Cela prévient la réutilisation d'un token volé.

**Token Blacklist** — Les tokens JWT sont révoqués par leur champ `jti` (id unique par token) stocké dans Redis avec un TTL en adéquation sur l'expiration du token.
