---
title: PulseDash
---

# PulseDash — Le jeu

## Concept

PulseDash est un jeu de rythme dans lequel vous contrôlez un personnage à travers des niveaux générés automatiquement à partir de vos morceaux de musique préférés. Chaque chanson devient un parcours unique avec des obstacles, des collectibles et des défis synchronisés sur les temps forts.

## Comment jouer

PulseDash se distribue sous forme de binaire compilé depuis Unity. Téléchargez la dernière version, extrayez l'archive et lancez l'exécutable. Aucune installation supplémentaire n'est requise.

1. Créez vous un compte utilisateur depuis le site suivant : 
2. Téléchargez l'exécutable : Depuis la racine du projet, allez dans unity/Builds puis téléchargez le fichier correspondant à votre système d'exploitation.
3. Dans le dossier que vous venez de télécharger, lancez PulseDash. Vous êtes maintenant sur le jeu !
4. Depuis l'écran d'accueil, connectez-vous avec le compte créé à l'étape 1 (ou autre compte préalablement existant).
5. Sélectionnez un morceau depuis la barre de recherche puis lancez le jeu.
6. Choisissez un mode de jeu.
7. Le backend génère le niveau à partir de l'analyse audio, patientez quelques secondes.
8. Le niveau se charge et la partie commence.
9. Le personnage avance automatiquement  déplacez-vous latéralement et sautez sur les pulsers (dragons).
10. À la fin du parcours, votre score est affiché.

## Gameplay

### Mécaniques principales

- **Course rythmique** : avancez au rythme de la musique
- **Actions temporisées** : sautez, esquivez et collectez sur les beats
- **Combos** : enchaînez les actions parfaites pour augmenter votre score
- **Synchronisation** : plus vous êtes précis, plus vous gagnez de points

### Objectifs

- Terminer le parcours sans perdre toutes vos vies
- Maximiser votre score en restant synchronisé
- Collecter les bonus disséminés sur le chemin
- Battre vos records personnels

## Niveaux

Chaque morceau de musique génère un niveau unique basé sur :

- **Le tempo** : la vitesse du parcours
- **Les beats** : le placement des obstacles et actions
- **L'énergie** : l'intensité des sections (calmes ou intenses)
- **La structure** : les variations entre couplets et refrains

## Modes de jeu

### Lazy

Le mode pour découvrir la musique sans pression. Les obstacles sont rares et espacés, les timings d'évitement des obstacles sont larges. Idéal pour explorer de nouveaux morceaux ou s'échauffer.

- Densité d'obstacles : faible
- Tolérance au timing : large
- Énergie de départ : maximale

### Easy

Le mode standard. Les obstacles suivent fidèlement les beats du morceau, les marges de timing restent confortables. Recommandé pour la majorité des parties.

- Densité d'obstacles : modérée
- Tolérance au timing : normale
- Énergie de départ : maximale

### Crazy

Le mode pour les joueurs expérimentés. Les obstacles sont denses et surviennent sur chaque subdivision rythmique,et les marges de timing sont serrées.

- Densité d'obstacles : élevée
- Tolérance au timing : stricte
- Énergie de départ : réduite

---
