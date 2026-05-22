using UnityEngine;

public class TokenManager
{
    private const string ACCESS_TOKEN_KEY = "access_token";
    private const string REFRESH_TOKEN_KEY = "refresh_token";

    public static string AccessToken { get; private set; }
    public static string RefreshToken { get; private set; }

    public static void Initialize()
    {
        AccessToken = PlayerPrefs.GetString(ACCESS_TOKEN_KEY, "");
        RefreshToken = PlayerPrefs.GetString(REFRESH_TOKEN_KEY, "");
    }

    public static void SetTokens(string access, string refresh)
    {
        AccessToken = access;
        RefreshToken = refresh;
        PlayerPrefs.SetString(ACCESS_TOKEN_KEY, access);
        PlayerPrefs.SetString(REFRESH_TOKEN_KEY, refresh);
        PlayerPrefs.Save();
    }

    public static void Clear()
    {
        AccessToken = "";
        RefreshToken = "";
        PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);
        PlayerPrefs.DeleteKey(REFRESH_TOKEN_KEY);
        PlayerPrefs.Save();
    }
}