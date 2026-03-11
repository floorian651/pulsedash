---
title: Schéma des données
---
TEMPORAIRE A MODIFIER (créé le 02/02/2026)
# Schéma des données

Cette page décrit les formats de données produits et consommés par PulseDash. Le schéma est centré sur l’analyse rythmique et les artefacts JSON exportés.

## Résultat d’analyse (JSON)

Le pipeline produit un fichier JSON (par défaut `resultat/analyse_rythme.json`).

### Structure

- **key** *(string)* : tonalité détectée (ex. `A#`, `C`, `F#`).
- **tempo** *(number)* : tempo en BPM.
- **beats** *(array)* : liste des beats détectés.
  - **timing** *(number)* : timestamp en secondes.
  - **puissance** *(number)* : intensité du beat (0–1 environ, dépend de l’analyse).
- **durée** *(number)* : durée totale de l’audio en secondes.

### Exemple

```json
{
  "key": "A#",
  "tempo": 127.4,
  "beats": [
    {"timing": 0.48, "puissance": 0.32},
    {"timing": 0.96, "puissance": 0.41}
  ],
  "durée": 182.7
}
```

## Réponse API `/analyze`

L’API renvoie un objet enveloppé dans `result` :

```json
{
  "result": {
    "resultat_path": "resultat/analyse_rythme.json",
    "key": "A#",
    "tempo": 127.4,
    "beats": [
      {"timing": 0.48, "puissance": 0.32},
      {"timing": 0.96, "puissance": 0.41}
    ],
    "duree": 182.7
  }
}
```

> ℹ️ Selon l’évolution du pipeline, des champs additionnels peuvent être ajoutés. Les consommateurs Unity doivent ignorer les champs inconnus.

## Stockage

- **MinIO** : fichiers d’entrée/sortie (artefacts d’analyse).
- **PostgreSQL** : métadonnées et état des jobs (prévu).
