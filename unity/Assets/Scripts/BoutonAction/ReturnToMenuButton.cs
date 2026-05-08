using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public static class ReturnToMenuButton
{
    public static void Create()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("ReturnToMenuButton: aucun Canvas trouvé.");
            return;
        }

        Transform existing = canvas.transform.Find("BackToMenuButton");
        if (existing != null)
            return;

        GameObject buttonGO = new GameObject("BackToMenuButton", typeof(RectTransform));
        buttonGO.transform.SetParent(canvas.transform, false);

        RectTransform rt = buttonGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -20);
        rt.sizeDelta = new Vector2(220, 40);

        Image img = buttonGO.AddComponent<Image>();
        img.color = new Color32(0x80, 0x95, 0xFF, 0xFF);

        Button btn = buttonGO.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() =>
        {
            GameObject obj = GameObject.Find("Player");
            AudioSource src = obj.GetComponentInChildren<AudioSource>();
            src.Stop();

            SceneManager.LoadScene("Platform_Streaming");
        });


        GameObject textGO = new GameObject("Label", typeof(RectTransform));
        textGO.transform.SetParent(buttonGO.transform, false);
        TextMeshProUGUI label = textGO.AddComponent<TextMeshProUGUI>();
        label.text = "Retour menu";
        label.fontSize = 18;
        label.color = Color.black;
        label.alignment = TextAlignmentOptions.Center;
        UIBuilder.ApplyMontserratFont(label);

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
    }
}
