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
    [MenuItem("Tools/UI/Generate Stylish UI Prefabs")]
    public static void Generate()
    {
        EnsurePrefabFolder();
        SetupRoundedSprite();

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(MontserratPath);
        if (font == null)
        {
            font = TMP_Settings.defaultFontAsset;
        }

        CreatePlayPauseButton(sprite, font);
        CreatePlaylistItemButton(sprite, font);

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

    private static void CreatePlayPauseButton(Sprite sprite, TMP_FontAsset font)
    {
        var root = new GameObject("PlayPauseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
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
        tmp.text = "Jouer";
        tmp.font = font;
        tmp.fontSize = 18f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        var path = Path.Combine(PrefabFolder, "PlayPauseButton.prefab").Replace("\\", "/");
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static void CreatePlaylistItemButton(Sprite sprite, TMP_FontAsset font)
    {
        var root = new GameObject("PlaylistItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(260, 48);

        var image = root.GetComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = new Color(0.93f, 0.95f, 0.98f, 1f); // light slate

        var button = root.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        var colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.88f, 0.91f, 0.96f, 1f);
        colors.pressedColor = new Color(0.82f, 0.86f, 0.92f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        button.colors = colors;

        var layout = root.GetComponent<LayoutElement>();
        layout.preferredWidth = 260f;
        layout.preferredHeight = 48f;

        var label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.transform.SetParent(root.transform, false);
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 0f);
        labelRt.anchorMax = new Vector2(1f, 1f);
        labelRt.offsetMin = new Vector2(16, 6);
        labelRt.offsetMax = new Vector2(-36, -6);

        var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text = "Playlist";
        tmp.font = font;
        tmp.fontSize = 16f;
        tmp.color = new Color(0.12f, 0.15f, 0.2f, 1f);
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        var chevron = new GameObject("Chevron", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        chevron.transform.SetParent(root.transform, false);
        var chevronRt = chevron.GetComponent<RectTransform>();
        chevronRt.anchorMin = new Vector2(1f, 0.5f);
        chevronRt.anchorMax = new Vector2(1f, 0.5f);
        chevronRt.pivot = new Vector2(1f, 0.5f);
        chevronRt.sizeDelta = new Vector2(20, 20);
        chevronRt.anchoredPosition = new Vector2(-12, 0);

        var chevronTmp = chevron.GetComponent<TextMeshProUGUI>();
        chevronTmp.text = ">";
        chevronTmp.font = font;
        chevronTmp.fontSize = 18f;
        chevronTmp.color = new Color(0.2f, 0.35f, 0.6f, 1f);
        chevronTmp.alignment = TextAlignmentOptions.Center;

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
}
