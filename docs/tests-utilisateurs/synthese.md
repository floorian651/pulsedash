# Synthèse des tests utilisateurs
## Profil et veille concurentielle

```mermaid
pie title Avez-vous déjà utilisé une plateforme de streaming musicale ?
    "Oui" : 15
    "Non" : 2
```

```mermaid
pie title Avez-vous déjà joué à un jeu vidéo ?
    "Oui" : 17
    "Non" : 0
```

```mermaid
pie title Avez-vous déjà joué à un jeu de rythme ?
    "Oui" : 13
    "Non" : 4
```

Jeux de rythmes connus: GeometryDash, JustDance, Piano tiles, Osu...

Concurrent potentiel: GeometryDash

Mais il y a quelques différences notables:

- Jeu en 2D
- Pas de plateforme de streaming

## Déroulé du test
```mermaid
xychart-beta
    title "Est-ce que notre plateforme de streaming est intuitive ?"
    x-axis ["1", "2", "3", "4"]
    y-axis "Nombre de réponses" 0 --> 8
    bar [3, 8, 3, 3]
```

```mermaid
xychart-beta
    title "Est-ce que la prise en main des commandes du jeu est intuitive ?"
    x-axis ["1", "2", "3", "4", "5"]
    y-axis "Nombre de réponses" 0 --> 6
    bar [5, 4, 4, 3, 1]
```

La plateforme de streaming et le jeu ne sont pas assez intuitifs

- Rendre l’interface de la plateforme de streaming plus claire
- Ajouter une explication des contrôles au début du niveau


## Ressentis de l'utilisateur
```mermaid
xychart-beta
    title "Sur une échelle de 1 à 4, comment avez vous apprecié ce test?"
    x-axis ["1", "2", "3", "4", "5"]
    y-axis "Nombre de réponses" 0 --> 8
    bar [0, 1, 8, 7, 1]
```

Ce que l’utilisateur a moins apprécié:

- Les bugs
- L’interface
- Manque de dynamisme du jeu
- Caméra

Ce que l’utilisateur a apprécié:

- Le concept/principe
- Le gameplay/jeu
- Le personnage
- Les dragons
- Pouvoir choisir sa musique (≠ GeometryDash)