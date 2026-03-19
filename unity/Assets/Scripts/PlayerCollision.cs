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
        Debug.Log("Collions avec " + collision.gameObject.name + " de tag " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("obstacle"))
        {
            if (player != null)
            {
                Debug.Log("Dommages pris");
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
