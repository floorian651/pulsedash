# Guide de contribution

Ce guide couvre le flux de travail attendu pour contribuer au **client Unity** (présent dépôt). Pour le backend, consulter le dépôt [floorian651/pulsedash_backend](https://github.com/floorian651/pulsedash_backend).

## Prérequis

- Git 2.x
- Unity Hub + Unity Editor 6000.3.5f1

## Conventions Git Flow (1 branche = 1 fonctionnalité)

Chaque fonctionnalité ou correction se fait dans **une branche dédiée** partant de `main`.

### Nommage des branches

| Préfixe | Usage |
|---|---|
| `feature/<nom-court>` | Nouvelle fonctionnalité |
| `fix/<nom-court>` | Correction de bug |
| `docs/<nom-court>` | Documentation |
| `chore/<nom-court>` | Maintenance, nettoyage |

### Exemple de flux

```bash
# 1. Partir de main à jour
git checkout main
git pull

# 2. Créer la branche
git checkout -b feature/obstacle-rythme

# 3. Développer et committer
git add unity/Assets/Scripts/MonScript.cs
git commit -m "feat: synchroniser les obstacles sur les beats"

# 4. Pousser la branche
git push -u origin feature/obstacle-rythme

# 5. Ouvrir une Pull Request vers main
```

Après validation et merge, supprimer la branche.

## Toujours mettre à jour son code

Avant de commencer une tâche, synchroniser avec `main` :

```bash
git checkout main
git pull

git checkout feature/ma-fonctionnalite
git pull
```

En cas de conflits sur des fichiers `.unity` ou `.asset`, les résoudre manuellement dans l'éditeur Unity.

## Qualité du code C#

- Nommer les scripts en **PascalCase**, les variables privées en **camelCase** avec underscore (`_maVariable`).
- Un `MonoBehaviour` par fichier.
- Ne pas laisser de `Debug.Log` en production.

## Documentation

La documentation est dans `docs/` et publiée via MkDocs. Pour vérifier localement :

```bash
pip install -r docs/requirements_doc.txt
mkdocs serve
```

## Pull Request

1. Pousser la branche vers le dépôt distant.
2. Ouvrir une **Pull Request** vers `main`.
3. Décrire le changement (contexte, impact, scènes ou scripts modifiés).

La fusion déclenche les workflows CI (lint, build docs).
