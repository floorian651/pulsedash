# PulseDash

**PulseDash** est un jeu de rythme 3D où les niveaux sont générés automatiquement à partir de l'analyse audio de tes morceaux. Importe une chanson, et le jeu construit un parcours d'obstacles synchronisé sur la musique — tempo, énergie, beats.

Le projet se compose d'un client Unity et d'un backend FastAPI qui pilote l'analyse audio (Celery + Redis) et le stockage (PostgreSQL + MinIO).

- Client Unity : [`Pulsedash`](https://github.com/floorian651/wavr) (ce dépôt)
- Backend : [`pulsedash_backend`](https://github.com/floorian651/pulsedash_backend)
- Documentation : [floorian651.github.io/pulsedash](https://floorian651.github.io/pulsedash/)

---

## 1. Vue d'ensemble

**PulseDash** est un jeu de rythme 3D dans lequel les niveaux sont générés procéduralement à partir de l'analyse audio de morceaux musicaux. Chaque chanson est analysée côté serveur pour produire un fichier de niveau JSON (placement des obstacles, tempo, énergie par section), que le client Unity consomme pour synchroniser le gameplay sur la musique.

Le projet repose sur une architecture qui contient deux composants indépendants :

- **Client** : application Unity 3D qui orchestre l'expérience de jeu, consomme les données de niveau générées par le backend, et interroge l'API Jamendo pour le streaming audio.
- **Backend** : API REST asynchrone qui lance le pipeline d'analyse audio et récupère les résultats.

---

## 2. Comment jouer

PulseDash se distribue sous forme de binaire compilé depuis Unity. Téléchargez la dernière version, extrayez l'archive et lancez l'exécutable. Aucune installation supplémentaire n'est requise.

1. Téléchargez l'exécutable correspondant à votre système d'exploitation dans Releases.
2. Dans le dossier que vous venez de télécharger, lancez PulseDash. Vous êtes maintenant sur le jeu !
3. Depuis l'écran d'accueil, créez vous un compte puis connectez-vous à ce compte (ou autre compte préalablement existant).
4. Sélectionnez un morceau depuis la barre de recherche puis lancez le jeu.
5. Choisissez un mode de jeu.
6. Le backend génère le niveau à partir de l'analyse audio, patientez quelques secondes.
7. Le niveau se charge et la partie commence.
8. Le personnage avance automatiquement  déplacez-vous latéralement et sautez sur les pulsers (dragons).
9. À la fin du parcours, votre score est affiché.

## 3. Architecture Système

Le présent dépôt héberge exclusivement le **client Unity**. Le backend est maintenu dans un dépôt dédié :

**[Pulsedash Backend](https://github.com/floorian651/pulsedash_backend)**

## 4. Prérequis Système

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

## 5. Configuration de l'Environnement Backend

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

## 6. Procédure d'Installation et de Démarrage

Documentation complète : [https://floorian651.github.io/pulsedash/](https://floorian651.github.io/pulsedash/)

### 6.1 Frontend

#### Ouvrir le projet dans Unity Hub

1. Lancer Unity Hub.
2. Cliquer sur **Add project from disk**.
3. Sélectionner le dossier `unity/` à la racine du dépôt.
4. Vérifier que l'éditeur **6000.3.5f1** est installé dans Unity Hub.
5. Ouvrir le projet — Unity importe les packages automatiquement (la première ouverture peut prendre plusieurs minutes).

### 6.2 Backend

#### 6.2.1 Cloner le dépôt backend

```bash
git clone https://github.com/floorian651/pulsedash_backend
cd pulsedash_backend
```

#### 6.2.2 Configurer les variables d'environnement

```bash
cp .env.example .env
# Renseigner toutes les variables de la section 4
```

## 7. Crédits

### 7.1 Assets récupérés sur poly.pizza
- **Ligne d’arrivée** : 034 by Daisuke Takeoka [CC-BY] (https://creativecommons.org/licenses/by/3.0/) via Poly Pizza (https://poly.pizza/m/67FjFVyAxq0)
- **Joueur** : Animated Platformer Character by Quaternius (https://poly.pizza/m/kKtL4zvS3n)
- **Pulsers (dragons)** : Dragon by Quaternius (https://poly.pizza/m/VBvzjFIYws)
- **Sol** : Path Straight by Quaternius (https://poly.pizza/m/ZuRHRsKWoz)
- **Lapins** : Rabbit by Poly by Google [CC-BY] (https://creativecommons.org/licenses/by/3.0/) via Poly Pizza (https://poly.pizza/m/dyeBDJxhDwP)
- **Cerfs** : Deer by Poly by Google [CC-BY] (https://creativecommons.org/licenses/by/3.0/) via Poly Pizza (https://poly.pizza/m/fUo4AIcd8XR)
- **Ratons laveurs** : Raccoon by Poly by Google [CC-BY] (https://creativecommons.org/licenses/by/3.0/) via Poly Pizza (https://poly.pizza/m/2iYORwFng3_)
- **Loups** : Wolf by Quaternius (https://poly.pizza/m/P1gU3Qkr9r)
- **Cerfs hémiones** : Mule deer by Poly by Google [CC-BY] (https://creativecommons.org/licenses/by/3.0/) via Poly Pizza (https://poly.pizza/m/e6mV6AYvIrE)


### 7.2 Autres assets
- **Forêt** : KayKit - Forest Nature Pack - by Kay Lousberg (https://kaylousberg.itch.io/kaykit-forest)
- **Assets médievaux pour l’image de fond de la plateforme de streaming** : KayKit - Medieval Hexagon Pack  - by Kay Lousberg (https://kaylousberg.itch.io/kaykit-medieval-hexagon)


### 7.3 Images
- **Image de fond pour les crédits** : Image trouvée sur Canva (https://www.canva.com/s/templates?query=beige+and+black+vintage+sketch)
- **Images des boutons et autres éléments de l’interface utilisateur** : Images trouvées sur Canva (https://www.canva.com)


### 7.4 Équipe de développement
- Florian ABADIE (https://github.com/floorian651)
- Chloé AUBRY (https://github.com/Chlaubry)
- Arthur BLAMART (https://github.com/Arthur-Blamart)
- Quentin BRULÉ (https://github.com/QuentinBrule)
- Sothaline HUOT
- Clément JOURDIN (https://github.com/Fanchoir302)
- Victor ROUET (https://github.com/Gyro25720)
