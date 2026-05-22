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
        buttonGO.transform.SetAsLastSibling();

        Button btn = buttonGO.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() =>
        {
            if (SessionData.Instance != null)
            {
                SessionData.Instance.mode = null;
                SessionData.Instance.levelData = null;
                string scenePrecedente = SessionData.Instance.scenePrecedente;
                SceneManager.LoadScene(scenePrecedente);
            }
        });

    }
}
