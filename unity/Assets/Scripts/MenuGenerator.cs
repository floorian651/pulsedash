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

        StartCoroutine(InitMenu());
    }

    IEnumerator InitMenu()
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
    Bouton.CreateMusicButton(centerContainer); 

    SceneLoader sceneloader = FindObjectOfType<SceneLoader>();

    if(sceneloader != null){
        Debug.Log("Créer bouton lancer jeu");
        // Créer un bouton pour lancer la scene du gameplay 
        Bouton.CreateButton(centerContainer, "Lancer jeu",new UnityEngine.Vector2(90,40),  () =>
    {   
        if(Context.TryGetAudioSource(out AudioSource source) && SessionData.Instance != null)
        {
            Debug.Log("Audiosource chargé pour la prochaine scène");
            SessionData.Instance.audioSource = source;
        }
        sceneloader.LoadSceneByName("Pulser_animated");
    }); 

    }
   
    
    Transform topBar = UIBuilder.CreateTopBar(panel);
    
    // Charger tous les fichiers mp3 déjà dans le cache
    yield return StartCoroutine(audioCache.LoadAllCachedMusic());

    
    // Afficher les titres des playlists déjà créées avec un bouton pour afficher les musiques dans la playlist sélectionnée
    PlaylistUI.AfficherBoutonPlaylist(audioCache.clips, leftContainer, playlistName =>
    {   
        UIBuilder.ShowMusiquesPlaylistInContainer(audioCache.clips, playlistName, rightContainer);
        
    });

    // Créer le bouton pour créer une playlist sous la forme d'une pop up 
    PlaylistUI.CreateButtonCreerPlaylist(leftContainer, (playlistName) =>
{
    PlaylistManager pm = FindObjectOfType<PlaylistManager>();
    if (pm != null)
    {
        pm.CreatePlaylist(playlistName);

        // Rafraîchir l’affichage des playlists
        PlaylistUI.AfficherBoutonPlaylist(audioCache.clips, leftContainer, playlistName =>
        {
            UIBuilder.ShowMusiquesPlaylistInContainer(audioCache.clips, playlistName, rightContainer);
        });


    }
});


    // Créer une barre de recherche avec menu déroulant constituté des musiques avec un bouton pour les ajouter à une playlist ou les écouter
    SearchUI searchUI = SearchUI.Create(topBar, Context);
    searchUI.Init(audioCache.clips);
    
    

    
}
}
