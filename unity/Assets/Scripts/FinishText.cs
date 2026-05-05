using UnityEngine;

public class FinishText : MonoBehaviour
{
    [SerializeField] private Player player;
    private GameObject musicItemPrefab;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string finishText;
        if (player != null)
        {
            finishText = "Bravo ! Vous avez terminé le niveau avec "  + player.GetEnergyLevel() + " / " + player.GetMaxEnergyLevel() + " énergie restante";
        }
        else
        {
            finishText = "Niveau terminé";
        }
        Debug.Log(finishText);
    }
}
