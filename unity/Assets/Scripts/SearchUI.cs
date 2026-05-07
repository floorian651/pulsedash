using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SearchUI
{
    private const float MusicItemRowHeight = 80f;
    private const float MusicItemRowScale = 4f;

    public List<AudioClip> musiques;
    private Context Context;
    private Transform resultsContainer;
    private ScrollRect builtScrollRect;

    private GameObject playlistItemPrefab;
    private GameObject musicItemPrefab;
    private GameObject averageButtonTransparent;

    public static SearchUI Create(Transform parent, Context context)
    {
        Transform searchContainer = UIBuilder.CreateSearchContainer(parent);
        TMP_InputField searchBar = UIBuilder.CreateSearchBar(searchContainer);
        Transform scroll = UIBuilder.CreateScrollView(searchContainer);

        SearchUI ui = new SearchUI();
        ui.resultsContainer = scroll;
        ui.builtScrollRect = scroll != null ? scroll.GetComponentInParent<ScrollRect>() : null;
        ui.Context = context;
        searchBar.onSubmit.AddListener(ui.OnSearchSubmit);

        return ui;
    }

    public void Init(List<AudioClip> clips, GameObject playlistItemPrefab, GameObject musicItemPrefab, GameObject averageButtonTransparent)
    {
        musiques = clips;
        this.playlistItemPrefab = playlistItemPrefab;
        this.musicItemPrefab = musicItemPrefab;
        this.averageButtonTransparent = averageButtonTransparent;
    }

    public static AudioClip RechercherClip(string nomMusique, List<AudioClip> musiques)
    {
        return musiques.FirstOrDefault(c => c.name.ToLower().Contains(nomMusique.ToLower()));
    }

    private void OnSearchSubmit(string nomTape)
    {
        RectTransform contentRT = UIBuilder.CreateScrollContent(resultsContainer);

        var vlg = contentRT.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.childControlHeight    = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth     = true;
            vlg.childForceExpandWidth  = true;
            vlg.spacing = 6;
            vlg.childAlignment = TextAnchor.UpperCenter;
        }

        if (string.IsNullOrWhiteSpace(nomTape))
            return;

        nomTape = nomTape.ToLower();

        var resultats = musiques
            .Where(c => c.name.ToLower().Contains(nomTape))
            .ToList();

        if (musicItemPrefab == null)
        {
            Debug.LogError("musicItemPrefab n'est pas assigné dans SearchUI.Init()");
            return;
        }

        foreach (var clip in resultats)
        {
            GameObject item = UIBuilder.InstantiateMusicItemRow(musicItemPrefab, contentRT, MusicItemRowHeight);

            TMP_Text txt = item.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.text = clip.name;

            Transform addButtonTransform = item.transform.Find("AddToPlaylistButton");
            if (addButtonTransform != null)
            {
                Button addButton = addButtonTransform.GetComponent<Button>();
                addButton.onClick.AddListener(() =>
                {
                    PopupManager.ShowPlaylistPopup(clip.name, averageButtonTransparent);
                });
            }

            Transform playButtonTransform = item.transform.Find("PlayButton");
            if (playButtonTransform == null)
            {
                Debug.LogError("PlayButton introuvable dans le prefab MusicItem !");
                continue;
            }

            Button playButton = playButtonTransform.GetComponent<Button>();
            playButton.onClick.AddListener(() =>
            {
                if (Context != null && Context.TryGetAudioSource(out AudioSource source))
                    source.clip = clip;

                if (Context != null)
                {
                    Context.SetSliderVisible(true);
                    Context.SetPlayPauseVisible(true);
                }

                PopupManager.Show("Musique sélectionnée : " + clip.name);
            });
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);

        // Recherche complémentaire via l'API Jamendo
        if (ApiClient.Instance != null && Context != null)
            Context.StartCoroutine(SearchJamendoApi(nomTape, contentRT));
    }

    // ── Recherche Jamendo via API ─────────────────────────────────────────────

    private IEnumerator SearchJamendoApi(string query, RectTransform container)
    {
        ApiJamendoTrack[] tracks = null;

        yield return ApiClient.Instance.StartCoroutine(ApiClient.Instance.SearchJamendo(
            query, 10,
            results => tracks = results,
            err => Debug.LogWarning("Jamendo search: " + err)
        ));

        if (tracks == null || tracks.Length == 0) yield break;

        // Séparateur
        GameObject header = new GameObject("JamendoHeader", typeof(RectTransform));
        header.transform.SetParent(container, false);
        TMP_Text headerTxt = header.AddComponent<TextMeshProUGUI>();
        headerTxt.text = "— Résultats Jamendo —";
        headerTxt.fontSize = 14;
        headerTxt.alignment = TextAlignmentOptions.Center;
        headerTxt.color = new Color(0.7f, 0.5f, 1f);
        header.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 30);

        foreach (var track in tracks)
        {
            ApiJamendoTrack captured = track;

            GameObject row = new GameObject("JamendoItem", typeof(RectTransform));
            row.transform.SetParent(container, false);
            row.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.05f);
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);

            HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.childControlWidth     = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight    = true;
            hlg.spacing = 8;
            hlg.padding = new RectOffset(6, 6, 4, 4);

            // Label nom + artiste
            GameObject labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(row.transform, false);
            TMP_Text label = labelGO.AddComponent<TextMeshProUGUI>();
            label.text = $"{captured.name}\n<size=11><color=#aaa>{captured.artist_name}</color></size>";
            label.fontSize = 14;
            label.color = Color.white;
            labelGO.AddComponent<LayoutElement>().flexibleWidth = 1;

            // Bouton Sélectionner
            GameObject btnGO = new GameObject("SelectBtn", typeof(RectTransform));
            btnGO.transform.SetParent(row.transform, false);
            btnGO.AddComponent<Image>().color = new Color(0.4f, 0.2f, 0.8f, 0.9f);
            Button btn = btnGO.AddComponent<Button>();
            LayoutElement btnLE = btnGO.AddComponent<LayoutElement>();
            btnLE.preferredWidth  = 100;
            btnLE.preferredHeight = 40;

            GameObject btnLabelGO = new GameObject("Text", typeof(RectTransform));
            btnLabelGO.transform.SetParent(btnGO.transform, false);
            TMP_Text btnTxt = btnLabelGO.AddComponent<TextMeshProUGUI>();
            btnTxt.text = "Sélectionner";
            btnTxt.fontSize = 12;
            btnTxt.alignment = TextAlignmentOptions.Center;
            btnTxt.color = Color.white;
            RectTransform btnLabelRT = btnLabelGO.GetComponent<RectTransform>();
            btnLabelRT.anchorMin = Vector2.zero;
            btnLabelRT.anchorMax = Vector2.one;
            btnLabelRT.offsetMin = Vector2.zero;
            btnLabelRT.offsetMax = Vector2.zero;

            btn.onClick.AddListener(() =>
            {
                if (SessionData.Instance != null)
                {
                    SessionData.Instance.jamendoTrackId = captured.id;
                    SessionData.Instance.titre          = captured.name;
                }
                PopupManager.Show("Sélectionné : " + captured.name);
            });
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }

    public void SetResultsContainer(Transform container)
    {
        this.resultsContainer = container;
        if (builtScrollRect != null)
        {
            bool useBuiltScroll =
                container == builtScrollRect.content ||
                container == builtScrollRect.transform ||
                (container != null && container.IsChildOf(builtScrollRect.transform));
            builtScrollRect.gameObject.SetActive(useBuiltScroll);
        }
    }
}
