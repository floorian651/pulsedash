using UnityEngine;

public class Player : MonoBehaviour
{
    public static float minEnergy = 0f;
    public static float maxEnergy = 100f;
    public float energy = maxEnergy;

    [SerializeField] private EnergyBar energyBar;

    void Start()
    {
        energyBar.SetEnergy(energy);
    }

    public void TakeDamage(float damage)
    {
        energy -= damage;
        energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

        energyBar.SetEnergy(energy);
    }

    public void Heal(float heal)
    {
        energy += heal;
        energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

        energyBar.SetEnergy(energy);
    }
}
