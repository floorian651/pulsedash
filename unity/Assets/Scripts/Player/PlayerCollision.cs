using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private ScreenFlash screenFlash;
    [SerializeField] private bool allowTriggerCollisions = true;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        if (screenFlash == null)
        {
            screenFlash = FindObjectOfType<ScreenFlash>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!allowTriggerCollisions) return;
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject other)
    {
        string tag = other.tag;

        if (tag != "obstacle" && tag != "Bonus" && tag != "pulser") return;

        if (player == null)
        {
            Debug.LogError("Player component not found!");
            return;
        }

        if (tag == "obstacle" || tag == "pulser")
        {
            player.TakeDamage(5f);
            if (screenFlash == null)
            {
                Debug.LogError("ScreenFlash not found in scene or not assigned!");
            }
            else
            {
                screenFlash.Flash();
            }
        }
        else if (tag == "Bonus")
        {
            player.Heal(5f);
        }

        Destroy(other.gameObject);
        Debug.Log("objet détruit");
    }
}
