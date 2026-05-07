using UnityEngine;
using System.Collections.Generic;

public class GenerateurNiveau : MonoBehaviour
{
    [Header("Paramètres de génération")]
    private int randomSeed;
    private float distanceDestruction = 30f;
    public GameObject player;
    public string analyse_rythme;
    private float vitesse;
    private float offsetZ;
    private LevelData data;
    private float chunkSize;
    private int espaceEntreObstacles = 4;
    private int chunksDepuisDernierObstacle = 0;
    private int nbChunksGeneres = 0;
    public GameObject GroundPrefab;
    public GameObject FinishLinePrefab;
    public GameObject obstacleLevel4;
    public GameObject obstacleLevel3;
    public GameObject obstacleLevel2;
    public GameObject obstacleLevel1;
    private float groundHeight = 0.0f;
    private float seuilLevel0;
    private float seuilLevel1;
    private float seuilLevel2;
    private float seuilLevel3;
    private float lastGeneratedZ = 0f;
    private float generationDistance = 30f;

    [SerializeField] private GameObject[] decosDroite;
    [SerializeField] private GameObject[] decosGauche;

    public GameObject pulser;

    void Awake()
    {
        data = SessionData.Instance?.levelData;
    }

    void Start()
    {
        UnityEngine.Debug.Log("Générer le niveau");
        ReturnToMenuButton.Create();

        if (data == null)
        {
            UnityEngine.Debug.LogError("LevelData absent de SessionData — niveau non généré.");
            return;
        }

        GenerateLevel();
    }

    void Update()
    {
        if (player == null) return;

        float playerZ = player.transform.position.z;

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
            if (child.CompareTag("Deco") || child.CompareTag("obstacle") || child.CompareTag("sol"))
            {
                if (child.position.z < limiteZ)
                    Destroy(child.gameObject);
            }
        }
    }

    // Appelé par EnergyBar.Awake() — lit directement SessionData en cas d'appel avant Awake()
    public float GetMusicDuration()
    {
        var ld = data ?? SessionData.Instance?.levelData;
        return ld != null ? ld.meta.duration : 0f;
    }

    public void GenerateLevel()
    {
        analyse_rythme = SessionData.Instance.titre;

        vitesse = player.GetComponent<PlayerMovementE5>().GetSpeed();
        chunkSize = vitesse * 0.25f;
        offsetZ = vitesse * 2.0f;

        randomSeed = (int)data.meta.duration;

        ClearLevel();

        for (float z = -20; z <= 0; z += 2)
        {
            Vector3 pos = new Vector3(0, groundHeight, z);
            GameObject newGround = Instantiate(GroundPrefab, pos, GroundPrefab.transform.rotation);
            newGround.transform.parent = this.transform;
        }

        majSeuilPuissance();
        loadChunks();
        generatePulsers(analyse_rythme);
    }

    [ContextMenu("Nettoyer le Niveau")]
    public void ClearLevel()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    private void loadChunks()
    {
        nbChunksGeneres = 0;
        while (nbChunksGeneres * chunkSize < data.meta.duration * vitesse)
        {
            chooseChunk();
            nbChunksGeneres++;
        }
    }

    private void chooseChunk()
    {
        if (chunksDepuisDernierObstacle < espaceEntreObstacles)
        {
            chunksDepuisDernierObstacle++;
            return;
        }

        float tempsDebutChunk = nbChunksGeneres * 0.25f;
        float tempsFinChunk   = (nbChunksGeneres + 1) * 0.25f;
        List<HitData> hitsInChunk = LevelDataHelpers.GetHitsInInterval(data.hits, tempsDebutChunk, tempsFinChunk);

        float puissanceMax = 0f;
        foreach (HitData hit in hitsInChunk)
            if (hit.strength > puissanceMax) puissanceMax = hit.strength;

        if (puissanceMax < seuilLevel0)
            return;
        else if (puissanceMax < seuilLevel1)
        { generateChunk(obstacleLevel1); chunksDepuisDernierObstacle = 0; }
        else if (puissanceMax < seuilLevel2)
        { generateChunk(obstacleLevel2); chunksDepuisDernierObstacle = 0; }
        else if (puissanceMax < seuilLevel3)
        { generateChunk(obstacleLevel3); chunksDepuisDernierObstacle = 0; }
        else
        { generateChunk(obstacleLevel4); chunksDepuisDernierObstacle = 0; }
    }

    private void generateChunk(GameObject obstacle)
    {
        float y = obstacle.transform.position.y;
        Vector3 pos = new Vector3(0, y, offsetZ + nbChunksGeneres * chunkSize);
        GameObject newObstacle = Instantiate(obstacle, pos, obstacle.transform.rotation);
        newObstacle.transform.parent = this.transform;
    }

    private void majSeuilPuissance()
    {
        float puissanceMaxGlobal = LevelDataHelpers.GetMaxStrength(data.hits);
        seuilLevel0 = puissanceMaxGlobal * 0.25f;
        seuilLevel1 = puissanceMaxGlobal * 0.50f;
        seuilLevel2 = puissanceMaxGlobal * 0.75f;
        seuilLevel3 = puissanceMaxGlobal * 0.90f;
    }

    private System.Random random = new System.Random();

    private void generatePulsers(string titreMusique)
    {
        int dureeMusique  = (int)data.meta.duration;
        int tailleNiveau  = (int)vitesse * dureeMusique;
        int nbrPulsers    = titreMusique.Length;
        int distMin       = nbrPulsers > 0 ? tailleNiveau / nbrPulsers : tailleNiveau;
        distMin = Mathf.Max(distMin, (int)(10 * chunkSize));

        int borneInf = (int)(distMin * 0.25f);
        int borneSup = (int)(distMin * 0.75f);

        for (int i = 0; i < tailleNiveau; i += distMin)
        {
            int positionZ = random.Next(i + borneInf, i + borneSup + 1);
            positionZ = (int)(Mathf.RoundToInt(positionZ / chunkSize) * chunkSize);
            Vector3 pos = new Vector3(0, groundHeight, positionZ);
            GameObject newPulser = Instantiate(pulser, pos, pulser.transform.rotation);
            newPulser.transform.parent = this.transform;
        }
    }

    private void generateGround(int positionD, int positionF)
    {
        for (int z = positionD; z < positionF; z += 2)
        {
            Vector3 pos = new Vector3(0, groundHeight, z);
            GameObject newGround = Instantiate(GroundPrefab, pos, GroundPrefab.transform.rotation);
            newGround.transform.parent = this.transform;
        }
    }

    private void generateDeco(int positionD, int positionF)
    {
        int indexD = Random.Range(0, decosDroite.Length);
        int indexG = Random.Range(0, decosGauche.Length);
        GameObject decoRandomD = decosDroite[indexD];
        GameObject decoRandomG = decosGauche[indexG];

        for (int i = positionD; i < positionF; i += 5)
        {
            Vector3 posD = decoRandomD.transform.position + new Vector3(0, groundHeight, i);
            Vector3 posG = decoRandomG.transform.position + new Vector3(0, groundHeight, i);
            GameObject newDecoD = Instantiate(decoRandomD, posD, decoRandomD.transform.rotation);
            GameObject newDecoG = Instantiate(decoRandomG, posG, decoRandomG.transform.rotation);
            newDecoD.transform.SetParent(this.transform, true);
            newDecoG.transform.SetParent(this.transform, true);
        }
    }
}
