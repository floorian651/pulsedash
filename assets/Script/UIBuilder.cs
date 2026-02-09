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

    // 1. Chercher un Canvas existant
    Canvas canvas = Object.FindObjectOfType<Canvas>();

    // 2. S'il n'existe pas, on en crée un proprement
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
    }

    // 3. Créer l’EventSystem si nécessaire
    if (Object.FindObjectOfType<EventSystem>() == null)
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    // 4. Créer le panel
    GameObject panelGO = new GameObject("MenuPanel", typeof(RectTransform), typeof(Image));
    panelGO.transform.SetParent(canvas.transform, false);

    RectTransform panelRT = panelGO.GetComponent<RectTransform>();

    Image panelImage = panelGO.GetComponent<Image>();
    panelImage.color =  new Color(0.78f, 0.65f, 0.88f, 1f); //new Color(0, 0, 0, 0.4f);

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
    rt.anchorMin = new Vector2(0, 1);
    rt.anchorMax = new Vector2(1, 1);
    rt.pivot = new Vector2(0.5f, 1);
    rt.sizeDelta = new Vector2(0, 50);
    //rt.anchoredPosition = Vector2.zero;
    rt.anchoredPosition = new Vector2(0, -10);


    // Ajouter un fond 
    Image bgImage = topBarGO.AddComponent<Image>();
    bgImage.color = new Color(0.78f, 0.65f, 0.88f, 1f);


    // Ajouter un layout horizontal pour organiser les éléments enfants
    HorizontalLayoutGroup layout = topBarGO.AddComponent<HorizontalLayoutGroup>();
    layout.childControlWidth = true;
    layout.childForceExpandWidth = false;
    layout.childAlignment = TextAnchor.MiddleRight;
    layout.spacing = 20;
    layout.padding = new RectOffset(20, 20, 10, 10);

    return topBarGO.transform;
}


    public static Transform CreateLeftMenu(Transform parent)
{
    GameObject go = new GameObject("LeftMenu", typeof(RectTransform));
    go.transform.SetParent(parent, false);

    RectTransform rt = go.GetComponent<RectTransform>();

    Image Image = go.AddComponent<Image>();
    Image.color = new Color(0.78f, 0.65f, 0.88f, 1f);


    // Ancré à gauche, sous la TopBar
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(0, 1);
    rt.pivot = new Vector2(0, 1);

    // Largeur fixe
    rt.sizeDelta = new Vector2(250, 0);

    // ESPACEMENT AVEC LA TOPBAR
    float topBarHeight = 100f;
    float spacing = 100f; // espace souhaité

    rt.offsetMin = new Vector2(0, 0);
    rt.offsetMax = new Vector2(250, -(topBarHeight + spacing));

    // Layout vertical interne
    VerticalLayoutGroup layout = go.AddComponent<VerticalLayoutGroup>();
    layout.childControlHeight = true;
    layout.childForceExpandHeight = false;
    layout.childControlWidth = true;
    layout.childForceExpandWidth = true;
    layout.spacing = 10;
    layout.childAlignment = TextAnchor.UpperCenter;

    return go.transform;
}

    public static Transform CreateMainContent(Transform parent)
{   
    GameObject go = new GameObject("MainContent", typeof(RectTransform));
    go.transform.SetParent(parent, false);

    RectTransform rt = go.GetComponent<RectTransform>();

    Image Image = go.AddComponent<Image>();
    Image.color = new Color(0.75f, 0.65f, 0.9f, 1f); // Lavande


    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1, 1);
    rt.pivot = new Vector2(0, 1);

    // Décalage pour éviter la top bar et le menu gauche
    rt.offsetMin = new Vector2(250, 0);   // marge gauche = largeur du menu
    rt.offsetMax = new Vector2(0, -150);   // marge haut = hauteur top bar

    //  Layout pour empiler les éléments
    var layout = go.AddComponent<VerticalLayoutGroup>();
    layout.childControlHeight = false;   // le bouton contrôle sa hauteur
    layout.childControlWidth = false;    // le bouton contrôle sa largeur
    layout.childForceExpandHeight = false;
    layout.childForceExpandWidth = false;

    layout.spacing = 10;
    layout.childAlignment = TextAnchor.MiddleCenter;

    //  Ajustement automatique de la hauteur
    var fitter = go.AddComponent<ContentSizeFitter>();
    //fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

    return go.transform;
}
    public static void ShowMusiquesPlaylistInContainer(List<AudioClip> clips, string playlistName, Transform mainContent)
    {

    // Conteneur principal pour la playlist

    // ScrollRect
    GameObject scrollGO = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect));
    scrollGO.transform.SetParent(mainContent.transform, false);

    RectTransform scrollRT = scrollGO.GetComponent<RectTransform>();
    scrollRT.anchorMin = new Vector2(0.05f, 0.05f);
    scrollRT.anchorMax = new Vector2(0.95f, 0.95f);
    scrollRT.offsetMin = Vector2.zero;
    scrollRT.offsetMax = Vector2.zero;
    scrollRT.sizeDelta = new Vector2(400, 100);


    ScrollRect scroll = scrollGO.GetComponent<ScrollRect>();
    scroll.horizontal = false;

    // Viewport
    GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
    viewportGO.transform.SetParent(scrollGO.transform, false);
    viewportGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);

    RectTransform viewportRT = viewportGO.GetComponent<RectTransform>();
    viewportRT.anchorMin = Vector2.zero;
    viewportRT.anchorMax = Vector2.one;
    viewportRT.offsetMin = Vector2.zero;
    viewportRT.offsetMax = Vector2.zero;
    

    scroll.viewport = viewportRT;

    // Content
    GameObject contentGO = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
    contentGO.transform.SetParent(viewportGO.transform, false);

    VerticalLayoutGroup layout = contentGO.GetComponent<VerticalLayoutGroup>();
    layout.childControlHeight = true;
    layout.childForceExpandHeight = false;
    layout.childControlWidth = true;
    layout.childForceExpandWidth = true;
    layout.spacing = 10;

    ContentSizeFitter fitter = contentGO.GetComponent<ContentSizeFitter>();
    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;


    RectTransform contentRT = contentGO.GetComponent<RectTransform>();
    contentRT.anchorMin = new Vector2(0, 1);
    contentRT.anchorMax = new Vector2(1, 1);
    contentRT.pivot = new Vector2(0.5f, 1);
    contentRT.anchoredPosition = Vector2.zero;
    contentRT.offsetMin = new Vector2(0, 0);
    contentRT.offsetMax = new Vector2(0, 0);
    


    scroll.content = contentRT;

    // Génération des boutons de playlists
    PlaylistUI.AfficherMusiquesParPlaylist(clips,playlistName,contentRT );

    PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>(); 
    if (pm == null) return;
    
    //Ajout des boutons next et before pour gérer la playlist
    Playlist playlist_recherche = pm.GetPlaylist(playlistName);

    //Récupérer la liste de toutes les musiques de la playlist sélectionnée
    List<Track> TracktoutesLesMusiques = playlist_recherche.tracks;
      
        Button nextBtn = Bouton.CreateButton(mainContent, "Next",new UnityEngine.Vector2(90,40), () => pm.OnNextPressed());
        Button prevBtn = Bouton.CreateButton(mainContent, "Before", new UnityEngine.Vector2(90,40), () => pm.OnPreviousPressed());
    
    
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
        texteTMP.color = Color.white;
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
    Image.color = new Color(0.78f, 0.65f, 0.88f, 1f);


    LayoutElement le = go.AddComponent<LayoutElement>();
    le.preferredWidth = 500;

    VerticalLayoutGroup vlg = go.AddComponent<VerticalLayoutGroup>();
    vlg.childControlWidth = false;
    vlg.childForceExpandWidth = false;
    vlg.childControlHeight = false;
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

        rt.sizeDelta = new Vector2(500, 40);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(-150, -30);

        Image bg = go.AddComponent<Image>();
        bg.color = new Color(1, 1, 1, 0.9f);

        TMP_InputField input = go.AddComponent<TMP_InputField>();

        // Zone de texte
        GameObject textAreaGO = new GameObject("Text Area", typeof(RectTransform));
        textAreaGO.transform.SetParent(go.transform, false);

        RectTransform textAreaRT = textAreaGO.GetComponent<RectTransform>();
        textAreaRT.anchorMin = Vector2.zero;
        textAreaRT.anchorMax = Vector2.one;
        textAreaRT.offsetMin = new Vector2(8, 2);
        textAreaRT.offsetMax = new Vector2(-8, -2);

        textAreaGO.AddComponent<RectMask2D>();

        // Texte principal
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(textAreaGO.transform, false);

        TMP_Text text = textGO.AddComponent<TextMeshProUGUI>();
        text.fontSize = 15;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.MidlineLeft;

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
        placeholder.fontSize = 15;
        placeholder.color = new Color(0, 0, 0, 0.5f);
        placeholder.alignment = TextAlignmentOptions.MidlineLeft;

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

    RectTransform scrollRT = scrollGO.GetComponent<RectTransform>();
    scrollRT.anchorMin = new Vector2(0, 1);
    scrollRT.anchorMax = new Vector2(0, 1);
    scrollRT.pivot = new Vector2(0, 1);
    scrollRT.anchoredPosition = new Vector2(-150, -90);

    scrollRT.sizeDelta = new Vector2(500, 40);

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

    viewportGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
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