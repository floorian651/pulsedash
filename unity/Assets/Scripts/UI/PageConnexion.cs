using UnityEngine;
using TMPro;

public class PageConnexion : MonoBehaviour
{
    void Awake()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI txt in texts)
        {   
            Debug.Log(txt);
            UIBuilder.ApplyMontserratFont(txt);
        }
    }
}