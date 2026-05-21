using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlaylistDAO : ApiClient
{
    [Serializable]
    private class PlaylistListWrapper { public List<PlaylistData> items; }

    public void GetAllPlaylists(Action<List<PlaylistData>> onResult)
    {
        StartCoroutine(FetchAllPlaylists(onResult));
    }

    IEnumerator FetchAllPlaylists(Action<List<PlaylistData>> onResult)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(ApiManager.GetUrl(ApiManager.PLAYLISTS)))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
                PopupManager.Show("Serveur inaccessible, vérifiez votre connexion.");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[GET {ApiManager.PLAYLISTS}] {request.error}");
                onResult?.Invoke(null);
                yield break;
            }

            string wrapped = "{\"items\":" + request.downloadHandler.text + "}";
            onResult?.Invoke(JsonUtility.FromJson<PlaylistListWrapper>(wrapped).items);
        }
    }

    public void CreatePlaylist(string name, Action<PlaylistData, bool> onResult)
    {
        StartCoroutine(PostRequestAuth<PlaylistData>(
            ApiManager.PLAYLISTS,
            new CreatePlaylistRequest { name = name },
            onResult
        ));
    }

    public void DeletePlaylist(string name, Action<bool> onResult)
    {
        StartCoroutine(DeleteAuth($"{ApiManager.PLAYLISTS}/{name}", onResult));
    }

    public void AddTrack(string playlistName, string musicTitle, Action<TrackData, bool> onResult)
    {
        StartCoroutine(PostRequestAuth<TrackData>(
            ApiManager.TRACKS,
            new AddTrackRequest { playlist_name = playlistName, music_title = musicTitle },
            onResult
        ));
    }

    public void RemoveTrack(int trackId, Action<bool> onResult)
    {
        StartCoroutine(DeleteAuth($"{ApiManager.TRACKS}/{trackId}", onResult));
    }

    IEnumerator DeleteAuth(string endpoint, Action<bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
                PopupManager.Show("Serveur inaccessible, vérifiez votre connexion.");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[DELETE {endpoint}] {request.responseCode}");
                onResult?.Invoke(false);
                yield break;
            }

            onResult?.Invoke(true);
        }
    }
}
