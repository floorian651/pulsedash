using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;


public class PopupManager : MonoBehaviour
{
    
    private static GameObject popupGO;

    public static void Show(string message)
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("PopupManager : aucun Canvas trouvé !");
            return;
        }

        GameObject popupGO = new GameObject("Popup");
        popupGO.transform.SetParent(canvas.transform, false);

        RectTransform rt = popupGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(300, 60);

        Image bg = popupGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.75f);

        TextMeshProUGUI txt = new GameObject("Text")
            .AddComponent<TextMeshProUGUI>();
        txt.transform.SetParent(popupGO.transform, false);
        txt.text = message;
        txt.fontSize = 20;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;
        txt.textWrappingMode = TextWrappingModes.NoWrap; // pas de retour à la ligne 
        txt.overflowMode = TextOverflowModes.Ellipsis; 

        Object.Destroy(popupGO, 1.5f);
    }

    
    public static void ShowInput(string message, System.Action<string> onConfirm)
    {
        // Supprimer l'ancien popup s'il existe
        if (popupGO != null)
        {
            GameObject.Destroy(popupGO);
        }

        // Canvas
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        // Panel
        popupGO = new GameObject("PopupPanel", typeof(RectTransform));
        popupGO.transform.SetParent(canvasGO.transform, false);
        popupGO.transform.SetAsLastSibling(); // pour qu'elle soit au-dessus du reste de l'interface

        Image bg = popupGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.6f);
        RectTransform rt = popupGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Conteneur central
        GameObject container = new GameObject("Container", typeof(RectTransform));
        container.transform.SetParent(popupGO.transform, false);
        RectTransform contRT = container.GetComponent<RectTransform>();
        contRT.sizeDelta = new Vector2(300, 150);
        contRT.anchoredPosition = Vector2.zero;

        Image contBg = container.AddComponent<Image>();
        contBg.color = new Color(1, 1, 1, 0.95f);

        // Message
        GameObject msgGO = new GameObject("Message", typeof(RectTransform));
        msgGO.transform.SetParent(container.transform, false);
        TextMeshProUGUI msgTxt = msgGO.AddComponent<TextMeshProUGUI>();
        msgTxt.text = message;
        msgTxt.alignment = TextAlignmentOptions.Center;
        msgTxt.fontSize = 18;
        msgTxt.color = Color.black;
        msgTxt.textWrappingMode = TextWrappingModes.NoWrap; // pas de retour à la ligne 
        msgTxt.overflowMode = TextOverflowModes.Ellipsis; 

        RectTransform msgRT = msgGO.GetComponent<RectTransform>();
        msgRT.anchorMin = new Vector2(0, 0.7f);
        msgRT.anchorMax = new Vector2(1, 1);
        msgRT.offsetMin = Vector2.zero;
        msgRT.offsetMax = Vector2.zero;

        // Input field
        GameObject inputGO = new GameObject("InputField", typeof(RectTransform));
        inputGO.transform.SetParent(container.transform, false);
        RectTransform inputRT = inputGO.GetComponent<RectTransform>();
        inputRT.anchorMin = new Vector2(0.1f, 0.3f);
        inputRT.anchorMax = new Vector2(0.9f, 0.6f);
        inputRT.offsetMin = Vector2.zero;
        inputRT.offsetMax = Vector2.zero;

        Image inputBG = inputGO.AddComponent<Image>();
        inputBG.color = Color.white;

        TMP_InputField inputField = inputGO.AddComponent<TMP_InputField>();

        // Text Area (OBLIGATOIRE)
        GameObject textAreaGO = new GameObject("Text Area", typeof(RectTransform));
        textAreaGO.transform.SetParent(inputGO.transform, false);

        RectTransform textAreaRT = textAreaGO.GetComponent<RectTransform>();
        textAreaRT.anchorMin = Vector2.zero;
        textAreaRT.anchorMax = Vector2.one;
        textAreaRT.offsetMin = new Vector2(5, 5);
        textAreaRT.offsetMax = new Vector2(-5, -5);

        inputField.textViewport = textAreaRT;

        // Placeholder
        GameObject placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
        placeholderGO.transform.SetParent(textAreaGO.transform, false);
        TextMeshProUGUI placeholder = placeholderGO.AddComponent<TextMeshProUGUI>();
        placeholder.text = "Entrez un nom...";
        placeholder.fontSize = 14;
        placeholder.color = new Color(0, 0, 0, 0.5f);
        placeholder.alignment = TextAlignmentOptions.Left;
        inputField.placeholder = placeholder;

        RectTransform phRT = placeholderGO.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = Vector2.zero;
        phRT.offsetMax = Vector2.zero;

        // Texte
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(textAreaGO.transform, false);
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.fontSize = 14;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Left;
        inputField.textComponent = text;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;


        // Bouton valider
        GameObject btnGO = new GameObject("ConfirmButton", typeof(RectTransform));
        btnGO.transform.SetParent(container.transform, false);
        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.3f, 0.05f);
        btnRT.anchorMax = new Vector2(0.7f, 0.25f);
        btnRT.offsetMin = Vector2.zero;
        btnRT.offsetMax = Vector2.zero;

        Button btn = btnGO.AddComponent<Button>();
        Image btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 1f, 0.9f);

        GameObject btnTextGO = new GameObject("Text", typeof(RectTransform));
        btnTextGO.transform.SetParent(btnGO.transform, false);
        TextMeshProUGUI btnText = btnTextGO.AddComponent<TextMeshProUGUI>();
        btnText.text = "Valider";
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;
        btnText.fontSize = 16;

        RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        // Listener du bouton
        btn.onClick.AddListener(() =>
        {
            string name = inputField.text;
            GameObject.Destroy(popupGO);
            popupGO = null;
            onConfirm?.Invoke(name);
        });
    }


public static void ShowPlaylistPopup(string trackName)
{
    // Détruire l'ancien popup
    if (popupGO != null)
        UnityEngine.Object.Destroy(popupGO);

    // Création du popup
    popupGO = new GameObject("PlaylistPopup", typeof(RectTransform));
    popupGO.transform.SetParent(GameObject.Find("Canvas").transform, false);

    Image bg = popupGO.AddComponent<Image>();
    bg.color = new Color(0, 0, 0, 0.6f);

    RectTransform rt = popupGO.GetComponent<RectTransform>();
    rt.anchorMin = Vector2.zero;
    rt.anchorMax = Vector2.one;
    rt.offsetMin = Vector2.zero;
    rt.offsetMax = Vector2.zero;

    // Conteneur
    GameObject container = new GameObject("Container", typeof(RectTransform));
    container.transform.SetParent(popupGO.transform, false);

    RectTransform contRT = container.GetComponent<RectTransform>();
    contRT.sizeDelta = new Vector2(300, 300);
    contRT.anchoredPosition = Vector2.zero;
    contRT.anchorMin = new Vector2(0.5f, 0.5f);
    contRT.anchorMax = new Vector2(0.5f, 0.5f);
    contRT.pivot = new Vector2(0.5f, 0.5f);
    contRT.anchoredPosition = Vector2.zero;


    Image contBg = container.AddComponent<Image>();
    contBg.color = new Color(1, 1, 1, 0.95f);

    // ScrollRect
    GameObject scrollGO = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect));
    scrollGO.transform.SetParent(container.transform, false);

    RectTransform scrollRT = scrollGO.GetComponent<RectTransform>();
    scrollRT.anchorMin = new Vector2(0.05f, 0.05f);
    scrollRT.anchorMax = new Vector2(0.95f, 0.95f);
    scrollRT.offsetMin = Vector2.zero;
    scrollRT.offsetMax = Vector2.zero;

    ScrollRect scroll = scrollGO.GetComponent<ScrollRect>();
    scroll.horizontal = false;

    // Viewport
    GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
    viewportGO.transform.SetParent(scrollGO.transform, false);

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
    PlaylistUI.AfficherBoutonPlaylist(null, contentRT, playlistName =>
    {
        PlaylistManager pm = UnityEngine.Object.FindObjectOfType<PlaylistManager>();

        if (pm != null)
        {
            pm.AddTrackToPlaylist(playlistName, trackName);
            UnityEngine.Object.Destroy(popupGO);
            popupGO = null;
            Show("Ajouté à : " + playlistName);
        }
    });
}


public static void ShowMusiquesPlaylistPopup(List<AudioClip> clips,string playlistName)
{
    // Détruire l'ancien popup
    if (popupGO != null)
        UnityEngine.Object.Destroy(popupGO);

    // Création du popup
    popupGO = new GameObject("PlaylistPopup", typeof(RectTransform));
    popupGO.transform.SetParent(GameObject.Find("Canvas").transform, false);

    Image bg = popupGO.AddComponent<Image>();
    bg.color = new Color(0, 0, 0, 0.6f);

    RectTransform rt = popupGO.GetComponent<RectTransform>();
    rt.anchorMin = Vector2.zero;
    rt.anchorMax = Vector2.one;
    rt.offsetMin = Vector2.zero;
    rt.offsetMax = Vector2.zero;

    // Conteneur
    GameObject container = new GameObject("Container", typeof(RectTransform));
    container.transform.SetParent(popupGO.transform, false);

    RectTransform contRT = container.GetComponent<RectTransform>();
    contRT.sizeDelta = new Vector2(300, 300);
    contRT.anchoredPosition = Vector2.zero;
    contRT.anchorMin = new Vector2(0.5f, 0.5f);
    contRT.anchorMax = new Vector2(0.5f, 0.5f);
    contRT.pivot = new Vector2(0.5f, 0.5f);
    contRT.anchoredPosition = Vector2.zero;


    Image contBg = container.AddComponent<Image>();
    contBg.color = new Color(1, 1, 1, 0.95f);

    // ----- Bouton de fermeture -----
GameObject closeBtnGO = new GameObject("CloseButton", typeof(RectTransform));
closeBtnGO.transform.SetParent(container.transform, false);

RectTransform closeRT = closeBtnGO.GetComponent<RectTransform>();
closeRT.anchorMin = new Vector2(1, 1);
closeRT.anchorMax = new Vector2(1, 1);
closeRT.pivot = new Vector2(1, 1);
closeRT.sizeDelta = new Vector2(30, 30);
closeRT.anchoredPosition = new Vector2(-10, -10); // marge depuis le coin

Button closeBtn = closeBtnGO.AddComponent<Button>();
Image closeImg = closeBtnGO.AddComponent<Image>();
closeImg.color = new Color(1, 0.3f, 0.3f, 1f); // rouge clair

// Texte "X"
GameObject closeTextGO = new GameObject("Text", typeof(RectTransform));
closeTextGO.transform.SetParent(closeBtnGO.transform, false);

TextMeshProUGUI closeTxt = closeTextGO.AddComponent<TextMeshProUGUI>();
closeTxt.text = "X";
closeTxt.fontSize = 24;
closeTxt.alignment = TextAlignmentOptions.Center;
closeTxt.color = Color.white;

RectTransform closeTxtRT = closeTextGO.GetComponent<RectTransform>();
closeTxtRT.anchorMin = Vector2.zero;
closeTxtRT.anchorMax = Vector2.one;
closeTxtRT.offsetMin = Vector2.zero;
closeTxtRT.offsetMax = Vector2.zero;

// Action du bouton
closeBtn.onClick.AddListener(() =>
{
    UnityEngine.Object.Destroy(popupGO);
});


    // ScrollRect
    GameObject scrollGO = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect));
    scrollGO.transform.SetParent(container.transform, false);

    RectTransform scrollRT = scrollGO.GetComponent<RectTransform>();
    scrollRT.anchorMin = new Vector2(0.05f, 0.05f);
    scrollRT.anchorMax = new Vector2(0.95f, 0.95f);
    scrollRT.offsetMin = Vector2.zero;
    scrollRT.offsetMax = Vector2.zero;

    ScrollRect scroll = scrollGO.GetComponent<ScrollRect>();
    scroll.horizontal = false;

    // Viewport
    GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
    viewportGO.transform.SetParent(scrollGO.transform, false);

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

    //Ajout des boutons next et before pour gérer la playlist
    /*PlaylistManager pm = FindObjectOfType<PlaylistManager>();
    if (pm != null)
    {   Sprite buttonSprite = Resources.Load<Sprite>("png_violet");
        Button nextBtn = Bouton.CreateButton(contentRT, "Next", buttonSprite, () => pm.Next(playlistName));
        Button prevBtn = Bouton.CreateButton(contentRT, "Before", buttonSprite, () => pm.Previous(playlistName));
    }*/
    
}

}

