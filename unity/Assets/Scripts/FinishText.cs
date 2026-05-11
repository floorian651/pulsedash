using UnityEngine;
using TMPro;

public class FinishText : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject finishTextPrefab;
    [SerializeField] private Transform uiParent;
    public GameObject backToMenuPrefab;

    private TextMeshProUGUI txt;

    void Start()
    {
        // Instanciation UI
        GameObject obj = Instantiate(finishTextPrefab, uiParent);

        txt = obj.GetComponentInChildren<TextMeshProUGUI>();

        if (txt == null)
        {
            Debug.LogError("TMP non trouvé dans le prefab !");
            return;
        }

        txt.fontSize = 20;
        UIBuilder.ApplyMontserratFont(txt);

        //  Récupération score
        float energy = 0;

        if (SessionData.Instance != null)
            energy = SessionData.Instance.score;

        // Détermination du texte
        if (energy > 0)
        {
            txt.text = "Bravo !\nNiveau réussi \nÉnergie restante : " + energy;
        }
        else
        {
            txt.text = "Échec \nPlus d'énergie";
        }

        // Bouton retour menu
        ReturnToMenuButton.prefab = backToMenuPrefab;
        ReturnToMenuButton.Create();

        Debug.Log(txt.text);
    }
}