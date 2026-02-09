using UnityEngine;

public class GenerateurNiveau : MonoBehaviour
{
    [Header("Paramètres de génération")]
    public GameObject GroundPrefab; // Préfab du sol
    public GameObject HardObstaclePrefab; // Préfab pour obstacle difficile
    public GameObject MediumObstaclePrefab; // Préfab pour obstacle de difficulté moyenne
    public GameObject EasyObstaclePrefab; // Préfab pour obstacle de difficulté facile
    public int width = 7; 
    public int depth = 7;
    
    public float spacing = 3.0f;

    [ContextMenu("Générer le Niveau")] // Permet de lancer via un clic droit sur le script
    public void GenerateLevel()
    {

        TextAsset jsonFile = Resources.Load<TextAsset>("analyse_rythme");
        if (jsonFile == null) {
            Debug.LogError("Il manque le fichier JSON dans le dossier Resources !");
            return;
        }
        MusicData data = JsonUtility.FromJson<MusicData>(jsonFile.text);

        ClearLevel();

        // On génère une base de sol avant les beats
        for(int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(2, 0, i * spacing * (-1)-1);
            GameObject newBlock = Instantiate(GroundPrefab, pos, Quaternion.identity);
            newBlock.transform.parent = this.transform;
        }

        for (int i = 0; i < data.beats.Length; i++)
        {
            // Pose du sol à chaque Beat
            Vector3 pos = new Vector3(2, 0, i * spacing -1);
            GameObject newBlock = Instantiate(GroundPrefab, pos, Quaternion.identity);
            
            newBlock.transform.parent = this.transform;
            if(data.beats[i].puissance > 3.0f){
                // Décider du type d'obstacle en fonction de la puissance
                GameObject obstaclePrefab = DecideBlockType(data.beats[i].puissance);
                Vector3 obstaclePos = new Vector3(2, obstaclePrefab.transform.position.y + obstaclePrefab.transform.localScale.y/2, i * spacing-1);
                Quaternion obstacleOrientation = Quaternion.Euler(0, 0, 0);
                GameObject obstacle = Instantiate(obstaclePrefab, obstaclePos, obstacleOrientation);
                obstacle.transform.parent = this.transform;
            }
        }
    }

    // Méthode pour nettoyer les blocs générés
    [ContextMenu("Nettoyer le Niveau")]
    public void ClearLevel()
    {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private GameObject DecideBlockType(float puissance)
    {
        if(puissance > 8.0f)
        {
            return HardObstaclePrefab;
        }
        else if(puissance > 5.0f)
        {
            return MediumObstaclePrefab;
        }
        else
        {
            return EasyObstaclePrefab;
        }
    }
}