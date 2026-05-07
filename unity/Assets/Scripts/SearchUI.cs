using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class SearchUI : MonoBehaviour
{
    private const float MusicItemRowHeight = 80f;
    private const float MusicItemRowScale = 4f;

	public List<AudioClip> musiques;
	private Context Context;
	    // Zone pour afficher les résultats du menu déroulant 
	private Transform resultsContainer;
	    // ScrollView créé sous la barre de recherche (peut bloquer les clics si on affiche les résultats ailleurs)
	private ScrollRect builtScrollRect;

    private GameObject playlistItemPrefab;
    private GameObject musicItemPrefab;
    private GameObject averageButtonTransparent;
    
    public static SearchUI Create(Transform parent, Context context)
    {
        // Conteneur vertical 
        Transform searchContainer = UIBuilder.CreateSearchContainer(parent);

        TMP_InputField searchBar = UIBuilder.CreateSearchBar(searchContainer);
        Transform scroll = UIBuilder.CreateScrollView(searchContainer);

        SearchUI ui = new SearchUI();
        ui.resultsContainer = scroll;
        ui.builtScrollRect = scroll != null ? scroll.GetComponentInParent<ScrollRect>() : null;
        ui.Context = context;
        // Soumission (Entrée) pour l'affichage long
        searchBar.onSubmit.AddListener(ui.OnSearchSubmit);
        //searchBar.onValueChanged.AddListener(ui.OnSearch); il faudra le remettre pour MenuGenerator

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

public List<AudioClip> LoadAllMusicTestLienBDD(){

        List<AudioClip> clips = new List<AudioClip>(Resources.LoadAll<AudioClip>("Musique"));


        foreach (AudioClip clip in clips)
        {
            Debug.Log("Titre : " + clip.name);
        }

        Debug.Log("Nombre de musiques chargées : " + clips.Count);

        return clips;
    }

// Pour l'utiliser il faut créer un prefab avec plusieurs attribut (titre, bouton play, ajouter à une playlist, etc) 
	//BEAUCOUP à MODIFIER

	private void OnSearchSubmit(string nomTape)
	{   
        RectTransform contentRT = UIBuilder.CreateScrollContent(resultsContainer);


		    // Le container du middle area doit empiler des items de façon compacte.
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

	    if (string.IsNullOrWhiteSpace(nomTape))
	        return;

    // nomTape = nomTape.ToLower();


    MusicDAO musicDAO = new MusicDAO();
    StartCoroutine(musicDAO.GetMusic(nomTape));
    List<AudioClip> musiques = LoadAllMusicTestLienBDD();

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
		        // Créer une "ligne" de liste, puis mettre le prefab dedans sans le redimensionner (asset inchangé).
		        GameObject item = UIBuilder.InstantiateMusicItemRow(musicItemPrefab, contentRT, MusicItemRowHeight);

		        // Mettre le nom de la musique
		        TMP_Text txt = item.GetComponentInChildren<TMP_Text>();
		        if (txt != null)
		            txt.text = clip.name;
	
		        // La hauteur visible en liste est pilotée par le conteneur "row".
	
	
	        // Récupérer le bouton + du prefab
	        Transform addButtonTransform = item.transform.Find("AddToPlaylistButton");
            if (addButtonTransform != null)
            {
                Button addButton = addButtonTransform.GetComponent<Button>();
                addButton.onClick.AddListener(() =>
                {
                    PopupManager.ShowPlaylistPopup(clip.name,averageButtonTransparent);
                });
            }

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

            if (Context != null)
            {
                Context.SetSliderVisible(true);
                Context.SetPlayPauseVisible(true);
            }

            PopupManager.Show("Musique sélectionnée : " + clip.name);

	            
	        });
	    }

	    var containerRT = contentRT as RectTransform;
		    if (containerRT != null)
		    {
		        LayoutRebuilder.ForceRebuildLayoutImmediate(containerRT);
		    }
		}

		


	public void SetResultsContainer(Transform container)
	    {
	        this.resultsContainer = container;
	        // Si on affiche les résultats dans un autre container (ex: CenterRight), on désactive le ScrollView
	        // créé sous la barre de recherche pour éviter qu'il capture les raycasts au-dessus des boutons.
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
