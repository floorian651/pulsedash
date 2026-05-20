# TODO — PulseDash (client Unity)

> Tâches atomiques, chaque tâche = une responsabilité.
> Généré le 2026-05-20.

---

## P0 — Critique

---

### [ ] T-01 — Supprimer les Debug.Log contenant des données sensibles

**Objectif** : Éliminer les logs qui exposent les tokens JWT et les corps de requêtes en clair.

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/ApiClient.cs` (lignes 14–15, 35)
- `unity/Assets/Scripts/LienBDD/UserDAO.cs` (lignes 53, 59, 64)

**Risques** : Logs visibles dans des builds de dev, fuite de tokens JWT dans des environnements partagés.

**Dépendances** : Aucune.

**Critères de validation** :
- Aucun `Debug.Log` n'affiche `url`, `jsonBody`, `access_token` ou `refresh_token`.
- Le build compile sans erreur.

---

### [ ] T-02 — Supprimer tous les Debug.Log restants (hors logs d'erreur)

**Objectif** : Respecter la convention CLAUDE.md « remove all Debug.Log calls before merging to main ».

**Fichiers impactés** :
- `unity/Assets/Scripts/GenerationNiveau/GenerationNiveau.cs`
- `unity/Assets/Scripts/SessionData.cs`
- `unity/Assets/Scripts/LienBDD/JsonDAO.cs`
- `unity/Assets/Scripts/LienBDD/MusicDAO.cs`
- `unity/Assets/Scripts/PlaylistManager.cs`
- `unity/Assets/Scripts/Player/PlayerCollision.cs`
- `unity/Assets/Scripts/FinishText.cs`

**Risques** : Performances légèrement impactées en build debug.

**Dépendances** : T-01 (faire T-01 d'abord pour s'assurer de ne pas supprimer des logs d'erreur utiles).

**Critères de validation** :
- Grep `Debug.Log` ne retourne aucun résultat dans `Assets/Scripts/` (hors `Debug.LogError` et `Debug.LogWarning` légitimes).

---

### [x] T-03 — Unifier la gestion de l'URL de base API

**Objectif** : Un seul point de configuration pour l'URL du backend, utilisé par tous les DAOs.

**Contexte** : `ApiManager.cs` hardcode `https://pulsedashapi.floabd.app` ; `DotEnv.cs` lit une variable d'environnement `API_URL`. `MusicDAO` et `JsonDAO` utilisent `DotEnv`, `UserDAO`/`ApiClient` utilisent `ApiManager`. Incohérence totale.

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/Managers/ApiManager.cs` (source de vérité à conserver)
- `unity/Assets/Scripts/LienBDD/DotEnv.cs` (à supprimer ou rediriger vers ApiManager)
- `unity/Assets/Scripts/LienBDD/JsonDAO.cs` (remplacer `DotEnv.GetURL()` par `ApiManager.GetUrl(...)`)
- `unity/Assets/Scripts/LienBDD/MusicDAO.cs` (idem)

**Risques** : Si `DotEnv` est utilisé ailleurs, risque de compilation cassée.

**Dépendances** : Aucune.

**Critères de validation** :
- `grep -r "DotEnv.GetURL"` ne retourne plus aucun résultat.
- Tous les DAOs passent par `ApiManager.GetUrl(endpoint)`.
- Le build compile.

---

### [x] T-04 — Brancher `JsonDAO` dans `GenerationNiveau` pour charger le JSON depuis le backend

**Objectif** : `GenerationNiveau.cs` doit récupérer `level.json` via le pipeline backend (POST `/generate` + polling GET) au lieu de lire un fichier local.

**Contexte** : Deux commentaires `// A MODIFIER POUR LA BDD` dans `GenerationNiveau.cs` (lignes 124 et 151). `JsonDAO.cs` implémente déjà le flux POST + polling mais n'est pas branché.

**Fichiers impactés** :
- `unity/Assets/Scripts/GenerationNiveau/GenerationNiveau.cs`
- `unity/Assets/Scripts/LienBDD/JsonDAO.cs`

**Risques** :
- Le polling (`WaitForSeconds(2f)`) bloque le démarrage de la scène — nécessite un état de chargement.
- `JsonDAO.SaveJson` sauvegarde dans `Application.dataPath` (ne fonctionne pas en build Android/iOS sans adaptations).
- Le champ `level` dans `GenerateResult` est un `string`, pas désérialisé en `MusicData`.

**Dépendances** : T-03 (URLs unifiées), T-05 (désérialisation `GenerateResult` → `MusicData`).

**Critères de validation** :
- En lançant une GameplayScene, le niveau est généré à partir du JSON retourné par l'API (vérifié avec proxy ou logs backend).
- Aucun fichier JSON local n'est nécessaire dans `Resources/JSON/`.
- Le jeu attend la réponse backend avant de lancer la génération.

---

### [x] T-05 — Corriger la désérialisation de `GenerateResult.level` en `MusicData`

**Objectif** : Le champ `level` dans `GenerateResult` est déclaré `string`, mais doit être une `MusicData` (ou désérialisé en deux étapes).

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/JsonDAO.cs`
- `unity/Assets/Scripts/GenerationNiveau/MusicData.cs`

**Risques** : `JsonUtility.FromJson` de Unity ne supporte pas les types imbriqués polymorphes.

**Dépendances** : Aucune (peut être fait en parallèle de T-04).

**Critères de validation** :
- `GenerateResult` expose une `MusicData` utilisable directement par `GenerationNiveau`.
- Test avec un JSON de réponse backend réel.

---

### [x] T-06 — Envoyer le score final au backend

**Objectif** : Après la collision avec la Finish Line, POST le score calculé vers l'API.

**Contexte** : `PlayerCollision.cs` ligne 71 contient un commentaire explicite. Le score est calculé (`energy / maxEnergy * 100`) mais jamais transmis.

**Fichiers impactés** :
- `unity/Assets/Scripts/Player/PlayerCollision.cs`
- `unity/Assets/Scripts/LienBDD/UserDAO.cs` (ajouter méthode `SubmitScore`)
- `unity/Assets/Scripts/LienBDD/Managers/ApiManager.cs` (ajouter constante endpoint scores)
- `unity/Assets/Scripts/LienBDD/Models/AuthModels.cs` (ajouter `ScoreRequest`, `ScoreResponse`)

**Risques** : L'endpoint de score n'est pas documenté dans ce repo — vérifier le contrat API backend avant d'implémenter.

**Dépendances** : Auth JWT fonctionnel (déjà fait), T-03.

**Critères de validation** :
- Après `FinishScene`, un appel POST visible dans les logs backend avec le score et le titre de la musique.
- En cas d'échec réseau, le jeu affiche un message d'erreur et ne crashe pas.

---

## P1 — Important

---

### [x] T-07 — Corriger le bug `static maxEnergy` dans `Player`

**Objectif** : `maxEnergy` est `static` dans `Player.cs`, ce qui partage la valeur entre toutes les instances — comportement incorrect si jamais plus d'une instance est créée.

**Fichiers impactés** :
- `unity/Assets/Scripts/Player/Player.cs`

**Risques** : Changement de visibilité peut casser des accès externes `Player.maxEnergy` (vérifier les usages).

**Dépendances** : Aucune.

**Critères de validation** :
- `maxEnergy` est un champ d'instance (`private float`).
- `GetMaxEnergyLevel()` retourne la valeur d'instance.
- Grep confirme qu'aucun accès externe statique `Player.maxEnergy` ne subsiste.

---

### [x] T-08 — Implémenter gestion d'erreur réseau visible utilisateur

**Objectif** : Quand une requête échoue (timeout, 5xx, pas de réseau), afficher un message clair à l'utilisateur au lieu d'un `Debug.LogError` silencieux.

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/ApiClient.cs`
- `unity/Assets/Scripts/BoutonAction/ActionsBouton.cs`

**Risques** : Nécessite de définir une stratégie de retry (combien de fois, délai).

**Dépendances** : T-01, T-02.

**Critères de validation** :
- Connexion sans réseau → popup "Serveur inaccessible, vérifiez votre connexion".
- Timeout API → popup avec message d'erreur, pas de freeze infini.

---

### [ ] T-09 — Déconnexion automatique si refresh token expiré

**Objectif** : Quand `RefreshAccessToken` échoue, rediriger vers `PageConnexion` au lieu de simplement logger l'erreur.

**Contexte** : `UserDAO.cs` appelle `TokenManager.Clear()` mais ne navigue pas vers la page de connexion.

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/UserDAO.cs`
- `unity/Assets/Scripts/LienBDD/ApiClient.cs`

**Risques** : La redirection de scène depuis un DAO rompt la séparation des responsabilités — préférer un event/callback.

**Dépendances** : T-01.

**Critères de validation** :
- Token refresh expiré → scène `PageConnexion` chargée automatiquement.
- `PlayerPrefs` vidés (tokens supprimés).

---

### [x] T-10 — Synchroniser les playlists avec le backend

**Objectif** : Remplacer la persistence locale (`playlists.json`) par des appels API CRUD.

**Contexte** : `PlaylistManager.cs` a ~8 commentaires `// Modifier pour la BDD`. Les méthodes sont déjà structurées correctement.

**Fichiers impactés** :
- `unity/Assets/Scripts/PlaylistManager.cs`
- `unity/Assets/Scripts/LienBDD/MusicDAO.cs` (ajouter endpoints playlists)
- `unity/Assets/Scripts/LienBDD/Managers/ApiManager.cs` (ajouter constantes endpoints)

**Risques** : Latence réseau dans les callbacks UI ; risque de désync si réseau coupé.

**Dépendances** : Auth JWT (fait), T-03, contrat API backend playlists à confirmer.

**Critères de validation** :
- Création/suppression playlist visible dans la BDD backend.
- Rechargement de l'app conserve les playlists sans fichier local.

---

### [ ] T-11 — Réactiver la recherche musicale Jamendo

**Objectif** : Débloquer `searchMusic` et `charger` dans `MusicDAO.cs` (actuellement commentés) pour permettre la recherche et l'import de musiques.

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/MusicDAO.cs`
- `unity/Assets/Scripts/UI/SearchUI.cs`

**Risques** : Les méthodes utilisaient `JsonNode` (API non disponible dans Unity) — nécessite adaptation à `JsonUtility` ou Newtonsoft.

**Dépendances** : T-03.

**Critères de validation** :
- Une recherche par titre retourne une liste de pistes.
- L'import déclenche le pipeline backend.

---

## P2 — Amélioration

---

### [ ] T-12 — Ajouter sélection de difficulté depuis l'UI

**Objectif** : Permettre au joueur de choisir Lazy/Easy/Crazy avant de lancer une musique.

**Fichiers impactés** :
- `unity/Assets/Scripts/SessionData.cs`
- Scène `Platform_Streaming` (UI à créer/modifier)

**Risques** : Dépend de l'UI de la scène Platform_Streaming qui n'a pas de script dédié visible.

**Dépendances** : Aucune.

**Critères de validation** :
- Le mode sélectionné est bien lu dans `GenerationNiveau.Start()`.

---

### [ ] T-13 — Ajouter un écran de chargement pendant le polling backend

**Objectif** : Pendant le polling de `JsonDAO`, afficher un spinner/loading screen au lieu de bloquer la scène.

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/JsonDAO.cs`
- Scène de transition à créer ou overlay dans `GameplayScene`

**Risques** : Complexité UX — nécessite coordination avec la scène de gameplay.

**Dépendances** : T-04.

**Critères de validation** :
- Lancement d'une musique → spinner visible jusqu'à réception du `level.json`.

---

### [ ] T-14 — Sécuriser le stockage des tokens JWT

**Objectif** : Les tokens JWT stockés en clair dans `PlayerPrefs` sont lisibles sur les plateformes non sécurisées.

**Fichiers impactés** :
- `unity/Assets/Scripts/LienBDD/Managers/TokenManager.cs`

**Risques** : Chiffrement Unity côté PlayerPrefs est limité — évaluer si le niveau de risque justifie la complexité.

**Dépendances** : Aucune.

**Critères de validation** :
- Les tokens ne sont plus lisibles en clair dans le registre Windows ou les plist iOS.
