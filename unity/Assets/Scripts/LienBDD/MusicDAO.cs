using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class MusicDAO : ApiClient
{
    [Serializable]
    private class JamendoListWrapper { public JamendoTrack[] items; }

    public void SearchJamendo(string query, Action<JamendoTrack[]> onResult)
    {
        StartCoroutine(FetchJamendoSearch(query, onResult));
    }

    IEnumerator FetchJamendoSearch(string query, Action<JamendoTrack[]> onResult)
    {
        string url = ApiManager.GetUrl(ApiManager.JAMENDO_SEARCH)
                     + "?q=" + UnityWebRequest.EscapeURL(query) + "&limit=10";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
                PopupManager.Show("Serveur inaccessible, vérifiez votre connexion.");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[GET jamendo/search] {request.responseCode} — {request.error}");
                onResult?.Invoke(null);
                yield break;
            }

            string wrapped = "{\"items\":" + request.downloadHandler.text + "}";
            onResult?.Invoke(JsonUtility.FromJson<JamendoListWrapper>(wrapped).items);
        }
    }

    public void ImportTrack(string trackId, Action<JamendoImportResponse, bool> onResult)
    {
        StartCoroutine(PostRequestAuth<JamendoImportResponse>(
            $"{ApiManager.JAMENDO_IMPORT}/{trackId}",
            new object(),
            onResult
        ));
    }

    public string GetAudioDownloadUrl(string musicTitle)
    {
        return ApiManager.GetUrl($"/api/v1/music/{Uri.EscapeDataString(musicTitle)}/download");
    }

    public IEnumerator DownloadAndCacheClip(string url, string fileName, Action<AudioClip> onClip)
    {
        string localPath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(localPath))
        {
            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(new Uri(localPath).AbsoluteUri, AudioType.MPEG))
            {
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                    clip.name = Path.GetFileNameWithoutExtension(fileName);
                    onClip?.Invoke(clip);
                    yield break;
                }
            }
        }

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.timeout = 60;
            yield return req.SendWebRequest();

            Debug.Log($"[Download] url={url} result={req.result} code={req.responseCode} bytes={req.downloadHandler?.data?.Length}");

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Erreur téléchargement MP3 : {req.error} ({req.responseCode})");
                PopupManager.Show($"Erreur téléchargement ({req.responseCode})");
                onClip?.Invoke(null);
                yield break;
            }

            File.WriteAllBytes(localPath, req.downloadHandler.data);
        }

        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(new Uri(localPath).AbsoluteUri, AudioType.MPEG))
        {
            yield return req.SendWebRequest();

            Debug.Log($"[LoadFile] path={localPath} result={req.result}");

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Erreur lecture fichier audio : {req.error}");
                onClip?.Invoke(null);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
            if (clip == null)
            {
                PopupManager.Show("Erreur création audio.");
                onClip?.Invoke(null);
                yield break;
            }
            clip.name = Path.GetFileNameWithoutExtension(fileName);
            onClip?.Invoke(clip);
        }
    }
}
