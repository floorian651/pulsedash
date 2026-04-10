using UnityEngine;
using UnityEngine.UI; //slider
using System.Collections; // IEnumerator
using TMPro;  // indispensable pour TextMeshProUGUI


public class PanelMenu : MonoBehaviour
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

    void InitMenu()
{       
    Debug.Log("Créer le panel");

    // PANEL PRINCIPAL
    Transform panel = UIBuilder.CreatePanel();

    // CONTENEURS PRINCIPAUX
    Transform middleArea = UIconteneur.CreateMiddleArea(panel, 80f);
    Transform leftContainer = UIconteneur.CreateLeftContainer(middleArea);
    Transform centerRightContainer = UIconteneur.CreateCenterRightContainer(middleArea);

    // BARRE AUDIO EN BAS
    Transform bottomBar = UIconteneur.CreateBottomAudioBar(panel, 80f);

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

    // TEXTE CENTRAL
    TextMeshProUGUI messageText = UIBuilder.CreerTexte(centerRightContainer);
    Context.Initialize(audioSource, messageText);

    // BOUTON LANCER LE JEU
    SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
    if (sceneloader != null)
    {
        GameObject launchGameGO = Instantiate(launchGameButtonPrefab, leftContainer);
        Button launchGameBtn = launchGameGO.GetComponent<Button>();

        launchGameBtn.onClick.AddListener(() =>
        {
            if (!Context.TryGetAudioSource(out AudioSource source) || source.clip == null)
            {
                PopupManager.Show("Aucune musique sélectionnée");
                return;
            }

            if (SessionData.Instance != null)
            {
                PopupManager.Show("Le jeu va commencer!");
                SessionData.Instance.titre = source.clip.name;
            }

            sceneloader.LoadSceneByName("GameplaySceneLocal");
        });
    }

    // TOP BAR (recherche)
    Transform topBar = UIBuilder.CreateTopBar(panel);

    // CHARGEMENT MUSIQUES
    audioCache.LoadAllMusicTestUtilisateur();

    // PLAYLISTS À GAUCHE
    PlaylistUI.AfficherBoutonPlaylist(audioCache.clips, leftContainer, playlistItemPrefab, playlistName =>
    {
        UIBuilder.ShowMusiquesPlaylistInContainer(averageButtonPrefab, audioCache.clips, playlistName, centerRightContainer);
    });

    PlaylistUI.CreateButtonCreerPlaylist(averageButtonPrefab, leftContainer, playlistName =>
    {
        PlaylistManager pm = FindObjectOfType<PlaylistManager>();
        if (pm != null)
        {
            pm.CreatePlaylist(playlistName);

            PlaylistUI.AfficherBoutonPlaylist(audioCache.clips, leftContainer, playlistItemPrefab, playlistName =>
            {
                UIBuilder.ShowMusiquesPlaylistInContainer(averageButtonPrefab, audioCache.clips, playlistName, centerRightContainer);
            });
        }
    });

    // BARRE DE RECHERCHE
    SearchUI searchUI = SearchUI.Create(topBar, Context);
    searchUI.Init(audioCache.clips, playlistItemPrefab);

    // Les résultats de recherche vont dans centerRightContainer
    searchUI.SetResultsContainer(centerRightContainer);
}

   
}
