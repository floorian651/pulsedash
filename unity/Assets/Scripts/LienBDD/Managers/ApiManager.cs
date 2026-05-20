public static class ApiManager
{
    private const string BaseUrl = "https://pulsedashapi.floabd.app";
    
    public const string LOGIN = "/api/v1/auth/login";
    public const string REGISTER = "/api/v1/auth/register";
    public const string PROFILE = "/api/v1/profile";
    public const string REFRESH = "/api/v1/auth/refresh";
    public const string GENERATE     = "/api/v1/generate";
    public const string GAME_SESSIONS = "/api/v1/game-sessions";
    
    public static string GetUrl(string endpoint) => $"{BaseUrl}{endpoint}";
}