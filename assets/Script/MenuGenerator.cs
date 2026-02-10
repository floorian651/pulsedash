using UnityEngine;
using UnityEngine.UI; //slider
using System.Collections; // IEnumerator
using TMPro;  // indispensable pour TextMeshProUGUI


public class MenuGenerator : MonoBehaviour
{
    public AudioCache audioCache;
    public static AudioSource audioSource;

    public static TextMeshProUGUI messageText; 
    public Slider sliderPrefab;
    private AudioClip clipMusique;  

    
    void Start()
    {   
        // Créer un gameobject AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        StartCoroutine(InitMenu());
    }

    IEnumerator InitMenu()
{       
    Debug.Log("Créer le panel");

    // Créer un panel   
    Transform panel = UIBuilder.CreatePanel();

    // Générer le main content
    //Transform mainContent = UIBuilder.CreateMainContent(panel);

    //Transform leftContent = UIBuilder.CreateLeftContent(panel);
    Transform middleArea = UIconteneur.CreateMiddleArea(panel, 80f);
    Transform leftContainer = UIconteneur.CreateLeftContainer(middleArea);
    Transform centerContainer = UIconteneur.CreateCenterContainer(middleArea);
    Transform rightContainer = UIconteneur.CreateRightContainer(middleArea);

    messageText = UIBuilder.CreerTexte(centerContainer);

    // Créer un curseur pour la musique 
    SliderMusiqueFactory.Create(centerContainer, sliderPrefab);

    // Créer le bouton pour lancer et arrêter une musique sélectionnée
    Bouton.CreateMusicButton(centerContainer); 


    // Générer le left menu
    //Transform leftMenu = UIBuilder.CreateLeftMenu(panel);

   
    
    Transform topBar = UIBuilder.CreateTopBar(panel);
    

    //yield return null; // attendre 1 frame
    // Charger tous les fichiers mp3 déjà dans le cache
    yield return StartCoroutine(audioCache.LoadAllCachedMusic());

    //Transform rightContent = UIBuilder.CreateRightContent(panel);
    
    // Afficher les titres des playlists déjà créées avec un bouton pour afficher les musiques dans la playlist sélectionnée
    //yield return null; // attendre 1 frame
    PlaylistUI.AfficherBoutonPlaylist(audioCache.clips, leftContainer, playlistName =>
    {   
        UIBuilder.ShowMusiquesPlaylistInContainer(audioCache.clips, playlistName, rightContainer);
        //PopupManager.ShowMusiquesPlaylistPopup(audioCache.clips,playlistName);
    });

    // Créer le bouton pour créer une playlist sous la forme d'une pop up 
    //yield return null; // attendre 1 frame
    PlaylistUI.CreateButtonCreerPlaylist(leftContainer, (playlistName) =>
{
    PlaylistManager pm = FindObjectOfType<PlaylistManager>();
    if (pm != null)
    {
        pm.CreatePlaylist(playlistName);

    }
});

    // Créer une barre de recherche avec menu déroulant constituté des musiques avec un bouton pour les ajouter à une playlist ou les écouter
    SearchUI searchUI = SearchUI.Create(topBar);
    searchUI.Init(audioCache.clips);
    
    

    
}
}
