using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
protected IEnumerator PostRequest<T>(string endpoint, object data, Action<T, bool> onResult)
{
    string url = ApiManager.GetUrl(endpoint);
    string jsonBody = JsonUtility.ToJson(data);
    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

    Debug.Log($"URL: {url}");
    Debug.Log($"JSON: {jsonBody}");
    
    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    {
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Code: {request.responseCode}");
            Debug.LogError($"Erreur: {request.error}");
            Debug.LogError($"Réponse: {request.downloadHandler.text}");
            onResult?.Invoke(default, false);
            yield break;
        }

        Debug.Log($"Réponse: {request.downloadHandler.text}");
        T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
        onResult?.Invoke(response, true);
    }
}
protected IEnumerator GetRequest<T>(string endpoint, Action<T, bool> onResult)
{
    string url = ApiManager.GetUrl(endpoint);

    using (UnityWebRequest request = UnityWebRequest.Get(url))
    {
        request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");
        yield return request.SendWebRequest();

        // Si 401 → Refresh et retry
        if (request.responseCode == 401)
        {
            Debug.LogWarning("Token expiré - Tentative refresh...");
            
            UserDAO userDAO = FindObjectOfType<UserDAO>();
            userDAO.RefreshAccessToken((success) =>
            {
                if (success)
                {
                    Debug.Log("Token rafraîchi - Retry requête...");
                    StartCoroutine(GetRequest<T>(endpoint, onResult)); // Retry
                }
                else
                {
                    onResult?.Invoke(default, false);
                }
            });
            yield break;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"API Error on {endpoint}: {request.error}");
            onResult?.Invoke(default, false);
            yield break;
        }

        T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
        onResult?.Invoke(response, true);
    }
}
}
