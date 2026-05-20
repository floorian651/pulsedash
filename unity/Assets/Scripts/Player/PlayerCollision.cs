using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private ScreenFlash screenFlash;
    [SerializeField] private GameSessionDAO gameSessionDAO;
    [SerializeField] private bool allowTriggerCollisions = true;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        if (screenFlash == null)
            screenFlash = FindObjectOfType<ScreenFlash>();
        if (gameSessionDAO == null)
            gameSessionDAO = FindObjectOfType<GameSessionDAO>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            if (Vector3.Dot(normal, Vector3.up) > 0.5f)
                return;
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
                Debug.LogError("ScreenFlash not found in scene or not assigned!");
            else
                screenFlash.Flash();
        }
        else if (tag == "Bonus")
        {
            player.Heal(5f);
        }
        else if (tag == "Finish")
        {
            float score = (player.GetEnergyLevel() / player.GetMaxEnergyLevel()) * 100f;

            if (SessionData.Instance != null)
                SessionData.Instance.score = score;

            string sessionId = SessionData.Instance?.sessionId;

            if (!string.IsNullOrEmpty(sessionId) && gameSessionDAO != null)
            {
                gameSessionDAO.EndSession(sessionId, score, false, success =>
                {
                    if (!success)
                        Debug.LogWarning("Échec envoi score — session non terminée côté backend.");
                });
            }

            SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
            if (sceneloader != null)
                sceneloader.LoadSceneByName("FinishScene");
        }

        Destroy(other.gameObject);
    }
}
