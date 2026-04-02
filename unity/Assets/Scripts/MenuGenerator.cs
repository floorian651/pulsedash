using UnityEngine;
using UnityEngine.UI; //slider
using System.Collections; // IEnumerator
using TMPro;  // indispensable pour TextMeshProUGUI


public class MenuGenerator : MonoBehaviour
{
    public AudioCache audioCache;
    private AudioSource audioSource;
    private Context Context;
    public Slider sliderPrefab;

    public GameObject playPauseButtonPrefab;
    public GameObject playlistItemPrefab;
    public GameObject launchGameButtonPrefab;
    public GameObject averageButtonPrefab;


    
    void Start()
    {   
        // Créer un gameobject AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        Context = gameObject.GetComponent<Context>();
        if (Context == null)
        {
            Context = gameObject.AddComponent<Context>();
        }

        //StartCoroutine(InitMenu());
        InitMenu();
    }

    //IEnumerator 
    void InitMenu()
{       
    Debug.Log("Créer le panel");

    // Créer un panel   
    Transform panel = UIBuilder.CreatePanel();

    // Créer les conteneurs 
    Transform middleArea = UIconteneur.CreateMiddleArea(panel, 80f);
    Transform leftContainer = UIconteneur.CreateLeftContainer(middleArea);
    Transform centerContainer = UIconteneur.CreateCenterContainer(middleArea);
    Transform rightContainer = UIconteneur.CreateRightContainer(middleArea);

    TextMeshProUGUI messageText = UIBuilder.CreerTexte(centerContainer);
    Context.Initialize(audioSource, messageText);

    // Créer un curseur pour la musique 
    SliderMusiqueFactory.Create(centerContainer, sliderPrefab, Context);

    // Créer le bouton pour lancer et arrêter une musique sélectionnée

    //Bouton.CreateMusicButton(centerContainer); 
    GameObject playPauseGO = Object.Instantiate(playPauseButtonPrefab, centerContainer);
    Button playPauseBtn = playPauseGO.GetComponent<Button>();
    Image playPauseImg = playPauseGO.GetComponent<Image>();
    if (playPauseImg == null)
    {
        playPauseImg = playPauseGO.GetComponentInChildren<Image>();
    }
    if (playPauseImg != null)
    {
        playPauseImg.color = new Color32(0xAA, 0x00, 0xFF, 0xFF);
    }
    MusicButton mb = playPauseGO.GetComponent<MusicButton>();
    if (mb == null) { mb = playPauseGO.AddComponent<MusicButton>(); }

    SceneLoader sceneloader = FindObjectOfType<SceneLoader>();

    if(sceneloader != null){
        Debug.Log("Créer bouton lancer jeu");
        // Créer un bouton pour lancer la scene du gameplay 

        GameObject launchGameGO = Object.Instantiate(launchGameButtonPrefab, centerContainer);
        Button launchGameBtn = launchGameGO.GetComponent<Button>();
        Image launchGameImg = launchGameGO.GetComponent<Image>();
        if (launchGameImg == null)
        {
            launchGameImg = launchGameGO.GetComponentInChildren<Image>();
        }
        if (launchGameImg != null)
        {
            launchGameImg.color = new Color32(0xAA, 0x99, 0xFF, 0xFF);
        }
        launchGameBtn.onClick.AddListener(() =>
{

        if(Context.TryGetAudioSource(out AudioSource source) && SessionData.Instance != null)
        {
            Debug.Log("Audiosource chargé pour la prochaine scène");
            //SessionData.Instance.audioSource = source;
            SessionData.Instance.titre = source.clip.name;
            Debug.Log(SessionData.Instance.titre);
        }

        sceneloader.LoadSceneByName("EnergyScene"); // Remplacer par GameplayScene 
    }); 

    }
    Transform topBar = UIBuilder.CreateTopBar(panel);
    
    // Charger tous les fichiers mp3 déjà dans le cache
    //yield return StartCoroutine(audioCache.LoadAllCachedMusic());
    audioCache.LoadAllMusicTestUtilisateur();
    
    // Afficher les titres des playlists déjà créées avec un bouton pour afficher les musiques dans la playlist sélectionnée

    PlaylistUI.AfficherBoutonPlaylist(audioCache.clips, leftContainer, playlistItemPrefab, playlistName =>
    {
        UIBuilder.ShowMusiquesPlaylistInContainer( averageButtonPrefab, audioCache.clips, playlistName, rightContainer);
    });

    // Créer le bouton pour créer une playlist sous la forme d'une pop up 
    PlaylistUI.CreateButtonCreerPlaylist(averageButtonPrefab, leftContainer, (playlistName) =>
{
    PlaylistManager pm = FindObjectOfType<PlaylistManager>();
    if (pm != null)
    {
        pm.CreatePlaylist(playlistName);

        // Rafraîchir l’affichage des playlists
        PlaylistUI.AfficherBoutonPlaylist(audioCache.clips, leftContainer, playlistItemPrefab, playlistName =>
        {
            UIBuilder.ShowMusiquesPlaylistInContainer( averageButtonPrefab, audioCache.clips, playlistName, rightContainer);
        });
    }
});

    // Créer une barre de recherche avec menu déroulant constituté des musiques avec un bouton pour les ajouter à une playlist ou les écouter
    SearchUI searchUI = SearchUI.Create(topBar, Context);
    searchUI.Init(audioCache.clips, playlistItemPrefab);

}
}