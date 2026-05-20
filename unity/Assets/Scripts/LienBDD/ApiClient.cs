using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    private static void HandleNetworkError(UnityWebRequest request, string label)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError)
            PopupManager.Show("Serveur inaccessible, vérifiez votre connexion.");
        Debug.LogError($"[{label}] {request.responseCode} — {request.error}");
    }

    protected IEnumerator PostRequest<T>(string endpoint, object data, Action<T, bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                HandleNetworkError(request, $"POST {endpoint}");
                onResult?.Invoke(default, false);
                yield break;
            }

            onResult?.Invoke(JsonUtility.FromJson<T>(request.downloadHandler.text), true);
        }
    }

    protected IEnumerator PostRequestAuth<T>(string endpoint, object data, Action<T, bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                HandleNetworkError(request, $"POST {endpoint}");
                onResult?.Invoke(default, false);
                yield break;
            }

            onResult?.Invoke(JsonUtility.FromJson<T>(request.downloadHandler.text), true);
        }
    }

    protected IEnumerator PatchRequest<T>(string endpoint, object data, Action<T, bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                HandleNetworkError(request, $"PATCH {endpoint}");
                onResult?.Invoke(default, false);
                yield break;
            }

            onResult?.Invoke(JsonUtility.FromJson<T>(request.downloadHandler.text), true);
        }
    }

    protected IEnumerator GetRequest<T>(string endpoint, Action<T, bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");
            yield return request.SendWebRequest();

            if (request.responseCode == 401)
            {
                UserDAO userDAO = FindObjectOfType<UserDAO>();
                userDAO.RefreshAccessToken(success =>
                {
                    if (success)
                        StartCoroutine(GetRequest<T>(endpoint, onResult));
                    else
                        onResult?.Invoke(default, false);
                });
                yield break;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                HandleNetworkError(request, $"GET {endpoint}");
                onResult?.Invoke(default, false);
                yield break;
            }

            onResult?.Invoke(JsonUtility.FromJson<T>(request.downloadHandler.text), true);
        }
    }
}
