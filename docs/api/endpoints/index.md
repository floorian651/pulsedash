# Vue d'ensemble des endpoints

## URL de base

```
https://pulsedashapi.floabd.app/api/v1
```

---

## Récapitulatif des routes

| Module | Méthode | Route | Auth | Description |
|---|---|---|---|---|
| **Auth** | `POST` | `/auth/register` | Non | Inscription |
| | `POST` | `/auth/login` | Non | Connexion |
| | `POST` | `/auth/refresh` | Non | Rotation des tokens |
| | `POST` | `/auth/logout` | Oui | Révocation des tokens |
| | `GET` | `/auth/me` | Oui | Profil courant |
| **Music** | `GET` | `/music` | Non | Liste des musiques |
| | `GET` | `/music/{title}` | Non | Détail d'une musique |
| | `GET` | `/music/{title}/download` | Non | Télécharger l'audio |
| | `GET` | `/music/{title}/level` | Non | Télécharger le niveau |
| | `POST` | `/music` | Oui | Créer une musique |
| | `PUT` | `/music/{title}` | Admin | Mettre à jour |
| | `DELETE` | `/music/{title}` | Admin | Supprimer |
| **Generate** | `POST` | `/generate` | Oui | Lancer une génération |
| **Jobs** | `GET` | `/jobs/{job_id}` | Oui | Statut d'un job |
| **Jamendo** | `GET` | `/jamendo/search` | Non | Recherche musicale |
| | `POST` | `/jamendo/import/{track_id}` | Oui | Import + génération |
| **Playlists** | `GET` | `/playlists` | Non | Liste des playlists |
| | `GET` | `/playlists/{name}` | Non | Détail d'une playlist |
| | `POST` | `/playlists` | Oui | Créer une playlist |
| | `PUT` | `/playlists/{name}` | Oui | Mettre à jour |
| | `DELETE` | `/playlists/{name}` | Oui | Supprimer |
| **Tracks** | `GET` | `/tracks` | Non | Liste des tracks |
| | `GET` | `/tracks/{track_id}` | Non | Détail d'un track |
| | `GET` | `/tracks/playlist/{playlist_name}` | Non | Tracks d'une playlist |
| | `POST` | `/tracks` | Oui | Ajouter un track |
| | `PUT` | `/tracks/{track_id}` | Oui | Mettre à jour |
| | `DELETE` | `/tracks/{track_id}` | Oui | Supprimer |
| **Scores** | `GET` | `/scores/top` | Non | Leaderboard par musique |
| | `GET` | `/scores/global` | Non | Classement global |
| | `GET` | `/scores/me` | Oui | Mes scores |
| **Game Sessions** | `POST` | `/game-sessions` | Oui | Démarrer une session |
| | `PATCH` | `/game-sessions/{id}/end` | Oui | Terminer une session |
| | `GET` | `/game-sessions/me` | Oui | Mes sessions |
| **Profile** | `GET` | `/profile/me` | Oui | Mon profil + stats |
| | `GET` | `/profile/{user_id}` | Non | Profil public |
| **WebSocket** | `WS` | `/ws/jobs/{job_id}` | Token query | Suivi temps réel |
