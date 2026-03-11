using UnityEngine;
using System.Collections.Generic; // Pour List

public class GenerateurNiveau : MonoBehaviour
{
    [Header("Paramètres de génération")]
    private int randomSeed; // Graine aléatoire pour la génération du niveau.
    public GameObject player; // Référence au joueur pour récupérer sa vitesse
    private float vitesse; // Sera init quand on l'aura

    private MusicData data; // Contiendra les données du fichier JSON
    private float chunkSize; // Taille d'un chunk du niveau (sera déterminé par la vitesse du joueur)
    private int nbChunksGeneres = 0; // Compteur du nombre de chunks générés, pour éviter de générer des chunks trop loin
    public GameObject GroundPrefab; // Préfab du sol
    public GameObject obstacleLevel4; // Préfab pour obstacle difficile
    public GameObject obstacleLevel3; // Préfab pour obstacle de difficulté moyenne
    public GameObject obstacleLevel2; // Préfab pour obstacle de difficulté facile
    public GameObject obstacleLevel1; // Préfab pour obstacle très facile    
    private float groundHeight = 0.0f; // Indique la coordonnée y du sol (actuellement)

    [ContextMenu("Générer le Niveau")] // Permet de lancer via un clic droit sur le script
    public void GenerateLevel()
    {
        // On récupère la vitesse du joueur, nécessaire à la synchro musique/obstacles
        vitesse = player.GetComponent<PlayerMovementE5>().GetSpeed();
        chunkSize = vitesse * 0.5f; // On définit la taille d'un chunk comme étant la distance parcourue par le joueur en 0.5 secondes

        // On récupère les données du fichier JSON

        TextAsset jsonFile = Resources.Load<TextAsset>("analyse_rythme");
        if (jsonFile == null) {
            Debug.LogError("Il manque le fichier JSON dans le dossier Resources !");
            return;
        }
        data = JsonUtility.FromJson<MusicData>(jsonFile.text);

        // Permet de générer une graine aléatoire de facon déterministe
        randomSeed = (int)data.duration;

        // On nettoie le niveau avant de générer les nouveaux éléments
        ClearLevel();

        // On génère le sol avant le niveau (de -10 à 0) pour éviter les problèmes de synchro au début du niveau+
        for(int i = -10; i < 0; i++)
        {
            Vector3 pos = new Vector3(2, 0, i * vitesse);
            pos.y = groundHeight;
            
            GameObject newGround = Instantiate(GroundPrefab, pos, Quaternion.identity);
            newGround.transform.parent = this.transform;
        }

        for (int i = 0; i < data.beats.Length; i++)
        {
            generateChunks();
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

    /** Permet à partir d'un beat de décider quel action le joueur doit faire
    * Et donc de générer l'obstacle correspondant, à la bonne position
     */
    // private void decideChunk(Beat beat)
    // {
    //     GameObject obstacle;

    //     if(beat.puissance < 2.5f)
    //     {
    //         obstacle = Instantiate(GroundPrefab, position, Quaternion.identity);
    //     }


    //     obstacle.transform.parent = this.transform;
    // }

    private void generateChunks()
    {
        // On va parcourir les chunk à générer
        nbChunksGeneres = 0;
        while(nbChunksGeneres * chunkSize < data.duration * vitesse)
        {
            chooseChunk();
            nbChunksGeneres++;
        }
    }

    private void chooseChunk()
    {
        // On récupère l'intervalle de beats correspondant au chunk à générer
        List<Beat> beatsInChunk = data.getBeatsInInterval(nbChunksGeneres * chunkSize, (nbChunksGeneres + 1) * chunkSize);
        Debug.Log("Chunk: " + nbChunksGeneres);
        
        foreach(Beat beat in beatsInChunk)
        {
            Debug.Log("Puissance: " + beat.puissance);
        }
    }


}