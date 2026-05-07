using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using System.IO;

public class JsonDAO : MonoBehaviour
{
    public void FetchLevelFromTitle(string title)
    {
        StartCoroutine(GenerateLevel(title));
    }

    // Étape 1 : lancer la génération
    IEnumerator GenerateLevel(string title)
    {
        string url = DotEnv.GetURL() + "/api/v1/generate";

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
                yield break;
            }

            string responseText = request.downloadHandler.text;
            Debug.Log("Réponse génération : " + responseText);

            // 👉 On récupère le job_id
            GenerateJobResponse response = JsonUtility.FromJson<GenerateJobResponse>(responseText);

            // Étape 2 : récupérer le niveau
            StartCoroutine(GetLevel(response.id));
        }
    }

    // Étape 2 : polling du résultat
    IEnumerator GetLevel(string jobId)
    {
        string url = DotEnv.GetURL() + "/api/v1/generate/" + jobId;

        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Erreur API : " + webRequest.error);
                    yield break;
                }

                string json = webRequest.downloadHandler.text;

                GenerateResult response = JsonUtility.FromJson<GenerateResult>(json);

                Debug.Log("State : " + response.state);

                if (response.state == "completed")
                {
                    SaveJson(json, jobId);
                    yield break;
                }
            }

            yield return new WaitForSeconds(2f);
        }
    }

    // Sauvegarde
    void SaveJson(string json, string jobId)
    {
        string path = Application.dataPath + "/Resources/JSON/";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string filePath = path + "level_" + jobId + ".json";

        File.WriteAllText(filePath, json);

        Debug.Log("Niveau sauvegardé : " + filePath);
    }
}

// 🔹 Réponse du POST /generate
[System.Serializable]
public class GenerateJobResponse
{
    public string id;
}

// 🔹 Réponse du GET /generate/{job_id}
[System.Serializable]
public class GenerateResult
{
    public string job_id;
    public string state;
    public string level; // adapte si c’est un objet complexe
}