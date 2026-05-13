using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using System.IO;
using System.Text.Json.Nodes;

public class MusicDAO : MonoBehaviour
{
    public IEnumerator GetMusic(string title)
    {
        string url = DotEnv.GetURL() + "/api/v1/music/" + title + "/download";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur API : " + webRequest.error);
                yield break;
            }

            string downloadUrl = webRequest.downloadHandler.text;

            Debug.Log("URL de téléchargement : " + downloadUrl);

            yield return StartCoroutine(DownloadMP3(downloadUrl, title));
        }
    }

    IEnumerator DownloadMP3(string url, string title)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur téléchargement : " + webRequest.error);
                yield break;
            }

            byte[] data = webRequest.downloadHandler.data;

            string path = Application.dataPath + "/Resources/Musique/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = path + title + ".mp3";

            File.WriteAllBytes(filePath, data);

            Debug.Log("Musique téléchargée : " + filePath);
        }
    }





    public JsonArray searchMusic(string query)
    {
        string url = DotEnv.GetURL() + "/api/v1/jamendo/search?q=" + UnityWebRequest.EscapeURL(query) + "&limit=10";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
                ;

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur API : " + webRequest.error);
                return null;
            }

            string json = webRequest.downloadHandler.text;

            JsonArray results = JsonNode.Parse(json).AsArray();

            return results;
        }
    }

    public void charger(JsonObject music)
    {
        string url = DotEnv.GetURL() + "/api/v1/jamendo/import/" + music["id"].ToString();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
                ;

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur API : " + webRequest.error);
                return;
            }

            string json = webRequest.downloadHandler.text;

            Debug.Log("Musique importée : " + json);
        }
    }
}