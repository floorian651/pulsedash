using System.Collections.Generic;

// Helpers statiques pour travailler sur les données de niveau reçues depuis l'API.
// HitData et LevelData sont définis dans ApiClient.cs.
public static class LevelDataHelpers
{
    public static List<HitData> GetHitsInInterval(HitData[] hits, float start, float end)
    {
        var result = new List<HitData>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].time >= end) break;
            if (hits[i].time >= start)
                result.Add(hits[i]);
        }
        return result;
    }

    public static float GetMaxStrength(HitData[] hits)
    {
        float max = 0f;
        for (int i = 0; i < hits.Length; i++)
            if (hits[i].strength > max) max = hits[i].strength;
        return max;
    }
}
