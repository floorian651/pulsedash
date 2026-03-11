using UnityEngine;

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
}