# PulseDash

**PulseDash** est un jeu de rythme 3D où les niveaux sont générés automatiquement à partir de l'analyse audio de tes morceaux. Importe une chanson, et le jeu construit un parcours d'obstacles synchronisé sur la musique — tempo, énergie, beats.

Le projet se compose d'un client Unity et d'un backend FastAPI qui pilote l'analyse audio (Celery + Redis) et le stockage (PostgreSQL + MinIO).

- Client Unity : [`Pulsedash`](https://github.com/floorian651/wavr) (ce dépôt)
- Backend : [`pulsedash_backend`](https://github.com/floorian651/pulsedash_backend)
- Documentation : [floorian651.github.io/pulsedash](https://floorian651.github.io/pulsedash/)

---

## Équipe

| Membre | Rôle |
|---|---|
| Florian Abadie | Git Master , référent backend & intégration Unity |
| Arthur Blamart |Product Owner & Développement Unity |
| Chloé Aubry | Développement Unity |
| Clément Jourdin | Développement Unity |
| Victor Rouet | Développement Unity |
| Quentin Brulé | Développement Unity |
