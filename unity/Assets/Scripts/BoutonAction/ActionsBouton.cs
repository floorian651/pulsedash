using UnityEngine;
using UnityEngine.UI; //slider
using System.Collections; // IEnumerator
using TMPro;  // indispensable pour TextMeshProUGUI


public class ActionsBouton : MonoBehaviour
{
 public string pseudo;

    public void Connexion(TMP_InputField inputArea)
    {
        pseudo = inputArea.text;
        Debug.Log(pseudo);
    }

    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }


}