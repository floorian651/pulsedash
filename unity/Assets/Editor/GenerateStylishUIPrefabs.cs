using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class GenerateStylishUIPrefabs
{
    private const string SpritePath = "Assets/UI/Sprites/RoundedRect_128x64.png";
    private const string PrefabFolder = "Assets/Prefabs/UI";
    private const string MontserratPath = "Assets/Resources/Fonts & Materials/MedievalSharp,Montserrat/MedievalSharp/MedievalSharp-Regular SDF.asset";
    private const string SpritePathPlay = "Assets/UI/Sprites/Play.png";
    private const string SpritePathPause = "Assets/UI/Sprites/Pause.png";

    [MenuItem("Tools/UI/Generate Stylish UI Prefabs")]
    public static void Generate()
    {
        EnsurePrefabFolder();
        SetupRoundedSprite();

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        var circleSprite = EditorGUIUtility.Load("UI/Skin/Knob.psd") as Sprite;


        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(MontserratPath);
        if (font == null)
        {
            font = TMP_Settings.defaultFontAsset;
        }

        CreatePlayPauseButton(circleSprite, font);
        CreatePlaylistItemButton(sprite, font);
        CreateMusicItemPrefab(sprite, font);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Stylish UI prefabs generated in Assets/Prefabs/UI");
    }

    private static void EnsurePrefabFolder()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }
    }

    private static void SetupRoundedSprite()
    {
        if (!File.Exists(SpritePath))
        {
            Debug.LogWarning($"Rounded sprite not found at {SpritePath}. The prefabs will use a null sprite.");
            return;
        }

        var importer = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = 100f;
        importer.spriteBorder = new Vector4(16, 16, 16, 16);
        importer.SaveAndReimport();
    }

    private static void CreatePlayPauseButton(Sprite circleSprite, TMP_FontAsset font)
    {
        var root = new GameObject("PlayPauseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(140, 44);

        var image = root.GetComponent<Image>();
        image.sprite = circleSprite;
        //image.type = Image.Type.Sliced; // ou Simple si ton cercle n’a pas de border

        image.type = Image.Type.Simple;
        image.color = new Color(0.96f, 0.92f, 0.84f, 0f);


        var icon = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        icon.transform.SetParent(root.transform, false);

        var iconRt = icon.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0.5f, 0.5f);
        iconRt.anchorMax = new Vector2(0.5f, 0.5f);
        iconRt.sizeDelta = new Vector2(250, 200);
        iconRt.anchoredPosition = Vector2.zero;

        var iconImg = icon.GetComponent<Image>();
        iconImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePathPlay);//récupérer via resource ;
        iconImg.color = Color.white;

        var button = root.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        var colors = button.colors;
        colors.normalColor = new Color(1,1,1,0);      // transparent
        colors.highlightedColor = new Color(1,1,1,0); // transparent
        colors.pressedColor = new Color(1,1,1,0);     // transparent
        colors.selectedColor = new Color(1,1,1,0);    // transparent
        button.colors = colors;


        var layout = root.GetComponent<LayoutElement>();
        layout.preferredWidth = 140f;
        layout.preferredHeight = 44f;

        var musicButton = root.AddComponent<MusicButton>();
        musicButton.icon = iconImg;
        musicButton.playSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePathPlay);
        musicButton.pauseSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePathPause);
        Debug.Log("Attribution des sprites");

        /*var label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.transform.SetParent(root.transform, false);
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = new Vector2(12, 6);
        labelRt.offsetMax = new Vector2(-12, -6);*/

        /*var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text = "Jouer";
        tmp.font = font;
        tmp.fontSize = 18f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;*/

        var path = Path.Combine(PrefabFolder, "PlayPauseButton.prefab").Replace("\\", "/");
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static void CreatePlaylistItemButton(Sprite sprite, TMP_FontAsset font)
    {
        // Racine (même base visuelle que MusicItem)
        var root = new GameObject("PlaylistItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(380, 32);

        var layout = root.GetComponent<LayoutElement>();
        layout.preferredWidth = 380f;
        layout.preferredHeight = 32f;

        var image = root.GetComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = new Color(0.93f, 0.95f, 0.98f, 1f);

        // Bouton racine (même rendu que le fond, juste un léger feedback)
        var button = root.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        var colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.92f, 0.94f, 0.97f, 1f);
        colors.pressedColor = new Color(0.86f, 0.9f, 0.95f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        button.colors = colors;

        // --- LABEL ---
        var label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.transform.SetParent(root.transform, false);
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 0f);
        labelRt.anchorMax = new Vector2(1f, 1f);
        labelRt.offsetMin = new Vector2(16, 4);
        labelRt.offsetMax = new Vector2(-60, -4);

        var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text = "Playlist";
        tmp.font = font;
        tmp.fontSize = 16f;
        tmp.color = new Color(0.12f, 0.15f, 0.2f, 1f);
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        // --- BOUTON CHEVRON (mêmes dimensions/couleurs que PlayButton) ---
        var chevronBtnGO = new GameObject("ChevronButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        chevronBtnGO.transform.SetParent(root.transform, false);

        var chevronRt = chevronBtnGO.GetComponent<RectTransform>();
        chevronRt.sizeDelta = new Vector2(40, 20);
        chevronRt.anchoredPosition = new Vector2(-10, 0);
        chevronRt.anchorMin = new Vector2(1f, 0.5f);
        chevronRt.anchorMax = new Vector2(1f, 0.5f);
        chevronRt.pivot = new Vector2(1f, 0.5f);

        var chevronImg = chevronBtnGO.GetComponent<Image>();
        chevronImg.color = new Color(0.2f, 0.35f, 0.6f, 1f);

        var chevronBtn = chevronBtnGO.GetComponent<Button>();
        chevronBtn.transition = Selectable.Transition.ColorTint;
        var chevronColors = chevronBtn.colors;
        chevronColors.normalColor = chevronImg.color;
        chevronColors.highlightedColor = new Color(0.25f, 0.45f, 0.75f, 1f);
        chevronColors.pressedColor = new Color(0.15f, 0.25f, 0.45f, 1f);
        chevronBtn.colors = chevronColors;

        var chevronTxtGO = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        chevronTxtGO.transform.SetParent(chevronBtnGO.transform, false);

        var chevronTmp = chevronTxtGO.GetComponent<TextMeshProUGUI>();
        chevronTmp.text = ">";
        chevronTmp.font = font;
        chevronTmp.fontSize = 16f;
        chevronTmp.color = Color.white;
        chevronTmp.alignment = TextAlignmentOptions.Center;

        var chevronTxtRt = chevronTxtGO.GetComponent<RectTransform>();
        chevronTxtRt.anchorMin = Vector2.zero;
        chevronTxtRt.anchorMax = Vector2.one;
        chevronTxtRt.offsetMin = Vector2.zero;
        chevronTxtRt.offsetMax = Vector2.zero;

        var path = Path.Combine(PrefabFolder, "PlaylistItem.prefab").Replace("\\", "/");
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }
    private static void CreateAverageButton(Sprite sprite, TMP_FontAsset font)
    {
        var root = new GameObject("AverageButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(140, 44);

        var image = root.GetComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = new Color(0.18f, 0.69f, 0.68f, 1f); // teal

        var button = root.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        var colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.21f, 0.78f, 0.76f, 1f);
        colors.pressedColor = new Color(0.14f, 0.56f, 0.55f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        button.colors = colors;

        var layout = root.GetComponent<LayoutElement>();
        layout.preferredWidth = 140f;
        layout.preferredHeight = 44f;

        var label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.transform.SetParent(root.transform, false);
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = new Vector2(12, 6);
        labelRt.offsetMax = new Vector2(-12, -6);

        var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text ="";
        tmp.font = font;
        tmp.fontSize = 18f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        var path = Path.Combine(PrefabFolder, "AverageButton.prefab").Replace("\\", "/");
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }
    private static void CreateMusicItemPrefab(Sprite sprite, TMP_FontAsset font)
{
        // Racine
        var root = new GameObject("MusicItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        var rt = root.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(380, 32);

        var layout = root.GetComponent<LayoutElement>();
        layout.preferredWidth = 380f;
        layout.preferredHeight = 32f;

        var bg = root.GetComponent<Image>();
        bg.sprite = sprite;
        bg.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        bg.color = new Color(0.93f, 0.95f, 0.98f, 1f);

        // --- LABEL ---
        var label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.transform.SetParent(root.transform, false);

        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 0f);
        labelRt.anchorMax = new Vector2(1f, 1f);
        labelRt.offsetMin = new Vector2(16, 4);
        labelRt.offsetMax = new Vector2(-60, -4);

        var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text = "Titre musique";
        tmp.font = font;
        tmp.fontSize = 16f;
        tmp.color = new Color(0.12f, 0.15f, 0.2f, 1f);
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.overflowMode = TextOverflowModes.Ellipsis;

        // --- BOUTON PLAY ---
        var playBtnGO = new GameObject("PlayButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        playBtnGO.transform.SetParent(root.transform, false);

        var playRt = playBtnGO.GetComponent<RectTransform>();
        playRt.sizeDelta = new Vector2(40, 20); // plus fin
        playRt.anchoredPosition = new Vector2(-10, 0);
        playRt.anchorMin = new Vector2(1f, 0.5f);
        playRt.anchorMax = new Vector2(1f, 0.5f);
        playRt.pivot = new Vector2(1f, 0.5f);

        var playImg = playBtnGO.GetComponent<Image>();
        playImg.color = new Color(0.2f, 0.35f, 0.6f, 1f); // bleu foncé

        var playBtn = playBtnGO.GetComponent<Button>();
        playBtn.transition = Selectable.Transition.ColorTint;

        var colors = playBtn.colors;
        colors.normalColor = playImg.color;
        colors.highlightedColor = new Color(0.25f, 0.45f, 0.75f, 1f);
        colors.pressedColor = new Color(0.15f, 0.25f, 0.45f, 1f);
        playBtn.colors = colors;

        // Icône "▶"
        var playTxtGO = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        playTxtGO.transform.SetParent(playBtnGO.transform, false);

        var playTxt = playTxtGO.GetComponent<TextMeshProUGUI>();
        playTxt.text = "▶";
        playTxt.font = font;
        playTxt.fontSize = 16f;
        playTxt.color = Color.white;
        playTxt.alignment = TextAlignmentOptions.Center;

        var playTxtRt = playTxtGO.GetComponent<RectTransform>();
        playTxtRt.anchorMin = Vector2.zero;
        playTxtRt.anchorMax = Vector2.one;
        playTxtRt.offsetMin = Vector2.zero;
        playTxtRt.offsetMax = Vector2.zero;


        // --- BOUTON AJOUTER À PLAYLIST ---
        var addBtnGO = new GameObject("AddToPlaylistButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        addBtnGO.transform.SetParent(root.transform, false);

        // Position du bouton (à droite du bouton Play)
        var addRt = addBtnGO.GetComponent<RectTransform>();
        addRt.sizeDelta = new Vector2(40, 20);
        addRt.anchorMin = new Vector2(1f, 0.5f);
        addRt.anchorMax = new Vector2(1f, 0.5f);
        addRt.pivot = new Vector2(1f, 0.5f);
        addRt.anchoredPosition = new Vector2(-55, 0); // décalé à gauche du bouton Play

        // Style du bouton
        var addImg = addBtnGO.GetComponent<Image>();
        addImg.color = new Color(0.35f, 0.6f, 0.2f, 1f); // vert foncé

        var addBtn = addBtnGO.GetComponent<Button>();
        addBtn.transition = Selectable.Transition.ColorTint;

        var addColors = addBtn.colors;
        addColors.normalColor = addImg.color;
        addColors.highlightedColor = new Color(0.45f, 0.75f, 0.3f, 1f);
        addColors.pressedColor = new Color(0.25f, 0.45f, 0.2f, 1f);
        addBtn.colors = addColors;

        // Icône "+"
        var addTxtGO = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        addTxtGO.transform.SetParent(addBtnGO.transform, false);

        var addTxt = addTxtGO.GetComponent<TextMeshProUGUI>();
        addTxt.text = "+";
        addTxt.font = font;
        addTxt.fontSize = 18f;
        addTxt.color = Color.white;
        addTxt.alignment = TextAlignmentOptions.Center;

        var addTxtRt = addTxtGO.GetComponent<RectTransform>();
        addTxtRt.anchorMin = Vector2.zero;
        addTxtRt.anchorMax = Vector2.one;
        addTxtRt.offsetMin = Vector2.zero;
        addTxtRt.offsetMax = Vector2.zero;

        // --- SAUVEGARDE PREFAB ---
        var path = Path.Combine(PrefabFolder, "MusicItem.prefab").Replace("\\", "/");
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

}
