using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class JsonDAO : ApiClient
{
    private const float POLL_INTERVAL = 0.5f;
    private const float POLL_TIMEOUT = 120f;

    public void FetchLevelFromTitle(string title, Action<int> onProgress, Action<MusicData> onReady)
    {
        StartCoroutine(FetchLevel(title, onProgress, onReady));
    }

    IEnumerator FetchLevel(string title, Action<int> onProgress, Action<MusicData> onReady)
    {
        string endpoint = $"{ApiManager.MUSIC_LEVEL}/{Uri.EscapeDataString(title)}/level";
        Debug.Log($"[JsonDAO] Appel endpoint : {ApiManager.GetUrl(endpoint)}");

        // L'endpoint peut retourner soit le niveau directement, soit un job à poller
        string rawJson = null;
        yield return StartCoroutine(GetRawRequest(endpoint, (raw, ok) =>
        {
            if (ok) rawJson = raw;
        }));

        if (rawJson == null)
        {
            Debug.LogError($"[JsonDAO] Impossible de contacter l'endpoint pour '{title}'");
            PopupManager.HideLoading();
            PopupManager.Show("Erreur : impossible de générer le niveau.");
            onReady?.Invoke(null);
            yield break;
        }

        // Cas 1 : réponse directe (meta + hits)
        LevelApiResponse direct = JsonUtility.FromJson<LevelApiResponse>(rawJson);
        if (direct != null && direct.meta != null && direct.hits != null && direct.hits.Length > 0)
        {
            Debug.Log($"[JsonDAO] Niveau reçu directement ({direct.hits.Length} hits)");
            onProgress?.Invoke(100);
            MusicData musicData = direct.ToMusicData();
            Debug.Log($"[JsonDAO] Niveau prêt! Durée={musicData.duration}s, Tempo={musicData.tempo} bpm, Beats={musicData.beats.Length}");
            onReady?.Invoke(musicData);
            yield break;
        }

        // Cas 2 : réponse de type job — poller /api/v1/jobs/{id}
        JobStatus jobStatus = JsonUtility.FromJson<JobStatus>(rawJson);
        if (jobStatus == null || string.IsNullOrEmpty(jobStatus.EffectiveId))
        {
            Debug.LogError($"[JsonDAO] Réponse inattendue : ni niveau ni job valide\n{rawJson}");
            PopupManager.HideLoading();
            PopupManager.Show("Erreur : réponse serveur invalide.");
            onReady?.Invoke(null);
            yield break;
        }

        Debug.Log($"[JsonDAO] Job créé : {jobStatus.EffectiveId}, state={jobStatus.state}");

        float elapsedTime = 0f;
        while (jobStatus.state != "completed" && jobStatus.state != "failed" && elapsedTime < POLL_TIMEOUT)
        {
            Debug.Log($"[JsonDAO] Job {jobStatus.EffectiveId} en cours... state={jobStatus.state}, progress={jobStatus.progress}%");
            onProgress?.Invoke(jobStatus.progress);

            yield return new WaitForSeconds(POLL_INTERVAL);
            elapsedTime += POLL_INTERVAL;

            JobStatus pollResult = null;
            yield return StartCoroutine(GetRequest<JobStatus>($"{ApiManager.JOBS}/{jobStatus.EffectiveId}", (resp, ok) =>
            {
                if (ok) pollResult = resp;
            }));

            if (pollResult != null)
                jobStatus = pollResult;
            else
                Debug.LogWarning($"[JsonDAO] Erreur lors du polling du job {jobStatus.EffectiveId}");
        }

        if (jobStatus.state == "failed")
        {
            Debug.LogError($"[JsonDAO] Job {jobStatus.EffectiveId} échoué : {jobStatus.error}");
            PopupManager.HideLoading();
            PopupManager.Show("La génération du niveau a échoué.");
            onReady?.Invoke(null);
            yield break;
        }

        if (jobStatus.state != "completed")
        {
            Debug.LogError($"[JsonDAO] Job {jobStatus.EffectiveId} timeout. state={jobStatus.state}");
            PopupManager.HideLoading();
            PopupManager.Show("La génération du niveau a pris trop de temps.");
            onReady?.Invoke(null);
            yield break;
        }

        onProgress?.Invoke(100);

        if (string.IsNullOrEmpty(jobStatus.result_url))
        {
            Debug.LogError($"[JsonDAO] Job terminé mais result_url est vide");
            PopupManager.HideLoading();
            PopupManager.Show("Le niveau généré est invalide.");
            onReady?.Invoke(null);
            yield break;
        }

        // Fetch du JSON depuis l'URL MinIO présignée
        LevelApiResponse levelData = null;
        yield return StartCoroutine(GetAbsoluteUrl<LevelApiResponse>(jobStatus.result_url, (resp, ok) =>
        {
            if (ok) levelData = resp;
        }));

        if (levelData == null || levelData.meta == null || levelData.hits == null || levelData.hits.Length == 0)
        {
            Debug.LogError($"[JsonDAO] Niveau invalide depuis result_url");
            PopupManager.HideLoading();
            PopupManager.Show("Le niveau généré est invalide.");
            onReady?.Invoke(null);
            yield break;
        }

        MusicData data = levelData.ToMusicData();
        Debug.Log($"[JsonDAO] Niveau prêt! Durée={data.duration}s, Tempo={data.tempo} bpm, Beats={data.beats.Length}");
        onReady?.Invoke(data);
    }

    // Retourne le JSON brut pour qu'on puisse choisir comment le désérialiser
    private IEnumerator GetRawRequest(string endpoint, Action<string, bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);
        Debug.Log($"[API GetRawRequest] {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");
            yield return request.SendWebRequest();

            if (request.responseCode == 401)
            {
                UserDAO userDAO = FindObjectOfType<UserDAO>();
                userDAO.RefreshAccessToken(success =>
                {
                    if (success)
                        StartCoroutine(GetRawRequest(endpoint, onResult));
                    else
                        onResult?.Invoke(null, false);
                });
                yield break;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[API GetRawRequest] ERREUR {request.responseCode}: {request.error}");
                onResult?.Invoke(null, false);
                yield break;
            }

            Debug.Log($"[API GetRawRequest] SUCCESS {request.responseCode} ({request.downloadHandler.text.Length} chars)");
            onResult?.Invoke(request.downloadHandler.text, true);
        }
    }

    // Fetch depuis une URL absolue sans auth header (MinIO présigné)
    private IEnumerator GetAbsoluteUrl<T>(string url, Action<T, bool> onResult)
    {
        Debug.Log($"[JsonDAO] Fetch niveau depuis URL absolue");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[JsonDAO] Erreur fetch niveau : {request.responseCode} — {request.error}");
                onResult?.Invoke(default, false);
                yield break;
            }

            Debug.Log($"[JsonDAO] Niveau JSON reçu ({request.downloadHandler.text.Length} chars)");
            onResult?.Invoke(JsonUtility.FromJson<T>(request.downloadHandler.text), true);
        }
    }
}
