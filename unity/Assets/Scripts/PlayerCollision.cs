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
        if (!collision.gameObject.CompareTag("obstacle")) return;

        // On boucle sur tous les points de contact de la collision
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            // Vérifier si la collision vient d'un côté (±X ou ±Z)
            // La composante Y normale sera proche de 0 si c'est un côté
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

                break; // On ne traite qu'un point de contact valide
            }
            else
            {
                // Collision par le dessus ou dessous : ignorée
                Debug.Log("Collision ignorée car venant de haut ou bas");
            }
        }
    }
}
