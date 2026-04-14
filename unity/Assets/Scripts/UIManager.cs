using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject panelMenu;
    public GameObject panelRecherche;
    public GameObject panelMusique;
    public GameObject panelPlaylist;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterPanels(
        GameObject menu,
        GameObject recherche,
        GameObject musique,
        GameObject playlist)
    {
        panelMenu = menu;
        panelRecherche = recherche;
        panelMusique = musique;
        panelPlaylist = playlist;
    }

    public void UpdateUI(string query)
    {
        HideAll();

        query = query.ToLower();

        if (string.IsNullOrWhiteSpace(query))
            panelMenu.SetActive(true);
        else if (query.Contains("musique"))
            panelMusique.SetActive(true);
        else if (query.Contains("playlist"))
            panelPlaylist.SetActive(true);
        else
            panelRecherche.SetActive(true);
    }

    private void HideAll()
    {
        panelMenu.SetActive(false);
        panelRecherche.SetActive(false);
        panelMusique.SetActive(false);
        panelPlaylist.SetActive(false);
    }
}
