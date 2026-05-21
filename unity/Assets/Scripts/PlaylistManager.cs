using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class PlaylistManager : MonoBehaviour
{
    public List<Playlist> playlists = new List<Playlist>();
    private Context Context;

    [SerializeField] private PlaylistDAO playlistDAO;
    [SerializeField] private MusicDAO musicDAO;

    public System.Action onLoaded;

    public bool forceNext = false;
    public bool forcePrevious = false;
    public bool stopCurrentTrack = false;

    void Start()
    {
        playlistDAO.GetAllPlaylists(OnPlaylistsLoaded);
    }

    void OnPlaylistsLoaded(List<PlaylistData> data)
    {
        if (data == null) return;
        playlists.Clear();
        foreach (PlaylistData pd in data)
        {
            Playlist p = new Playlist { name = pd.name };
            if (pd.tracks != null)
            {
                foreach (TrackData td in pd.tracks)
                    p.tracks.Add(new Track { id = td.id, title = td.music_title, order = td.position });
            }
            playlists.Add(p);
        }
        onLoaded?.Invoke();
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

    public void CreatePlaylist(string playlistName, System.Action onSuccess = null)
    {
        playlistDAO.CreatePlaylist(playlistName, (data, success) =>
        {
            if (success)
            {
                playlists.Add(new Playlist { name = data.name });
                onSuccess?.Invoke();
            }
            else
                PopupManager.Show("Erreur lors de la création de la playlist.");
        });
    }

    public void AddTrackToPlaylist(string playlistName, string trackName)
    {
        Playlist p = GetPlaylist(playlistName);
        if (p == null) return;

        if (p.tracks.Any(tr => tr.title == trackName)) return;

        playlistDAO.AddTrack(playlistName, trackName, (data, success) =>
        {
            if (success)
                p.tracks.Add(new Track { id = data.id, title = data.music_title, order = data.position });
            else
                PopupManager.Show("Erreur lors de l'ajout de la musique.");
        });
    }

    public void RemoveTrackFromPlaylist(string playlistName, string trackName)
    {
        Playlist p = playlists.Find(x => x.name == playlistName);
        if (p == null) return;

        Track track = p.tracks.FirstOrDefault(tr => tr.title == trackName);
        if (track == null) return;

        playlistDAO.RemoveTrack(track.id, success =>
        {
            if (success)
            {
                p.tracks.Remove(track);
                for (int i = 0; i < p.tracks.Count; i++)
                    p.tracks[i].order = i;
            }
            else
                PopupManager.Show("Erreur lors de la suppression de la musique.");
        });
    }

    public bool RemovePlaylist(string playlistName, System.Action onSuccess = null)
    {
        Playlist p = playlists.Find(x => x.name == playlistName);
        if (p == null) return false;

        playlistDAO.DeletePlaylist(playlistName, success =>
        {
            if (success)
            {
                playlists.Remove(p);
                onSuccess?.Invoke();
            }
            else
                PopupManager.Show("Erreur lors de la suppression de la playlist.");
        });
        return true;
    }

    public Playlist GetPlaylist(string playlistName)
    {
        return playlists.Find(x => x.name == playlistName);
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
    public void LancerPlaylist(GameObject PreviousButtonPrefab, GameObject NextButtonPrefab,GameObject averageButtonPrefab, GameObject musicItemPrefab, Track trackactuel, List<AudioClip> clips, List<Track> toutesLesMusiques, string playlistName, Transform centerRightContainer)
    {   
        UIBuilder.ShowMusiquesPlaylistInContainer(PreviousButtonPrefab, NextButtonPrefab,averageButtonPrefab, musicItemPrefab, clips, playlistName, centerRightContainer);

        if (trackactuel == null)
        {
            PopupManager.Show("Playlist vide");
            return;
    }

    StartCoroutine(RoutinePlaylist( trackactuel, clips, toutesLesMusiques));
}   
   
    IEnumerator FetchAndCacheClip(string musicTitle, List<AudioClip> clips)
    {
        bool urlDone = false;
        string downloadUrl = null;
        musicDAO.GetMusicDownloadUrl(musicTitle, url => { downloadUrl = url; urlDone = true; });
        yield return new WaitUntil(() => urlDone);

        if (string.IsNullOrEmpty(downloadUrl)) yield break;

        yield return musicDAO.DownloadAndCacheClip(downloadUrl, musicTitle + ".mp3", clip =>
        {
            if (clip != null) clips.Add(clip);
        });
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
        if (SearchUI.RechercherClip(trackActuel.title, clips) == null && musicDAO != null)
            yield return StartCoroutine(FetchAndCacheClip(trackActuel.title, clips));

        PlayTrack(trackActuel, clips, true);

        // Attendre fin réelle OU action utilisateur (ne pas avancer si pause)
        while (!stopCurrentTrack && source.clip != null && (source.isPlaying || source.time < source.clip.length - 0.01f))
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
