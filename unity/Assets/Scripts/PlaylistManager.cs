using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using System.IO;
using System.Linq;

public class PlaylistManager : MonoBehaviour
{
    public List<Playlist> playlists = new List<Playlist>(); // Liste des playlist
    private Context Context;

    private string savePath; //chemin

    public bool forceNext = false;
    public bool forcePrevious = false;

    public bool stopCurrentTrack = false;



    void Awake()
    {
        // Préparer le chemin et charger tôt pour éviter les problèmes d'ordre d'exécution
        savePath = Path.Combine(Application.persistentDataPath, "playlists.json");
        LoadPlaylists();
    }

    void Start()
    {
        Debug.Log("PlaylistManager peut start!");
    }

    private bool TryGetAudioSource(out AudioSource source)
    {
        if (Context == null)
        {
            Context = Object.FindObjectOfType<Context>();
        }

        if (Context != null)
        {
            return Context.TryGetAudioSource(out source);
        }

        source = null;
        return false;
    }

    // Créer une playlist
    public void CreatePlaylist(string playlistName)
    {
        Playlist p = new Playlist();
        p.name = playlistName; // donner un nom à la playlist créée
        playlists.Add(p); //ajouter la nouvelle playlist à la liste des playlists
        SavePlaylists();
    }

    public void AddTrackToPlaylist(string playlistName, string trackName)
    {   
        // Récupérer la playlist
        Playlist p = playlists.Find(x => x.name == playlistName);

        // Si la playlist existe

        if (p != null)
        {   
            bool dejaDansPlaylist = p.tracks.Any(tr => tr.title == trackName);
        // Si la musique n'est pas déjà dans la playlist
            if (!dejaDansPlaylist)
            {
                Track track = new Track
                {
                    title = trackName,
                    order = p.tracks.Count

                };

                p.tracks.Add(track);

            }
            SavePlaylists();
        }
    }

    // Supprimer une musique d'une playlist
    public void RemoveTrackFromPlaylist(string playlistName, string trackName)
    {
        Playlist p = playlists.Find(x => x.name == playlistName);
        if (p != null)
        {   
            Track trackCherche = p.tracks.FirstOrDefault(tr => tr.title == trackName);
            if (trackCherche != null)
            {
                p.tracks.Remove(trackCherche);

                for (int i = 0; i < p.tracks.Count; i++)
                    p.tracks[i].order = i;
                
            SavePlaylists();
        }
    }}

    // Supprimer une playlist entière
    public bool RemovePlaylist(string playlistName)
    {
        Playlist p = playlists.Find(x => x.name == playlistName);
        if (p == null)
        {
            return false;
        }

        playlists.Remove(p);
        SavePlaylists();
        return true;
    }

    // récupérer une playlist en fonction de son nom
    public Playlist GetPlaylist(string playlistName)
    {
        return playlists.Find(x => x.name == playlistName);
    }

    // sauvegarder en json les playlists dans une fichier 
    public void SavePlaylists()
    {
        string json = JsonUtility.ToJson(new Wrapper { playlists = this.playlists }, true);
        File.WriteAllText(savePath, json);
    }

    // Récupérer la liste de playlist
    public void LoadPlaylists()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            Wrapper w = JsonUtility.FromJson<Wrapper>(json);
            playlists = w.playlists;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<Playlist> playlists;
    }

    public void OnNextPressed()
    {
        forceNext = true;
        stopCurrentTrack = true;
    }

    public void OnPreviousPressed()
    {
        forcePrevious = true;
        stopCurrentTrack = true;
    }
    public void LancerPlaylist(GameObject averageButtonPrefab, GameObject playlistItemPrefab, Track trackactuel, List<AudioClip> clips, List<Track> toutesLesMusiques, string playlistName, Transform centerRightContainer)
{   
    UIBuilder.ShowMusiquesPlaylistInContainer(averageButtonPrefab, playlistItemPrefab, clips, playlistName, centerRightContainer);

    if (trackactuel == null)
    {
        PopupManager.Show("Playlist vide");
        return;
    }

    StartCoroutine(RoutinePlaylist( trackactuel, clips, toutesLesMusiques));
}   
   
    IEnumerator RoutinePlaylist(Track trackActuel, List<AudioClip> clips, List<Track> toutesLesMusiques)
{
    if (!TryGetAudioSource(out AudioSource source))
    {
        Debug.LogError("Context ou AudioSource introuvable.");
        yield break;
    }

    while (trackActuel != null)
    {
        PlayTrack(trackActuel, clips, true);

        // Attendre fin réelle OU action utilisateur (ne pas avancer si pause)
        while (!stopCurrentTrack && (source.isPlaying || source.time < source.clip.length - 0.01f))
            yield return null;

        source.Stop();

        int order = trackActuel.order;
        Track nextTrack = null;

        if (forceNext)
        {
            nextTrack = toutesLesMusiques.Find(t => t.order == order + 1);

            if (nextTrack == null)
            {
                PopupManager.Show("Fin de la playlist");
                break;
            }
        }
        else if (forcePrevious)
        {
            if (order > 0)
                nextTrack = toutesLesMusiques.Find(t => t.order == order - 1);
            else
                nextTrack = trackActuel; // rester sur le premier
        }
        else
        {
            // fin normale
            nextTrack = toutesLesMusiques.Find(t => t.order == order + 1);

            if (nextTrack == null)
            {
                PopupManager.Show("Fin de la playlist");
                break;
            }
        }

        // reset flags
        forceNext = false;
        forcePrevious = false;
        stopCurrentTrack = false;

        trackActuel = nextTrack;
    }

    Debug.Log("Fin de la playlist");
}

    public void PlayTrack(Track trackactuel, List<AudioClip> clips, bool aJouer)
    {   
        if (!TryGetAudioSource(out AudioSource source))
        {
            Debug.LogError("Context ou AudioSource introuvable.");
            return;
        }

        // Affichages pour le nouveau trackactuel
        PopupManager.Show(trackactuel.title +" sélectionnée");
        Context.SetMessage(trackactuel.title);

        // Chercher le clip correspondant
        AudioClip clip = SearchUI.RechercherClip(trackactuel.title, clips);
        if (clip == null)
        {
            PopupManager.Show("Clip introuvable : " + trackactuel.title);
            Debug.LogError("Clip introuvable : " + trackactuel.title);
            return;
        }

        // Attribuer le clip trouvé à l'audio source
        source.clip = clip;
        Context.SetSliderVisible(true);
        Context.SetPlayPauseVisible(true);

        if (aJouer)
        {
            // Jouer la musique
        source.Play();
        Debug.Log("Jouer musique " +trackactuel.title);
        }
        
    }

    IEnumerator PlayNextWhenFinished(AudioClip nextClip)
{
    if (!TryGetAudioSource(out AudioSource source))
    {
        yield break;
    }

    // Attendre la fin réelle du morceau
    while (source.isPlaying || source.time < source.clip.length)
        yield return null;

    Debug.Log("Fin réelle du morceau détectée");

    source.clip = nextClip;
    source.Play();
    Context.SetSliderVisible(true);
    Context.SetPlayPauseVisible(true);
}



}
