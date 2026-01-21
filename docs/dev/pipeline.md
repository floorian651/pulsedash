# Documentation du pipeline Python

Ce document décrit la structure du **pipeline Python** pour l'analyse musicale et l'export des données vers Unity.

---

## Arborescence du pipeline

```text
src/pipeline/
├── __init__.py
├── config.py
├── context.py
├── errors.py
├── pipeline.py
├── loaders/
│   ├── __init__.py
│   ├── audio_loader.py
│   └── metadata_loader.py
├── analyzers/
│   ├── __init__.py
│   ├── tempo.py
│   ├── energy.py
│   ├── spectrum.py
│   ├── onsets.py
│   └── structure.py
├── processors/
│   ├── __init__.py
│   ├── quantize.py
│   ├── smooth.py
│   └── threshold.py
└── exporters/
    ├── __init__.py
    ├── json_exporter.py
    └── binary_exporter.py
```
---
## Description des fichiers
### Fichiers principaux

| Fichier       | Rôle                                                                                                                                              |
| ------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| `__init__.py` | Rend le dossier `pipeline/` importable comme package Python.                                                                                      |
| `config.py`   | Contient les paramètres globaux du pipeline (ex: seuils d’énergie, taille de fenêtre pour l’analyse audio, bpm par défaut, chemins de fichiers…). |
| `context.py`  | Stocke le contexte d’exécution pour chaque morceau analysé (ex: nom du fichier, durée, sample rate).                                              |
| `errors.py`   | Définition des exceptions personnalisées pour gérer les erreurs spécifiques au pipeline.                                                          |
| `pipeline.py` | Orchestrateur principal : charge les données, exécute les analyses, applique les traitements, et exporte les résultats pour Unity.                |

### Dossier loaders/

| Fichier              | Rôle                                                                                              |
| -------------------- | ------------------------------------------------------------------------------------------------- |
| `__init__.py`        | Rend le sous-dossier `loaders/` importable comme package.                                         |
| `audio_loader.py`    | Charge les fichiers audio (`.wav`, `.mp3`) et renvoie les samples + sample rate.                  |
| `metadata_loader.py` | Charge des métadonnées supplémentaires pour le morceau (BPM connu, marqueurs, titres, artistes…). |

---

### Dossier analyzers/

| Fichier        | Rôle                                                                                    |
| -------------- | --------------------------------------------------------------------------------------- |
| `__init__.py`  | Rend le sous-dossier `analyzers/` importable comme package.                             |
| `tempo.py`     | Analyse la musique pour détecter le BPM et la grille des beats.                         |
| `energy.py`    | Calcule l’énergie (RMS, loudness) de la musique sur le temps.                           |
| `spectrum.py`  | Analyse spectrale : découpe les fréquences en bass, mid, high pour la lecture gameplay. |
| `onsets.py`    | Détecte les transitoires ou "onsets" (pics) dans la musique.                            |
| `structure.py` | Détecte les sections musicales (intro, build, drop, break) pour structurer le gameplay. |

---

### Dossier processors/

| Fichier        | Rôle                                                                                                        |
| -------------- | ----------------------------------------------------------------------------------------------------------- |
| `__init__.py`  | Rend le sous-dossier `processors/` importable comme package.                                                |
| `quantize.py`  | Aligne les événements sur la grille rythmique ou sur le BPM détecté.                                        |
| `smooth.py`    | Lisse les courbes (énergie, fréquence) pour éviter des variations trop brusques.                            |
| `threshold.py` | Détecte les événements importants pour le gameplay (ex: seuil d’énergie déclenchant un ennemi ou un effet). |

---

### Dossier exporters/

| Fichier              | Rôle                                                                                                  |
| -------------------- | ----------------------------------------------------------------------------------------------------- |
| `__init__.py`        | Rend le sous-dossier `exporters/` importable comme package.                                           |
| `json_exporter.py`   | Exporte les données analysées et traitées dans un format JSON lisible par Unity (`StreamingAssets/`). |
| `binary_exporter.py` | Optionnel : export en binaire pour améliorer la performance ou réduire la taille des fichiers.        |

---

## Exemple de flux d’analyse

- pipeline.py charge un fichier audio avec audio_loader.py.

- Les analyzers (tempo.py, energy.py, onsets.py, etc.) extraient les features musicales.

- Les processors (quantize.py, threshold.py) préparent les données pour le gameplay.

- Les exporters (json_exporter.py) écrivent les fichiers dans unity/Assets/StreamingAssets/.

- Unity lit la timeline et déclenche les événements du gameplay.
