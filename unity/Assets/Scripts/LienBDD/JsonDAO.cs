using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class JsonDAO : ApiClient
{
    public void FetchLevelFromTitle(string title, Action<MusicData> onReady)
    {
        StartCoroutine(FetchLevel(title, onReady));
    }

    IEnumerator FetchLevel(string title, Action<MusicData> onReady)
    {
        string endpoint = $"{ApiManager.MUSIC_LEVEL}/{Uri.EscapeDataString(title)}/level";
        string fullUrl = ApiManager.GetUrl(endpoint);
        Debug.Log($"[JsonDAO] Appel endpoint : {fullUrl}");

        LevelApiResponse apiResp = null;
        yield return StartCoroutine(GetRequest<LevelApiResponse>(endpoint, (resp, ok) =>
        {
            Debug.Log($"[JsonDAO] Callback: ok={ok}, resp={resp}");
            if (resp != null && resp.meta != null)
                Debug.Log($"[JsonDAO] Réponse valide: bpm={resp.meta.bpm}, duration={resp.meta.duration}, hits={resp.hits?.Length ?? 0}");
            if (ok) apiResp = resp;
        }));
        
        if (apiResp == null || apiResp.meta == null)
        {
            Debug.LogError($"[JsonDAO] Impossible de récupérer les données du niveau pour '{title}'");
            PopupManager.HideLoading(); 
            PopupManager.Show("Le niveau n'est pas encore prêt ou introuvable sur le serveur.");
            onReady?.Invoke(null);
            yield break;
        }

        if (apiResp.hits == null || apiResp.hits.Length == 0)
        {
            Debug.LogError($"[JsonDAO] Données manquantes: hits vide");
            PopupManager.HideLoading();
            PopupManager.Show("Données du niveau incomplètes.");
            onReady?.Invoke(null);
            yield break;
        }

        MusicData musicData = apiResp.ToMusicData();
        Debug.Log($"[JsonDAO] ✓ JSON parsé avec succès. Durée={musicData.duration}s, Tempo={musicData.tempo} bpm, Beats={musicData.beats.Length}");

        PopupManager.HideLoading();
        onReady?.Invoke(musicData);
    }
}