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
    // Fond transparent
    bg.color = new Color32(0x80, 0x95, 0xFF, 0x00);

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
    // Fond transparent
    bg.color = new Color32(0x80, 0x95, 0xFF, 0x00);

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
    // Fond transparent
    bg.color = new Color32(0x80, 0x95, 0xFF, 0x00);

    return rightGO.transform;
}

public static Transform CreateCenterRightContainer(Transform parent)
{
    GameObject fusionGO = new GameObject("CenterRight", typeof(RectTransform));
    fusionGO.transform.SetParent(parent, false);

    RectTransform rt = fusionGO.GetComponent<RectTransform>();
    rt.sizeDelta = new Vector2(600, 300); // 400 + 200

	    VerticalLayoutGroup layout = fusionGO.AddComponent<VerticalLayoutGroup>();
	    layout.spacing = 0;
	    layout.childAlignment = TextAnchor.UpperCenter;
	    layout.childControlHeight = false;
	    layout.childForceExpandHeight = false;
	    layout.childControlWidth = false;
	    layout.childForceExpandWidth = false;


    Image bg = fusionGO.AddComponent<Image>();
    bg.color = new Color32(0x80, 0x95, 0xFF, 0x00); // transparent

    return fusionGO.transform;
}


// A implémenter pour faire un conteneur en bas du panel
public static Transform CreateBottomAudioBar(Transform parent,float height = 60f)
{
    GameObject bottomGO = new GameObject("BottomAudioBar", typeof(RectTransform));
    bottomGO.transform.SetParent(parent, false);

    RectTransform rt = bottomGO.GetComponent<RectTransform>();

    // Le conteneur occupe toute la largeur, collé en bas
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1, 0);
    rt.pivot = new Vector2(0.5f, 0);
    rt.sizeDelta = new Vector2(0, height); // hauteur fixe

    // Layout horizontal pour slider + boutons
    HorizontalLayoutGroup layout = bottomGO.AddComponent<HorizontalLayoutGroup>();
    layout.childAlignment = TextAnchor.MiddleCenter;
    layout.spacing = 20;
    layout.padding = new RectOffset(20, 20, 10, 10);
    layout.childControlHeight = true;
    layout.childControlWidth = false;
    layout.childForceExpandWidth = true;

    // Fond léger (optionnel)
    Image bg = bottomGO.AddComponent<Image>();
    bg.color = new Color32(0x80, 0x95, 0xFF, 20);

    return bottomGO.transform;
}


}
