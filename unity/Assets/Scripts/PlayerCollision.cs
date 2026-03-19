using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            if (player != null)
            {
                player.TakeDamage(1f);
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.LogError("Player component not found on this GameObject!");
            }
        }
    }
}
