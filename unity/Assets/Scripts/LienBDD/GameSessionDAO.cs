using System;
using UnityEngine;

public class GameSessionDAO : ApiClient
{
    public void StartSession(string musicTitle, Action<string> onSessionId)
    {
        StartCoroutine(PostRequestAuth<StartSessionResponse>(
            ApiManager.GAME_SESSIONS,
            new StartSessionRequest { music_title = musicTitle },
            (response, success) => onSessionId?.Invoke(success ? response.id : null)
        ));
    }

    public void EndSession(string sessionId, float finalScore, bool abandoned, Action<bool> onResult)
    {
        string endpoint = $"{ApiManager.GAME_SESSIONS}/{sessionId}/end";
        StartCoroutine(PatchRequest<EndSessionResponse>(
            endpoint,
            new EndSessionRequest { final_score = finalScore, abandoned = abandoned },
            (response, success) => onResult?.Invoke(success)
        ));
    }
}
