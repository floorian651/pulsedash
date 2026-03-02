using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Slider energySlider;
    public float decreaseSpeed = 5f;

    void Awake()
    {
        energySlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (energySlider != null && energySlider.value > 0)
        {
            energySlider.value -= decreaseSpeed * Time.deltaTime;
        }
    }
}