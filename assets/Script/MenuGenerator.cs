using UnityEngine;

public class MenuGenerator : MonoBehaviour
{
    public AudioCache audioCache;
    public static AudioSource audioSource;
    public Slider sliderPrefab;
    private AudioClip clipMusique;  
    private Sprite buttonSprite;

    
    void Start()
    {   
        // Créer un gameobject AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        StartCoroutine(InitMenu());
    }

    IEnumerator InitMenu()
{   
    // Charger tous les fichiers mp3 déjà dans le cache
    yield return StartCoroutine(audioCache.LoadAllCachedMusic());

    Debug.Log("Créer le panel");

    // Créer un panel   
    Transform panel = UIBuilder.CreatePanel();

    // Créer des conteneurs en haut à gauche, en bas à gauche, et en haut au milieu

    Transform topBar = UIBuilder.CreateTopBar(panel);

    Transform leftMenu = UIBuilder.CreateLeftMenu(panel);

    Transform mainContent = UIBuilder.CreateMainContent(panel);

    Debug.Log("Théoriquement conteneurs créés!");

    // Créer une barre de recherche avec menu déroulant constituté des musiques avec un bouton pour les ajouter à une playlist ou les écouter
    SearchUI searchUI = SearchUI.Create(topBar);
    searchUI.Init(audioCache.clips, audioSource);

    // Créer un curseur pour la musique 
    SliderMusiqueFactory.Create(mainContent, sliderPrefab, audioSource);


    // Créer le bouton pour lancer et arrêter une musique sélectionnée
    Bouton.CreateMusicButton(mainContent,  audioSource, audioSource.clip); 

  

    // Afficher les titres des playlists déjà créées avec un bouton pour afficher les musiques dans la playlist sélectionnée
    yield return null; // attendre 1 frame
    PlaylistUI.AfficherBoutonPlaylist(leftMenu, playlistName =>
    {   
        PopupManager.ShowMusiquesPlaylistPopup(audioCache.clips,playlistName);
    });

      // Créer le bouton pour créer une playlist sous la forme d'une pop up 
    yield return null; // attendre 1 frame
    PlaylistUI.CreateButtonCreerPlaylist(leftMenu, (playlistName) =>
{
    PlaylistManager pm = FindObjectOfType<PlaylistManager>();
    if (pm != null)
    {
        pm.CreatePlaylist(playlistName);

    }
});

    
}
}
