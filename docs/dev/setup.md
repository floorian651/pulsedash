# Installation — Client Unity

Ce guide couvre la mise en place de l'environnement de développement pour le **client Unity** (présent dépôt). Pour le backend, consulter le dépôt [floorian651/pulsedash_backend](https://github.com/floorian651/pulsedash_backend).

## Prérequis

| Outil | Version requise | Remarque |
|---|---|---|
| Git | 2.x | Contrôle de version |
| Unity Hub | 3.x | Gestionnaire d'installations Unity |
| Unity Editor | **6000.3.5f1** | Version exacte — requis pour la compatibilité des packages |

## Étape 1 — Cloner le dépôt

```bash
# Via HTTPS
git clone https://github.com/floorian651/wavr.git
cd wavr

# Via SSH
git clone git@github.com:floorian651/wavr.git
cd wavr
```

## Étape 2 — Ouvrir le projet dans Unity Hub

1. Lancer **Unity Hub**.
2. Cliquer sur **Add project from disk**.
3. Sélectionner le dossier `unity/` à la racine du dépôt.
4. Vérifier que l'éditeur **6000.3.5f1** est installé — Unity Hub propose de le télécharger si ce n'est pas le cas.
5. Ouvrir le projet.

Unity importe automatiquement tous les packages déclarés dans `unity/Packages/manifest.json`. La première ouverture peut prendre plusieurs minutes.

## Étape 3 — Lancer le jeu en mode Éditeur

1. Dans le panneau **Project**, ouvrir `Assets/Scenes/Accueil.unity`.
2. Appuyer sur le bouton **Play**.

Pour tester le flux complet (génération de niveau), le backend doit être démarré au préalable. Consulter le guide d'installation du backend.

## Compiler pour la plateforme cible

```
File > Build Settings > sélectionner la plateforme > Build
```

## Vérification de l'installation

Après l'import des packages, la console Unity ne doit pas afficher d'erreurs de compilation. Si des erreurs apparaissent :

- Vérifier que la version de l'éditeur est exactement **6000.3.5f1**.
- Forcer la réimportation des assets : `Assets > Reimport All`.
