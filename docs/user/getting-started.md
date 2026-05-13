# Premiers pas

Ce guide explique comment lancer PulseDash en local pour la première fois.

## Prérequis

- Unity Hub installé
- Unity Editor **6000.3.5f1** installé via Unity Hub
- Backend PulseDash démarré (voir le [dépôt backend](https://github.com/floorian651/pulsedash_backend))

## Lancer le jeu depuis l'éditeur

1. Cloner le dépôt et ouvrir le dossier `unity/` dans Unity Hub.
2. Une fois le projet ouvert, naviguer dans `Assets/Scenes/` et ouvrir la scène `Accueil`.
3. Appuyer sur **Play**.

## Flux de jeu

1. Depuis l'écran d'accueil, sélectionner un morceau de musique.
2. Le client envoie l'identifiant de la piste au backend, qui génère un niveau JSON.
3. Une fois la génération terminée, le niveau se charge et la partie commence.
4. Le personnage avance automatiquement — utilisez les touches directionnelles pour vous déplacer latéralement et `Espace` pour sauter.
5. Restez synchronisé sur les beats pour maximiser votre score.

## Contrôles

| Action | Touche |
|---|---|
| Déplacement latéral | Flèches gauche / droite |
| Saut | Espace |
