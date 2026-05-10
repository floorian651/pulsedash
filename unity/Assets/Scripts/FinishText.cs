using UnityEngine;
using System;

public class FinishText : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject finishTextPrefab;
    [SerializeField] private Transform uiParent;
    string finishText;
    public GameObject backToMenuPrefab;
    private TMPro.TextMeshProUGUI txt;

    void Awake()
    {
        //player = FindObjectOfType<Player>();
        //if (player != null)
        //{
            GameObject obj = Instantiate(finishTextPrefab, uiParent);

            txt = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (txt != null)
            {
                txt.fontSize = 25;
                UIBuilder.ApplyMontserratFont(txt);
                txt.text = finishText;
           }
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        ReturnToMenuButton.prefab = backToMenuPrefab;
        ReturnToMenuButton.Create();
        //if (player != null)
        //{   
            // A MODIFIER POUR LA BDD récupérer le score du joueur via la BDD si possible
            //float scorePercentage = (player.GetEnergyLevel() / player.GetMaxEnergyLevel()) * 100;
            if (SessionData.Instance != null){
                float scorePercentage = SessionData.Instance.score;

                finishText = "Bravo !\n Vous avez terminé le niveau avec \n"  + Mathf.Round(scorePercentage * 100.0f) * 0.01f + "% d'énergie restante";
                //Destroy(player.gameObject);
                }
            
        /*}
        else
        {
            finishText = "Niveau terminé";
        }*/
        if (txt != null)
            {
                txt.text = finishText;
            }
        Debug.Log(finishText);
    }
}
