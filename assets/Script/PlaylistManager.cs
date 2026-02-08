using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using System.IO;
using System.Linq;

public class PlaylistManager : MonoBehaviour
{
    public List<Playlist> playlists = new List<Playlist>(); // Liste des playlist

    private string savePath; //chemin


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

    public void LancerPlaylist(string nomplaylist, Track trackactuel, List<AudioClip> clips, List<Track> toutesLesMusiques)
{
    StartCoroutine(RoutinePlaylist(nomplaylist, trackactuel, clips, toutesLesMusiques));
}   
    IEnumerator RoutinePlaylist(string nomplaylist, Track trackactuel, List<AudioClip> clips, List<Track> toutesLesMusiques)
    {   
        AudioClip clip = SearchUI.RechercherClip(trackactuel.title, clips);

        // Attribuer au clip static le clip récupéré
        MenuGenerator.audioSource.clip = clip;

        Debug.Log("Musique de la playlist sélectionnée : "+ trackactuel.title);
        PopupManager.Show(trackactuel.title +" sélectionnée");
        
        MenuGenerator.messageText.text = trackactuel.title;
        //MenuGenerator.audioSource.Play();        

        // attendre que la musique est commencée

        while (!MenuGenerator.audioSource.isPlaying)
            yield return null;

        while (trackactuel != null )
        {
            //Récupérer le numérdo du track
            int orderTrack = trackactuel.order;

            // Récupérer le track de la musique suivante
            Track trackNext = toutesLesMusiques.Find(t => t.order == orderTrack + 1);

            // Passe automatiquement à la musique suivante de la playlist 
            if (trackNext != null)
            {
                Debug.Log("Prochaine musique de la playlist sélectionnée : "+ trackNext.title);

                // Attendre la fin réelle du morceau
                while (MenuGenerator.audioSource.isPlaying)//|| MenuGenerator.audioSource.time < MenuGenerator.audioSource.clip.length)
                    yield return null;    

                Debug.Log("Le morceau est fini!");            

            }
            else
                {   
                    trackNext = toutesLesMusiques.Find(t => t.order == 0);
                    Debug.Log("Fin de la playlist! Donc reprend au début de la playlist!");
                    
                }
                
                trackactuel = trackNext;

                PopupManager.Show(trackactuel.title +" sélectionnée");
                MenuGenerator.messageText.text = trackactuel.title;

                clip = SearchUI.RechercherClip(trackactuel.title, clips);
                MenuGenerator.audioSource.clip = clip;
                MenuGenerator.audioSource.Play();
                Debug.Log("Jouer musique " +trackactuel.title);

        }
        Debug.Log("PROBLEME!");
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
