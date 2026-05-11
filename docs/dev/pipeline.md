# Pipeline audio

Le pipeline d'analyse audio est intégralement géré côté backend. Le code source, la documentation et les instructions d'exécution sont disponibles dans le dépôt dédié :

[floorian651/pulsedash_backend](https://github.com/floorian651/pulsedash_backend)

## Résumé du fonctionnement

Le pipeline est déclenché par une tâche Celery (`generate_level`) à la suite d'un appel `POST /api/v1/generate`. Il exécute les étapes suivantes :

1. Téléchargement du fichier MP3 depuis l'API Jamendo.
2. Stockage de l'audio dans MinIO (bucket `audio`).
3. Analyse audio via **librosa** : extraction du tempo (BPM), détection des beats, analyse spectrale.
4. Génération du fichier `level.json` structuré.
5. Stockage du niveau dans MinIO (bucket `levels`).
6. Mise à jour de l'état du job dans PostgreSQL (`état=done`, `result_path`).

## Format de sortie

Le fichier `level.json` produit est documenté dans [Schéma des données](data-schema.md).

## Technologies utilisées

| Bibliothèque | Rôle |
|---|---|
| librosa | Analyse audio (tempo, beats, spectral features) |
| numpy | Calcul numérique |
| scipy | Traitement du signal |
