# Tracks

Gestion des associations entre playlists et musiques. Un track représente une entrée dans une playlist avec une position optionnelle.

**Préfixe :** `/api/v1/tracks`

---

## GET /tracks

Liste tous les tracks (toutes playlists confondues) avec pagination.

**`GET /api/v1/tracks`**

### Query Parameters

| Paramètre | Type | Défaut | Description |
|---|---|---|---|
| `skip` | integer | `0` | Éléments à ignorer |
| `limit` | integer | `100` | Maximum de résultats |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/tracks"
    ```

=== "200 OK"
    ```json
    [
      { "id": 1, "playlist_name": "Best of Synthwave", "music_title": "Midnight Drive", "position": 1 },
      { "id": 2, "playlist_name": "Best of Synthwave", "music_title": "Neon Lights", "position": 2 },
      { "id": 3, "playlist_name": "Mes favoris", "music_title": "Midnight Drive", "position": null }
    ]
    ```

---

## GET /tracks/playlist/{playlist_name}

Retourne tous les tracks d'une playlist donnée, triés par position.

**`GET /api/v1/tracks/playlist/{playlist_name}`**

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `playlist_name` | string | Nom de la playlist |

!!! note
    Retourne une liste vide `[]` si la playlist n'a pas de tracks ou n'existe pas.

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/tracks/playlist/Best%20of%20Synthwave"
    ```

=== "200 OK"
    ```json
    [
      { "id": 1, "playlist_name": "Best of Synthwave", "music_title": "Midnight Drive", "position": 1 },
      { "id": 2, "playlist_name": "Best of Synthwave", "music_title": "Neon Lights", "position": 2 }
    ]
    ```

=== "200 OK — Playlist vide"
    ```json
    []
    ```

---

## GET /tracks/{track_id}

Retourne un track par son identifiant numérique.

**`GET /api/v1/tracks/{track_id}`**

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `track_id` | integer | Identifiant du track |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/tracks/1"
    ```

=== "200 OK"
    ```json
    { "id": 1, "playlist_name": "Best of Synthwave", "music_title": "Midnight Drive", "position": 1 }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Track not found" }
    ```

---

## POST /tracks

Ajoute une musique dans une playlist (crée un track).

**`POST /api/v1/tracks`** 🔒 *Authentification requise*

### Body (application/json)

| Champ | Type | Requis | Description |
|---|---|---|---|
| `playlist_name` | string | Oui | Nom de la playlist cible (doit exister) |
| `music_title` | string | Oui | Titre de la musique (doit exister) |
| `position` | integer | Non | Position dans la playlist |

=== "Requête"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/tracks \
      -H "Authorization: Bearer <token>" \
      -H "Content-Type: application/json" \
      -d '{
        "playlist_name": "Best of Synthwave",
        "music_title": "Neon Lights",
        "position": 3
      }'
    ```

=== "200 OK"
    ```json
    { "id": 5, "playlist_name": "Best of Synthwave", "music_title": "Neon Lights", "position": 3 }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "404 Not Found — Playlist inexistante"
    ```json
    { "detail": "Playlist not found" }
    ```

=== "404 Not Found — Musique inexistante"
    ```json
    { "detail": "Music not found" }
    ```

---

## PUT /tracks/{track_id}

Met à jour la position d'un track dans sa playlist.

**`PUT /api/v1/tracks/{track_id}`** 🔒 *Authentification requise*

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `track_id` | integer | Identifiant du track |

### Body (application/json)

| Champ | Type | Description |
|---|---|---|
| `position` | integer ou null | Nouvelle position |

=== "Requête"
    ```bash
    curl -X PUT https://pulsedashapi.floabd.app/api/v1/tracks/5 \
      -H "Authorization: Bearer <token>" \
      -H "Content-Type: application/json" \
      -d '{"position": 1}'
    ```

=== "200 OK"
    ```json
    { "id": 5, "playlist_name": "Best of Synthwave", "music_title": "Neon Lights", "position": 1 }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Track not found" }
    ```

---

## DELETE /tracks/{track_id}

Supprime un track d'une playlist.

**`DELETE /api/v1/tracks/{track_id}`** 🔒 *Authentification requise*

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `track_id` | integer | Identifiant du track à supprimer |

=== "Requête"
    ```bash
    curl -X DELETE https://pulsedashapi.floabd.app/api/v1/tracks/5 \
      -H "Authorization: Bearer <token>"
    ```

=== "200 OK"
    ```json
    { "status": "deleted", "track_id": 5 }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Track not found" }
    ```
