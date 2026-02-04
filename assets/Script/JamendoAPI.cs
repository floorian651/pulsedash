using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class JamendoTrack {
    public string name;
    public string audio;
    public string artist_name;
    public string tags;
}

[System.Serializable]
public class JamendoResponse {
    public JamendoTrack[] results;
}

public class JamendoAPI : MonoBehaviour
{
    public string clientID = "ac3af075"; // Mets ta clé ici

    public IEnumerator GetTrackByGenre(string genre, System.Action<JamendoTrack[]> callback)

    {
        string url =
            "https://api.jamendo.com/v3.0/tracks/?client_id=" + clientID +
            "&format=json&tags=" + genre +
            "&audioformat=mp32&limit=3";

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            JamendoResponse data = JsonUtility.FromJson<JamendoResponse>(req.downloadHandler.text);

            if (data.results.Length > 0)
                callback(data.results);

            else
                callback(null);
        }
        else
        {
            Debug.LogError("Erreur Jamendo : " + req.error);
            callback(null);
        }
    }

    public IEnumerator SearchTrackByTitle(string title, System.Action<JamendoTrack[]> callback)
{
        string url =
            "https://api.jamendo.com/v3.0/tracks/?client_id=" + clientID +
            "&format=json&namesearch=" + title +
            "&audioformat=mp32&limit=3";

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            JamendoResponse data = JsonUtility.FromJson<JamendoResponse>(req.downloadHandler.text);
            callback(data.results);
        }
        else
        {
            callback(null);
        }
    }

}
