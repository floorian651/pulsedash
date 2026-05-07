using UnityEngine;

public class SessionData : MonoBehaviour
{
    public static SessionData Instance;
    public int score;
    public string playerName;
    public string titre;
    public LevelData levelData;


    void Awake()
    {
        Debug.Log("Session Data");
        if (Instance == null)
        {
            Instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject); // Pour éviter les doublons de SessionData
    }
}