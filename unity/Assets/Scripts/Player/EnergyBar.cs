using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Slider energySlider;

    void Awake()
    {
        energySlider = GetComponent<Slider>();
        if (energySlider != null)
        {
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