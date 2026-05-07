using UnityEngine;
using UnityEngine.UI; //slider
using System.Collections; // IEnumerator
using TMPro;  // indispensable pour TextMeshProUGUI


public class PanelMenu : MonoBehaviour
{
    public AudioCache audioCache;
    private AudioSource audioSource;
    public Context Context;
    public Slider sliderPrefab;

    public GameObject playPauseButtonPrefab;
    public GameObject playlistItemPrefab;
    public GameObject musicItemPrefab;
    public GameObject playlistmusicItemPrefab;
    public GameObject launchGameButtonPrefab;
    public GameObject averageButtonPrefab;
    public GameObject NextButtonPrefab;
    public GameObject PreviousButtonPrefab;
    public GameObject averageButtonTransparent;
    public GameObject creerPlaylistButton;


    
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
        // Enregistrer l'AudioSource dans le Context pour que MusicButton puisse le trouver
        Context.Initialize(audioSource, null);

        InitMenu();
    }

void InitMenu()
{       
    Debug.Log("Créer le panel");
    UIBuilder.SetTMPDefaultFontToMontserrat();

    // PANEL PRINCIPAL
    Transform panel = UIBuilder.CreatePanel();

    // CONTENEURS PRINCIPAUX
    Transform middleArea = UIconteneur.CreateMiddleArea(panel, 0f);
    Transform leftContainer = UIconteneur.CreateLeftContainer(middleArea);
    Transform centerRightContainer = UIconteneur.CreateCenterRightContainer(middleArea);

    // BARRE AUDIO EN BAS
    Transform bottomBar = UIconteneur.CreateBottomAudioBar(panel, 90f);

    // Slider musique (caché au départ)
    SliderMusique sliderMusique = SliderMusiqueFactory.Create(bottomBar, sliderPrefab, Context);
    Context.SetSliderMusique(sliderMusique);
    sliderMusique.gameObject.SetActive(false);

    // Bouton play/pause (caché au départ)
    GameObject playPauseGO = Object.Instantiate(playPauseButtonPrefab, bottomBar);
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
    Context.SetPlayPauseButton(playPauseGO);
    playPauseGO.SetActive(false);

    // BOUTON LANCER LE JEU
    {
        GameObject launchGameGO = Instantiate(launchGameButtonPrefab, bottomBar);
        Button launchGameBtn = launchGameGO.GetComponent<Button>();
        launchGameBtn.onClick.AddListener(() =>
        {
            GameLauncher launcher = FindObjectOfType<GameLauncher>();
            if (launcher != null)
                launcher.Launch();
            else
                Debug.LogError("GameLauncher introuvable — ajoute-le comme composant dans la scène.");
        });
    }

    // TOP BAR (barre de recherche)
    Transform topBar = UIBuilder.CreateTopBar(panel);

    // CHARGEMENT MUSIQUES
    audioCache.LoadAllMusicTestUtilisateur();

    // S'assurer que les playlists sont chargées avant d'afficher les boutons
    PlaylistManager pm = FindObjectOfType<PlaylistManager>();
    if (pm != null)
    {
        pm.LoadPlaylists();
    }

        // PLAYLISTS À GAUCHE
    
    PlaylistUI.CreateButtonCreerPlaylist(creerPlaylistButton, leftContainer, playlistName =>
    {
        PlaylistManager pm = FindObjectOfType<PlaylistManager>();
        if (pm != null)
        {
            pm.CreatePlaylist(playlistName);

            PlaylistUI.AfficherBoutonPlaylist(PreviousButtonPrefab, NextButtonPrefab, averageButtonPrefab,audioCache.clips, leftContainer, centerRightContainer, playlistItemPrefab, playlistmusicItemPrefab, playlistName =>
            {
                UIBuilder.ShowMusiquesPlaylistInContainer(PreviousButtonPrefab, NextButtonPrefab, averageButtonPrefab,playlistmusicItemPrefab, audioCache.clips, playlistName, centerRightContainer);
            });
        }
    });
    

    // BARRE DE RECHERCHE
    SearchUI searchUI = SearchUI.Create(topBar, Context);
    searchUI.Init(audioCache.clips, playlistItemPrefab, musicItemPrefab,averageButtonTransparent);

    // Les résultats de recherche vont dans centerRightContainer
    searchUI.SetResultsContainer(centerRightContainer);


    PlaylistUI.AfficherBoutonPlaylist( PreviousButtonPrefab,  NextButtonPrefab, averageButtonPrefab, audioCache.clips, leftContainer, centerRightContainer, playlistItemPrefab,playlistmusicItemPrefab, playlistName =>
    {
        UIBuilder.ShowMusiquesPlaylistInContainer(PreviousButtonPrefab, NextButtonPrefab,averageButtonPrefab,playlistmusicItemPrefab, audioCache.clips, playlistName, centerRightContainer);
    });

    // Appliquer Montserrat à tout ce qui existe déjà dans l'UI (labels de prefabs inclus).
    UIBuilder.ApplyMontserratFontRecursive(panel);
}

   
}
