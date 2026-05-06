using UnityEngine;
using System;

public class FinishText : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject finishTextPrefab;
    [SerializeField] private Transform uiParent;
    string finishText;
    private TMPro.TextMeshProUGUI txt;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            GameObject obj = Instantiate(finishTextPrefab, uiParent);

            txt = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (txt != null)
            {
                txt.fontSize = 25;
                UIBuilder.ApplyMontserratFont(txt);
                txt.text = finishText;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player != null)
        {
            float scorePercentage = (player.GetEnergyLevel() / player.GetMaxEnergyLevel()) * 100;
            finishText = "Bravo !\n Vous avez terminé le niveau avec \n"  + Mathf.Round(scorePercentage * 100.0f) * 0.01f + "% d'énergie restante";
        }
        else
        {
            finishText = "Niveau terminé";
        }
        if (txt != null)
            {
                txt.text = finishText;
            }
        Debug.Log(finishText);
    }
}
