# Auth

Gestion de l'identité : inscription, connexion, renouvellement et révocation des tokens JWT.

**Préfixe :** `/api/v1/auth`

---

## POST /register

Crée un nouveau compte utilisateur et retourne une paire de tokens JWT.

!!! info "Rate limit"
    5 requêtes / minute par IP.

**`POST /api/v1/auth/register`**

### Body (application/json)

| Champ | Type | Requis | Contraintes |
|---|---|---|---|
| `email` | string (email) | Oui | Format email valide |
| `password` | string | Oui | Minimum 8 caractères |
| `username` | string | Non | Doit être unique |

=== "Requête"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/auth/register \
      -H "Content-Type: application/json" \
      -d '{
        "email": "joueur@exemple.com",
        "password": "motdepasse123",
        "username": "SuperJoueur"
      }'
    ```

=== "201 Created"
    ```json
    {
      "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "token_type": "bearer"
    }
    ```

=== "409 Conflict — Email déjà pris"
    ```json
    { "detail": "Email déjà utilisé" }
    ```

=== "409 Conflict — Username déjà pris"
    ```json
    { "detail": "Nom d'utilisateur déjà utilisé" }
    ```

=== "422 Unprocessable Entity"
    ```json
    {
      "detail": [
        {
          "loc": ["body", "password"],
          "msg": "Le mot de passe doit faire au moins 8 caractères",
          "type": "value_error"
        }
      ]
    }
    ```

=== "429 Too Many Requests"
    ```json
    { "error": "Rate limit exceeded: 5 per 1 minute" }
    ```

---

## POST /login

Authentifie un utilisateur existant et retourne une paire de tokens JWT.

!!! info "Rate limit"
    10 requêtes / minute par IP.

**`POST /api/v1/auth/login`**

### Body (application/json)

| Champ | Type | Requis |
|---|---|---|
| `email` | string (email) | Oui |
| `password` | string | Oui |

=== "Requête"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/auth/login \
      -H "Content-Type: application/json" \
      -d '{
        "email": "joueur@exemple.com",
        "password": "motdepasse123"
      }'
    ```

=== "200 OK"
    ```json
    {
      "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "token_type": "bearer"
    }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Identifiants incorrects" }
    ```

=== "403 Forbidden — Compte désactivé"
    ```json
    { "detail": "Compte désactivé" }
    ```

---

## POST /refresh

Échange un refresh token valide contre une nouvelle paire de tokens. **L'ancien refresh token est immédiatement révoqué** (rotation).

!!! info "Rate limit"
    10 requêtes / minute par IP.

**`POST /api/v1/auth/refresh`**

### Body (application/json)

| Champ | Type | Requis |
|---|---|---|
| `refresh_token` | string | Oui |

=== "Requête"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/auth/refresh \
      -H "Content-Type: application/json" \
      -d '{
        "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
      }'
    ```

=== "200 OK"
    ```json
    {
      "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...(nouveau)",
      "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...(nouveau)",
      "token_type": "bearer"
    }
    ```

=== "401 Unauthorized — Token invalide/expiré"
    ```json
    { "detail": "Refresh token invalide" }
    ```

=== "401 Unauthorized — Utilisateur introuvable"
    ```json
    { "detail": "Utilisateur introuvable" }
    ```

!!! warning "Sécurité — Rotation"
    Un refresh token ne peut être utilisé qu'**une seule fois**. Toute tentative de réutilisation d'un token déjà consommé échouera avec `401`.

---

## POST /logout

Révoque simultanément l'access token (via l'en-tête `Authorization`) et le refresh token (via le body). Les deux tokens sont blacklistés dans Redis pour la durée de leur expiration résiduelle.

**`POST /api/v1/auth/logout`** 🔒 *Authentification requise*

### Headers

| En-tête | Valeur |
|---|---|
| `Authorization` | `Bearer <access_token>` |

### Body (application/json)

| Champ | Type | Requis |
|---|---|---|
| `refresh_token` | string | Oui |

=== "Requête"
    ```bash
    curl -X POST https://pulsedashapi.floabd.app/api/v1/auth/logout \
      -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
      -H "Content-Type: application/json" \
      -d '{
        "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
      }'
    ```

=== "200 OK"
    ```json
    { "message": "Déconnecté avec succès" }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

---

## GET /me

Retourne le profil de l'utilisateur actuellement authentifié.

**`GET /api/v1/auth/me`** 🔒 *Authentification requise*

=== "Requête"
    ```bash
    curl https://pulsedashapi.floabd.app/api/v1/auth/me \
      -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    ```

=== "200 OK"
    ```json
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "joueur@exemple.com",
      "username": "SuperJoueur",
      "is_active": true,
      "is_admin": false
    }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```
