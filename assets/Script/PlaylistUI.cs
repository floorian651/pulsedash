using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;


public static class PlaylistUI
{
    public static void CreateButtonCreerPlaylist(Transform parent, Action<string> onPlaylistCreated)
    {   
        Debug.Log("Création du bouton pour créer les playlists");

        // ----- Créer le bouton -----
        GameObject go = new GameObject("CreateButtonCreerPlaylist", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 30;
        le.preferredWidth = 75;

        // ----- Image du bouton -----
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.75f, 0.5f, 1f, 1f);


        // ----- Bouton UI -----
        Button btn = go.AddComponent<Button>();

        // ----- Texte du bouton -----
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(10, 5);
        textRT.offsetMax = new Vector2(-10, -5);

        TextMeshProUGUI txt = textGO.AddComponent<TextMeshProUGUI>();
        txt.text = "Créer une playlist";
        txt.fontSize = 16;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        // Assigner un font par défaut pour TMP si nécessaire
        if (txt.font == null)
            txt.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        // ----- Listener pour le popup -----
        btn.onClick.AddListener(() =>
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

            onPlaylistCreated?.Invoke(playlistName);
            PopupManager.Show("Playlist créée : " + playlistName);
        });
    }

   public static void AfficherBoutonPlaylist(List<AudioClip> clips, Transform resultsContainer, Action<string> onClick)
{
    PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>(); 

    if (pm != null) {
        List<Playlist> toutesLesPlaylists = pm.playlists;
        

    foreach (Transform child in resultsContainer)
        UnityEngine.Object.Destroy(child.gameObject);


    foreach (var playlist in toutesLesPlaylists)
{
    GameObject boutonGO = new GameObject("PlaylistButton", typeof(RectTransform));

    LayoutElement le = boutonGO.AddComponent<LayoutElement>();
    le.preferredHeight = 40;
    le.minHeight = 40;


    boutonGO.transform.SetParent(resultsContainer, false);

    RectTransform btnRT = boutonGO.GetComponent<RectTransform>();
    //btnRT.sizeDelta = new Vector2(180, 30);

    Button btn = boutonGO.AddComponent<Button>();
    Image img = boutonGO.AddComponent<Image>();
    img.color = new Color(1, 1, 1, 0.2f);

    
    // Texte
    GameObject textGO = new GameObject("Text", typeof(RectTransform));
    textGO.transform.SetParent(boutonGO.transform, false);

    TextMeshProUGUI txt = textGO.AddComponent<TextMeshProUGUI>();
    txt.text = playlist.name;
    txt.fontSize = 15;
    txt.color = Color.black;
    txt.alignment = TextAlignmentOptions.MidlineLeft;
    txt.textWrappingMode = TextWrappingModes.NoWrap;
    txt.overflowMode = TextOverflowModes.Ellipsis;


    RectTransform txtRT = textGO.GetComponent<RectTransform>();
    txtRT.anchorMin = Vector2.zero;
    txtRT.anchorMax = Vector2.one;
    txtRT.offsetMin = new Vector2(10, 5);
    txtRT.offsetMax = new Vector2(-10, -5);

    Debug.Log("Playlist : "+ playlist.name);

    btn.onClick.AddListener(() =>
    {
        onClick?.Invoke(playlist.name);
    });

    // Création du bouton secondaire (ex: supprimer ou action)
    GameObject lancerPlaylistButtonGO = new GameObject("lancerPlaylistButton", typeof(RectTransform));
    lancerPlaylistButtonGO.transform.SetParent(boutonGO.transform, false);

    Button lancerPlaylistBtn = lancerPlaylistButtonGO.AddComponent<Button>();
    Image lancerPlaylistImg = lancerPlaylistButtonGO.AddComponent<Image>();
    lancerPlaylistImg.color = new Color(0.6f, 0, 1f, 1f); // rouge semi-transparent par exemple

    // Taille et position du bouton secondaire (en haut à droite du bouton principal)
    RectTransform lancerPlaylistRT = lancerPlaylistButtonGO.GetComponent<RectTransform>();
    lancerPlaylistRT.anchorMin = new Vector2(1, 1); // coin supérieur droit
    lancerPlaylistRT.anchorMax = new Vector2(1, 1);
    lancerPlaylistRT.pivot = new Vector2(1, 1);
    lancerPlaylistRT.sizeDelta = new Vector2(20, 20); // taille du petit bouton
    lancerPlaylistRT.anchoredPosition = new Vector2(-5, -5); // léger décalage vers l'intérieur

    GameObject textlancerPlaylist = new GameObject("Text", typeof(RectTransform));
    textlancerPlaylist.transform.SetParent(lancerPlaylistBtn.transform, false);

    TextMeshProUGUI txtlancerPlaylist = textlancerPlaylist.AddComponent<TextMeshProUGUI>();
    txtlancerPlaylist.text = ">"; // symbole play
    txtlancerPlaylist.fontSize = 20;
    txtlancerPlaylist.alignment = TextAlignmentOptions.Center;
    txtlancerPlaylist.color = Color.white;

    RectTransform txtRTlancerPlaylist = textlancerPlaylist.GetComponent<RectTransform>();
    txtRTlancerPlaylist.anchorMin = Vector2.zero;
    txtRTlancerPlaylist.anchorMax = Vector2.one;
    txtRTlancerPlaylist.offsetMin = Vector2.zero;
    txtRTlancerPlaylist.offsetMax = Vector2.zero;


    Playlist playlist_recherche = pm.GetPlaylist(playlist.name);

    //Récupérer la liste de toutes les musiques de la playlist sélectionnée
    List<Track> TracktoutesLesMusiques = playlist_recherche.tracks;
    

    // Action du bouton secondaire
    lancerPlaylistBtn.onClick.AddListener(() =>
    {
        Debug.Log("Lancer la playlist: " + playlist.name);
        Track track = TracktoutesLesMusiques.Find(t => t.order == 0);
        pm.LancerPlaylist(playlist.name, track, clips,TracktoutesLesMusiques);
    });

}
    }}

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
        img.color = new Color(1, 1, 1, 0.2f);

        LayoutElement le = boutonGO.AddComponent<LayoutElement>();
        le.preferredHeight = 30; 
        //le.preferredWidth = 100;
        le.flexibleWidth = 1;


        // Texte du bouton
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(boutonGO.transform, false);
        TextMeshProUGUI txt = textGO.AddComponent<TextMeshProUGUI>();
        txt.text = track.title;
        txt.fontSize = 15;
        txt.color = Color.black;
        txt.alignment = TextAlignmentOptions.MidlineLeft;

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
        addImg.color = new Color(0.7f, 0f, 1f, 1f);

        Button addBtn = addBtnGO.AddComponent<Button>();

        RectTransform addRT = addBtnGO.GetComponent<RectTransform>();
        addRT.anchorMin = new Vector2(1, 0);
        addRT.anchorMax = new Vector2(1, 1);
        addRT.pivot = new Vector2(1, 0.5f);
        addRT.sizeDelta = new Vector2(30, 0);
        addRT.anchoredPosition = new Vector2(-5, 0);

        // Texte du bouton +
        GameObject addTextGO = new GameObject("Text");
        addTextGO.transform.SetParent(addBtnGO.transform, false);
        TextMeshProUGUI addTxt = addTextGO.AddComponent<TextMeshProUGUI>();
        addTxt.text = "-";
        addTxt.fontSize = 20;
        addTxt.color = Color.white;
        addTxt.alignment = TextAlignmentOptions.Center;
        txt.textWrappingMode = TextWrappingModes.NoWrap; // pas de retour à la ligne 
        txt.overflowMode = TextOverflowModes.Ellipsis; 
        RectTransform addTxtRT = addTextGO.GetComponent<RectTransform>();
        addTxtRT.anchorMin = Vector2.zero;
        addTxtRT.anchorMax = Vector2.one;
        addTxtRT.offsetMin = Vector2.zero;
        addTxtRT.offsetMax = Vector2.zero;

        btn.onClick.AddListener(() =>
        {   
            pm.LancerPlaylist(nomplaylist, track, clips,TracktoutesLesMusiques);
         
        });
        // --- LISTENER DU BOUTON AJOUT À PLAYLIST ---
        addBtn.onClick.AddListener(() =>
    {   
        pm.RemoveTrackFromPlaylist(nomplaylist, track.title);
        PopupManager.Show("Musique supprimée : " + track.title);
    });

    }}
}
}
