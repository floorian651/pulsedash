using UnityEditor;
using UnityEngine;

public class ForceBuildScript
{
    // Cela va créer un nouveau menu tout en haut de ton Unity !
    [MenuItem("PulseRunner/💥 FORCER LE BUILD LINUX")]
    public static void BuildGame()
    {
        Debug.Log("Lancement du build forcé...");

        // On liste tes 3 scènes exactement comme dans ta capture d'écran
        string[] scenes = { 
            "Assets/Scenes/Platform_Streaming.unity", 
            "Assets/Scenes/Pulser_animated.unity", 
            "Assets/Scenes/GameplaySceneLocal.unity" 
        };

        // Unity va compiler directement dans un dossier "Builds" à la racine de ton projet
        BuildPipeline.BuildPlayer(scenes, "Builds/PulseRunner.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.None);
        
        Debug.Log("Build terminé ! Allez voir dans le dossier Builds de votre projet.");
    }
}