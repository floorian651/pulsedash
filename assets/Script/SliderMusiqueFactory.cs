using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System.Linq;


public static class SliderMusiqueFactory
{
    public static SliderMusique Create(
        Transform parent,
        Slider sliderPrefab)
    {
        Transform existing = parent.Find(sliderPrefab.name + "(Clone)");
    if (existing != null)
        return existing.GetComponent<SliderMusique>();


        Slider slider = Object.Instantiate(sliderPrefab, parent);


        SliderMusique sm = slider.gameObject.AddComponent<SliderMusique>();
        sm.slider = slider;

        // Placement (optionnel si déjà géré par le prefab)
        RectTransform rt = slider.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0,50);
        rt.sizeDelta = new Vector2(250, 30);

        return sm;
    }
}
