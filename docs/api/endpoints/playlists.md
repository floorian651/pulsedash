# Playlists

Gestion des playlists musicales. Chaque playlist regroupe des tracks (associations musique + position).

**Préfixe :** `/api/v1/playlists`

---

## GET /playlists

Liste toutes les playlists avec leurs tracks inclus.

**`GET /api/v1/playlists`**

### Query Parameters

| Paramètre | Type | Défaut | Description |
|---|---|---|---|
| `skip` | integer | `0` | Pagination : éléments à ignorer |
| `limit` | integer | `100` | Pagination : max de résultats |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/playlists?limit=10"
    ```

=== "200 OK"
    ```json
    [
      {
        "name": "Best of Synthwave",
        "description": "Les meilleurs morceaux synthwave",
        "created_at": "2024-03-15T14:30:00Z",
        "tracks": [
          { "id": 1, "music_title": "Midnight Drive", "position": 1 },
          { "id": 2, "music_title": "Neon Lights", "position": 2 }
        ]
      }
    ]
    ```

---

## GET /playlists/{name}

Retourne une playlist par son nom avec tous ses tracks.

**`GET /api/v1/playlists/{name}`**

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `name` | string | Nom exact de la playlist |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/playlists/Best%20of%20Synthwave"
    ```

=== "200 OK"
    ```json
    {
      "name": "Best of Synthwave",
      "description": "Les meilleurs morceaux synthwave",
      "created_at": "2024-03-15T14:30:00Z",
      "tracks": [
        { "id": 1, "music_title": "Midnight Drive", "position": 1 },
        { "id": 2, "music_title": "Neon Lights", "position": 2 }
      ]
    }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Playlist not found" }
    ```

---

## POST /playlists

Crée une nouvelle playlist.

**`POST /api/v1/playlists`** 🔒 *Authentification requise*

### Body (application/json)

| Champ | Type | Requis | Description |
|---|---|---|---|
| `name` | string | Oui | Nom unique de la playlist |
| `description` | string | Non | Description libre |

=== "Requête"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/playlists \
      -H "Authorization: Bearer <token>" \
      -H "Content-Type: application/json" \
      -d '{"name": "Mes favoris", "description": "Ma sélection personnelle"}'
    ```

=== "200 OK"
    ```json
    {
      "name": "Mes favoris",
      "description": "Ma sélection personnelle",
      "created_at": "2024-05-22T10:00:00Z",
      "tracks": []
    }
    ```

=== "400 Bad Request — Nom déjà utilisé"
    ```json
    { "detail": "Playlist with this name already exists" }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

---

## PUT /playlists/{name}

Met à jour la description d'une playlist existante.

**`PUT /api/v1/playlists/{name}`** 🔒 *Authentification requise*

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `name` | string | Nom de la playlist à modifier |

### Body (application/json)

| Champ | Type | Description |
|---|---|---|
| `description` | string ou null | Nouvelle description |

=== "Requête"
    ```bash
    curl -X PUT "https://pulsedashapi.floabd.app/api/v1/playlists/Mes%20favoris" \
      -H "Authorization: Bearer <token>" \
      -H "Content-Type: application/json" \
      -d '{"description": "Ma sélection mise à jour"}'
    ```

=== "200 OK"
    ```json
    {
      "name": "Mes favoris",
      "description": "Ma sélection mise à jour",
      "created_at": "2024-05-22T10:00:00Z",
      "tracks": [...]
    }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Playlist not found" }
    ```

---

## DELETE /playlists/{name}

Supprime une playlist et tous ses tracks associés (cascade).

**`DELETE /api/v1/playlists/{name}`** 🔒 *Authentification requise*

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `name` | string | Nom de la playlist à supprimer |

=== "Requête"
    ```bash
    curl -X DELETE "https://pulsedashapi.floabd.app/api/v1/playlists/Mes%20favoris" \
      -H "Authorization: Bearer <token>"
    ```

=== "200 OK"
    ```json
    { "status": "deleted", "name": "Mes favoris" }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Playlist not found" }
    ```
