using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Beat
{
    public float timing;
    public float puissance;
}

[System.Serializable]
public class MusicData
{
    public int tempo;
    public string key;
    public Beat[] beats;
    public float duration;

    public List<Beat> getBeatsInInterval(float start, float end)
    {
        // On va parcourir les beats et récupérer ceux qui sont dans l'intervalle
        List<Beat> beatsInInterval = new List<Beat>();
        for (int i = 0; i < beats.Length; i++)
        {
            if (beats[i].timing >= start && beats[i].timing < end)
            {
                beatsInInterval.Add(beats[i]);
            }
            else if (beats[i].timing >= end)
            {
                break; // Les beats sont triés par timing, on peut arrêter la boucle
            }
        }
        return beatsInInterval;
    }
}