using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SearchUI
{
    private List<AudioClip> musiques;
    // Zone pour afficher les résultats du menu déroulant 
    private Transform resultsContainer;

    public static SearchUI Create(Transform parent)
    {
        // Conteneur vertical 
        Transform searchContainer = UIBuilder.CreateSearchContainer(parent);

        TMP_InputField searchBar = UIBuilder.CreateSearchBar(searchContainer);
        Transform scroll = UIBuilder.CreateScrollView(searchContainer);

        SearchUI ui = new SearchUI();
        ui.resultsContainer = scroll;

        searchBar.onValueChanged.AddListener(ui.OnSearch);

        return ui;
    }

    public void Init(List<AudioClip> clips)
    {
        musiques = clips;
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
        Button btn = Bouton.CreateButton(resultsContainer, clip.name, () =>
        {
            MenuGenerator.audioSource.clip = clip;
            
            PopupManager.Show("Musique sélectionnée : " + clip.name);

            // modifier le texte dans le mainContent
            MenuGenerator.messageText.text = "Musique sélectionnée : " + clip.name;
            if (MenuGenerator.messageText != null)
            {
                MenuGenerator.messageText.text = "Musique sélectionnée : " + clip.name;
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
        Button addBtn = Bouton.CreateButton(btn.transform, "+", () =>
        {
            PopupManager.ShowPlaylistPopup(clip.name);
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
}