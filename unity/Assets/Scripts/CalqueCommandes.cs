using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CalqueCommandes : MonoBehaviour
{
    [Header("Réglages")]
    public float displayDuration = 3f;   // Durée d'affichage
    public float fadeDuration    = 1f; // Durée du fondu de disparition

    private Image _image;
    private RectTransform _rectTransform;

    [Header("Taille")]
    public float screenRatio = 0.9f; // 9/10 de l'écran
    public float gapRatio = 0.1f;
    

    void Start()
    {
        _image = GetComponent<Image>(); 

        _rectTransform = GetComponent<RectTransform>();
        
        AdjustSize();
        _image.enabled = true;
        StartCoroutine(HideAfterDelay());
    }

    void AdjustSize()
    {
        // Récupère la taille de l'écran
        float screenWidth  = Screen.width;
        float screenHeight = Screen.height;

        // Calcule 9/10 de la taille de l'écran
        float targetWidth  = screenWidth  * screenRatio;
        float targetHeight = screenHeight * screenRatio;

        float targetGap  = screenHeight  * gapRatio;
        

        // Applique la taille au RectTransform
        _rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
        _rectTransform.anchoredPosition = new Vector2(0f,targetGap); 
    }

    IEnumerator HideAfterDelay()
    {
        // Attendre la durée d'affichage
        yield return new WaitForSeconds(displayDuration);

        // Fondu de disparition
        float elapsed = 0f;
        Color c = _image.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            _image.color = c;
            yield return null;
        }

        gameObject.SetActive(false); // Désactive complètement l'objet
    }
}