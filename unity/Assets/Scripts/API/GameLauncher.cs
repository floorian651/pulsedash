using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Orchestre le lancement d'une partie :
///   1. Appel API ImportAndGenerate (track Jamendo → analyse audio)
///   2. Polling du job avec mise à jour d'une barre de progression
///   3. Stockage du LevelData dans SessionData
///   4. Chargement de la scène de jeu
///
/// À placer sur un GameObject de la scène menu.
/// Wirer dans l'inspecteur : LoadingOverlay, ProgressBar, StatusText.
/// </summary>
public class GameLauncher : MonoBehaviour
{
    [Header("UI de chargement")]
    [SerializeField] private GameObject loadingOverlay;
    [SerializeField] private Slider     progressBar;
    [SerializeField] private TMP_Text   statusText;

    [Header("Configuration")]
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    // ── Point d'entrée appelé par le bouton "Lancer le jeu" ──────────────────

    public void Launch()
    {
        if (ApiClient.Instance == null)
        {
            PopupManager.Show("ApiClient introuvable dans la scène.");
            return;
        }

        string trackId = SessionData.Instance?.jamendoTrackId;
        if (string.IsNullOrEmpty(trackId))
        {
            PopupManager.Show("Sélectionne une musique Jamendo avant de lancer.");
            return;
        }

        StartCoroutine(LaunchRoutine(trackId));
    }

    // ── Coroutine principale ──────────────────────────────────────────────────

    private IEnumerator LaunchRoutine(string trackId)
    {
        SetOverlay(true, "Préparation...", 0);

        // Étape 1 : import + démarrage de la génération
        ImportAccepted accepted = null;
        string error = null;

        yield return StartCoroutine(ApiClient.Instance.ImportAndGenerate(
            trackId,
            a => accepted = a,
            e => error = e
        ));

        if (error != null) { ShowError(error); yield break; }

        SetStatus("Analyse en cours...", 5);

        // Étape 2 : polling jusqu'à completion
        LevelData levelData = null;

        yield return StartCoroutine(ApiClient.Instance.PollJobUntilDone(
            accepted.job_id,
            progress => SetStatus("Analyse en cours...", progress),
            ld => levelData = ld,
            e => error = e
        ));

        if (error != null) { ShowError(error); yield break; }

        // Étape 3 : stocker les données et charger la scène
        SessionData.Instance.levelData = levelData;
        SessionData.Instance.titre     = accepted.music_title;

        SceneManager.LoadScene(gameplaySceneName);
    }

    // ── Helpers UI ────────────────────────────────────────────────────────────

    private void SetOverlay(bool visible, string status, int progress)
    {
        if (loadingOverlay != null) loadingOverlay.SetActive(visible);
        SetStatus(status, progress);
    }

    private void SetStatus(string status, int progress)
    {
        if (statusText  != null) statusText.text  = status;
        if (progressBar != null) progressBar.value = progress;
    }

    private void ShowError(string message)
    {
        if (loadingOverlay != null) loadingOverlay.SetActive(false);
        PopupManager.Show("Erreur : " + message);
    }
}
