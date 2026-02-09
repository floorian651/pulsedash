using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using System.IO;
using System.Linq;

public class PlaylistManager : MonoBehaviour
{
    public List<Playlist> playlists = new List<Playlist>(); // Liste des playlist

    private string savePath; //chemin

    public bool forceNext = false;
    public bool forcePrevious = false;

    public bool stopCurrentTrack = false;



    void Start()
    {
        Debug.Log("PlaylistManager peut start!");
        savePath = Path.Combine(Application.persistentDataPath, "playlists.json");
        LoadPlaylists();
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
    public void LancerPlaylist( Track trackactuel, List<AudioClip> clips, List<Track> toutesLesMusiques)
{
    StartCoroutine(RoutinePlaylist( trackactuel, clips, toutesLesMusiques));
}   
    /*
    IEnumerator RoutinePlaylist(string nomplaylist, Track trackactuel, List<AudioClip> clips, List<Track> toutesLesMusiques)
    {   
        PlayTrack(trackactuel, clips, false);  

        // attendre que la musique est commencée

        while (!MenuGenerator.audioSource.isPlaying)
            yield return null;

        while (trackactuel != null )
        {
            //Récupérer le numérdo du track
            int orderTrack = trackactuel.order;

            Track trackNext = null;

            // Récupérer le track de la musique suivante

            switch (mode)
                {
                    case PlayMode.Normal:
                        Debug.Log("Mode normal");
                        trackNext = toutesLesMusiques.Find(t => t.order == orderTrack + 1);

                         if (trackNext != null)
                        {                               
                            Debug.Log("Prochaine musique de la playlist sélectionnée : "+ trackNext.title);

                            // Attendre la fin réelle du morceau
                            while (MenuGenerator.audioSource.isPlaying)
                                yield return null;    

                            Debug.Log("Le morceau est fini!");                   
                        }
                        break;

                    case PlayMode.Next:
                        Debug.Log("Mode next");
                        
                        trackNext = toutesLesMusiques.Find(t => t.order == orderTrack + 1);
                        if (trackNext == null)
                        {  PopupManager.Show("Fin de la playlist");
                        }
                        break;

                    case PlayMode.Previous:
                        Debug.Log("Mode previous");
                        if (orderTrack > 0)
                        {
                            trackNext = toutesLesMusiques.Find(t => t.order == orderTrack - 1);   
                        }                        
                        break;

                    default:
                        Debug.Log("Autre valeur");
                        break;
                }   

                mode = PlayMode.Normal;     
                trackactuel = trackNext;       
                if (trackactuel != null)
                    {                                
                    PlayTrack(trackactuel, clips, true);
                    }}
        Debug.Log("PROBLEME!");
        }
    */
    IEnumerator RoutinePlaylist(
    Track trackActuel,
    List<AudioClip> clips,
    List<Track> toutesLesMusiques
)
{
    while (trackActuel != null)
    {
        PlayTrack(trackActuel, clips, true);

        // attendre fin OU action utilisateur
        while (MenuGenerator.audioSource.isPlaying && !stopCurrentTrack)
            yield return null;

        MenuGenerator.audioSource.Stop();

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
        // Affichages pour le nouveau trackactuel
        PopupManager.Show(trackactuel.title +" sélectionnée");
        MenuGenerator.messageText.text = trackactuel.title;

        // Chercher le clip correspondant
        AudioClip clip = SearchUI.RechercherClip(trackactuel.title, clips);

        // Attribuer le clip trouvé à l'audio source
        MenuGenerator.audioSource.clip = clip;

        if (aJouer)
        {
            // Jouer la musique
        MenuGenerator.audioSource.Play();
        Debug.Log("Jouer musique " +trackactuel.title);
        }
        
    }

    IEnumerator PlayNextWhenFinished(AudioClip nextClip)
{

    // Attendre la fin réelle du morceau
    while (MenuGenerator.audioSource.isPlaying || MenuGenerator.audioSource.time < MenuGenerator.audioSource.clip.length)
        yield return null;

    Debug.Log("Fin réelle du morceau détectée");

    MenuGenerator.audioSource.clip = nextClip;
    MenuGenerator.audioSource.Play();
}



}
