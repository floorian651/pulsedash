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

    void Damage(float damage)
    {
        if (energySlider.value - damage > 0)
        {
            energySlider.value -= damage;
        }
        else
        {
            energySlider.value = 0;
        }
    }

    void Heal(float heal)
    {
        if (energySlider.value + heal < 100)
        {
            energySlider.value += heal;
        }
        else
        {
            energySlider.value = 100;
        }
    }
}