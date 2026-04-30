using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System.Linq;


public static class UIBuilder
{   

    // Créer un panel pour l'interface
    public static Transform CreatePanel()
{   
    Debug.Log("Création du panel activé!!!");

    // Chercher un Canvas existant
    Canvas canvas = Object.FindObjectOfType<Canvas>();

    //  S'il n'existe pas, on en crée un
    if (canvas == null)
    {
        GameObject canvasGO = new GameObject("Canvas", 
            typeof(Canvas), 
            typeof(CanvasScaler), 
            typeof(GraphicRaycaster));

        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution = new Vector2(900f, 600f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

    }

    // Créer l’EventSystem si nécessaire
    if (Object.FindObjectOfType<EventSystem>() == null)
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    // Créer le panel
    GameObject panelGO = new GameObject("MenuPanel", typeof(RectTransform), typeof(Image));
    panelGO.transform.SetParent(canvas.transform, false);

    RectTransform panelRT = panelGO.GetComponent<RectTransform>();

    Image panelImage = panelGO.GetComponent<Image>();
    // Fond transparent pour laisser voir le background du canvas
    panelImage.color = new Color32(0x80, 0x95, 0xFF, 0x00);



    panelRT.anchorMin = Vector2.zero;
    panelRT.anchorMax = Vector2.one;
    panelRT.offsetMin = Vector2.zero;
    panelRT.offsetMax = Vector2.zero;

    return panelGO.transform;
}

    // Créer le conteneur pour la barre de recherche et le menu déroulant
    public static Transform CreateTopBar(Transform parent)
{
    // Créer le GameObject avec RectTransform obligatoire pour l'UI
    GameObject topBarGO = new GameObject("TopBar", typeof(RectTransform));
    topBarGO.transform.SetParent(parent, false);

    // Configurer le RectTransform
    RectTransform rt = topBarGO.GetComponent<RectTransform>();
    // anchorMin=(0,1), anchorMax=(1,1),
    rt.anchorMin = new Vector2(0, 1);
    rt.anchorMax = new Vector2(1, 1);
    rt.pivot = new Vector2(0.5f, 1);
    rt.sizeDelta = new Vector2(0, 100);
    //rt.anchoredPosition = Vector2.zero;
    rt.anchoredPosition = new Vector2(0, -10);


    // Ajouter un fond 
    Image bgImage = topBarGO.AddComponent<Image>();
    // Fond transparent
    bgImage.color = new Color32(0x80, 0x95, 0xFF, 0x00);
    // Évite de bloquer les clics sur les éléments en dessous (image transparente)
    bgImage.raycastTarget = false;


    // Ajouter un layout horizontal pour organiser les éléments enfants
    HorizontalLayoutGroup layout = topBarGO.AddComponent<HorizontalLayoutGroup>();
    layout.childControlWidth = true;
    layout.childForceExpandWidth = false;
    layout.childControlHeight = false;
    layout.childForceExpandHeight = false;
    layout.childAlignment = TextAnchor.UpperLeft;

    layout.spacing = 20;
    layout.padding = new RectOffset(20, 20, 10, 10);

    return topBarGO.transform;
}
    public static void ShowMusiquesPlaylistInContainer(GameObject averageButtonPrefab, GameObject musicItemPrefab, List<AudioClip> clips, string playlistName, Transform mainContent)
    {   
        foreach (Transform child in mainContent)
    {
        UnityEngine.Object.Destroy(child.gameObject);
    }

    // Conteneur principal pour la playlist

    // ScrollRect
    GameObject scrollGO = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect));
    scrollGO.transform.SetParent(mainContent.transform, false);

    RectTransform scrollRT = scrollGO.GetComponent<RectTransform>();
    scrollRT.anchorMin = new Vector2(0.05f, 0.05f);
    scrollRT.anchorMax = new Vector2(0.95f, 0.95f);
    scrollRT.offsetMin = Vector2.zero;
    scrollRT.offsetMax = Vector2.zero;
    scrollRT.sizeDelta = new Vector2(250, 250);


    ScrollRect scroll = scrollGO.GetComponent<ScrollRect>();
    scroll.horizontal = false;

    // Viewport
    GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
    viewportGO.transform.SetParent(scrollGO.transform, false);
    // Fond transparent
    //viewportGO.GetComponent<Image>().color = new Color32(0x80, 0x95, 0xFF, 0x00);

    var img = viewportGO.GetComponent<Image>();
    img.color = new Color32(0x80, 0x95, 0xFF, 20); // léger alpha visible
    img.raycastTarget = false;


    RectTransform viewportRT = viewportGO.GetComponent<RectTransform>();
    viewportRT.anchorMin = Vector2.zero;
    viewportRT.anchorMax = Vector2.one;
    viewportRT.offsetMin = Vector2.zero;
    viewportRT.offsetMax = Vector2.zero;
    

    // Content
    GameObject contentGO = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
    contentGO.transform.SetParent(viewportGO.transform, false);

	VerticalLayoutGroup layout = contentGO.GetComponent<VerticalLayoutGroup>();
	    // Dans les listes, on préfère piloter la hauteur via `LayoutElement` sur les items
	    // (plutôt que la taille du prefab), sinon ils prennent trop de place à l'écran.
	layout.childControlHeight = true;
	layout.childForceExpandHeight = false;
	layout.childControlWidth = true;
	layout.childForceExpandWidth = true;
	layout.spacing = 6;
	layout.childAlignment = TextAnchor.UpperCenter;

    ContentSizeFitter fitter = contentGO.GetComponent<ContentSizeFitter>();
    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;


    RectTransform contentRT = contentGO.GetComponent<RectTransform>();
    contentRT.anchorMin = new Vector2(0, 1);
    contentRT.anchorMax = new Vector2(1, 1);
    contentRT.pivot = new Vector2(0.5f, 1);
    contentRT.anchoredPosition = Vector2.zero;
    contentRT.sizeDelta = new Vector2(0, 0);
    
    scroll.content = contentRT;
    scroll.viewport = viewportRT;
    ClearContainer(contentRT);
    
    // Génération des boutons de playlists
    
    PlaylistUI.AfficherMusiquesParPlaylist(averageButtonPrefab, musicItemPrefab, clips, playlistName,contentRT);

    BoutonNextBeforeInContainer(averageButtonPrefab,clips,playlistName,mainContent);
    }


    public static void BoutonNextBeforeInContainer(GameObject averageButtonPrefab, List<AudioClip> clips, string playlistName, Transform mainContent)
    {
    PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>(); 
    if (pm == null) return;
    
    //Ajout des boutons next et before pour gérer la playlist
    Playlist playlist_recherche = pm.GetPlaylist(playlistName);

    //Récupérer la liste de toutes les musiques de la playlist sélectionnée
    List<Track> TracktoutesLesMusiques = playlist_recherche.tracks;

    // Conteneur horizontal pour aligner Avant / Après sur la même hauteur
    GameObject navGO = new GameObject("NavButtons", typeof(RectTransform));
    navGO.transform.SetParent(mainContent, false);

    HorizontalLayoutGroup hlg = navGO.AddComponent<HorizontalLayoutGroup>();
    hlg.childAlignment = TextAnchor.MiddleCenter;
    hlg.spacing = 10;
    hlg.childForceExpandWidth = false;
    hlg.childForceExpandHeight = false;

    LayoutElement navLE = navGO.AddComponent<LayoutElement>();
    navLE.preferredHeight = 40;
      
    Button prevBtn = Bouton.CreateButtonEditor(navGO.transform, averageButtonPrefab, "<<", () => pm.OnPreviousPressed());
    Button nextBtn = Bouton.CreateButtonEditor(navGO.transform, averageButtonPrefab, ">>", () => pm.OnNextPressed());
    
    }
    public static void ClearContainer(Transform container)
{
    // On parcourt tous les enfants
    for (int i = container.childCount - 1; i >= 0; i--)
    {
        GameObject child = container.GetChild(i).gameObject;
        Object.Destroy(child); // détruit l'objet
    }
}

    public static  TextMeshProUGUI CreerTexte(Transform parent)
    {
        GameObject texteGO = new GameObject("Texte", typeof(TextMeshProUGUI));
        texteGO.transform.SetParent(parent.transform, false);

        TextMeshProUGUI texteTMP = texteGO.GetComponent<TextMeshProUGUI>();

        // Texte par défaut
        texteTMP.text = "Bienvenue!";
        texteTMP.fontSize = 20;
        texteTMP.alignment = TextAlignmentOptions.Center;
        texteTMP.color = new Color(0.918f, 0.937f, 0.969f, 1f);
        texteTMP.enableWordWrapping = true;
        texteTMP.overflowMode = TextOverflowModes.Overflow;


        // Stretch dans le parent
        RectTransform rt = texteGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        rt.sizeDelta = new Vector2(300, 50);

        return texteTMP;
    }

    // Créer le conteneur dans le conteneur topbar pour faire en sorte que le menu déroulant soit en dessous de la barre de recherche
    public static Transform CreateSearchContainer(Transform parent)
{
    GameObject go = new GameObject("SearchContainer");
    go.transform.SetParent(parent, false);
    

    RectTransform rt = go.AddComponent<RectTransform>();

    Image Image = go.AddComponent<Image>();
    // Fond transparent 
    Image.color = new Color32(0x80, 0x95, 0xFF, 0x00);
    // Évite de bloquer les clics (fond transparent)
    Image.raycastTarget = false;
   


    LayoutElement le = go.AddComponent<LayoutElement>();
    le.preferredWidth = 500;
    le.preferredHeight = 30;

    VerticalLayoutGroup vlg = go.AddComponent<VerticalLayoutGroup>();
    vlg.childControlWidth = false;
    vlg.childForceExpandWidth = false;
    vlg.childControlHeight = true;
    vlg.childForceExpandHeight = false;
    vlg.spacing = 4;

    return go.transform;
}


// Créer la barre de recherche
public static TMP_InputField CreateSearchBar(Transform parent)
    {
        // Création du GameObject principal avec RectTransform
        GameObject go = new GameObject("SearchBar", typeof(RectTransform));

        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 32);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -10);
        //rt.anchoredPosition = Vector2.zero;

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 400;
        //le.preferredHeight = 40;
        le.preferredHeight = 30;

        Image bg = go.AddComponent<Image>();
        bg.color = new Color32(17, 17, 17, 255);
        //bg.color = new Color32(255, 255, 255, 120);   //transparent

        TMP_InputField input = go.AddComponent<TMP_InputField>();

        input.customCaretColor = true;
        input.caretColor = Color.white;
        input.caretWidth = 10;

        // Zone de texte
        GameObject textAreaGO = new GameObject("Text Area", typeof(RectTransform));
        textAreaGO.transform.SetParent(go.transform, false);

        RectTransform textAreaRT = textAreaGO.GetComponent<RectTransform>();
        textAreaRT.anchorMin = Vector2.zero;
        textAreaRT.anchorMax = Vector2.one;
        // Padding plus léger pour éviter le clipping vertical
        textAreaRT.offsetMin = new Vector2(14, 6);
        textAreaRT.offsetMax = new Vector2(-14, -6);
        
        textAreaGO.AddComponent<RectMask2D>();

        input.textViewport = textAreaRT;

        // Texte principal
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(textAreaGO.transform, false);

        TMP_Text text = textGO.AddComponent<TextMeshProUGUI>();
        text.fontSize = 20;
        text.color = new Color32(240, 240, 240, 255);
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.enableWordWrapping = false;
        text.extraPadding = true;


        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        input.textComponent = text;

        // Placeholder
        GameObject placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
        placeholderGO.transform.SetParent(textAreaGO.transform, false);

        TMP_Text placeholder = placeholderGO.AddComponent<TextMeshProUGUI>();
        placeholder.text = "Rechercher...";
        placeholder.fontSize = 20;
        placeholder.color = new Color32(120, 120, 120, 255);
        //placeholder.color = new Color32(50, 50, 50, 255); //gris clair

        placeholder.alignment = TextAlignmentOptions.MidlineLeft;
        placeholder.enableWordWrapping = false;
        placeholder.extraPadding = true;


        RectTransform phRT = placeholderGO.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = Vector2.zero;
        phRT.offsetMax = Vector2.zero;

        input.placeholder = placeholder;

        return input;
    }

    // Créer le menu déroulant 
   public static Transform CreateScrollView(Transform parent)
{
    // ----- ScrollRect -----
    GameObject scrollGO = new GameObject("SearchScrollView", typeof(RectTransform));
    scrollGO.transform.SetParent(parent, false);
    scrollGO.transform.SetAsLastSibling(); // faire apparaitre au dessus du fond d'écran

    RectTransform scrollRT = scrollGO.GetComponent<RectTransform>();
    scrollRT.anchorMin = new Vector2(0, 1);
    scrollRT.anchorMax = new Vector2(0, 1);
    scrollRT.pivot = new Vector2(0, 1);
    scrollRT.anchoredPosition = Vector2.zero;

    scrollRT.sizeDelta = new Vector2(500, 160);

    LayoutElement le = scrollGO.AddComponent<LayoutElement>();
    le.preferredWidth = 500;
    le.preferredHeight = 160;


    ScrollRect scroll = scrollGO.AddComponent<ScrollRect>();
    scroll.horizontal = false;

    // ----- Viewport -----
    GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
    viewportGO.transform.SetParent(scrollGO.transform, false);

    RectTransform viewportRT = viewportGO.GetComponent<RectTransform>();
    viewportRT.anchorMin = Vector2.zero;
    viewportRT.anchorMax = Vector2.one;
    viewportRT.offsetMin = Vector2.zero;
    viewportRT.offsetMax = Vector2.zero;

    // Fond transparent viewportGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    viewportGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);

    viewportGO.GetComponent<Mask>().showMaskGraphic = false;

    scroll.viewport = viewportRT;

    // ----- Content -----
    GameObject contentGO = new GameObject("Content", typeof(RectTransform));
    contentGO.transform.SetParent(viewportGO.transform, false);

    RectTransform contentRT = contentGO.GetComponent<RectTransform>();
    contentRT.anchorMin = new Vector2(0, 1);
    contentRT.anchorMax = new Vector2(1, 1);
    contentRT.pivot = new Vector2(0.5f, 1);
    contentRT.anchoredPosition = Vector2.zero;
    contentRT.sizeDelta = new Vector2(0, 0);

    VerticalLayoutGroup layout = contentGO.AddComponent<VerticalLayoutGroup>();
    layout.childAlignment = TextAnchor.UpperLeft;
    layout.spacing = 5;
    layout.childForceExpandHeight = false;
    layout.childForceExpandWidth = true;

    ContentSizeFitter fitter = contentGO.AddComponent<ContentSizeFitter>();
    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

    scroll.content = contentRT;

    return contentGO.transform; // parent pour ajouter les éléments
}
}

