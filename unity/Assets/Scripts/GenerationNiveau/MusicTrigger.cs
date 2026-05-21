using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class Trigger : MonoBehaviour
{
    public AudioSource musicSource;
    private bool musicStarted = false;
    public float delaiAvantMusique = 2.0f;
    public string titre_musique;

    void OnEnable()
    {
        if (!musicStarted)
        {
            titre_musique = SessionData.Instance != null ? SessionData.Instance.titre : "";
            StartCoroutine(LoadClip());
        }
    }

    IEnumerator LoadClip()
    {
        string path = Path.Combine(Application.persistentDataPath, titre_musique + ".mp3");
        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                musicSource.clip = DownloadHandlerAudioClip.GetContent(req);
            else
                Debug.LogError("MusicTrigger: impossible de charger " + path + " — " + req.error);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!musicStarted && musicSource.clip != null && other.CompareTag("Player"))
        {
            musicStarted = true;
            double heureDepartMusique = AudioSettings.dspTime + delaiAvantMusique;
            musicSource.PlayScheduled(heureDepartMusique);
        }
    }
}
