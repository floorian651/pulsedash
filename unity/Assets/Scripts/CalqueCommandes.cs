using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CalqueCommandes : MonoBehaviour
{
    [Header("Réglages")]
    public float displayDuration = 3f;   // Durée d'affichage
    public float fadeDuration    = 1f; // Durée du fondu de disparition

    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
        _image.enabled = true;
        StartCoroutine(HideAfterDelay());
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