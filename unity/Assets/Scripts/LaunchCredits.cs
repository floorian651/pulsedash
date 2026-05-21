using UnityEngine;

public class LaunchCredits : MonoBehaviour
{
    SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
    public void launchCreditsScene() {
        sceneloader.LoadSceneByName("CreditsScene");
    }
}
