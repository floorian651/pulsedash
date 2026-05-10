using UnityEngine;
using UnityEngine.UI;

public class SliderMusique : MonoBehaviour
{
    public Slider slider;
    public Context Context;

    private bool utilisateurChangeValeur = false;

    void Start()
    {
        if (slider == null)
        {
            Debug.LogError("SliderMusique: champ 'slider' non initialisé (prefab/factory?).");
            enabled = false;
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = 1f;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        if (Context == null || !Context.TryGetAudioSource(out AudioSource source) || source.clip == null)
            return;

        // Si l'utilisateur n'est PAS en train de déplacer le curseur
        if (!utilisateurChangeValeur)
        {
            slider.value = source.time / source.clip.length;
        }

        // Cacher le curseur à la fin de la musique (pas en pause)
        if (!source.isPlaying && source.time >= source.clip.length - 0.01f)
        {
            Context.SetSliderVisible(false);
        }

    }

    void OnSliderChanged(float value)
    {   
        //Debug.Log("Position curseur : "+value);

        utilisateurChangeValeur = true;

        if (Context != null && Context.TryGetAudioSource(out AudioSource source) && source.clip != null)
        {
            source.time = value * source.clip.length;
            Debug.Log("Temps : " + source.time);
        }

        utilisateurChangeValeur = false;
    }
}
