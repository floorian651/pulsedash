public static class ApiManager
{
    private const string BaseUrl = "https://pulsedashapi.floabd.app";
    
    public const string LOGIN = "/api/v1/auth/login";
    public const string REGISTER = "/api/v1/auth/register";
    public const string PROFILE = "/api/v1/profile/me";
    public const string REFRESH = "/api/v1/auth/refresh";
    public const string GENERATE     = "/api/v1/generate";
    public const string GAME_SESSIONS = "/api/v1/game-sessions";
    public const string PLAYLISTS      = "/api/v1/playlists";
    public const string TRACKS         = "/api/v1/tracks";
    public const string JAMENDO_SEARCH = "/api/v1/jamendo/search";
    public const string JAMENDO_IMPORT = "/api/v1/jamendo/import";
    
    public static string GetUrl(string endpoint) => $"{BaseUrl}{endpoint}";
}