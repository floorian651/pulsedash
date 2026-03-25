using UnityEngine;

public class SessionData : MonoBehaviour
{
    public static SessionData Instance;
    public int score;
    public string playerName;
    //public AudioSource audioSource;
    public string titre;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject); // Pour éviter les doublons de SessionData
    }
}
