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
            /*UnityEngine.GameObject obj = UnityEngine.GameObject.Find("Player");
            if (obj!=null){
                AudioSource src = obj.GetComponentInChildren<AudioSource>(); // A enlever car on pourra récupérer les scores via SessionData
                src.Stop();
                //UnityEngine.Object.Destroy(obj);
            }*/
            

            if (SessionData.Instance != null)
            {
                string scenePrecedente = SessionData.Instance.scenePrecedente;
                Debug.Log("Scene précédente récupérée : "+ scenePrecedente);
                SceneManager.LoadScene(scenePrecedente);
            }
            Debug.Log( "youou");


            
        });

    }
}
