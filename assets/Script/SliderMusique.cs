using UnityEngine;
using UnityEngine.UI;

public class SliderMusique : MonoBehaviour
{
    public Slider slider;

    private bool utilisateurChangeValeur = false;

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        if (MenuGenerator.audioSource.clip == null)
            return;

        // Si l'utilisateur n'est PAS en train de déplacer le curseur
        if (!utilisateurChangeValeur)
        {
            slider.value = MenuGenerator.audioSource.time / MenuGenerator.audioSource.clip.length;
        }

    }

    void OnSliderChanged(float value)
    {   
        //Debug.Log("Position curseur : "+value);

        utilisateurChangeValeur = true;

        if (MenuGenerator.audioSource.clip != null)
        {
            MenuGenerator.audioSource.time = value * MenuGenerator.audioSource.clip.length;
            Debug.Log("Temps : "+ MenuGenerator.audioSource.time);
        }

        utilisateurChangeValeur = false;
    }
}
