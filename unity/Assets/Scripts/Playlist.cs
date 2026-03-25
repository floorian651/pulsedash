using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Playlist
{
    public string name; // nom de la playlist
    public List<Track> tracks = new List<Track>();// noms des fichiers MP3
}
