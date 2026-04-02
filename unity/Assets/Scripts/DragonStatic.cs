using UnityEngine;
using System.Collections;



public class DragonStatic : MonoBehaviour
{
    public float distanceSol = 3f;

    public Rigidbody bonus;   

    Animator anim;
    Rigidbody rb;

    bool estMort = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Position fixe du dragon
        //transform.position = new Vector3(positionX, distanceSol, 0);

        // Le dragon ne doit pas bouger
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Lancer l’animation de vol
        anim.SetTrigger("isFlying");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Mort si le joueur tombe dessus (normal.y < -0.5)
        if (!estMort &&
            collision.gameObject.CompareTag("Player") &&
            collision.contacts[0].normal.y < -0.5f)
        {
            Mourir();
        }
    }

    void Mourir()
    {
        estMort = true;

        // Jouer l’animation de mort
        anim.SetTrigger("mort");

        // Désactiver le collider après la mort
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
