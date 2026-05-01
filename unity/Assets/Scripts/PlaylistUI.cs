using UnityEngine;
using static UnityEngine.Object;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;


public static class PlaylistUI
{   
    private const float MusicItemListHeight = 80f;
    private const float MusicItemRowScale = 4f;
    private static string MontserratPath = "Fonts & Materials/MedievalSharp,Montserrat/MedievalSharp/MedievalSharp-Regular SDF.asset";
    // Autre police "Fonts & Materials/Montserrat-Regular SDF"
    
    private static TMP_FontAsset LoadMontserratFont()
    {
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(MontserratPath);
        if (font == null)
        {
            font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }
        return font;
    }

    public static void CreateButtonCreerPlaylist(GameObject averageButtonPrefab, Transform parent, Action<string> onPlaylistCreated)
    {   
        Debug.Log("Création du bouton pour créer les playlists");
        Bouton.CreateButtonEditor( parent, averageButtonPrefab, "Créer playlist", () =>
        {
            OpenCreatePlaylistPopup(onPlaylistCreated);
        });
        
    }

    private static void OpenCreatePlaylistPopup(Action<string> onPlaylistCreated)
    {
        PopupManager.ShowInput("Nom de la playlist :", (string playlistName) =>
        {
            if (string.IsNullOrEmpty(playlistName))
            {
                PopupManager.Show("Nom invalide !");
                return;
            }
            // Si l'action onPlaylistCreated est non null faire onPlaylistCreated(playlistName)
            // dans MenuGenerator onPlaylistCreated correspond à créer un objet Playlist et le sauvegarder
            onPlaylistCreated?.Invoke(playlistName);
            PopupManager.Show("Playlist créée : " + playlistName);
        });
    }

    public static void AfficherBoutonPlaylist(GameObject PreviousButtonPrefab, GameObject NextButtonPrefab, GameObject averageButtonPrefab, List<AudioClip> clips, Transform resultsContainer, Transform containerListeMusique, GameObject playlistItemPrefab, Action<string> onClick, bool showActions = true)
    {
        AfficherBoutonPlaylist(PreviousButtonPrefab, NextButtonPrefab,averageButtonPrefab, clips, resultsContainer, containerListeMusique, playlistItemPrefab, null, onClick, showActions);
    }

    public static void AfficherBoutonPlaylist(GameObject PreviousButtonPrefab, GameObject NextButtonPrefab,GameObject averageButtonPrefab, List<AudioClip> clips, Transform resultsContainer, Transform containerListeMusique, GameObject playlistItemPrefab, GameObject musicItemPrefab, Action<string> onClick, bool showActions = true)
{
    PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>(); 

    if (pm != null) {
        List<Playlist> toutesLesPlaylists = pm.playlists;
        

    foreach (Transform child in resultsContainer){

        if (!child.CompareTag("AverageButton") && !child.CompareTag("LaunchGameButton")){
            UnityEngine.Object.Destroy(child.gameObject);
            Debug.Log("Destroy childrend");
        }}
        

    // Parcourir la liste des playlist et afficher un bouton pour chaque playlist
    foreach (var playlist in toutesLesPlaylists)
{
    GameObject boutonGO = UnityEngine.Object.Instantiate(playlistItemPrefab, resultsContainer);
    Button btn = boutonGO.GetComponent<Button>();

    TextMeshProUGUI label = boutonGO.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
    if (label != null)
    {
        label.text = playlist.name;
        RectTransform labelRT = label.GetComponent<RectTransform>();
        if (labelRT != null && showActions)
        {
            labelRT.offsetMax = new Vector2(-64, labelRT.offsetMax.y);
        }
    }
    Transform chevron = boutonGO.transform.Find("ChevronButton");
    if (chevron != null)
    {
        chevron.gameObject.SetActive(false);
    }

    btn.onClick.AddListener(() =>
    {
        onClick?.Invoke(playlist.name);
    });

    Playlist playlist_recherche = pm.GetPlaylist(playlist.name);
    if (playlist_recherche == null)
    {
        continue;
    }

    //Récupérer la liste de toutes les musiques de la playlist sélectionnée
    List<Track> TracktoutesLesMusiques = playlist_recherche.tracks;
    
    if (!showActions)
    {
        continue;
    }

    // --- BOUTON "..." (ACTIONS PLAYLIST) ---
    GameObject moreBtnGO = new GameObject("MoreButton");
    moreBtnGO.transform.SetParent(boutonGO.transform, false);

    Image moreImg = moreBtnGO.AddComponent<Image>();
    moreImg.color = new Color32(0x4D, 0x88, 0xFF, 0xFF);

    Button moreBtn = moreBtnGO.AddComponent<Button>();

    RectTransform moreRT = moreBtnGO.GetComponent<RectTransform>();
    moreRT.anchorMin = new Vector2(1, 0.5f);
    moreRT.anchorMax = new Vector2(1, 0.5f);
    moreRT.pivot = new Vector2(1, 0.5f);
    moreRT.sizeDelta = new Vector2(36, 24);
    moreRT.anchoredPosition = new Vector2(-12, 0);

    GameObject moreTextGO = new GameObject("Text");
    moreTextGO.transform.SetParent(moreBtnGO.transform, false);
    TextMeshProUGUI moreTxt = moreTextGO.AddComponent<TextMeshProUGUI>();
    moreTxt.text = "...";
    moreTxt.fontSize = 16;
    moreTxt.color = Color.white;
    moreTxt.alignment = TextAlignmentOptions.Center;
    moreTxt.font = LoadMontserratFont();

    RectTransform moreTxtRT = moreTextGO.GetComponent<RectTransform>();
    moreTxtRT.anchorMin = Vector2.zero;
    moreTxtRT.anchorMax = Vector2.one;
    moreTxtRT.offsetMin = Vector2.zero;
    moreTxtRT.offsetMax = Vector2.zero;

    if (averageButtonPrefab != null){ 
    moreBtn.onClick.AddListener(() =>
    {
        PopupManager.ShowPlaylistActionsPopup(
            playlist.name,
            () =>
            {
                Debug.Log("Lancer la playlist: " + playlist.name);
                Track track = TracktoutesLesMusiques.Find(t => t.order == 0);

                              
                pm.LancerPlaylist(PreviousButtonPrefab, NextButtonPrefab,averageButtonPrefab, musicItemPrefab, track, clips, TracktoutesLesMusiques, playlist.name, containerListeMusique); 
            },
            () =>
            {
                bool removed = pm.RemovePlaylist(playlist.name);
                if (removed)
                {
                    PopupManager.Show("Playlist supprimée : " + playlist.name);
                }
                else
                {
                    PopupManager.Show("Playlist introuvable");
                }

                AfficherBoutonPlaylist(PreviousButtonPrefab, NextButtonPrefab,averageButtonPrefab, clips, resultsContainer, containerListeMusique, playlistItemPrefab, onClick);
            }
        );
    });
    }
}}}
    
    public static void AfficherMusiquesParPlaylist(
        GameObject averageButtonPrefab,
        GameObject musicItemPrefab,
        List<AudioClip> clips,
        string nomplaylist,
        Transform resultsContainer)
    {
        PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>();
        if (pm == null) return;

        // Récupérer la playlist en fonction de son nom
        Playlist playlist_recherche = pm.GetPlaylist(nomplaylist);
        if (playlist_recherche == null) return;

        //Récupérer la liste de toutes les musiques de la playlist sélectionnée
        List<Track> TracktoutesLesMusiques = playlist_recherche.tracks;
        
        var rt = resultsContainer as RectTransform;

        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

        foreach (var track in TracktoutesLesMusiques)
        {

            // Créer une "ligne" de liste, puis mettre le prefab dedans sans le redimensionner (asset inchangé).
		    GameObject item = UIBuilder.InstantiateMusicItemRow(musicItemPrefab, resultsContainer, MusicItemListHeight);

		        // Mettre le nom de la musique
		    TMP_Text txt = item.GetComponentInChildren<TMP_Text>();
		    if (txt != null)
		        txt.text =track.title;

		    	
	        // Récupérer le bouton - du prefab
	        Transform subButtonTransform = item.transform.Find("SubToPlaylistButton");
            if (subButtonTransform != null)
            {   Debug.Log("Bouton subutton créé");
                Button subButton = subButtonTransform.GetComponent<Button>();
                subButton.onClick.AddListener(() =>
                {
                    pm.RemoveTrackFromPlaylist(nomplaylist, track.title);
                    PopupManager.Show("Musique supprimée : " + track.title);
                });
            }

            // Récupérer le bouton Play du prefab
            Transform playButtonTransform = item.transform.Find("PlayButton");
            if (playButtonTransform == null)
            {
                Debug.LogError("PlayButton introuvable dans le prefab MusicItem !");
                continue;
            }

            Button playButton = playButtonTransform.GetComponent<Button>();

            // Ajouter l'action Play
            PanelMenu panelMenu = FindObjectOfType<PanelMenu>();
                playButton.onClick.AddListener(() =>
                {
                if (panelMenu != null && panelMenu.Context.TryGetAudioSource(out AudioSource source))
                {
                    source.clip = clips.FirstOrDefault(c => c.name.ToLower().Contains(track.title.ToLower()));
                }

                if (panelMenu != null)
                {
                    panelMenu.Context.SetSliderVisible(true);
                    panelMenu.Context.SetPlayPauseVisible(true);
                }

                PopupManager.Show("Musique sélectionnée : " + track.title);
            });
	    }

	    var containerRT = resultsContainer as RectTransform;
		    if (containerRT != null)
		    {
		        LayoutRebuilder.ForceRebuildLayoutImmediate(containerRT);
		    }
    }


    

}
