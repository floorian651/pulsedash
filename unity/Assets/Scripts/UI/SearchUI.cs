using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class SearchUI
{
    private const float MusicItemRowHeight = 80f;

    public List<AudioClip> musiques;
    private Context Context;
    private Transform resultsContainer;
    private ScrollRect builtScrollRect;

    private GameObject musicItemPrefab;
    private GameObject averageButtonTransparent;

    private MusicDAO musicDAO;

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

    public void Init(List<AudioClip> clips, GameObject playlistItemPrefab, GameObject musicItemPrefab, GameObject averageButtonTransparent, MusicDAO musicDAO = null)
    {
        musiques = clips;
        this.musicItemPrefab = musicItemPrefab;
        this.averageButtonTransparent = averageButtonTransparent;
        this.musicDAO = musicDAO;
    }

    public static AudioClip RechercherClip(string nomMusique, List<AudioClip> musiques)
    {
        return musiques.FirstOrDefault(c => c.name.ToLower().Contains(nomMusique.ToLower()));
    }

    private void OnSearchSubmit(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;

        RectTransform contentRT = UIBuilder.CreateScrollContent(resultsContainer);
        var vlg = contentRT.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
            vlg.spacing = 6;
            vlg.childAlignment = TextAnchor.UpperCenter;
        }

        // Résultats locaux
        var localResults = musiques?
            .Where(c => c.name.ToLower().Contains(query.ToLower()))
            .ToList() ?? new List<AudioClip>();

        foreach (AudioClip clip in localResults)
            AddLocalMusicItem(contentRT, clip);

        // Résultats Jamendo
        if (musicDAO != null)
            musicDAO.StartCoroutine(SearchJamendoAndDisplay(query, contentRT));
        else
            Debug.LogWarning("MusicDAO non assigné dans SearchUI.");

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);
    }

    IEnumerator SearchJamendoAndDisplay(string query, RectTransform contentRT)
    {
        bool done = false;
        JamendoTrack[] tracks = null;

        musicDAO.SearchJamendo(query, results =>
        {
            tracks = results;
            done = true;
        });

        yield return new WaitUntil(() => done);

        if (tracks == null || tracks.Length == 0) yield break;

        foreach (JamendoTrack track in tracks)
            AddJamendoMusicItem(contentRT, track);

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);
    }

    private void AddLocalMusicItem(RectTransform contentRT, AudioClip clip)
    {
        if (musicItemPrefab == null) return;

        GameObject item = UIBuilder.InstantiateMusicItemRow(musicItemPrefab, contentRT, MusicItemRowHeight);

        TMP_Text txt = item.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.text = clip.name;

        Transform addBtn = item.transform.Find("AddToPlaylistButton");
        if (addBtn != null)
        {
            addBtn.gameObject.SetActive(true);
            addBtn.GetComponent<Button>().onClick.AddListener(() =>
                PopupManager.ShowPlaylistPopup(clip.name, averageButtonTransparent));
        }

        Transform playBtn = item.transform.Find("PlayButton");
        if (playBtn != null)
            playBtn.GetComponent<Button>().onClick.AddListener(() => PlayClip(clip));
    }

    private void AddJamendoMusicItem(RectTransform contentRT, JamendoTrack track)
    {
        if (musicItemPrefab == null) return;

        GameObject item = UIBuilder.InstantiateMusicItemRow(musicItemPrefab, contentRT, MusicItemRowHeight);

        TMP_Text txt = item.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.text = $"{track.name} — {track.artist_name} [Jamendo]";

        Transform addBtn = item.transform.Find("AddToPlaylistButton");
        if (addBtn != null)
        {
            addBtn.gameObject.SetActive(true);
            addBtn.GetComponent<Button>().onClick.AddListener(() =>
                musicDAO.StartCoroutine(ImportAndAddToPlaylist(track)));
        }

        Transform playBtn = item.transform.Find("PlayButton");
        if (playBtn != null)
            playBtn.GetComponent<Button>().onClick.AddListener(() =>
                musicDAO.StartCoroutine(ImportAndPlay(track, addBtn)));
    }

    IEnumerator ImportAndPlay(JamendoTrack track, Transform addBtn = null)
    {
        PopupManager.ShowLoading("Import en cours...");

        bool done = false;
        JamendoImportResponse importResponse = null;

        musicDAO.ImportTrack(track.id, (response, success) =>
        {
            importResponse = success ? response : null;
            done = true;
        });

        yield return new WaitUntil(() => done);

        if (importResponse == null)
        {
            PopupManager.HideLoading();
            PopupManager.Show("Échec de l'import.");
            yield break;
        }

        string musicTitle = importResponse.music_title;
        PopupManager.ShowLoading("Téléchargement...");

        string downloadUrl = musicDAO.GetAudioDownloadUrl(musicTitle);
        AudioClip clip = null;
        yield return musicDAO.DownloadAndCacheClip(downloadUrl, musicTitle + ".mp3", result => { clip = result; });

        PopupManager.HideLoading();

        if (clip == null)
        {
            PopupManager.Show("Erreur lors du chargement audio.");
            yield break;
        }

        musiques?.Add(clip);

        if (SessionData.Instance != null)
        {
            SessionData.Instance.titre = musicTitle;
            if (string.IsNullOrEmpty(SessionData.Instance.mode))
                SessionData.Instance.mode = "Lazy";
        }
        SceneManager.LoadScene("GameplayScene");
    }

    IEnumerator ImportAndAddToPlaylist(JamendoTrack track)
    {
        PopupManager.ShowLoading("Import en cours...");

        bool done = false;
        JamendoImportResponse importResponse = null;
        musicDAO.ImportTrack(track.id, (resp, ok) => { importResponse = ok ? resp : null; done = true; });
        yield return new WaitUntil(() => done);

        PopupManager.HideLoading();

        if (importResponse == null)
        {
            PopupManager.Show("Échec de l'import.");
            yield break;
        }

        if (string.IsNullOrEmpty(importResponse.job_id))
        {
            PopupManager.Show("Ajouté avec le stockage serveur");
            yield break;
        }

        PopupManager.ShowPlaylistPopup(importResponse.music_title, averageButtonTransparent);
    }

    private void PlayClip(AudioClip clip)
    {
        if (Context != null && Context.TryGetAudioSource(out AudioSource source))
        {
            source.clip = clip;
            source.Play();
        }
        if (Context != null)
        {
            Context.SetSliderVisible(true);
            Context.SetPlayPauseVisible(true);
        }
    }

    public void SetResultsContainer(Transform container)
    {
        resultsContainer = container;
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
