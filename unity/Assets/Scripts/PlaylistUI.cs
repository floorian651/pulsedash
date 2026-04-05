using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;


public static class PlaylistUI
{   
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
    public static void AfficherBoutonPlaylist(List<AudioClip> clips, Transform resultsContainer, GameObject playlistItemPrefab, Action<string> onClick, bool showActions = true)
{
    PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>(); 

    if (pm != null) {
        List<Playlist> toutesLesPlaylists = pm.playlists;
        

    foreach (Transform child in resultsContainer)
        if (!child.CompareTag("AverageButton")){
            UnityEngine.Object.Destroy(child.gameObject);
        }
        

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

    moreBtn.onClick.AddListener(() =>
    {
        PopupManager.ShowPlaylistActionsPopup(
            playlist.name,
            () =>
            {
                Debug.Log("Lancer la playlist: " + playlist.name);
                Track track = TracktoutesLesMusiques.Find(t => t.order == 0);
                pm.LancerPlaylist(track, clips, TracktoutesLesMusiques);
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

                AfficherBoutonPlaylist(clips, resultsContainer, playlistItemPrefab, onClick);
            }
        );
    });
}}}

    public static void AfficherMusiquesParPlaylist(List<AudioClip> clips, string nomplaylist, Transform resultsContainer)
{   
    PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>(); 

    if (pm != null) {

        // Récupérer la playlist en fonction de son nom
        Playlist playlist_recherche = pm.GetPlaylist(nomplaylist);

    // Nettoyer les anciens résultats
    foreach (Transform child in resultsContainer)
        UnityEngine.Object.Destroy(child.gameObject);


    //Récupérer la liste de toutes les musiques de la playlist sélectionnée
    List<Track> TracktoutesLesMusiques = playlist_recherche.tracks;


    foreach (var track in TracktoutesLesMusiques)
    {
        GameObject boutonGO = new GameObject("ResultButton");
        boutonGO.transform.SetParent(resultsContainer, false);

        Button btn = boutonGO.AddComponent<Button>();
        Image img = boutonGO.AddComponent<Image>();
        img.color = new Color32(0x4D, 0x88, 0xFF, 0xFF);

        LayoutElement le = boutonGO.AddComponent<LayoutElement>();
        le.preferredHeight = 30; 
        le.flexibleWidth = 1;


        // Texte du bouton
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(boutonGO.transform, false);
        TextMeshProUGUI txt = textGO.AddComponent<TextMeshProUGUI>();
        txt.text = track.title;
        txt.fontSize = 15;
        txt.color = Color.black;
        txt.alignment = TextAlignmentOptions.MidlineLeft;
        txt.font = LoadMontserratFont();

        txt.textWrappingMode = TextWrappingModes.NoWrap; // pas de retour à la ligne 
        txt.overflowMode = TextOverflowModes.Ellipsis; // ajoute "..." si trop long

        
        // Position du texte 
        RectTransform txtRT = textGO.GetComponent<RectTransform>(); 
        txtRT.anchorMin = Vector2.zero; 
        txtRT.anchorMax = Vector2.one; 
        txtRT.offsetMin = new Vector2(10, 5); 
        txtRT.offsetMax = new Vector2(-10, -5);

        
        // --- BOUTON ENLEVER MUSIQUE À PLAYLIST ---
        GameObject addBtnGO = new GameObject("RemoveButton");
        addBtnGO.transform.SetParent(boutonGO.transform, false);

        Image addImg = addBtnGO.AddComponent<Image>();
        addImg.color = new Color32(0x4D, 0x88, 0xFF, 0xFF);

        Button addBtn = addBtnGO.AddComponent<Button>();

        RectTransform addRT = addBtnGO.GetComponent<RectTransform>();
        addRT.anchorMin = new Vector2(1, 0);
        addRT.anchorMax = new Vector2(1, 1);
        addRT.pivot = new Vector2(1, 0.5f);
        addRT.sizeDelta = new Vector2(24, 0);
        addRT.anchoredPosition = new Vector2(-5, 0);

        // Texte du bouton -
        GameObject addTextGO = new GameObject("Text");
        addTextGO.transform.SetParent(addBtnGO.transform, false);
        TextMeshProUGUI addTxt = addTextGO.AddComponent<TextMeshProUGUI>();
        addTxt.text = "-";
        addTxt.fontSize = 16;
        addTxt.color = Color.white;
        addTxt.alignment = TextAlignmentOptions.Center;
        addTxt.font = LoadMontserratFont();
        txt.textWrappingMode = TextWrappingModes.NoWrap; // pas de retour à la ligne 
        txt.overflowMode = TextOverflowModes.Ellipsis; 
        RectTransform addTxtRT = addTextGO.GetComponent<RectTransform>();
        addTxtRT.anchorMin = Vector2.zero;
        addTxtRT.anchorMax = Vector2.one;
        addTxtRT.offsetMin = Vector2.zero;
        addTxtRT.offsetMax = Vector2.zero;

        btn.onClick.AddListener(() =>
        {   
            pm.LancerPlaylist(track, clips,TracktoutesLesMusiques);
         
        });
        // --- LISTENER DU BOUTON ENLEVER À PLAYLIST ---
        addBtn.onClick.AddListener(() =>
    {   
        pm.RemoveTrackFromPlaylist(nomplaylist, track.title);
        PopupManager.Show("Musique supprimée : " + track.title);
    });

    }}
}
}
