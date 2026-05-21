using UnityEngine;
using System.Collections.Generic;
using System;

public class GenerateurNiveau : MonoBehaviour
{
    [Header("Paramètres de génération")]
    private int randomSeed; // Graine aléatoire pour la génération du niveau.
    private float distanceDestruction = 30f; // distance derrière le joueur avant destruction
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
    public GameObject FinishLinePrefab; // Préfab de la ligne d'arrivée
    public GameObject obstacleLevel4; // Préfab pour obstacle difficile
    public GameObject obstacleLevel3; // Préfab pour obstacle de difficulté moyenne
    public GameObject obstacleLevel2; // Préfab pour obstacle de difficulté facile
    public GameObject obstacleLevel1; // Préfab pour obstacle très facile    
    private float groundHeight = 0.0f; // Indique la coordonnée y du sol (actuellement)
    private float seuilLevel0;
    private float seuilLevel1;
    private float seuilLevel2;
    private float seuilLevel3;
    private float lastGeneratedZ = 0f;
    private float generationDistance = 30f;

    public GameObject backToMenuPrefab;

    public enum Difficulty
    {
        Lazy, // 30% d'énergie en plus
        Easy, // 15% d'énergie en plus
        Crazy // Pas de bonus d'énergie
        }
    public Difficulty difficulty = Difficulty.Lazy; // Difficulté par défaut

    private int compt = 0;

    [SerializeField] private GameObject[] decosDroite;
    [SerializeField] private GameObject[] decosGauche;


    public GameObject pulser;

    [SerializeField] private JsonDAO jsonDAO;
    [SerializeField] private GameSessionDAO gameSessionDAO;

    void Start()
    {
        ReturnToMenuButton.prefab = backToMenuPrefab;
        ReturnToMenuButton.Create();

        if (SessionData.Instance != null)
        {
            string mode = SessionData.Instance.mode;
            difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), mode, true);
            analyse_rythme = SessionData.Instance.titre;
        }

        if (jsonDAO == null)
            jsonDAO = FindObjectOfType<JsonDAO>();

        if (gameSessionDAO == null)
            gameSessionDAO = FindObjectOfType<GameSessionDAO>();

        PopupManager.ShowLoading("Génération du niveau en cours...");

        if (jsonDAO != null)
            jsonDAO.FetchLevelFromTitle(analyse_rythme, OnLevelReady);
        else
            Debug.LogError("JsonDAO not found in scene");
    }

    void OnLevelReady(MusicData levelData)
    {
        PopupManager.HideLoading();

        if (levelData == null)
        {
            Debug.LogError("Échec du chargement du niveau depuis le backend.");
            PopupManager.Show("Erreur : impossible de charger le niveau.");
            return;
        }
        
        if (levelData.beats == null || levelData.beats.Length == 0)
        {
            Debug.LogError("Données du niveau invalides : beats manquants");
            PopupManager.Show("Erreur : données du niveau incomplètes.");
            return;
        }
        
        data = levelData;
        Debug.Log($"[GenerateurNiveau] Niveau reçu: durée={data.duration}s, beats={data.beats.Length}, tempo={data.tempo}");

        if (gameSessionDAO == null)
        {
            Debug.LogError("gameSessionDAO est null ! Impossible de créer la session.");
            PreparePlayer();
            GenerateLevel();
            return;
        }

        gameSessionDAO.StartSession(analyse_rythme, sessionId =>
        {
            if (sessionId != null && SessionData.Instance != null)
                SessionData.Instance.sessionId = sessionId;
            else
                Debug.LogWarning("Session non créée — le score ne sera pas enregistré.");

            PreparePlayer();
            GenerateLevel();
        });
    }

    void Update()
    {
        if (player == null || data == null) return;

        float playerZ = player.transform.position.z;

        // Génération propre basée sur distance
        if (playerZ + generationDistance > lastGeneratedZ)
        {
            generateDeco((int)lastGeneratedZ, (int)(lastGeneratedZ + generationDistance));
            generateGround((int)lastGeneratedZ, (int)(lastGeneratedZ + generationDistance));

            lastGeneratedZ += generationDistance;
        }

        float limiteZ = playerZ - distanceDestruction;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (child.CompareTag("Deco") || child.CompareTag("obstacle") || child.CompareTag("sol") || child.CompareTag("pulser"))
            {
                if (child.position.z < limiteZ)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    void PreparePlayer()
    {
        if (player == null)
        {
            Debug.LogError("[PreparePlayer] player est null!");
            return;
        }
        
        player.transform.position = new Vector3(0, 1, -5);
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            float energieMax = GetMusicDuration() * playerScript.decreaseSpeed;
            switch(difficulty)
            {
                case Difficulty.Lazy:
                    energieMax *= 1.3f;
                    break;
                case Difficulty.Easy:
                    energieMax *= 1.15f;
                    break;
                case Difficulty.Crazy:
                    energieMax *= 1.05f;
                    break;
            }
            playerScript.SetEnergyMax(energieMax);
            Debug.Log($"[PreparePlayer] Player configuré. Énergie max={energieMax}");
        }
        else
        {
            Debug.LogWarning("[PreparePlayer] Player script non trouvé");
        }
    }
    public float GetMusicDuration()
    {
        return data.duration;
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        difficulty = newDifficulty;
    }
    
    public void GenerateLevel()
    {
        if (data == null || data.beats == null || data.beats.Length == 0)
        {
            Debug.LogError("[GenerateLevel] Données du niveau invalides!");
            return;
        }
        
        if (player == null)
        {
            Debug.LogError("[GenerateLevel] player est null!");
            return;
        }

        vitesse = player.GetComponent<PlayerMovementE5>().GetSpeed();
        chunkSize = vitesse * 0.25f;
        offsetZ = vitesse * 2.0f;

        randomSeed = (int)data.duration;
        ClearLevel();
        Debug.Log($"[GenerateLevel] Début. Vitesse={vitesse}, ChunkSize={chunkSize}, Offset={offsetZ}");


        for (float z = -20; z <= 0; z += 2)
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

         // Génération de la ligne d'arrivée
        if (FinishLinePrefab == null)
        {
            UnityEngine.Debug.LogError("FinishLinePrefab non assigné");
            return;
        }

        Vector3 posFinish = new Vector3(0, groundHeight + 1.5f, offsetZ + nbChunksGeneres * chunkSize);
        // Vector3 posFinish = new Vector3(0, groundHeight + 1.5f, -10);    // FinishLine put at the beginning of the level for testing purposes
        Quaternion rotFinish = Quaternion.Euler(0, -25, 0);

        GameObject finishLine = Instantiate(FinishLinePrefab, posFinish, rotFinish);
        finishLine.transform.localScale = new Vector3(6, 6, 6);
        finishLine.transform.parent = this.transform;

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
            //generateChunk(GroundPrefab);
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
                //generateChunk(GroundPrefab);
                return;
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
        }}
    

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

    private void generateGround(int positionD, int positionF){
        for (int z = positionD; z < positionF; z += 2)
        {
            Vector3 pos = new Vector3(0, groundHeight, z);
            GameObject newGround = Instantiate(GroundPrefab, pos, GroundPrefab.transform.rotation);
            // Parent
            newGround.transform.parent = this.transform;
        }
    }

    private void generateDeco(int positionD, int positionF){

        int dureeMusique = (int)data.duration;
        //int tailleNiveau = (int)vitesse * dureeMusique;

        int indexD = UnityEngine.Random.Range(0, decosDroite.Length);
        int indexG = UnityEngine.Random.Range(0, decosGauche.Length);
            
        GameObject decoRandomD = decosDroite[indexD];
        GameObject decoRandomG = decosGauche[indexG];

        //for (int i = 0; i < tailleNiveau; i += 5){
        for (int i = positionD; i < positionF; i += 5){      
            

            Vector3 posD = decoRandomD.transform.position + new Vector3(0, groundHeight, i);
            Vector3 posG = decoRandomG.transform.position+ new Vector3(0, groundHeight, i); ;

            GameObject newDecoD = Instantiate(decoRandomD, posD,  decoRandomD.transform.rotation);
            GameObject newDecoG = Instantiate(decoRandomG, posG,  decoRandomG.transform.rotation);
            
            newDecoD.transform.SetParent(this.transform, true);
            newDecoG.transform.SetParent(this.transform, true);

        }

    }

 }
