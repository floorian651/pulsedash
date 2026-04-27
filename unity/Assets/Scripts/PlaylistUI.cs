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

    public static void AfficherBoutonPlaylist(GameObject averageButtonPrefab, List<AudioClip> clips, Transform resultsContainer, Transform containerListeMusique, GameObject playlistItemPrefab, Action<string> onClick, bool showActions = true)
    {
        AfficherBoutonPlaylist(averageButtonPrefab, clips, resultsContainer, containerListeMusique, playlistItemPrefab, null, onClick, showActions);
    }

    public static void AfficherBoutonPlaylist(GameObject averageButtonPrefab, List<AudioClip> clips, Transform resultsContainer, Transform containerListeMusique, GameObject playlistItemPrefab, GameObject musicItemPrefab, Action<string> onClick, bool showActions = true)
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

                              
                pm.LancerPlaylist(averageButtonPrefab, musicItemPrefab, track, clips, TracktoutesLesMusiques, playlist.name, containerListeMusique); 
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

                AfficherBoutonPlaylist(averageButtonPrefab, clips, resultsContainer, containerListeMusique, playlistItemPrefab, onClick);
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

        // Nettoyer les anciens résultats
        foreach (Transform child in resultsContainer)
            UnityEngine.Object.Destroy(child.gameObject);

        //Récupérer la liste de toutes les musiques de la playlist sélectionnée
        List<Track> TracktoutesLesMusiques = playlist_recherche.tracks;

        foreach (var track in TracktoutesLesMusiques)
        {
            GameObject boutonGO;

            if (musicItemPrefab != null)
            {
                boutonGO = InstantiateMusicItemRow(musicItemPrefab, resultsContainer, MusicItemListHeight);
                if (boutonGO != null)
                {
                    boutonGO.name = "MusicItem_" + track.title;
                }
            }
            else
            {
                boutonGO = new GameObject("ResultButton");
                boutonGO.transform.SetParent(resultsContainer, false);
            }

            // Label
            TextMeshProUGUI txt = null;
            var labelTf = boutonGO != null ? boutonGO.transform.Find("Label") : null;
            if (labelTf != null)
            {
                txt = labelTf.GetComponent<TextMeshProUGUI>();
            }
            if (txt == null)
            {
                txt = boutonGO.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (txt != null)
            {
                txt.text = track.title;
                txt.font = LoadMontserratFont();
                txt.fontSize = 16;
                txt.color = new Color(0.12f, 0.15f, 0.2f, 1f);
                txt.alignment = TextAlignmentOptions.MidlineLeft;
                txt.textWrappingMode = TextWrappingModes.NoWrap;
                txt.overflowMode = TextOverflowModes.Ellipsis;
            }

            // Bouton "remove" — on réutilise le bouton droit du prefab si présent
            Button removeBtn = null;
            TextMeshProUGUI removeTxt = null;
            var chevronTf = boutonGO != null ? boutonGO.transform.Find("ChevronButton") : null;
            if (chevronTf == null && boutonGO != null) chevronTf = boutonGO.transform.Find("PlayButton");
            if (chevronTf != null)
            {
                removeBtn = chevronTf.GetComponent<Button>();
                removeTxt = chevronTf.GetComponentInChildren<TextMeshProUGUI>();
            }

            if (removeBtn == null)
            {
                // Fallback si pas de bouton droit dans le prefab
                GameObject addBtnGO = new GameObject("RemoveButton");
                addBtnGO.transform.SetParent(boutonGO.transform, false);

                Image addImg = addBtnGO.AddComponent<Image>();
                addImg.color = new Color32(0x4D, 0x88, 0xFF, 0xFF);

                removeBtn = addBtnGO.AddComponent<Button>();

                RectTransform addRT = addBtnGO.GetComponent<RectTransform>();
                addRT.anchorMin = new Vector2(1, 0);
                addRT.anchorMax = new Vector2(1, 1);
                addRT.pivot = new Vector2(1, 0.5f);
                addRT.sizeDelta = new Vector2(24, 0);
                addRT.anchoredPosition = new Vector2(-5, 0);

                GameObject addTextGO = new GameObject("Text");
                addTextGO.transform.SetParent(addBtnGO.transform, false);
                removeTxt = addTextGO.AddComponent<TextMeshProUGUI>();
                removeTxt.font = LoadMontserratFont();
                removeTxt.alignment = TextAlignmentOptions.Center;
                RectTransform addTxtRT = addTextGO.GetComponent<RectTransform>();
                addTxtRT.anchorMin = Vector2.zero;
                addTxtRT.anchorMax = Vector2.one;
                addTxtRT.offsetMin = Vector2.zero;
                addTxtRT.offsetMax = Vector2.zero;
            }

            if (removeTxt != null)
            {
                removeTxt.text = "-";
                removeTxt.fontSize = 16;
                removeTxt.color = Color.white;
            }

            if (removeBtn != null)
            {
                removeBtn.onClick.AddListener(() =>
                {
                    pm.RemoveTrackFromPlaylist(nomplaylist, track.title);
                    PopupManager.Show("Musique supprimée : " + track.title);
                });
            }
        }

        var containerRT = resultsContainer as RectTransform;
        if (containerRT != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRT);
        }
    }

    public static GameObject InstantiateMusicItemRow(GameObject prefab, Transform parent, float rowHeight)
    {
        if (prefab == null) return null;

        var rowGO = new GameObject("MusicItemRow", typeof(RectTransform), typeof(LayoutElement));
        rowGO.transform.SetParent(parent, false);

        var rowLE = rowGO.GetComponent<LayoutElement>();
        rowLE.minHeight = rowHeight;
        rowLE.preferredHeight = rowHeight;
        rowLE.flexibleHeight = 0f;
        rowLE.preferredWidth = -1;

        var item = UnityEngine.Object.Instantiate(prefab, rowGO.transform, false);

        // Assure que les boutons sont "au-dessus" (visuel + clic) même si le label chevauche leur zone.
        var label = item.transform.Find("Label")?.GetComponent<TMP_Text>();
        if (label != null)
        {
            label.raycastTarget = false;
        }

        var playButtonTf = item.transform.Find("PlayButton");
        if (playButtonTf != null) playButtonTf.SetAsLastSibling();

        var addToPlaylistButtonTf = item.transform.Find("AddToPlaylistButton");
        if (addToPlaylistButtonTf != null) addToPlaylistButtonTf.SetAsLastSibling();


        var itemRT = item.GetComponent<RectTransform>();
        if (itemRT != null)
        {
            float prefabHeight = Mathf.Max(1f, itemRT.sizeDelta.y);
            float scale = Mathf.Clamp(rowHeight / prefabHeight, 0.05f, 1f);
            itemRT.anchorMin = new Vector2(0.5f, 0.5f);
            itemRT.anchorMax = new Vector2(0.5f, 0.5f);
            itemRT.pivot = new Vector2(0.5f, 0.5f);
            itemRT.anchoredPosition = Vector2.zero;
            itemRT.localScale = new Vector3(scale*MusicItemRowScale, scale*MusicItemRowScale, 1f);
        }

        return item;
    }

     /*public static GameObject InstantiateMusicItemRow(GameObject prefab, Transform parent, float rowHeight)
    {
        if (prefab == null) return null;

        var rowGO = new GameObject("MusicItemRow", typeof(RectTransform), typeof(LayoutElement));
        rowGO.transform.SetParent(parent, false);

        var rowLE = rowGO.GetComponent<LayoutElement>();
        float scaledRowHeight = rowHeight * MusicItemRowScale;
        rowLE.minHeight = scaledRowHeight;
        rowLE.preferredHeight = scaledRowHeight;
        rowLE.flexibleHeight = 0f;
        rowLE.preferredWidth = -1;

        var item = UnityEngine.Object.Instantiate(prefab, rowGO.transform, false);

        var itemRT = item.GetComponent<RectTransform>();
        if (itemRT != null)
        {
            itemRT.anchorMin = new Vector2(0.5f, 0.5f);
            itemRT.anchorMax = new Vector2(0.5f, 0.5f);
            itemRT.pivot = new Vector2(0.5f, 0.5f);
            itemRT.anchoredPosition = Vector2.zero;
            itemRT.localScale = new Vector3(MusicItemRowScale, MusicItemRowScale, 1f);
        }

        return item;
    }*/
}
