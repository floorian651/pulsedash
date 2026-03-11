using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Slider energySlider;
    public float decreaseSpeed = 1f;

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

    void Update()
    {
        if (energySlider != null && energySlider.value > 0)
        {
            energySlider.value -= decreaseSpeed * Time.deltaTime;
        }
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