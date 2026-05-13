using UnityEditor;
using UnityEngine;

public class ForceBuildScript
{
    // On déclare la liste des scènes une seule fois pour ne pas se répéter
    static string[] scenes = { 
        "Assets/Scenes/Accueil.unity",
        "Assets/Scenes/PageConnexion.unity",
        "Assets/Scenes/PageInscription.unity",
        "Assets/Scenes/Platform_Streaming.unity",
        "Assets/Scenes/GameplayScene.unity" 
    };

    [MenuItem("PulseDash/🐧 FORCER LE BUILD LINUX")]
    public static void BuildGameLinux()
    {
        Debug.Log("Lancement du build forcé pour Linux...");
        
        // Exporte dans Builds/Linux/
        BuildPipeline.BuildPlayer(scenes, "Builds/Linux/PulseRunner.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.None);
        
        Debug.Log("Build Linux terminé ! Allez voir dans le dossier Builds/Linux.");
    }

    [MenuItem("PulseDash/🪟 FORCER LE BUILD WINDOWS")]
    public static void BuildGameWindows()
    {
        Debug.Log("Lancement du build forcé pour Windows...");
        
        // Exporte dans Builds/Windows/ avec l'extension .exe
        BuildPipeline.BuildPlayer(scenes, "Builds/Windows/PulseRunner.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        
        Debug.Log("Build Windows terminé ! Allez voir dans le dossier Builds/Windows.");
    }
}