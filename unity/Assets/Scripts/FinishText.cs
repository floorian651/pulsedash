using UnityEngine;

public class FinishText : MonoBehaviour
{
    [SerializeField] private Player player;
    private GameObject musicItemPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player != null)
        {
            Debug.Log("Énergie restante : " + player.GetEnergyLevel() + " / " + player.GetMaxEnergyLevel());
        }
        else
        {
            Debug.Log("Test");
        }
    }
}
