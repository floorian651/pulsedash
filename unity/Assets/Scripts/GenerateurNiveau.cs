using UnityEngine;

public class GenerateurNiveau : MonoBehaviour
{
    [Header("Paramètres de génération")]
    public GameObject GroundPrefab; // Préfab du sol
    public GameObject obstacleLevel4; // Préfab pour obstacle difficile
    public GameObject obstacleLevel3; // Préfab pour obstacle de difficulté moyenne
    public GameObject obstacleLevel2; // Préfab pour obstacle de difficulté facile
    public GameObject obstacleLevel1; // Préfab pour obstacle très facile    
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

        // On génère un peu de sol avant les beats pour que le joueur puisse démarrer
        for(int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(2, 0, i * spacing * (-1)-1);
            GameObject newGround = Instantiate(GroundPrefab, pos, Quaternion.identity);
            newGround.transform.parent = this.transform;
        }

        for (int i = 0; i < data.beats.Length; i++)
        {
            // Pose du sol à chaque Beat
            Vector3 pos = new Vector3(2, 0, i * spacing -1);
            GameObject newGround = Instantiate(GroundPrefab, pos, Quaternion.identity);
            newGround.transform.parent = this.transform;

            

            if(data.beats[i].puissance > 3.0f){
                // Décider du type d'obstacle en fonction de la puissance
                Vector3 obstaclePos = new Vector3(2, 0, i * spacing-1);
                CreateObstacle(data.beats[i], obstaclePos);
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

    private void CreateObstacle(Beat beat, Vector3 position)
    {
        if(beat.puissance < 2.0f)
        {
            return; // Pas d'obstacle pour les beats faibles
        }

        GameObject obstacle;

        if(beat.puissance < 3.5f)
        {
            obstacle = Instantiate(obstacleLevel1, position, Quaternion.identity); // Obstacle très facile pour les beats faibles
        }
        else if(beat.puissance < 5.5f)
        {
            obstacle = Instantiate(obstacleLevel2, position, Quaternion.identity); // Obstacle facile pour les beats modérés
        }
        else if(beat.puissance < 7.5f)
        {
            obstacle = Instantiate(obstacleLevel3, position, Quaternion.identity); // Obstacle difficile pour les beats très forts
        }
        else
        {
            obstacle = Instantiate(obstacleLevel4, position, Quaternion.identity); // Obstacle très difficile pour les beats très forts
        }

        obstacle.transform.parent = this.transform;
    }
}