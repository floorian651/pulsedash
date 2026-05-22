# Codes d'erreur

Référence complète des codes HTTP retournés par l'API et des messages d'erreur associés.

---

## Format des erreurs

Toutes les erreurs de l'API sont retournées en JSON selon le format FastAPI standard :

```json
{ "detail": "Description de l'erreur" }
```

Les erreurs de validation Pydantic (`422 Unprocessable Entity`) retournent un format enrichi :

```json
{
  "detail": [
    {
      "loc": ["body", "email"],
      "msg": "value is not a valid email address",
      "type": "value_error.email"
    }
  ]
}
```

---

## Référence par code HTTP

### 400 Bad Request

| Route | Message | Cause |
|---|---|---|
| `POST /auth/register` | `"Erreur lors de la création du compte"` | IntegrityError inattendue en base |
| `POST /music` | `"Music with this title already exists"` | Titre déjà présent |
| `POST /music` | `"Nom de fichier invalide après sanitisation"` | Nom de fichier dangereux |
| `GET /music/{title}/download` | `"Music has no associated file"` | `file_path` est null |
| `POST /generate` | `"Music has no audio file in storage"` | `file_path` est null |

### 401 Unauthorized

| Route | Message | Cause |
|---|---|---|
| Toutes les routes 🔒 | `"Token invalide"` | JWT absent, malformé, expiré ou blacklisté |
| `POST /auth/login` | `"Identifiants incorrects"` | Email ou mot de passe incorrect |
| `POST /auth/refresh` | `"Refresh token invalide"` | Token invalide, expiré ou déjà utilisé |
| `POST /auth/refresh` | `"Utilisateur introuvable"` | Compte supprimé |

### 403 Forbidden

| Route | Message | Cause |
|---|---|---|
| `POST /auth/login` | `"Compte désactivé"` | `is_active = false` |
| `GET /jobs/{job_id}` | `"Accès refusé"` | Job appartenant à un autre utilisateur |
| `PATCH /game-sessions/{id}/end` | `"Not your session"` | Session appartenant à un autre utilisateur |
| Routes admin | `"Accès réservé aux administrateurs"` | `is_admin = false` |

### 404 Not Found

| Route | Message | Cause |
|---|---|---|
| `GET /music/{title}` | `"Music not found"` | Titre inexistant |
| `GET /music/{title}/download` | `"Music not found"` | Titre inexistant |
| `GET /music/{title}/level` | `"Music not found"` | Titre inexistant |
| `GET /music/{title}/level` | `"Level not generated yet"` | `level_path` est null |
| `PUT /music/{title}` | `"Music not found"` | Titre inexistant |
| `DELETE /music/{title}` | `"Music not found"` | Titre inexistant |
| `POST /generate` | `"Music not found"` | Titre inexistant |
| `GET /jobs/{job_id}` | `"Job non trouvé"` | UUID inexistant |
| `GET /playlists/{name}` | `"Playlist not found"` | Nom inexistant |
| `PUT /playlists/{name}` | `"Playlist not found"` | Nom inexistant |
| `DELETE /playlists/{name}` | `"Playlist not found"` | Nom inexistant |
| `GET /tracks/{id}` | `"Track not found"` | ID inexistant |
| `PUT /tracks/{id}` | `"Track not found"` | ID inexistant |
| `DELETE /tracks/{id}` | `"Track not found"` | ID inexistant |
| `POST /tracks` | `"Playlist not found"` | Playlist cible inexistante |
| `POST /tracks` | `"Music not found"` | Musique cible inexistante |
| `POST /game-sessions` | `"Music not found"` | Musique inexistante |
| `PATCH /game-sessions/{id}/end` | `"Session not found"` | UUID inexistant |
| `GET /profile/me` | `"User not found"` | Compte supprimé post-auth |
| `GET /profile/{user_id}` | `"User not found"` | UUID inexistant |
| `POST /jamendo/import/{id}` | `"Track <id> not found on Jamendo"` | ID Jamendo invalide |

### 409 Conflict

| Route | Message | Cause |
|---|---|---|
| `POST /auth/register` | `"Email déjà utilisé"` | Email existant |
| `POST /auth/register` | `"Nom d'utilisateur déjà utilisé"` | Username existant |
| `PATCH /game-sessions/{id}/end` | `"Session already ended"` | Appel sur une session déjà clôturée |

### 413 Request Entity Too Large

| Route | Message | Cause |
|---|---|---|
| `POST /music` | `"Fichier trop volumineux. Limite : 50 Mo."` | Fichier > 50 Mo |

### 415 Unsupported Media Type

| Route | Message | Cause |
|---|---|---|
| `POST /music` | `"Format non supporté : <type>. Formats acceptés : mp3, wav, ogg, flac."` | Content-type non autorisé |

### 422 Unprocessable Entity

Retourné automatiquement par FastAPI/Pydantic lors d'une validation échouée (type incorrect, champ requis manquant, contrainte non respectée).

```json
{
  "detail": [
    {
      "loc": ["body", "final_score"],
      "msg": "ensure this value is greater than or equal to 0",
      "type": "value_error.number.not_ge"
    }
  ]
}
```

### 429 Too Many Requests

```json
{ "error": "Rate limit exceeded: 10 per 1 minute" }
```

Géré par SlowAPI. Le header de réponse `Retry-After` indique le délai avant de réessayer.

### 500 Internal Server Error

| Route | Message | Cause |
|---|---|---|
| `POST /music` | `"Upload failed"` | Erreur inattendue lors de l'upload MinIO |
| `GET /music/{title}/download` | `"Failed to prepare download response: <msg>"` | Erreur MinIO |
| `GET /music/{title}/level` | `"Failed to prepare download response: <msg>"` | Erreur MinIO |

### 502 Bad Gateway

| Route | Message | Cause |
|---|---|---|
| `GET /jamendo/search` | `"Jamendo unavailable: <msg>"` | API Jamendo inaccessible |
| `POST /jamendo/import/{id}` | `"Jamendo unavailable: <msg>"` | API Jamendo inaccessible |
| `POST /jamendo/import/{id}` | `"Audio download failed: <msg>"` | Échec du téléchargement MP3 |
| `POST /jamendo/import/{id}` | `"No audio URL returned by Jamendo"` | Track sans URL audio |

---

## Codes de fermeture WebSocket

| Code | Signification | Cause |
|---|---|---|
| `1000` | Normal Closure | Connexion fermée proprement (job terminé) |
| `1008` | Policy Violation | Token invalide, utilisateur inactif ou accès non autorisé |
