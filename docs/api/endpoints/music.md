# Music

Gestion du catalogue musical : création, consultation, téléchargement audio, récupération des niveaux générés.

**Préfixe :** `/api/v1/music`

---

## GET /music

Liste toutes les musiques du catalogue avec pagination.

**`GET /api/v1/music`**

### Query Parameters

| Paramètre | Type | Défaut | Description |
|---|---|---|---|
| `skip` | integer | `0` | Nombre d'éléments à ignorer |
| `limit` | integer | `100` | Nombre maximum de résultats |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/music?skip=0&limit=20"
    ```

=== "200 OK"
    ```json
    [
      {
        "title": "Midnight Drive",
        "artist": "Synthwave Artist",
        "bpm": 128.0,
        "duration": 213.5,
        "bucket_name": "music",
        "file_path": "music_files/Midnight_Drive/midnight_drive.mp3",
        "level_path": "levels/Midnight_Drive/level.json"
      },
      {
        "title": "Neon Lights",
        "artist": null,
        "bpm": null,
        "duration": null,
        "bucket_name": "music",
        "file_path": null,
        "level_path": null
      }
    ]
    ```

=== "500 Internal Server Error"
    ```json
    { "detail": "Internal server error" }
    ```

---

## GET /music/{title}

Retourne le détail d'une musique par son titre (clé primaire).

**`GET /api/v1/music/{title}`**

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `title` | string | Titre exact de la musique |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/music/Midnight%20Drive"
    ```

=== "200 OK"
    ```json
    {
      "title": "Midnight Drive",
      "artist": "Synthwave Artist",
      "bpm": 128.0,
      "duration": 213.5,
      "bucket_name": "music",
      "file_path": "music_files/Midnight_Drive/midnight_drive.mp3",
      "level_path": "levels/Midnight_Drive/level.json"
    }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Music not found" }
    ```

---

## GET /music/{title}/download

Télécharge le fichier audio d'une musique. Retourne un `StreamingResponse` avec le contenu binaire du fichier audio depuis MinIO.

**`GET /api/v1/music/{title}/download`**

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `title` | string | Titre exact de la musique |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/music/Midnight%20Drive/download" \
      --output midnight_drive.mp3
    ```

=== "200 OK"
    ```
    Content-Type: audio/mpeg
    Content-Disposition: attachment; filename="midnight_drive.mp3"

    <données binaires>
    ```

=== "400 Bad Request — Pas de fichier"
    ```json
    { "detail": "Music has no associated file" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Music not found" }
    ```

=== "500 Internal Server Error"
    ```json
    { "detail": "Failed to prepare download response: <message>" }
    ```

---

## GET /music/{title}/level

Télécharge le fichier de niveau généré (JSON) pour une musique. Ce fichier est produit par le pipeline Celery après une génération réussie.

**`GET /api/v1/music/{title}/level`**

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `title` | string | Titre exact de la musique |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/music/Midnight%20Drive/level" \
      --output level.json
    ```

=== "200 OK"
    ```
    Content-Type: application/json
    Content-Disposition: attachment; filename="level.json"

    { "bpm": 128.0, "beats": [...], "sections": [...] }
    ```

=== "404 Not Found — Musique inexistante"
    ```json
    { "detail": "Music not found" }
    ```

=== "404 Not Found — Niveau non généré"
    ```json
    { "detail": "Level not generated yet" }
    ```

=== "500 Internal Server Error"
    ```json
    { "detail": "Failed to prepare download response: <message>" }
    ```

---

## POST /music

Crée une nouvelle entrée dans le catalogue musical. Supporte l'upload optionnel d'un fichier audio.

**`POST /api/v1/music`** 🔒 *Authentification requise*

### Body (multipart/form-data)

| Champ | Type | Requis | Description |
|---|---|---|---|
| `title` | string | Oui | Titre unique de la musique |
| `artist` | string | Non | Nom de l'artiste |
| `bpm` | float | Non | Tempo en BPM |
| `duration` | float | Non | Durée en secondes |
| `file` | file | Non | Fichier audio (mp3, wav, ogg, flac) |

**Formats acceptés :** `audio/mpeg`, `audio/mp3`, `audio/wav`, `audio/ogg`, `audio/flac`

**Taille maximale :** 50 Mo

=== "Requête sans fichier"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/music \
      -H "Authorization: Bearer <token>" \
      -F "title=Midnight Drive" \
      -F "artist=Synthwave Artist" \
      -F "bpm=128.0" \
      -F "duration=213.5"
    ```

=== "Requête avec fichier audio"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/music \
      -H "Authorization: Bearer <token>" \
      -F "title=Midnight Drive" \
      -F "artist=Synthwave Artist" \
      -F "file=@/path/to/track.mp3"
    ```

=== "201 Created"
    ```json
    {
      "title": "Midnight Drive",
      "artist": "Synthwave Artist",
      "bpm": 128.0,
      "duration": 213.5,
      "bucket_name": "music",
      "file_path": "music_files/Midnight_Drive/track.mp3",
      "level_path": null
    }
    ```

=== "400 Bad Request — Titre déjà existant"
    ```json
    { "detail": "Music with this title already exists" }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "413 Request Entity Too Large"
    ```json
    { "detail": "Fichier trop volumineux. Limite : 50 Mo." }
    ```

=== "415 Unsupported Media Type"
    ```json
    { "detail": "Format non supporté : audio/aac. Formats acceptés : mp3, wav, ogg, flac." }
    ```

=== "500 Internal Server Error"
    ```json
    { "detail": "Upload failed" }
    ```

---

## PUT /music/{title}

Met à jour les métadonnées d'une musique existante. **Réservé aux administrateurs.**

**`PUT /api/v1/music/{title}`** 🔐 *Admin requis*

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `title` | string | Titre de la musique à mettre à jour |

### Body (application/json)

| Champ | Type | Description |
|---|---|---|
| `artist` | string ou null | Nouveau nom d'artiste |
| `bpm` | float ou null | Nouveau tempo |
| `duration` | float ou null | Nouvelle durée |

=== "Requête"
    ```bash
    curl -X PUT https://pulsedashapi.floabd.app/api/v1/music/Midnight%20Drive \
      -H "Authorization: Bearer <admin_token>" \
      -H "Content-Type: application/json" \
      -d '{"bpm": 130.5, "artist": "Synthwave Pro"}'
    ```

=== "200 OK"
    ```json
    {
      "title": "Midnight Drive",
      "artist": "Synthwave Pro",
      "bpm": 130.5,
      "duration": 213.5,
      "bucket_name": "music",
      "file_path": "music_files/Midnight_Drive/track.mp3",
      "level_path": null
    }
    ```

=== "403 Forbidden"
    ```json
    { "detail": "Accès réservé aux administrateurs" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Music not found" }
    ```

---

## DELETE /music/{title}

Supprime une musique du catalogue. **Réservé aux administrateurs.**

**`DELETE /api/v1/music/{title}`** 🔐 *Admin requis*

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `title` | string | Titre de la musique à supprimer |

=== "Requête"
    ```bash
    curl -X DELETE https://pulsedashapi.floabd.app/api/v1/music/Midnight%20Drive \
      -H "Authorization: Bearer <admin_token>"
    ```

=== "200 OK"
    ```json
    { "status": "deleted", "title": "Midnight Drive" }
    ```

=== "403 Forbidden"
    ```json
    { "detail": "Accès réservé aux administrateurs" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Music not found" }
    ```
