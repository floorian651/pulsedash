# Modèles de données

Cette section détaille les schémas Pydantic (validation I/O de l'API) et les modèles SQLAlchemy (persistance en base de données).

---

## Auth

### RegisterRequest

```python
class RegisterRequest(BaseModel):
    email: EmailStr          # Format email valide, requis
    password: str            # Min 8 caractères, requis
    username: str | None     # Optionnel, doit être unique
```

### LoginRequest

```python
class LoginRequest(BaseModel):
    email: EmailStr
    password: str
```

### TokenResponse

```python
class TokenResponse(BaseModel):
    access_token: str        # JWT valide 60 minutes
    refresh_token: str       # JWT valide 30 jours
    token_type: str          # Toujours "bearer"
```

### RefreshRequest

```python
class RefreshRequest(BaseModel):
    refresh_token: str
```

### UserProfile

```python
class UserProfile(BaseModel):
    id: str                  # UUID de l'utilisateur
    email: str
    username: str | None
    is_active: bool
    is_admin: bool
```

---

## Music

### MusicResponse

```python
class MusicResponse(BaseModel):
    title: str               # Clé primaire
    artist: Optional[str]
    bpm: Optional[float]
    duration: Optional[float]  # Durée en secondes
    bucket_name: str         # Nom du bucket MinIO ("music")
    file_path: Optional[str] # Chemin objet dans MinIO
    level_path: Optional[str] # Chemin du niveau généré dans MinIO
```

### MusicUpdate

```python
class MusicUpdate(BaseModel):
    artist: Optional[str] = None
    bpm: Optional[float] = None
    duration: Optional[float] = None
```

---

## Generate

### GenerateRequest

```python
class GenerateRequest(BaseModel):
    music_title: str         # Titre exact de la musique à traiter
```

### GenerateAccepted

```python
class GenerateAccepted(BaseModel):
    job_id: str              # UUID du job créé
    state: str               # "pending"
```

---

## Jobs

Réponse directe du router (dict) :

| Champ | Type | Description |
|---|---|---|
| `job_id` | string | UUID du job |
| `state` | string | `pending`, `running`, `completed`, `failed` |
| `progress` | integer | 0–100 |
| `result_url` | string ou null | URL présignée MinIO si `completed` |
| `error` | string ou null | Message d'erreur si `failed` |

---

## Jamendo

### JamendoTrack

```python
class JamendoTrack(BaseModel):
    id: str                  # Identifiant Jamendo
    name: str                # Titre du morceau
    artist_name: str
    duration: int            # Durée en secondes
    image: Optional[str]     # URL de la pochette
    audio: Optional[str]     # URL de streaming
```

### ImportAccepted

```python
class ImportAccepted(BaseModel):
    job_id: str
    music_title: str         # Titre détecté depuis Jamendo
    state: str               # "pending"
```

---

## Playlists

### PlaylistCreate

```python
class PlaylistCreate(BaseModel):
    name: str                # Unique, requis
    description: Optional[str] = None
```

### PlaylistUpdate

```python
class PlaylistUpdate(BaseModel):
    description: Optional[str] = None
```

### PlaylistResponse

```python
class PlaylistResponse(BaseModel):
    name: str
    description: Optional[str]
    created_at: datetime
    tracks: List[TrackInPlaylist] = []
```

### TrackInPlaylist

```python
class TrackInPlaylist(BaseModel):
    id: int
    music_title: str
    position: Optional[int]
```

---

## Tracks

### TrackCreate

```python
class TrackCreate(BaseModel):
    playlist_name: str       # Doit référencer une playlist existante
    music_title: str         # Doit référencer une musique existante
    position: Optional[int] = None
```

### TrackUpdate

```python
class TrackUpdate(BaseModel):
    position: Optional[int] = None
```

### TrackResponse

```python
class TrackResponse(BaseModel):
    id: int
    playlist_name: str
    music_title: str
    position: Optional[int]
```

---

## Scores

### ScoreResponse

```python
class ScoreResponse(BaseModel):
    id: int
    user_id: str             # UUID de l'utilisateur
    session_id: Optional[str] # UUID de la session associée
    music_title: str
    points: int
    accuracy: Optional[float]  # 0.0–1.0
    created_at: datetime
```

### LeaderboardEntry

```python
class LeaderboardEntry(BaseModel):
    rank: int                # Position dans le classement (1-based)
    user_id: str
    username: Optional[str]
    points: int
    accuracy: Optional[float]
```

### GlobalLeaderboardEntry

```python
class GlobalLeaderboardEntry(BaseModel):
    rank: int
    user_id: str
    username: Optional[str]
    total_points: int        # Cumul de tous les scores
    games_played: int        # Nombre de parties complétées
```

---

## Game Sessions

### GameSessionStart

```python
class GameSessionStart(BaseModel):
    music_title: str
```

### GameSessionEnd

```python
class GameSessionEnd(BaseModel):
    final_score: int         # >= 0, requis
    accuracy: Optional[float] = None  # 0.0–1.0
    abandoned: bool = False
```

### GameSessionResponse

```python
class GameSessionResponse(BaseModel):
    id: str                  # UUID
    user_id: str
    music_title: str
    status: str              # "active", "completed", "abandoned"
    started_at: datetime
    ended_at: Optional[datetime]
    final_score: Optional[int]
    accuracy: Optional[float]
```

---

## Profile

### ProfileStats

```python
class ProfileStats(BaseModel):
    total_games: int
    completed_games: int
    total_points: int
    best_score: Optional[int]
    average_accuracy: Optional[float]  # Arrondi à 4 décimales
```

### ProfileResponse

```python
class ProfileResponse(BaseModel):
    user_id: str
    username: Optional[str]
    member_since: datetime
    stats: ProfileStats
```

---

## Modèles SQLAlchemy (ORM)

### User

| Colonne | Type SQL | Contraintes |
|---|---|---|
| `id` | VARCHAR | PK, UUID v4 auto-généré |
| `username` | VARCHAR | UNIQUE, nullable |
| `email` | VARCHAR | UNIQUE, NOT NULL |
| `password` | VARCHAR | nullable (compte OAuth possible) |
| `is_active` | BOOLEAN | défaut `true` |
| `is_admin` | BOOLEAN | défaut `false`, NOT NULL |
| `created_at` | TIMESTAMP WITH TZ | défaut `now()` UTC |

### Music

| Colonne | Type SQL | Contraintes |
|---|---|---|
| `title` | VARCHAR | PK |
| `artist` | VARCHAR | nullable |
| `bpm` | FLOAT | nullable |
| `duration` | FLOAT | nullable |
| `bucket_name` | VARCHAR | défaut `"musics"` |
| `file_path` | VARCHAR | nullable |
| `level_path` | VARCHAR | nullable |

### Job

| Colonne | Type SQL | Contraintes |
|---|---|---|
| `id` | VARCHAR | PK (UUID) |
| `user_id` | VARCHAR | FK → users.id |
| `state` | VARCHAR / ENUM | `pending`, `running`, `completed`, `failed` |
| `progress` | INTEGER | 0–100 |
| `result_path` | VARCHAR | nullable |
| `error_message` | VARCHAR | nullable |

### GameSession

| Colonne | Type SQL | Contraintes |
|---|---|---|
| `id` | VARCHAR | PK (UUID) |
| `user_id` | VARCHAR | FK → users.id |
| `music_title` | VARCHAR | FK → music.title |
| `status` | VARCHAR | `active`, `completed`, `abandoned` |
| `started_at` | TIMESTAMP | NOT NULL |
| `ended_at` | TIMESTAMP | nullable |
| `final_score` | INTEGER | nullable |
| `accuracy` | FLOAT | nullable |

### Score

| Colonne | Type SQL | Contraintes |
|---|---|---|
| `id` | SERIAL | PK |
| `user_id` | VARCHAR | FK → users.id |
| `session_id` | VARCHAR | FK → game_sessions.id, nullable |
| `music_title` | VARCHAR | FK → music.title |
| `points` | INTEGER | NOT NULL |
| `accuracy` | FLOAT | nullable |
| `created_at` | TIMESTAMP | NOT NULL |

### Playlist

| Colonne | Type SQL | Contraintes |
|---|---|---|
| `name` | VARCHAR | PK |
| `description` | VARCHAR | nullable |
| `created_at` | TIMESTAMP WITH TZ | NOT NULL |

### Track

| Colonne | Type SQL | Contraintes |
|---|---|---|
| `id` | SERIAL | PK |
| `playlist_name` | VARCHAR | FK → playlists.name |
| `music_title` | VARCHAR | FK → music.title |
| `position` | INTEGER | nullable |
