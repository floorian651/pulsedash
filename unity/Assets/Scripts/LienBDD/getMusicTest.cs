using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class getMusicTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string title = "Meme";
        
        StartCoroutine(GetMusic(title));
    }

    public void GetMusic(string title)
    {
        string url = DotEnv.GetURL() + "/api/v1/music/" + title + "/download";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                Debug.Log("Réponse : " + response);
            }
            else
            {
                Debug.LogError("Erreur : " + webRequest.error);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
