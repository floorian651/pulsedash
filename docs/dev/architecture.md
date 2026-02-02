## Architecture

Ce document décrit l'architecture logique et l'architecture de déploiement du projet. Les schémas ci‑dessous montrent le flux principal entre le client Unity, l'API FastAPI, le broker Redis, les workers asynchrones, et les services de stockage et de base de données.

## Architecture logique

### Rôles des composants

- **Client Unity** : envoie les requêtes (ex. upload, paramètres d'analyse) et consomme les résultats.
- **API FastAPI** : point d'entrée HTTP. Valide les requêtes, déclenche le traitement asynchrone, et sert les résultats.
- **Redis (broker)** : file d'attente pour les tâches Celery, assurant le découplage entre API et workers.
- **Workers Celery** : exécutent les pipelines de traitement (analyse, transformation, génération de métadonnées).
- **PostgreSQL** : stocke les métadonnées, états des jobs et résultats structurés.
- **MinIO** : stockage objet des fichiers d'entrée/sortie (ex. artefacts, exports, ressources).
- **Beat** : ordonnance les tâches planifiées (jobs récurrents, nettoyage, re‑traitements).

### Flux principal

1. Le **client Unity** envoie une requête HTTP à l'**API**.
2. L'**API** enregistre la requête et la **met en file** via **Redis**.
3. Les **workers** consomment la tâche, exécutent le pipeline, stockent les artefacts dans **MinIO** et les métadonnées dans **PostgreSQL**.
4. L'**API** récupère et sert les résultats au client (lecture en base et fichiers via MinIO).

```mermaid
%%{init: {'themeVariables': { 'fontFamily': 'Roboto', 'fontSize': '14px'}, 'flowchart': { 'curve': 'monotoneX' } } }%%
flowchart LR
  subgraph Client [ Zone Client ]
    Unity(["fa:fa-gamepad Unity Client"])
  end

  subgraph API_Service [ Service Web ]
    API["fa:fa-server FastAPI"]
  end

  subgraph Broker_Backend [ Broker & État ]
    Redis[("fa:fa-memory Redis (broker)")]
    Postgres[("fa:fa-database PostgreSQL")]
  end

  subgraph Workers [ Traitement Asynchrone ]
    Worker1{{"fa:fa-cogs Worker A"}}
    Worker2{{"fa:fa-cogs Worker B"}}
    Beat{{"fa:fa-clock Beat"}}
  end

  subgraph Storage [ Stockage Fichiers ]
    MinIO[("fa:fa-hdd MinIO")]
  end

  Unity -->|"HTTP POST"| API
  API -->|"enqueue"| Redis
  API -.->|"direct (opt)"| Worker1
  Beat -->|"schedule"| Redis
  Redis -->|"deliver"| Worker1
  Redis -->|"deliver"| Worker2
  Worker1 -->|"pipeline"| MinIO
  Worker2 -->|"pipeline"| MinIO
  Worker1 -->|"meta"| Postgres
  Worker2 -->|"meta"| Postgres
  API -->|"read"| Postgres
  API -->|"serve"| MinIO
```

## Architecture de déploiement

### Principes

- Les services sont **conteneurisés** et orchestrés via **Docker Compose**.
- L'API et les workers partagent le broker Redis et le stockage objet MinIO.
- Les volumes persistants garantissent la durabilité des données (PostgreSQL et MinIO).

### Réseaux et ports

- **Redis** : port 6379 (broker Celery)
- **PostgreSQL** : port 5432 (métadonnées)
- **MinIO** : port 9000 (stockage objet)

```mermaid
%%{init: {'themeVariables': { 'fontFamily': 'Fira Code, Consolas, Monaco, monospace', 'fontSize': '13px'}, 'flowchart': { 'curve': 'basis' } } }%%
flowchart TD
  subgraph Host [ Machine Hôte ]
    DockerEngine["fa:fa-docker Docker Engine"]

    subgraph ComposeStack [ Stack Docker Compose ]
        direction TB

        subgraph Compute [ Compute ]
            app["fa:fa-server app (uvicorn)"]
            worker{{"fa:fa-cogs celery_worker"}}
        end

        subgraph DataServices [ Data Services ]
            redis[("fa:fa-memory redis")]
            db[("fa:fa-database postgres")]
            minio[("fa:fa-hdd minio")]
        end
    end

    Volumes[("fa:fa-folder-open Volumes Persistants")]
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
```mermaid
%%{init: {'themeVariables': { 'fontFamily': 'Roboto', 'fontSize': '14px'}, 'flowchart': { 'curve': 'monotoneX' } } }%%
flowchart LR
  subgraph Client [ Zone Client ]
    Unity(["fa:fa-gamepad Unity Client"])
  end

  subgraph API_Service [ Service Web ]
    API["fa:fa-server FastAPI"]
  end

  subgraph Broker_Backend [ Broker & État ]
    Redis[("fa:fa-memory Redis (broker)")]
    Postgres[("fa:fa-database PostgreSQL")]
  end

  subgraph Workers [ Traitement Asynchrone ]
    Worker1{{"fa:fa-cogs Worker A"}}
    Worker2{{"fa:fa-cogs Worker B"}}
    Beat{{"fa:fa-clock Beat"}}
  end

  subgraph Storage [ Stockage Fichiers ]
    MinIO[("fa:fa-hdd MinIO")]
  end

  Unity -->|"HTTP POST"| API
  API -->|"enqueue"| Redis
  API -.->|"direct (opt)"| Worker1
  Beat -->|"schedule"| Redis
  Redis -->|"deliver"| Worker1
  Redis -->|"deliver"| Worker2
  Worker1 -->|"pipeline"| MinIO
  Worker2 -->|"pipeline"| MinIO
  Worker1 -->|"meta"| Postgres
  Worker2 -->|"meta"| Postgres
  API -->|"read"| Postgres
  API -->|"serve"| MinIO
```

```mermaid
%%{init: {'themeVariables': { 'fontFamily': 'Fira Code, Consolas, Monaco, monospace', 'fontSize': '13px'}, 'flowchart': { 'curve': 'basis' } } }%%
flowchart TD
  subgraph Host [ Machine Hôte ]
    DockerEngine["fa:fa-docker Docker Engine"]

    subgraph ComposeStack [ Stack Docker Compose ]
        direction TB

        subgraph Compute [ Compute ]
            app["fa:fa-server app (uvicorn)"]
            worker{{"fa:fa-cogs celery_worker"}}
        end

        subgraph DataServices [ Data Services ]
            redis[("fa:fa-memory redis")]
            db[("fa:fa-database postgres")]
            minio[("fa:fa-hdd minio")]
        end
    end

    Volumes[("fa:fa-folder-open Volumes Persistants")]
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
