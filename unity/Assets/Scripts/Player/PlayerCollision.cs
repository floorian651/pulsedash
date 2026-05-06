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
        if (collision.contacts.Length > 0)
        {
            Vector3 normal = collision.contacts[0].normal;

            // Si la collision vient du dessus (le joueur tombe sur l'objet)
            if (Vector3.Dot(normal, Vector3.up) > 0.5f)
            {
                return; // ignore la collision
            }
        }
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

        if (tag != "obstacle" && tag != "Bonus" && tag != "pulser" && tag != "Finish") return;

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
        else if (tag == "Finish")
        {
            Debug.Log("Collision finish");

            // Loading the finish scene
            SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
            if (sceneloader != null)
            {
                sceneloader.LoadSceneByName("FinishScene");
            }
        }

        Destroy(other.gameObject);
        Debug.Log("objet détruit");
    }
}
