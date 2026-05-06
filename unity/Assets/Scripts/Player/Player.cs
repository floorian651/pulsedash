using UnityEngine;

public class Player : MonoBehaviour
{   
    Animator anim;

    public static float minEnergy = 0f;
    public static float maxEnergy = 500f;
    public float decreaseSpeed = 1f;
    [SerializeField] public float energy;

    [SerializeField] private EnergyBar energyBar;
    public OverlayEffect overlayEffect;

    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }
    void Start()
    {   
        energy = maxEnergy;
        energyBar.SetEnergy(energy);
    }

    void Update()
    {
        if (energy > 0)
        {
            energy -= decreaseSpeed * Time.deltaTime;
            energyBar.SetEnergy(energy);
        }

    }

    public void TakeDamage(float damage)
    {
        if (damage < 0f)
        {
            damage = 0f;
        }
        energy -= damage;
        energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

        energyBar.SetEnergy(energy);

        if (overlayEffect != null)
        {
            overlayEffect.Flash(new Color(1, 0, 0, 0.4f));  // rouge
        }
    }

    public void Heal(float heal)
    {
        if (heal < 0f)
        {
            heal = 0f;
        }
        energy += heal;
        energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

        energyBar.SetEnergy(energy);

        if (overlayEffect != null)
        {
            overlayEffect.Flash(new Color(0, 1, 0, 0.4f));  // vert
        }
    }

    public float GetEnergyLevel()
    {
        return energy;
    }

    public float GetMaxEnergyLevel()
    {
        return maxEnergy;
    }
}