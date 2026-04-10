using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SearchUI
{
    private List<AudioClip> musiques;
    private Context Context;
    // Zone pour afficher les résultats du menu déroulant 
    private Transform resultsContainer;

    private GameObject playlistItemPrefab;

    public static SearchUI Create(Transform parent, Context context)
    {
        // Conteneur vertical 
        Transform searchContainer = UIBuilder.CreateSearchContainer(parent);

        TMP_InputField searchBar = UIBuilder.CreateSearchBar(searchContainer);
        Transform scroll = UIBuilder.CreateScrollView(searchContainer);

        SearchUI ui = new SearchUI();
        ui.resultsContainer = scroll;
        ui.Context = context;
        //searchBar.onSubmit.AddListener(ui.OnSearchSubmit);


        searchBar.onValueChanged.AddListener(ui.OnSearch);

        return ui;
    }

    public void Init(List<AudioClip> clips, GameObject playlistItemPrefab)
    {
        musiques = clips;
        this.playlistItemPrefab = playlistItemPrefab;
    }

    public static AudioClip RechercherClip(string nomMusique, List<AudioClip> musiques)
{
    return musiques.FirstOrDefault(c => c.name.ToLower().Contains(nomMusique.ToLower()));
}


    private void OnSearch(string nomTape)
{
    // Nettoyage des anciens résultats
    foreach (Transform child in resultsContainer)
        Object.Destroy(child.gameObject);

    if (string.IsNullOrWhiteSpace(nomTape))
        return;

    nomTape = nomTape.ToLower();

    var resultats = musiques
        .Where(c => c.name.ToLower().Contains(nomTape))
        .ToList();

    foreach (var clip in resultats)
    {
        // --- BOUTON PRINCIPAL ---
        Button btn = Bouton.CreateButton(resultsContainer, clip.name, new UnityEngine.Vector2(80, 70), () =>
        {
            if (Context != null && Context.TryGetAudioSource(out AudioSource source))
            {
                source.clip = clip;
            }

            if (Context != null)
            {
                Context.SetSliderVisible(true);
                Context.SetPlayPauseVisible(true);
            }
            
            PopupManager.Show("Musique sélectionnée : " + clip.name);

            // modifier le texte dans le mainContent
            if (Context != null && Context.MessageText != null)
            {
                Context.SetMessage(clip.name);
            }
            else
            {
                Debug.LogError("messageText n'est pas encore initialisé");
            }       

});

        // Redimensionner le bouton principal pour le menu déroulant
        LayoutElement le = btn.gameObject.GetComponent<LayoutElement>();
        le.preferredWidth = 180;
        le.preferredHeight = 30;

        // Ajuster le texte à gauche avec un peu de marge
        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        RectTransform txtRT = txt.GetComponent<RectTransform>();
        txtRT.offsetMin = new Vector2(10, 0);
        txtRT.offsetMax = new Vector2(-40, 0);
        txt.alignment = TextAlignmentOptions.MidlineLeft;
        txt.fontSize = 15;

        // --- BOUTON “+” AJOUT À PLAYLIST ---
        Button addBtn = Bouton.CreateButton(btn.transform, "+",new UnityEngine.Vector2(80,70), () =>
        {

            PopupManager.ShowPlaylistPopup(clip.name, playlistItemPrefab);

        });

        RectTransform addRT = addBtn.GetComponent<RectTransform>();
        addRT.anchorMin = new Vector2(1, 0);
        addRT.anchorMax = new Vector2(1, 1);
        addRT.pivot = new Vector2(1, 0.5f);
        addRT.sizeDelta = new Vector2(30, 0);
        addRT.anchoredPosition = new Vector2(-5, 0);

        // Ajuster le texte du "+" pour qu’il soit centré
        TMP_Text addTxt = addBtn.GetComponentInChildren<TMP_Text>();
        addTxt.alignment = TextAlignmentOptions.Center;
        addTxt.fontSize = 20;
    }
}

// Pour l'utiliser il faut créer un prefab avec plusieurs attribut (titre, bouton play, ajouter à une playlist, etc) 
//BEAUCOUP à MODIFIER

private void OnSearchSubmit(string nomTape)
{
    // Nettoyer le container CenterRight
    foreach (Transform child in resultsContainer)
        Object.Destroy(child.gameObject);

    if (string.IsNullOrWhiteSpace(nomTape))
        return;

    nomTape = nomTape.ToLower();

    var resultats = musiques
        .Where(c => c.name.ToLower().Contains(nomTape))
        .ToList();

    foreach (var clip in resultats)
    {
        // Créer un item dans le container CenterRight
        GameObject item = Object.Instantiate(MusicItemPrefab, resultsContainer);

        // Mettre le nom de la musique
        TMP_Text txt = item.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.text = clip.name;

        // Récupérer le bouton Play du prefab
        Transform playButtonTransform = item.transform.Find("PlayButton");
        if (playButtonTransform == null)
        {
            Debug.LogError("PlayButton introuvable dans le prefab MusicItem !");
            continue;
        }

        Button playButton = playButtonTransform.GetComponent<Button>();

        // Ajouter l'action Play
        playButton.onClick.AddListener(() =>
        {
            if (Context != null && Context.TryGetAudioSource(out AudioSource source))
            {
                source.clip = clip;
            }

            Context?.SetSliderVisible(true);
            Context?.SetPlayPauseVisible(true);
            Context?.SetMessage(clip.name);

            PopupManager.Show("Musique sélectionnée : " + clip.name);
        });
    }
}


public void SetResultsContainer(Transform container)
    {
        this.resultsContainer = container;
    }

}
