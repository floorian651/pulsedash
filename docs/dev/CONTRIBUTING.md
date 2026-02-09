# Guide de contribution

Ce guide résume le flux de travail attendu pour le code, l'API et la documentation.

## Prérequis

- **Docker & Docker Compose**
- **VS Code + Dev Containers**


## Mise en place
Dev Container

1. Ouvrir le dépôt dans VS Code
2. Command Palette → **Dev Containers: Reopen in Container**

## Conventions Git Flow (1 branche = 1 fonctionnalité)

Nous appliquons un **Git Flow simplifié** : chaque fonctionnalité ou correction se fait dans **une branche dédiée**.

### Règles principales

- **Une branche = une fonctionnalité**
- La branche doit être **courte, ciblée et supprimée après merge**.
- La branche part **toujours de `main`**.
- Utiliser des **commits atomiques** et des messages clairs.

### Nommage des branches

- `feature/<nom-court>` pour une feature
- `fix/<nom-court>` pour un bug
- `docs/<nom-court>` pour la documentation

### Exemple de flux

1. Créer la branche :

  ```bash
  git checkout main
  git pull
  git checkout -b feature/analyse-tempo
  ```

2. Développer et committer :

  ```bash
  git add .
  git commit -m "feat: analyse tempo améliorée"
  ```

3. Pousser la branche :

  ```bash
  git push -u origin feature/analyse-tempo
  ```

4. Ouvrir une Pull Request vers `main`.

5. Après validation : **merge**, puis **suppression de la branche**.

> ℹ️ Si une nouvelle fonctionnalité est demandée, créez **une nouvelle branche** même si vous êtes encore sur une branche existante.

## Toujours mettre à jour son code

Avant de commencer une tâche (ou après une pause), **mettez toujours votre branche à jour** pour éviter les conflits et travailler sur la dernière version.

### Mettre à jour `main`

```bash
git checkout main
git pull
```

### Mettre à jour votre branche de travail

```bash
git checkout feature/ma-fonctionnalite
git pull
```

> ℹ️ Si des conflits apparaissent, résolvez-les puis terminez avec un commit de merge.

### Mettre à jour une branche de feature avec une autre branche

Si votre fonctionnalité dépend d'une autre branche (ex. `feature/refacto-audio`), vous pouvez **fusionner** cette branche dans la vôtre.

Exemple : intégrer `feature/refacto-audio` dans `feature/analyse-tempo` :

```bash
git checkout feature/analyse-tempo
git pull
git merge feature/refacto-audio
```

> ℹ️ En cas de conflits, résolvez-les puis validez le merge avec un commit.

## Lancer le projet

- **API FastAPI** :

  ```bash
  uvicorn src.api.main:app --reload
  ```

- **Pipeline local** :

> i WIP pas encore implémenté

```bash
python -m src.pipeline.main
```

## Qualité de code

Le projet utilise **black**, **isort** et **pre-commit**. Avant de soumettre une PR :

```bash
pre-commit run -a
```

## Documentation

La documentation est dans `docs/` et publiée via MkDocs.

- Modifier ou ajouter des pages dans `docs/`.
- Vérifier localement :

  ```bash
  mkdocs serve
  ```

## Pull Request

1. Pousser votre branche vers le dépôt distant.
2. Ouvrir une **Pull Request** vers `main`.
3. Décrire clairement le changement (contexte, impact, tests effectués).

> ℹ️ **Note :** La fusion déclenche les workflows CI (tests, lint, build docs).
