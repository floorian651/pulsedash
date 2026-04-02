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
        Debug.LogError("Collision avec " + collision.gameObject.tag);
        if (!collision.gameObject.CompareTag("obstacle")) return;
        Debug.LogError("Collision avec un obstacle");
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;
            if (Mathf.Abs(normal.y) < 0.5f)
            {
                if (player != null)
                {
                    player.TakeDamage(1f);
                    Destroy(collision.gameObject);
                }
                else
                {
                    Debug.LogError("Player component not found!");
                }

                break;
            }
        }
    }
}