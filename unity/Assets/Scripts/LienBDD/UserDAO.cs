using System.Net.Http.Headers;

public class UserDAO : MonoBehaviour
{
    public bool Login(string pseudo, string mdp) {}
        public const string BaseUrl = DotEnv.GetURL() + "/api/v1";
        public const string WsBaseUrl = DotEnv.GetWebSocketURL();

        public static string AccessToken { get; private set; }
        public static string RefreshToken { get; private set; }

        public static void SetTokens(string access, string refresh)
        {
            AccessToken = access;
            RefreshToken = refresh;
        }
    }

