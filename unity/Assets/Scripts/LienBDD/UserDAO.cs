using System;
using UnityEngine;

public class UserDAO : ApiClient
{
    void Awake()
    {
        TokenManager.Initialize();
    }

    public void Register(string email, string mdp, string username, Action<bool> onResult)
    {
        StartCoroutine(PostRequest<RegisterResponse>(
            ApiManager.REGISTER,
            new RegisterRequest { email = email, password = mdp, username = username },
            (response, success) =>
            {
                if (success)
                    TokenManager.SetTokens(response.access_token, response.refresh_token);
                onResult?.Invoke(success);
            }
        ));
    }

    public void Login(string email, string mdp, Action<bool> onResult)
    {
        StartCoroutine(PostRequest<LoginResponse>(
            ApiManager.LOGIN,
            new LoginRequest { email = email, password = mdp },
            (response, success) =>
            {
                if (success)
                    TokenManager.SetTokens(response.access_token, response.refresh_token);
                onResult?.Invoke(success);
            }
        ));
    }

    public void GetProfile(Action<UserProfile, bool> onResult)
    {
        StartCoroutine(GetRequest<UserProfile>(ApiManager.PROFILE, onResult));
    }

    public void RefreshAccessToken(Action<bool> onResult){
    if (string.IsNullOrEmpty(TokenManager.RefreshToken))
    {
        Debug.LogWarning("Pas de refresh token!");
        onResult?.Invoke(false);
        return;
    }

    StartCoroutine(PostRequest<RefreshResponse>(
        ApiManager.REFRESH,
        new RefreshRequest { refresh_token = TokenManager.RefreshToken },
        (response, success) =>
        {
            if (success)
            {
                Debug.Log("Token rafraîchi!");
                // Garder le même refresh token, juste mettre à jour l'access token
                TokenManager.SetTokens(response.access_token, TokenManager.RefreshToken);
                onResult?.Invoke(true);
            }
            else
            {
                Debug.LogError("Refresh échoué - Déconnexion");
                TokenManager.Clear();
                onResult?.Invoke(false);
            }
        }
    ));
}
    public static void Logout()
    {
        TokenManager.Clear();
    }
}