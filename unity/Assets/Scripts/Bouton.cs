using UnityEngine.UI;
using UnityEngine;
using TMPro;
public static class Bouton
{   

    public static Button CreateButton(Transform parent, string text, UnityEngine.Vector2 size, UnityEngine.Events.UnityAction action)
    {
        // GameObject du bouton
        GameObject buttonGO = new GameObject(text + "Button");
        buttonGO.transform.SetParent(parent, false);

        // Image NECESSAIRE au Button, mais sans sprite
        Image img = buttonGO.AddComponent<Image>();
        
        img.color = new Color(0.78f, 0.80f, 0.82f, 1f);

        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = img;
        button.onClick.AddListener(action);

        RectTransform rt = buttonGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.pivot = new Vector2(0.5f, 0);

        LayoutElement le = buttonGO.AddComponent<LayoutElement>();
        le.preferredWidth = size.x;
        le.preferredHeight = size.y;

        // Texte TMP
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        TMP_Text tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 22;
        tmp.color = new Color(0.118f, 0.141f, 0.192f, 1f);
        tmp.alignment = TextAlignmentOptions.Center;
        UIBuilder.ApplyMontserratFont(tmp);

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        return button;
    }

    public static Button CreateMusicButton(Transform parent)
{   
        Button btn = CreateButton(parent, "Jouer",new UnityEngine.Vector2(90,40), () => {}); 

        MusicButton mb = btn.gameObject.AddComponent<MusicButton>(); 
  
        return btn;
}   
    public static Button CreateButtonEditor(Transform parent, GameObject averageButtonPrefab, string text, UnityEngine.Events.UnityAction action){
        
        GameObject boutonGO = UnityEngine.Object.Instantiate(averageButtonPrefab,parent);
        UIBuilder.ApplyMontserratFontRecursive(boutonGO.transform);
        Button btn = boutonGO.GetComponent<Button>();

        TextMeshProUGUI label = boutonGO.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = text;
            UIBuilder.ApplyMontserratFont(label);
        }
        
        btn.onClick.AddListener(action);

        LayoutElement le = boutonGO.AddComponent<LayoutElement>();
        le.preferredHeight = 40;
        le.preferredWidth = 160;

        return btn;
}

}
