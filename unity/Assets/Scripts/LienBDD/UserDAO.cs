using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UserDAO : MonoBehaviour
{
    public static string AccessToken { get; private set; }
    public static string RefreshToken { get; private set; }

    public static void SetTokens(string access, string refresh)
    {
        AccessToken = access;
        RefreshToken = refresh;
    }

    public void Register(string email, string mdp, string username, Action<bool> onResult)
    {
        StartCoroutine(RegisterCoroutine(email, mdp, username, onResult));
    }

    private IEnumerator RegisterCoroutine(string email, string mdp, string username, Action<bool> onResult)
    {
        string url = DotEnv.GetURL() + "/api/v1/auth/register";

        string jsonBody = JsonUtility.ToJson(new RegisterRequest { email = email, password = mdp, username = username });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur register : " + request.error);
                onResult?.Invoke(false);
                yield break;
            }

            RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
            SetTokens(response.access_token, response.refresh_token);
            onResult?.Invoke(true);
        }
    }

    public void Login(string email, string mdp, Action<bool> onResult)
    {
        StartCoroutine(LoginCoroutine(email, mdp, onResult));
    }

    private IEnumerator LoginCoroutine(string email, string mdp, Action<bool> onResult)
    {
        string url = DotEnv.GetURL() + "/api/v1/auth/login";

        string jsonBody = JsonUtility.ToJson(new LoginRequest { email = email, password = mdp });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur login : " + request.error);
                onResult?.Invoke(false);
                yield break;
            }

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            SetTokens(response.access_token, response.refresh_token);
            onResult?.Invoke(true);
        }
    }
}

[System.Serializable]
public class RegisterRequest
{
    public string email;
    public string password;
    public string username;
}

[System.Serializable]
public class RegisterResponse
{
    public string access_token;
    public string refresh_token;
    public string token_type;
}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public string access_token;
    public string refresh_token;
    public string token_type;
}
