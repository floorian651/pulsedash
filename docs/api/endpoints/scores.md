# Scores

Consultation des scores et classements. Les scores sont créés automatiquement par l'API lors de la clôture d'une session de jeu (via `PATCH /game-sessions/{id}/end`).

**Préfixe :** `/api/v1/scores`

---

## GET /scores/top

Retourne le classement des meilleurs scores pour une musique donnée.

**`GET /api/v1/scores/top`**

### Query Parameters

| Paramètre | Type | Requis | Contraintes | Description |
|---|---|---|---|---|
| `music_title` | string | Oui | — | Titre exact de la musique |
| `limit` | integer | Non | 1–100, défaut `10` | Nombre de résultats |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/scores/top?music_title=Midnight%20Drive&limit=5"
    ```

=== "200 OK"
    ```json
    [
      {
        "rank": 1,
        "user_id": "550e8400-e29b-41d4-a716-446655440000",
        "username": "SuperJoueur",
        "points": 98500,
        "accuracy": 0.9842
      },
      {
        "rank": 2,
        "user_id": "660f9511-f3ac-52e5-b827-557766551111",
        "username": "RythmMaster",
        "points": 95200,
        "accuracy": 0.9601
      },
      {
        "rank": 3,
        "user_id": "770a0622-g4bd-63f6-c938-668877662222",
        "username": null,
        "points": 87000,
        "accuracy": 0.9100
      }
    ]
    ```

=== "200 OK — Aucun score"
    ```json
    []
    ```

---

## GET /scores/global

Retourne le classement global des joueurs, trié par total de points cumulés.

**`GET /api/v1/scores/global`**

### Query Parameters

| Paramètre | Type | Requis | Contraintes | Description |
|---|---|---|---|---|
| `limit` | integer | Non | 1–500, défaut `100` | Nombre de résultats |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/scores/global?limit=10"
    ```

=== "200 OK"
    ```json
    [
      {
        "rank": 1,
        "user_id": "550e8400-e29b-41d4-a716-446655440000",
        "username": "SuperJoueur",
        "total_points": 485200,
        "games_played": 42
      },
      {
        "rank": 2,
        "user_id": "660f9511-f3ac-52e5-b827-557766551111",
        "username": "RythmMaster",
        "total_points": 312000,
        "games_played": 28
      }
    ]
    ```

=== "200 OK — Aucun score"
    ```json
    []
    ```

---

## GET /scores/me

Retourne les scores de l'utilisateur authentifié, du plus récent au plus ancien.

**`GET /api/v1/scores/me`** 🔒 *Authentification requise*

### Query Parameters

| Paramètre | Type | Requis | Contraintes | Description |
|---|---|---|---|---|
| `limit` | integer | Non | 1–200, défaut `50` | Nombre de résultats |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/scores/me?limit=10" \
      -H "Authorization: Bearer <token>"
    ```

=== "200 OK"
    ```json
    [
      {
        "id": 101,
        "user_id": "550e8400-e29b-41d4-a716-446655440000",
        "session_id": "b2c3d4e5-f6a7-8901-bcde-f01234567890",
        "music_title": "Midnight Drive",
        "points": 98500,
        "accuracy": 0.9842,
        "created_at": "2024-05-22T15:30:00Z"
      },
      {
        "id": 87,
        "user_id": "550e8400-e29b-41d4-a716-446655440000",
        "session_id": "c3d4e5f6-a7b8-9012-cdef-012345678901",
        "music_title": "Neon Lights",
        "points": 72000,
        "accuracy": 0.8750,
        "created_at": "2024-05-21T18:45:00Z"
      }
    ]
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

---

## Remarques

!!! info "Création automatique"
    Les scores ne sont pas créés directement via cet endpoint. Ils sont créés automatiquement lors de la clôture d'une session de jeu **non abandonnée** via `PATCH /game-sessions/{id}/end`. Voir [Sessions de jeu](game-sessions.md).

!!! note "Précision de l'accuracy"
    Le champ `accuracy` est un flottant entre `0.0` (0%) et `1.0` (100%). Il représente la précision globale du joueur sur la session.
