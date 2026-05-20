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

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[POST {endpoint}] {request.responseCode} — {request.downloadHandler.text}");
                onResult?.Invoke(default, false);
                yield break;
            }

            T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            onResult?.Invoke(response, true);
        }
    }

    protected IEnumerator PostRequestAuth<T>(string endpoint, object data, Action<T, bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);
        string jsonBody = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[POST {endpoint}] {request.responseCode} — {request.downloadHandler.text}");
                onResult?.Invoke(default, false);
                yield break;
            }

            T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            onResult?.Invoke(response, true);
        }
    }

    protected IEnumerator PatchRequest<T>(string endpoint, object data, Action<T, bool> onResult)
    {
        string url = ApiManager.GetUrl(endpoint);
        string jsonBody = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.AccessToken}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[PATCH {endpoint}] {request.responseCode} — {request.downloadHandler.text}");
                onResult?.Invoke(default, false);
                yield break;
            }

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

            if (request.responseCode == 401)
            {
                UserDAO userDAO = FindObjectOfType<UserDAO>();
                userDAO.RefreshAccessToken((success) =>
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
                Debug.LogError($"[GET {endpoint}] {request.responseCode} — {request.error}");
                onResult?.Invoke(default, false);
                yield break;
            }

            T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            onResult?.Invoke(response, true);
        }
    }
}
