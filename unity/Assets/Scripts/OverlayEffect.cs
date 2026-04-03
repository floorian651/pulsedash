using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OverlayEffect : MonoBehaviour
{
    public Image overlayImage;
    public float flashDuration = 0.2f;

    public void Flash(Color color)
    {
        StartCoroutine(FlashRoutine(color));
    }

    IEnumerator FlashRoutine(Color color)
    {
        overlayImage.color = color;

        yield return new WaitForSeconds(flashDuration);

        overlayImage.color = Color.clear;
    }
}