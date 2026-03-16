using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float distanceSol = 3f;

    public float distJoueur = 5f;


    Animator anim;

    // bool isGrounded = true; 

    void Start()
    {
        Destroy(gameObject,10);

    }


    void OnCollisionStay(Collision collision)
    {
        // Détection sol
        if (collision.gameObject.CompareTag("sol"))
        {
            
            Destroy(gameObject,1);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Mort si joueur
        if (collision.gameObject.CompareTag("Player"))
        {
            Mourir();
        }
    }

    public void Mourir()
    {
        moveSpeed = 0;
    }
}
