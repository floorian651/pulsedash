using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;


public class ScreenFlash : MonoBehaviour
{
    public CanvasGroup flashGroup;
    public float flashDuration = 0.2f;

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        flashGroup.alpha = 1f;

        float t = 0;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            flashGroup.alpha = Mathf.Lerp(1f, 0f, t / flashDuration);
            yield return null;
        }

        flashGroup.alpha = 0f;
    }
}
