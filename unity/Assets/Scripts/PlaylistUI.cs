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

    public static void CreateButtonCreerPlaylist(GameObject averageButtonPrefab, Transform parent, Action<string> onPlaylistCreated)
    {   
        Debug.Log("Création du bouton pour créer les playlists");
        Button createPlaylistButton = Bouton.CreateButtonEditor(parent, averageButtonPrefab, "Créer playlist", () =>
        {
            OpenCreatePlaylistPopup(onPlaylistCreated);
        });

        if (createPlaylistButton != null)
        {
            createPlaylistButton.transform.SetAsFirstSibling();

            RectTransform rt = createPlaylistButton.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0f, 50f);
                rt.sizeDelta = new Vector2(250, 200);
                            }

            TextMeshProUGUI label = createPlaylistButton.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            if (label != null)
            {
                RectTransform labelRt = label.GetComponent<RectTransform>();

                labelRt.anchorMin = new Vector2(0.5f, 0.5f);
                labelRt.anchorMax = new Vector2(0.5f, 0.5f);
                labelRt.pivot = new Vector2(0.5f, 0.5f);
                labelRt.anchoredPosition = Vector2.zero;

                label.enableWordWrapping = false;

                label.alignment = TextAlignmentOptions.Center;
                labelRt.anchoredPosition = new Vector2(-40f, -5f);
                label.margin = Vector4.zero;
            }
  }
                
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
        

    Transform creerPlaylistButton = null;
    foreach (Transform child in resultsContainer){
        if (child.CompareTag("CreerPlaylistButton"))
        {
            creerPlaylistButton = child;
        }

        if (!child.CompareTag("CreerPlaylistButton") && !child.CompareTag("LancerJeuButton")){
            UnityEngine.Object.Destroy(child.gameObject);
            Debug.Log("Destroy children");
        }

        }   
    
    const float topPadding = 100f;
    const float verticalGap = 50f;
    float currentY = -topPadding;

	    // Parcourir la liste des playlist et afficher un bouton pour chaque playlist
	    foreach (var playlist in toutesLesPlaylists)
	    {
	        GameObject boutonGO = UnityEngine.Object.Instantiate(playlistItemPrefab, resultsContainer);
	        UIBuilder.ApplyMontserratFontRecursive(boutonGO.transform);
	        Button btn = boutonGO.GetComponent<Button>();

	        bool forceRaycastTargets =
	            playlistItemPrefab != null &&
	            playlistItemPrefab.name.IndexOf("AverageButtonTransparent", StringComparison.OrdinalIgnoreCase) >= 0;

	        // Taille des items playlist
	        RectTransform boutonRT = boutonGO.GetComponent<RectTransform>();

        boutonRT.anchorMin = new Vector2(0.5f, 1f);
        boutonRT.anchorMax = new Vector2(0.5f, 1f);
        //boutonRT.pivot = new Vector2(0.5f, 1f);
        boutonRT.pivot = new Vector2(0.5f, 0.5f);


        if (boutonRT != null)
        {
            boutonRT.sizeDelta = new Vector2(boutonRT.sizeDelta.x,200);
            boutonRT.anchoredPosition = new Vector2(0f, currentY);
        }
	        currentY -=verticalGap;

	        Image background = boutonGO.GetComponent<Image>();
	        if (background != null)
	        {
	            background.raycastTarget = forceRaycastTargets;
	        }

	        if (forceRaycastTargets && btn != null && btn.targetGraphic != null)
	        {
	            btn.targetGraphic.raycastTarget = true;
	        }

	        TextMeshProUGUI labelRaycast = boutonGO.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
	        if (labelRaycast != null)
	        {
            labelRaycast.raycastTarget = false;
        }


        TextMeshProUGUI label = boutonGO.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = playlist.name;
            UIBuilder.ApplyMontserratFont(label);
            RectTransform labelRT = label.GetComponent<RectTransform>();
            if (labelRT != null && showActions)
            {
                labelRT.offsetMax = new Vector2(-64, labelRT.offsetMax.y);
            }
        }
        Transform chevron = boutonGO.transform.Find("ChevronButton");
        Button chevronBtn = null;
        if (chevron != null)
        {
            chevron.gameObject.SetActive(showActions);
            chevronBtn = chevron.GetComponent<Button>();
            if (chevronBtn == null)
            {
                chevronBtn = chevron.GetComponentInChildren<Button>(true);
            }
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

        // --- ACTIONS PLAYLIST sur le bouton Chevron (suppression du MoreButton) ---
        if (chevronBtn == null)
        {
            Debug.LogWarning($"PlaylistUI: ChevronButton introuvable ou sans Button pour la playlist '{playlist.name}'.");
            continue;
        }

        chevronBtn.onClick.RemoveAllListeners();

        if (averageButtonPrefab != null){ 
        chevronBtn.onClick.AddListener(() =>
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
                    bool found = pm.RemovePlaylist(playlist.name, () =>
                    {
                        PopupManager.Show("Playlist supprimée : " + playlist.name);
                        AfficherBoutonPlaylist(PreviousButtonPrefab, NextButtonPrefab, averageButtonPrefab, clips, resultsContainer, containerListeMusique, playlistItemPrefab, onClick);
                    });
                    if (!found)
                        PopupManager.Show("Playlist introuvable");
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
                Button rootBtn = item.GetComponent<Button>();
                subButton.onClick.AddListener(() =>
                {
                    if (rootBtn != null) rootBtn.interactable = false;
                    string trackTitle = track.title;
                    pm.RemoveTrackFromPlaylist(nomplaylist, trackTitle,
                        () =>
                        {
                            PopupManager.Show("Musique supprimée : " + trackTitle);
                            foreach (Transform child in resultsContainer)
                                UnityEngine.Object.Destroy(child.gameObject);
                            AfficherMusiquesParPlaylist(averageButtonPrefab, musicItemPrefab, clips, nomplaylist, resultsContainer);
                        },
                        () => { if (rootBtn != null) rootBtn.interactable = true; });
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
