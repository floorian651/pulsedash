using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Slider energySlider;
    [SerializeField] private GenerateurNiveau generateurNiveau;
    private float duration;

    void Awake()
    {
        energySlider = GetComponent<Slider>();
        if (energySlider != null)
        {
            if (generateurNiveau != null)
            {
                UnityEngine.Debug.Log("generateurNiveau non null");
                duration = generateurNiveau.GetMusicDuration();
            }
            else
            {
                UnityEngine.Debug.Log("generateurNiveau null");
                duration = 100;
            }
            energySlider.maxValue = 100;
            energySlider.value = 100;
            energySlider.interactable = false;
        }
    }

    public void SetEnergy(float value)
    {
        if (energySlider == null) return;

        energySlider.value = value;
    }
}