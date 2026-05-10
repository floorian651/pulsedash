using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public static class ReturnToMenuButton
{

    public static GameObject prefab;
    
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

        GameObject buttonGO = Object.Instantiate(prefab, canvas.transform);
        buttonGO.name = "BackToMenuButton";

        Button btn = buttonGO.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() =>
        {
            UnityEngine.GameObject obj = UnityEngine.GameObject.Find("Player");
            AudioSource src = obj.GetComponentInChildren<AudioSource>();
            src.Stop();
            //UnityEngine.Object.Destroy(obj);

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
