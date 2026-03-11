using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {   
        //Load une scène en fonction de son nom
        SceneManager.LoadScene(sceneName);
    }
}
