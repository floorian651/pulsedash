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

        if (!EnsureReferences())
        {
            enabled = false;
            return;
        }

        InitMenu();
    }

    private bool EnsureReferences()
    {
        bool ok = true;

        if (audioCache == null)
        {
            audioCache = FindObjectOfType<AudioCache>();
        }

        if (audioCache == null)
        {
            Debug.LogError("PanelMenu: 'audioCache' n'est pas assigné (et aucun AudioCache trouvé dans la scène).");
            ok = false;
        }

        if (sliderPrefab == null)
        {
            Debug.LogError("PanelMenu: 'sliderPrefab' n'est pas assigné.");
            ok = false;
        }

        if (playPauseButtonPrefab == null)
        {
            Debug.LogError("PanelMenu: 'playPauseButtonPrefab' n'est pas assigné.");
            ok = false;
        }

        if (playlistItemPrefab == null)
        {
            Debug.LogError("PanelMenu: 'playlistItemPrefab' n'est pas assigné.");
            ok = false;
        }

        if (musicItemPrefab == null)
        {
            Debug.LogError("PanelMenu: 'musicItemPrefab' n'est pas assigné.");
            ok = false;
        }

        if (playlistmusicItemPrefab == null)
        {
            Debug.LogError("PanelMenu: 'playlistmusicItemPrefab' n'est pas assigné.");
            ok = false;
        }

        if (averageButtonTransparent == null)
        {
            Debug.LogError("PanelMenu: 'averageButtonTransparent' n'est pas assigné.");
            ok = false;
        }

        if (creerPlaylistButton == null)
        {
            Debug.LogError("PanelMenu: 'creerPlaylistButton' (prefab bouton créer playlist) n'est pas assigné.");
            ok = false;
        }

        if (averageButtonPrefab == null)
        {
            Debug.LogWarning("PanelMenu: 'averageButtonPrefab' n'est pas assigné (certaines actions playlist seront désactivées).");
        }

        if (NextButtonPrefab == null || PreviousButtonPrefab == null)
        {
            Debug.LogWarning("PanelMenu: 'NextButtonPrefab' et/ou 'PreviousButtonPrefab' non assignés (navigation playlist).");
        }

        return ok;
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
    if (sliderMusique != null)
    {
        Context.SetSliderMusique(sliderMusique);
        sliderMusique.gameObject.SetActive(false);
    }

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
    SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
    if (sceneloader != null)
    {
        if (launchGameButtonPrefab == null)
        {
            Debug.LogWarning("PanelMenu: SceneLoader trouvé, mais 'launchGameButtonPrefab' n'est pas assigné (bouton lancer jeu non créé).");
        }
        else
        {
	        GameObject launchGameGO = Instantiate(launchGameButtonPrefab, bottomBar);
	        Button launchGameBtn = launchGameGO.GetComponent<Button>();
	        if (launchGameBtn == null)
	        {
	            Debug.LogError("PanelMenu: launchGameButtonPrefab ne contient pas de composant Button.");
	            return;
	        }

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
	
	        sceneloader.LoadSceneByName("GameplayScene");
	    });
        }
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
