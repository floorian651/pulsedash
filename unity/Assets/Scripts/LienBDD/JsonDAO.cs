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

        LevelUrlResponse urlResp = null;
        yield return StartCoroutine(GetRequest<LevelUrlResponse>(endpoint, (resp, ok) =>
        {
            if (ok) urlResp = resp;
        }));

        if (urlResp == null || string.IsNullOrEmpty(urlResp.url))
        {
            Debug.LogError("Impossible de récupérer l'URL du niveau.");
            onReady?.Invoke(null);
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(urlResp.url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur téléchargement niveau : " + request.error);
                onReady?.Invoke(null);
                yield break;
            }

            onReady?.Invoke(JsonUtility.FromJson<MusicData>(request.downloadHandler.text));
        }
    }
}

[System.Serializable]
public class LevelUrlResponse
{
    public string url;
}
