using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Beat
{
    public float timing;
    public float puissance;
}

[System.Serializable]
public class ApiHit
{
    public float time;
    public int lane;
    public string type;
    public float strength;
}

[System.Serializable]
public class ApiMeta
{
    public float bpm;
    public string key;
    public float duration;
}

[System.Serializable]
public class LevelApiResponse
{
    public ApiMeta meta;
    public ApiHit[] hits;
    
    public MusicData ToMusicData()
    {
        MusicData data = new MusicData();
        data.tempo = (int)meta.bpm;
        data.key = meta.key;
        data.duration = meta.duration;
        
        if (hits != null)
        {
            data.beats = new Beat[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                data.beats[i] = new Beat
                {
                    timing = hits[i].time,
                    puissance = hits[i].strength
                };
            }
        }
        
        return data;
    }
}

[System.Serializable]
public class JobStatus
{
    public string id;          // certains endpoints retournent "id"
    public string job_id;      // certains retournent "job_id"
    public string state;       // "pending", "processing", "completed", "failed"
    public int progress;
    public string result_url;
    public string error;

    public string EffectiveId => !string.IsNullOrEmpty(job_id) ? job_id : id;
}

[System.Serializable]
public class MusicData
{
    public int tempo;
    public string key;
    public Beat[] beats;
    public float duration;
    public float durée => duration;

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

    public float getPuissanceMaxGlobale(){
        float puissanceMaxGlobale=0;
        for (int i=0; i <beats.Length; i++){
            if (puissanceMaxGlobale<beats[i].puissance){
                puissanceMaxGlobale = beats[i].puissance;
            }
        }
        return puissanceMaxGlobale;
    }
}