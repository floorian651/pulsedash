---
title: Schéma des données
---

# Schéma des données

Ce document décrit les formats de données produits par le backend et consommés par le client Unity.

## Niveau généré (`level.json`)

Le pipeline d'analyse audio produit un fichier JSON pour chaque morceau traité. Ce fichier est stocké dans MinIO (bucket `levels`) et mis à disposition du client Unity via une URL présignée.

### Structure

| Champ | Type | Description |
|---|---|---|
| `key` | `string` | Tonalité détectée (ex. `A#`, `C`, `F#`) |
| `tempo` | `number` | Tempo en BPM |
| `beats` | `array` | Liste des beats détectés dans la piste |
| `beats[].timing` | `number` | Timestamp du beat en secondes |
| `beats[].puissance` | `number` | Intensité normalisée du beat (0–1) |
| `durée` | `number` | Durée totale de la piste en secondes |

### Exemple

```json
{
  "key": "A#",
  "tempo": 127.4,
  "beats": [
    { "timing": 0.48, "puissance": 0.32 },
    { "timing": 0.96, "puissance": 0.41 },
    { "timing": 1.44, "puissance": 0.38 }
  ],
  "durée": 182.7
}
```

> Les consommateurs Unity doivent ignorer les champs inconnus pour rester compatibles avec les évolutions futures du pipeline.

## Contrat API — Endpoints consommés par Unity

### `POST /api/v1/generate`

Lance la génération d'un niveau à partir d'un identifiant de piste Jamendo.

**Corps de la requête :**

```json
{ "track_id": "1890" }
```

**Réponse :**

```json
{ "job_id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "state": "pending" }
```

### `GET /api/v1/jobs/{job_id}`

Consulte l'état d'un job en cours ou terminé.

**Réponse (job terminé) :**

```json
{
  "job_id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "state": "done",
  "progress": 100,
  "result_url": "http://minio:9000/levels/...?X-Amz-Signature=..."
}
```

| Champ | Valeurs possibles | Description |
|---|---|---|
| `state` | `pending`, `running`, `done`, `failed` | État du job |
| `progress` | `0`–`100` | Avancement en pourcentage |
| `result_url` | URL ou `null` | URL présignée MinIO du `level.json` (disponible si `state=done`) |

## Stockage

| Bucket MinIO | Contenu |
|---|---|
| `audio` | Fichiers audio MP3 téléchargés depuis Jamendo |
| `levels` | Fichiers `level.json` générés par le pipeline |
