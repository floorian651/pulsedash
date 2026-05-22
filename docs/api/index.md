# PulseDash API

**PulseDash** est une API REST développée avec [FastAPI](https://fastapi.tiangolo.com/) qui constitue le backend d'un jeu de rythme. Elle orchestre la gestion des utilisateurs, le catalogue musical, la génération automatique de niveaux de jeu à partir d'un fichier audio, ainsi que le suivi des scores et sessions de jeu.

---

## Informations générales

| Propriété        | Valeur                                  |
|------------------|-----------------------------------------|
| **URL de base**  | `https://pulsedashapi.floabd.app`       |
| **Préfixe API**  | `/api/v1`                               |
| **Format**       | JSON (application/json)                 |
| **Authentification** | JWT               |
| **Version**      | 1.0.0                                   |

Toutes les routes API sont accessibles sous le préfixe `/api/v1`. Exemple :

```
GET https://pulsedashapi.floabd.app/api/v1/music
```

---

## Healthcheck

=== "Requête"
    ```http
    GET /
    ```

=== "Réponse 200"
    ```json
    {
      "status": "ok",
      "app": "PulseDash API"
    }
    ```

---

## Authentification rapide

L'API utilise des tokens JWT. Pour accéder aux routes protégées, inclure le token dans l'en-tête HTTP :

```http
Authorization: Bearer <access_token>
```

> Voir la section [Authentification](authentication.md) pour le détail du cycle de vie des tokens.

---

## Limites de débit (Rate Limiting)

| Route                     | Limite        |
|---------------------------|---------------|
| `POST /auth/register`     | 5 / minute    |
| `POST /auth/login`        | 10 / minute   |
| `POST /auth/refresh`      | 10 / minute   |
| `GET /jamendo/search`     | 30 / minute   |
| `POST /jamendo/import`    | 10 / minute   |
| `POST /generate`          | 10 / minute   |

En cas de dépassement, l'API retourne `429 Too Many Requests`.

---

## Ressources de la documentation

| Section | Description |
|---|---|
| [Architecture](architecture.md) | Vue d'ensemble technique, flux de données, infrastructure |
| [Authentification](authentication.md) | JWT, refresh tokens, blacklist |
| [Endpoints](endpoints/auth.md) | Référence complète de chaque route |
| [Modèles de données](models.md) | Schémas Pydantic et modèles SQLAlchemy |
| [WebSocket](websocket.md) | Suivi temps réel des jobs de génération |
| [Codes d'erreur](errors.md) | Référence des codes HTTP retournés |
