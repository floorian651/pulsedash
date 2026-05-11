# PulseDash

Documentation de référence technique du projet.

---

## 1. Vue d'ensemble

**PulseDash** est un jeu de rythme 3D dans lequel les niveaux sont générés procéduralement à partir de l'analyse audio de morceaux musicaux. Chaque chanson est analysée côté serveur pour produire un fichier de niveau JSON (placement des obstacles, tempo, énergie par section), que le client Unity consomme pour synchroniser le gameplay sur la musique.

Le projet repose sur une architecture qui contient deux composants indépendants :

- **Client** : application Unity 3D qui orchestre l'expérience de jeu, consomme les données de niveau générées par le backend, et interroge l'API Jamendo pour le streaming audio.
- **Backend** : API REST asynchrone qui lance le pipeline d'analyse audio et récupère les résultats.

---

## 2. Architecture Système

Le présent dépôt héberge exclusivement le **client Unity**. Le backend est maintenu dans un dépôt dédié :

**[Pulsedash Backend](https://github.com/floorian651/pulsedash_backend)**

## 3. Prérequis Système

### Client Unity

| Outil | Version requise | Remarque |
|---|---|---|
| Unity Hub | 3.x ou supérieure | Gestionnaire d'installations Unity |
| Unity Editor | **6000.3.5f1** | Version exacte requise pour la compatibilité des packages |
| Packages Python (Documentation) | Python 3.12 | A installer dans un .venv (pip ou uv) |
| Git | 2.x | Contrôle de version |

### Backend

| Outil | Version requise | Remarque |
|---|---|---|
| Podman Engine | 24.x ou supérieure | Exécution des conteneurs |
| Podman Compose | v2.x | Orchestration locale des services |
| Git | 2.x | Contrôle de version |
---

## 4. Configuration de l'Environnement Backend

### Backend — fichier `.env`

Créer un fichier `.env` à la racine du **dépôt backend** (après clonage, voir section 5). Les services Python lisent ce fichier via `pydantic-settings` ; les outils CLI (`celery`, `alembic`) nécessitent un sourcing manuel dans chaque terminal :

**Variables :**

| Variable | Exemple | Description |
|---|---|---|
| `POSTGRES_HOST` | `db` | Nom du service PostgreSQL dans Docker Compose |
| `POSTGRES_PORT` | `5432` | Port TCP de PostgreSQL |
| `POSTGRES_DB` | `pulsedash` | Nom de la base de données |
| `POSTGRES_USER` | `pulsedash` | Utilisateur PostgreSQL |
| `POSTGRES_PASSWORD` | `pulsedash_secret` | Mot de passe PostgreSQL |
| `REDIS_HOST` | `redis` | Nom du service Redis dans Docker Compose |
| `REDIS_PORT` | `6379` | Port TCP de Redis |
| `REDIS_DB` | `0` | Index de la base Redis utilisée |
| `MINIO_ENDPOINT` | `http://minio:9000` | URL interne du service MinIO  |
| `MINIO_ACCESS_KEY` | `minio` | Clé d'accès MinIO |
| `MINIO_SECRET_KEY` | `minio123` | Clé secrète MinIO |
| `JAMENDO_CLIENT_ID` | `ac3af075` | Identifiant client de l'API Jamendo |
| `SECRET_KEY` | `<clé-aléatoire>` | Clé de signature pour la sécurité applicative |

> **Important :** Ne pas versionner le fichier `.env`. L'ajouter impérativement à `.gitignore`.
> Les variables `CELERY_BROKER_URL` et `CELERY_RESULT_BACKEND` ne doivent **pas** figurer dans `.env` ,elles sont construites par `celery_app.py` à partir des variables Redis individuelles (`REDIS_HOST`, `REDIS_PORT`, `REDIS_DB`).
> Voir .env projet backend si il y a besoin d'exposer l'api via le tunnel cloudflare.

## 5. Procédure d'Installation et de Démarrage

Documentation complète : [https://floorian651.github.io/wavr/](https://floorian651.github.io/wavr/)

### 5.1 Frontend

#### Ouvrir le projet dans Unity Hub

1. Lancer Unity Hub.
2. Cliquer sur **Add project from disk**.
3. Sélectionner le dossier `unity/` à la racine du dépôt.
4. Vérifier que l'éditeur **6000.3.5f1** est installé dans Unity Hub.
5. Ouvrir le projet — Unity importe les packages automatiquement (la première ouverture peut prendre plusieurs minutes).

### 5.2 Backend

#### 5.2.1 Cloner le dépôt backend

```bash
git clone https://github.com/floorian651/pulsedash_backend
cd pulsedash_backend
```

#### 5.2.2 Configurer les variables d'environnement

```bash
cp .env.example .env
# Renseigner toutes les variables de la section 4
```


