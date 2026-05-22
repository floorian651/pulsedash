# Profil

Consultation des profils utilisateurs et de leurs statistiques de jeu agrégées.

**Préfixe :** `/api/v1/profile`

---

## GET /profile/me

Retourne le profil complet et les statistiques de jeu de l'utilisateur actuellement authentifié.

**`GET /api/v1/profile/me`** 🔒 *Authentification requise*

=== "Requête"
    ```bash
    curl https://pulsedashapi.floabd.app/api/v1/profile/me \
      -H "Authorization: Bearer <token>"
    ```

=== "200 OK"
    ```json
    {
      "user_id": "550e8400-e29b-41d4-a716-446655440000",
      "username": "SuperJoueur",
      "member_since": "2024-01-15T09:30:00Z",
      "stats": {
        "total_games": 42,
        "completed_games": 38,
        "total_points": 485200,
        "best_score": 98500,
        "average_accuracy": 0.9412
      }
    }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "User not found" }
    ```

---

## GET /profile/{user_id}

Retourne le profil public et les statistiques de jeu d'un utilisateur par son UUID. Cette route est **publique** (aucune authentification requise).

**`GET /api/v1/profile/{user_id}`**

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `user_id` | string (UUID) | Identifiant unique de l'utilisateur |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/profile/550e8400-e29b-41d4-a716-446655440000"
    ```

=== "200 OK"
    ```json
    {
      "user_id": "550e8400-e29b-41d4-a716-446655440000",
      "username": "SuperJoueur",
      "member_since": "2024-01-15T09:30:00Z",
      "stats": {
        "total_games": 42,
        "completed_games": 38,
        "total_points": 485200,
        "best_score": 98500,
        "average_accuracy": 0.9412
      }
    }
    ```

=== "404 Not Found"
    ```json
    { "detail": "User not found" }
    ```

---

## Schéma de réponse détaillé

### ProfileResponse

| Champ | Type | Description |
|---|---|---|
| `user_id` | string | UUID de l'utilisateur |
| `username` | string ou null | Nom d'affichage (peut être non défini) |
| `member_since` | datetime (ISO 8601) | Date d'inscription |
| `stats` | ProfileStats | Statistiques agrégées |

### ProfileStats

| Champ | Type | Description |
|---|---|---|
| `total_games` | integer | Nombre total de sessions (y compris abandonnées) |
| `completed_games` | integer | Sessions terminées normalement |
| `total_points` | integer | Cumul de tous les scores enregistrés |
| `best_score` | integer ou null | Meilleur score toutes musiques confondues |
| `average_accuracy` | float ou null | Précision moyenne sur les parties complétées (0.0–1.0) |
