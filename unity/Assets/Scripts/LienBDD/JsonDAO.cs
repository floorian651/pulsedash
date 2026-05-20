using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class JsonDAO : MonoBehaviour
{
    public void FetchLevelFromTitle(string title, Action<MusicData> onReady)
    {
        StartCoroutine(GenerateLevel(title, onReady));
    }

    IEnumerator GenerateLevel(string title, Action<MusicData> onReady)
    {
        string url = ApiManager.GetUrl(ApiManager.GENERATE);
        string jsonBody = "{\"track_id\":\"" + title + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur génération : " + request.error);
                onReady?.Invoke(null);
                yield break;
            }

            GenerateJobResponse response = JsonUtility.FromJson<GenerateJobResponse>(request.downloadHandler.text);
            yield return StartCoroutine(PollLevel(response.id, onReady));
        }
    }

    IEnumerator PollLevel(string jobId, Action<MusicData> onReady)
    {
        string url = ApiManager.GetUrl($"{ApiManager.GENERATE}/{jobId}");
        int maxRetries = 60;

        while (maxRetries-- > 0)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Erreur polling : " + request.error);
                    onReady?.Invoke(null);
                    yield break;
                }

                GenerateResult response = JsonUtility.FromJson<GenerateResult>(request.downloadHandler.text);

                if (response.state == "completed")
                {
                    onReady?.Invoke(response.level);
                    yield break;
                }
            }

            yield return new WaitForSeconds(2f);
        }

        Debug.LogError("Timeout : le niveau n'a pas été généré en 2 minutes.");
        onReady?.Invoke(null);
    }
}

[System.Serializable]
public class GenerateJobResponse
{
    public string id;
}

[System.Serializable]
public class GenerateResult
{
    public string job_id;
    public string state;
    public MusicData level;
}
