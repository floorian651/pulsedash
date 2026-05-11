---
title: Changelog
---

# Changelog

Toutes les modifications notables du projet PulseDash sont documentées ici.

## [Non publié]

### En cours

- Intégration complète du pipeline audio côté backend
- Notifications WebSocket vers Unity à la fin des jobs
- Tests d'intégration des endpoints API

## [0.2.0] - 2026-05-11

### Modifié

- Séparation du projet en deux dépôts indépendants : client Unity (présent dépôt) et backend ([floorian651/pulsedash_backend](https://github.com/floorian651/pulsedash_backend))
- Suppression du backend (API, Celery, Redis, MinIO, pipeline, tests) de ce dépôt
- Mise à jour de la documentation pour refléter l'architecture découplée

## [0.1.0] - 2026-02-02

### Ajouté

- Architecture backend avec FastAPI et Celery
- Pipeline d'analyse rythmique avec librosa (tempo, beats, tonalité)
- Export JSON des résultats d'analyse (`level.json`)
- Infrastructure Docker Compose (Redis, PostgreSQL, MinIO)
- Intégration Jamendo API (téléchargement MP3)
- Documentation MkDocs Material
- Guide de contribution Git Flow
- Dev Container avec Python 3.12

## [0.0.1] - Initialisation

### Ajouté

- Structure initiale du projet Unity
- Configuration de base
- Documentation préliminaire
