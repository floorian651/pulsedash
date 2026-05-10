using UnityEngine;
using TMPro;

public class AppliquerStyle : MonoBehaviour
{
    public GameObject backToMenuPrefab;

    void Start()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI txt in texts)
        {   
            Debug.Log(txt);
            UIBuilder.ApplyMontserratFont(txt);
        }
        if (backToMenuPrefab != null){
            ReturnToMenuButton.prefab = backToMenuPrefab;
            ReturnToMenuButton.Create();
        }
        
    }
}