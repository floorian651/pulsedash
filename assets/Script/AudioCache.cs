using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;


public class AudioCache : MonoBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();


    public void LoadMusic(string url, string fileName)
    {
        StartCoroutine(LoadMusicRoutine(url, fileName));
    }

    IEnumerator LoadMusicRoutine(string url, string fileName)
    {
        string localPath = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(localPath))
        {   /*
            Debug.Log("Chargement depuis le cache : " + localPath);
            yield return StartCoroutine(LoadLocalFile(localPath));*/
            Debug.Log("Téléchargement depuis Jamendo : " + url);
            yield return StartCoroutine(DownloadAndCache(url, localPath));
        }/*
        else
        {
            Debug.Log("Téléchargement depuis Jamendo : " + url);
            yield return StartCoroutine(DownloadAndCache(url, localPath));
        }*/
    }
    /*
    IEnumerator LoadLocalFile(string path)
    {
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                MenuGenerator.audioSource.clip = DownloadHandlerAudioClip.GetContent(req);
                Debug.LogError("Chargement réussi");
            }
            else
            {
                Debug.LogError("Erreur chargement local : " + req.error);
            }
        }
    }*/

    IEnumerator DownloadAndCache(string url, string localPath)
    {
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                MenuGenerator.audioSource.clip = clip;

                byte[] data = req.downloadHandler.data;
                File.WriteAllBytes(localPath, data);

                Debug.Log("Musique mise en cache : " + localPath);
            }
            else
            {
                Debug.LogError("Erreur téléchargement : " + req.error);
            }
        }
    }
    public IEnumerator LoadAllCachedMusic()
{
    clips.Clear();

    //Chemin jusqu'au cache
    string path = Application.persistentDataPath;

    // Récupérer le nom de tous les fichiers mp3 dans le cache 
    string[] files = Directory.GetFiles(path, "*.mp3");

    foreach (string filePath in files)
    {   
        // Charger les fichiers mp3 pour les attribuer à clip 
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);

                // Renommer le clip avec le nom du fichier 
                clip.name = Path.GetFileNameWithoutExtension(filePath);
                
                clips.Add(clip);
            }
            else
            {
                Debug.LogError("Erreur chargement : " + req.error);
            }
        }
    }

    Debug.Log("Nombre de musiques chargées : " + clips.Count);
}

}
