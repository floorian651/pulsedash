using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class getMusicTest : MonoBehaviour
{
    static void test()
    {
        string title = "Meme";
        StartCoroutine(DownloadMP3(title));
    }

    IEnumerator GetMusic(string title)
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

            // 👉 Ici on récupère l'URL de téléchargement
            string downloadUrl = webRequest.downloadHandler.text;

            Debug.Log("URL de téléchargement : " + downloadUrl);

            // 👉 On lance le téléchargement du MP3
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

            // 👉 Chemin de sauvegarde
            string path = Application.dataPath + "/Resources/Musique/";

            // Crée le dossier si nécessaire
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = path + title + ".mp3";

            File.WriteAllBytes(filePath, data);

            Debug.Log("Musique téléchargée : " + filePath);
        }
    }
}