using UnityEngine;
using UnityEngine.UI; //slider
using System.Collections; // IEnumerator
using TMPro;  // indispensable pour TextMeshProUGUI


public class ActionsBouton : MonoBehaviour
{

public void HideButton(GameObject button)
{
    button.SetActive(false);
}

}