using UnityEngine;

public class LaunchCredits : MonoBehaviour
{
    private SceneLoader sceneloader;

    void Awake()
    {
        sceneloader = FindObjectOfType<SceneLoader>();
    }

    public void launchCreditsScene()
    {
        sceneloader.LoadSceneByName("CreditsScene");
        Debug.Log("Crédits lancés");
        Debug.Log(sceneloader);
    }
}