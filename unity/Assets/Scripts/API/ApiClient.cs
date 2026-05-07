using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// ---------------------------------------------------------------------------
// Modèles de réponse API
// ---------------------------------------------------------------------------

[Serializable]
public class ApiJamendoTrack
{
    public string id;
    public string name;
    public string artist_name;
    public int    duration;
    public string audio;
}

[Serializable]
class ApiJamendoTrackList { public ApiJamendoTrack[] items; }

[Serializable]
public class TokenResponse
{
    public string access_token;
    public string refresh_token;
}

[Serializable]
public class ImportAccepted
{
    public string job_id;
    public string music_title;
    public string state;
}

[Serializable]
public class JobResponse
{
    public string job_id;
    public string state;
    public int    progress;
    public string result_url;
    public string error;
}

[Serializable]
public class LevelMeta
{
    public float  bpm;
    public float  duration;
    public string key;
}

[Serializable]
public class HitData
{
    public float  time;
    public float  strength;
    public int    lane;
    public string type;
}

[Serializable]
public class SectionData
{
    public float  start;
    public float  end;
    public string label;
}

[Serializable]
public class LevelData
{
    public LevelMeta    meta;
    public HitData[]    hits;
    public SectionData[] sections;
}

// ---------------------------------------------------------------------------
// ApiClient — singleton MonoBehaviour
// ---------------------------------------------------------------------------

public class ApiClient : MonoBehaviour
{
    // ── Config ───────────────────────────────────────────────────────────────

    [Header("Configuration")]
    [SerializeField] private string baseUrl = "http://localhost:8000/api/v1";

    public static string BaseUrl => Instance._baseUrl;

    // ── Singleton ─────────────────────────────────────────────────────────────

    public static ApiClient Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ── Token management ──────────────────────────────────────────────────────

    private const string KeyAccess  = "api_access_token";
    private const string KeyRefresh = "api_refresh_token";

    public static string AccessToken  => PlayerPrefs.GetString(KeyAccess,  "");
    public static string RefreshToken => PlayerPrefs.GetString(KeyRefresh, "");
    public static bool   IsLoggedIn   => !string.IsNullOrEmpty(AccessToken);

    public static void SaveTokens(TokenResponse tokens)
    {
        PlayerPrefs.SetString(KeyAccess,  tokens.access_token);
        PlayerPrefs.SetString(KeyRefresh, tokens.refresh_token);
        PlayerPrefs.Save();
    }

    public static void ClearTokens()
    {
        PlayerPrefs.DeleteKey(KeyAccess);
        PlayerPrefs.DeleteKey(KeyRefresh);
        PlayerPrefs.Save();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private UnityWebRequest MakePost(string url, string jsonBody, bool withAuth = true)
    {
        var req = new UnityWebRequest(url, "POST");
        req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        if (withAuth)
            req.SetRequestHeader("Authorization", "Bearer " + AccessToken);
        return req;
    }

    private UnityWebRequest MakePatch(string url, string jsonBody)
    {
        var req = new UnityWebRequest(url, "PATCH");
        req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", "Bearer " + AccessToken);
        return req;
    }

    private UnityWebRequest MakeGet(string url, bool withAuth = true)
    {
        var req = UnityWebRequest.Get(url);
        if (withAuth)
            req.SetRequestHeader("Authorization", "Bearer " + AccessToken);
        return req;
    }

    // Renvoie true si la réponse est une erreur (gère le refresh automatique sur 401)
    private IEnumerator HandleResponse(
        UnityWebRequest req,
        Action<string> onSuccess,
        Action<string> onError,
        bool retried = false)
    {
        if (req.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(req.downloadHandler.text);
            yield break;
        }

        // Token expiré → refresh puis une seule nouvelle tentative
        if (req.responseCode == 401 && !retried)
        {
            bool refreshed = false;
            yield return StartCoroutine(RefreshToken_Internal(ok => refreshed = ok));

            if (refreshed)
            {
                // Recréer la même requête avec le nouveau token
                UnityWebRequest retryReq;
                if (req.method == "POST")
                {
                    string body = Encoding.UTF8.GetString(req.uploadHandler.data);
                    retryReq = MakePost(req.url, body);
                }
                else if (req.method == "PATCH")
                {
                    string body = Encoding.UTF8.GetString(req.uploadHandler.data);
                    retryReq = MakePatch(req.url, body);
                }
                else
                {
                    retryReq = MakeGet(req.url);
                }

                yield return retryReq.SendWebRequest();
                yield return StartCoroutine(HandleResponse(retryReq, onSuccess, onError, retried: true));
                yield break;
            }

            ClearTokens();
            onError?.Invoke("Session expirée, veuillez vous reconnecter.");
            yield break;
        }

        string errDetail = req.downloadHandler?.text ?? req.error;
        onError?.Invoke($"[{req.responseCode}] {errDetail}");
    }

    // ── Auth ──────────────────────────────────────────────────────────────────

    public IEnumerator Register(
        string email, string password, string username,
        Action<TokenResponse> onSuccess,
        Action<string> onError)
    {
        string body = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"username\":\"{username}\"}}";
        var req = MakePost(BaseUrl + "/auth/register", body, withAuth: false);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req,
            json => onSuccess?.Invoke(JsonUtility.FromJson<TokenResponse>(json)),
            onError));
    }

    public IEnumerator Login(
        string email, string password,
        Action<TokenResponse> onSuccess,
        Action<string> onError)
    {
        string body = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";
        var req = MakePost(BaseUrl + "/auth/login", body, withAuth: false);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req,
            json => onSuccess?.Invoke(JsonUtility.FromJson<TokenResponse>(json)),
            onError));
    }

    // Interne : appelé automatiquement sur 401
    private IEnumerator RefreshToken_Internal(Action<bool> callback)
    {
        if (string.IsNullOrEmpty(RefreshToken)) { callback(false); yield break; }

        string body = $"{{\"refresh_token\":\"{RefreshToken}\"}}";
        var req = MakePost(BaseUrl + "/auth/refresh", body, withAuth: false);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var tokens = JsonUtility.FromJson<TokenResponse>(req.downloadHandler.text);
            SaveTokens(tokens);
            callback(true);
        }
        else
        {
            callback(false);
        }
    }

    // ── Génération de niveau ──────────────────────────────────────────────────

    /// <summary>
    /// Lance l'import d'un track Jamendo et la génération du niveau.
    /// Retourne un job_id à passer à PollJobUntilDone.
    /// </summary>
    public IEnumerator ImportAndGenerate(
        string jamendoTrackId,
        Action<ImportAccepted> onAccepted,
        Action<string> onError)
    {
        var req = MakePost(BaseUrl + "/jamendo/import/" + jamendoTrackId, "", withAuth: true);
        // POST sans body (track_id est dans l'URL)
        req.uploadHandler = new UploadHandlerRaw(Array.Empty<byte>());
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req,
            json => onAccepted?.Invoke(JsonUtility.FromJson<ImportAccepted>(json)),
            onError));
    }

    /// <summary>
    /// Interroge GET /jobs/{jobId} toutes les `intervalSeconds` secondes
    /// jusqu'à ce que l'état soit "completed" ou "failed".
    /// onProgress(0-100), onComplete(LevelData), onError(message).
    /// </summary>
    public IEnumerator PollJobUntilDone(
        string jobId,
        Action<int> onProgress,
        Action<LevelData> onComplete,
        Action<string> onError,
        float intervalSeconds = 3f)
    {
        while (true)
        {
            var req = MakeGet(BaseUrl + "/jobs/" + jobId);
            yield return req.SendWebRequest();

            JobResponse job = null;
            bool callbackCalled = false;

            yield return StartCoroutine(HandleResponse(req,
                json => job = JsonUtility.FromJson<JobResponse>(json),
                err  => { onError?.Invoke(err); callbackCalled = true; }));

            if (callbackCalled) yield break;
            if (job == null)    { onError?.Invoke("Réponse vide du serveur."); yield break; }

            onProgress?.Invoke(job.progress);

            if (job.state == "completed")
            {
                if (string.IsNullOrEmpty(job.result_url))
                {
                    onError?.Invoke("Job terminé mais result_url manquante.");
                    yield break;
                }
                yield return StartCoroutine(DownloadLevel(job.result_url, onComplete, onError));
                yield break;
            }

            if (job.state == "failed")
            {
                onError?.Invoke(string.IsNullOrEmpty(job.error) ? "Génération échouée." : job.error);
                yield break;
            }

            yield return new WaitForSeconds(intervalSeconds);
        }
    }

    /// <summary>
    /// Télécharge le level.json depuis l'URL presignée MinIO.
    /// </summary>
    private IEnumerator DownloadLevel(
        string resultUrl,
        Action<LevelData> onComplete,
        Action<string> onError)
    {
        // L'URL presignée MinIO ne nécessite pas de header Authorization
        var req = MakeGet(resultUrl, withAuth: false);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req,
            json => onComplete?.Invoke(JsonUtility.FromJson<LevelData>(json)),
            onError));
    }

    // ── Sessions de jeu ───────────────────────────────────────────────────────

    [Serializable] private class GameSessionStart { public string music_title; }
    [Serializable] private class GameSessionStartWrapper { public string id; public string status; }

    public IEnumerator StartGameSession(
        string musicTitle,
        Action<string> onSessionId,
        Action<string> onError)
    {
        string body = $"{{\"music_title\":\"{musicTitle}\"}}";
        var req = MakePost(BaseUrl + "/game-sessions", body);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req,
            json =>
            {
                var s = JsonUtility.FromJson<GameSessionStartWrapper>(json);
                onSessionId?.Invoke(s.id);
            },
            onError));
    }

    public IEnumerator EndGameSession(
        string sessionId, int finalScore, float accuracy, bool abandoned,
        Action onSuccess,
        Action<string> onError)
    {
        // float → string sans virgule selon la locale
        string acc  = accuracy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        string aband = abandoned ? "true" : "false";
        string body = $"{{\"final_score\":{finalScore},\"accuracy\":{acc},\"abandoned\":{aband}}}";
        var req = MakePatch(BaseUrl + "/game-sessions/" + sessionId + "/end", body);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req,
            _ => onSuccess?.Invoke(),
            onError));
    }

    // ── Recherche Jamendo ─────────────────────────────────────────────────────

    public IEnumerator SearchJamendo(
        string query, int limit,
        Action<ApiJamendoTrack[]> onResults,
        Action<string> onError)
    {
        string url = BaseUrl + "/jamendo/search?q=" + UnityWebRequest.EscapeURL(query) + "&limit=" + limit;
        var req = MakeGet(url, withAuth: false);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req,
            json =>
            {
                var wrapper = JsonUtility.FromJson<ApiJamendoTrackList>("{\"items\":" + json + "}");
                onResults?.Invoke(wrapper?.items ?? Array.Empty<ApiJamendoTrack>());
            },
            onError));
    }

    // ── Scores ────────────────────────────────────────────────────────────────

    public IEnumerator GetLeaderboard(
        string musicTitle, int limit,
        Action<string> onJson,
        Action<string> onError)
    {
        string url = BaseUrl + "/scores/top?music_title=" + UnityWebRequest.EscapeURL(musicTitle) + "&limit=" + limit;
        var req = MakeGet(url, withAuth: false);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req, onJson, onError));
    }

    public IEnumerator GetGlobalLeaderboard(
        int limit,
        Action<string> onJson,
        Action<string> onError)
    {
        var req = MakeGet(BaseUrl + "/scores/global?limit=" + limit, withAuth: false);
        yield return req.SendWebRequest();
        yield return StartCoroutine(HandleResponse(req, onJson, onError));
    }
}
