using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionData : MonoBehaviour
{
    public static SessionData Instance;
    public float score;
    public string playerName;
    public string titre;
    public string mode;
    public string scenePrecedente;
    public string sessionId;


    void Awake()
    {
        Debug.Log("Session Data");

        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            // écouter les changements de scène
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneActuelle = scene.name;

        UpdateScenePrecedente(sceneActuelle);

        Debug.Log("Scene actuelle : " + sceneActuelle);
        Debug.Log("Scene précédente est : " + scenePrecedente);
    }



    void UpdateScenePrecedente(string scene)
{
    switch (scene)
    {
        case string s when s.EndsWith("GameplayScene"):
        case "FinishScene":
            scenePrecedente = "Platform_Streaming";
            break;

        case "PageConnexion":
        case "PageInscription":
            scenePrecedente = "Accueil";
            break;

        default:
            scenePrecedente = "Inconnue";
            break;
    }
}

}