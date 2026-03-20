using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System.Linq;


public class MusiqueManager : MonoBehaviour
{
    public JamendoAPI jamendoAPI;
    public AudioCache audioCache;

    public string genre = "Mettre le style";

    public string title = "Mettre le titre";

    // Télécharger les musiques selon le genre ou le titre
    void Awake()
    {
        StartCoroutine(jamendoAPI.GetTrackByGenre(genre, OnTrackReceived));

        StartCoroutine(jamendoAPI.SearchTrackByTitle(title, OnTrackReceived));
        
        Debug.Log("Télécharger des musiques selon le genre et le titre");

    }

    void OnTrackReceived(JamendoTrack[] tracks)
    {
        if (tracks == null || tracks.Length == 0) { 
            Debug.LogError("Aucune musique trouvée."); 
            return; }

        foreach (var track in tracks)
        {
            string fileName = track.name.Replace(" ", "_") + ".mp3";
            audioCache.LoadMusic(track.audio, fileName);
            Debug.Log("Télécharger la musique dans le cache: "+ fileName);
        }

    }
}
