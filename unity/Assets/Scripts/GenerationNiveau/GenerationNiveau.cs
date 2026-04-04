using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics; // Pour List

public class GenerateurNiveau : MonoBehaviour
{
    [Header("Paramètres de génération")]
    private int randomSeed; // Graine aléatoire pour la génération du niveau.
    public GameObject player; // Référence au joueur pour récupérer sa vitesse
    public string analyse_rythme;
    private float vitesse; // Sera init quand on l'aura
    private float offsetZ; // Distance avant le début de la musique pour commencer à générer les chunk (parce que délai avant début musique)
    private MusicData data; // Contiendra les données du fichier JSON
    private float chunkSize; // Taille d'un chunk du niveau (sera déterminé par la vitesse du joueur)
    private int espaceEntreObstacles = 4; // Combien de chunks vides entre les obstacles
    private int chunksDepuisDernierObstacle = 0; // Compteur de chunks depuis le dernier obstacle, pour espacer les obstacles
    private int nbChunksGeneres = 0; // Compteur du nombre de chunks générés, pour éviter de générer des chunks trop loin
    public GameObject GroundPrefab; // Préfab du sol
    public GameObject obstacleLevel4; // Préfab pour obstacle difficile
    public GameObject obstacleLevel3; // Préfab pour obstacle de difficulté moyenne
    public GameObject obstacleLevel2; // Préfab pour obstacle de difficulté facile
    public GameObject obstacleLevel1; // Préfab pour obstacle très facile    
    private float groundHeight = 0.0f; // Indique la coordonnée y du sol (actuellement)
    private float seuilLevel0;
    private float seuilLevel1;
    private float seuilLevel2;
    private float seuilLevel3;

    [SerializeField] private GameObject[] decos;


    public GameObject pulser;

   
    void Start(){
        UnityEngine.Debug.Log("Générer le niveau");
        ReturnToMenuButton.Create();
        GenerateLevel();
    }
    //[ContextMenu("Générer le Niveau")] // Permet de lancer via un clic droit sur le script

    public float GetMusicDuration()
    {
        analyse_rythme = SessionData.Instance.titre;
        TextAsset jsonFile = Resources.Load<TextAsset>("JSON/"+analyse_rythme);
        data = JsonUtility.FromJson<MusicData>(jsonFile.text);
        UnityEngine.Debug.Log("Data duration " + data.duration);
        return data.duration;
    }
    
    public void GenerateLevel()
    {   
        // On récupére le titre de la musique
        analyse_rythme = SessionData.Instance.titre;

        // On récupère la vitesse du joueur, nécessaire à la synchro musique/obstacles
        vitesse = player.GetComponent<PlayerMovementE5>().GetSpeed();
        chunkSize = vitesse * 0.25f;
        offsetZ = vitesse * 2.0f; // Il y a 2 secondes de pauses avant le début de la musique

        // On récupère les données du fichier JSON
        TextAsset jsonFile = Resources.Load<TextAsset>("JSON/"+analyse_rythme);
        if (jsonFile == null) {
            UnityEngine.Debug.LogError("Il manque le fichier JSON dans le dossier Resources !");
            return;
        }
        data = JsonUtility.FromJson<MusicData>(jsonFile.text);

        // Permet de générer une graine aléatoire de facon déterministe
        randomSeed = (int)data.duration;

        // On nettoie le niveau avant de générer les nouveaux éléments
        ClearLevel();

        // Générer le sol avant de -10 à 0 en amont du niveau 


        for (float z = -20; z <= 10; z += 2)
        {
            Vector3 pos = new Vector3(0, groundHeight, z);
            GameObject newGround = Instantiate(GroundPrefab, pos, GroundPrefab.transform.rotation);
            // Parent
            newGround.transform.parent = this.transform;
        }

        // Mettre à jour la valeur des seuils des puissances
        majSeuilPuissance();

        loadChunks();

        generatePulsers(analyse_rythme);

        generateDeco();
    }

    // Méthode pour nettoyer les blocs générés
    [ContextMenu("Nettoyer le Niveau")]
    public void ClearLevel()
    {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }


    private void loadChunks()
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
        if(chunksDepuisDernierObstacle < espaceEntreObstacles)
        {
            generateChunk(GroundPrefab);
            chunksDepuisDernierObstacle++;
            return;
        }
        else{
            // On récupère l'intervalle de beats correspondant au chunk à générer
            float tempsDebutChunk = nbChunksGeneres * 0.25f;
            float tempsFinChunk = (nbChunksGeneres + 1) * 0.25f;
            List<Beat> beatsInChunk = data.getBeatsInInterval(tempsDebutChunk, tempsFinChunk);
            
            // On récuère la puissance maximale dans l'intervalle
            float puissanceMax = 0f;
            foreach(Beat beat in beatsInChunk)
            {
                if(beat.puissance > puissanceMax)
                    puissanceMax = beat.puissance;
            }

            if(puissanceMax < seuilLevel0)
            {
                generateChunk(GroundPrefab);
            }
            else if(puissanceMax < seuilLevel1)
            {
                generateChunk(obstacleLevel1);
                chunksDepuisDernierObstacle = 0;
            }
            else if(puissanceMax < seuilLevel2)
            {
                generateChunk(obstacleLevel2);
                chunksDepuisDernierObstacle = 0;
            }
            else if(puissanceMax < seuilLevel3)
            {
                generateChunk(obstacleLevel3);
                chunksDepuisDernierObstacle = 0;
            }
            else
            {
                generateChunk(obstacleLevel4);
                chunksDepuisDernierObstacle = 0;
        }
        }
    }

    private void generateChunk(GameObject obstacle)
    {
        // On génère le chunk à la position correspondante
        float y = obstacle.transform.position.y;
        Vector3 pos = new Vector3(0, y, offsetZ + nbChunksGeneres * chunkSize);
       
        
        GameObject newObstacle = Instantiate(obstacle, pos, obstacle.transform.rotation);
        //newObstacle.transform.localScale = scale;
        newObstacle.transform.parent = this.transform;
    }

    private void majSeuilPuissance(){
        float puissanceMaxGlobal = data.getPuissanceMaxGlobale();

        seuilLevel0 = puissanceMaxGlobal *0.25f;
        seuilLevel1 = puissanceMaxGlobal *0.5f;
        seuilLevel2 = puissanceMaxGlobal *0.75f;
        seuilLevel3 = puissanceMaxGlobal *0.90f;
    }

    private System.Random random = new System.Random();

    private void generatePulsers(string titreMusique){
        
        int dureeMusique = (int)data.duration;
        print("durée :"+dureeMusique);
        int tailleNiveau = (int)vitesse * dureeMusique;
        int nbrPulsers = titreMusique.Length; 
        print("Nombre de pulsers :" + nbrPulsers);
        int distMin = tailleNiveau/nbrPulsers;
        print("DistMin :"+distMin);
        distMin = Mathf.Max(distMin, (int)(10 *chunkSize)); // Par défaut si distMin est inférieur à un chunksize ou prend par défaut 10 chunsize 
        print("DistMin :"+distMin);

        
        int borneInf = (int)(distMin * 0.25f);
        int borneSup = (int)(distMin * 0.75f);

        for (int i = 0; i < tailleNiveau; i += distMin){

            int positionZ = random.Next(i+borneInf,i+borneSup+1);

            // Conversion en chunkSize
            positionZ = (int) (Mathf.RoundToInt(positionZ / chunkSize) * chunkSize);

            Vector3 pos = new Vector3(0, groundHeight, positionZ);

            GameObject newPulser = Instantiate(pulser, pos, pulser.transform.rotation);
            
            newPulser.transform.parent = this.transform;
        }
    }

    private void generateDeco(){

        int dureeMusique = (int)data.duration;
        int tailleNiveau = (int)vitesse * dureeMusique;

        for (int i = 0; i < tailleNiveau; i += 5){

            Vector3 pos = new Vector3(3, groundHeight, i);

            int index = Random.Range(0, decos.Length);
            GameObject decoRandom = decos[index];

            GameObject newDeco = Instantiate(decoRandom, pos,  decoRandom.transform.rotation);
            
            newDeco.transform.SetParent(this.transform, true);

        }

    }


}
