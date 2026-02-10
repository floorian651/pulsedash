using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;



public static class UIconteneur
{   

public static Transform CreateMiddleArea(Transform parent, float topBarHeight)
{
    GameObject middleGO = new GameObject("MiddleArea", typeof(RectTransform));
    middleGO.transform.SetParent(parent, false);

    RectTransform rt = middleGO.GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1, 1);
    rt.pivot = new Vector2(0.5f, 0.5f);

    // On laisse de la place pour la TopBar
    rt.offsetMin = Vector2.zero;
    rt.offsetMax = new Vector2(0, -topBarHeight);

    // Layout horizontal pour gérer gauche / centre / droite
    HorizontalLayoutGroup layout = middleGO.AddComponent<HorizontalLayoutGroup>();
    layout.childAlignment = TextAnchor.MiddleCenter;
    //layout.childControlWidth = false;
    //layout.childForceExpandWidth = false;
    //layout.childControlHeight = true;
    //layout.childForceExpandHeight = true;
    //layout.childControlWidth = true;
    //layout.childForceExpandWidth = true;
    layout.spacing = 30;
    layout.padding = new RectOffset(20, 20, 20, 20);

    return middleGO.transform;
}

public static Transform CreateCenterContainer(Transform parent)
{
    GameObject centerGO = new GameObject("Center", typeof(RectTransform));
    centerGO.transform.SetParent(parent, false);

    RectTransform rt = centerGO.GetComponent<RectTransform>();
    rt.sizeDelta = new Vector2(400, 300);

    VerticalLayoutGroup layout = centerGO.AddComponent<VerticalLayoutGroup>();
    layout.spacing = 20;
    layout.childAlignment = TextAnchor.MiddleCenter;
    layout.childControlHeight = false;
    layout.childForceExpandHeight = false;
    layout.childControlWidth = false;
    layout.childForceExpandWidth = false;

    Image bg = centerGO.AddComponent<Image>();
    bg.color = new Color(0.6f, 0.8f, 0.9f, 1f);

    return centerGO.transform;
}

public static Transform CreateLeftContainer(Transform parent)
{
    GameObject leftGO = new GameObject("Left", typeof(RectTransform));
    leftGO.transform.SetParent(parent, false);

    RectTransform rt = leftGO.GetComponent<RectTransform>();
    rt.sizeDelta = new Vector2(200, 250);

    VerticalLayoutGroup layout = leftGO.AddComponent<VerticalLayoutGroup>();
    layout.spacing = 20;
    layout.childAlignment = TextAnchor.MiddleCenter;
    layout.childControlHeight = false;
    layout.childForceExpandHeight = false;
    layout.childControlWidth = false;
    layout.childForceExpandWidth = false;

    Image bg = leftGO.AddComponent<Image>();
    bg.color = new Color(0.9f, 0.7f, 0.7f, 1f);

    return leftGO.transform;
}

public static Transform CreateRightContainer(Transform parent)
{
    GameObject rightGO = new GameObject("Right", typeof(RectTransform));
    rightGO.transform.SetParent(parent, false);

    RectTransform rt = rightGO.GetComponent<RectTransform>();
    rt.sizeDelta = new Vector2(200, 250);

    VerticalLayoutGroup layout = rightGO.AddComponent<VerticalLayoutGroup>();
    layout.spacing = 20;
    layout.childAlignment = TextAnchor.MiddleCenter;
    layout.childControlHeight = false;
    layout.childForceExpandHeight = false;
    layout.childControlWidth = false;
    layout.childForceExpandWidth = false;

    Image bg = rightGO.AddComponent<Image>();
    bg.color = new Color(0.7f, 0.9f, 0.7f, 1f);

    return rightGO.transform;
}

}