using UnityEngine;
using System.Collections;



public class DragonStatic : MonoBehaviour
{
    public float distanceSol = 3f;

    public Rigidbody bonus;   

    Animator anim;
    Rigidbody rb;

    bool estMort = false;

    float positionDragon;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Le dragon ne doit pas bouger
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Lancer l’animation de vol
        anim.SetTrigger("isFlying");

        positionDragon = transform.position.z;
    }

    void OnCollisionEnter(Collision collision)
    {   
        positionDragon = transform.position.z;

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

        // Mettre des bonus
        for (int i = ((int) positionDragon)+5; i <= ((int) positionDragon)+ 55; i += 10){
            Debug.Log(positionDragon);
            Rigidbody p = Instantiate(bonus, new Vector3(0,0.7f,1*i),  Quaternion.Euler(-90,0,0));
        }
    }
}
