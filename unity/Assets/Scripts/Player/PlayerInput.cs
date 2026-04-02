using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementE5 : MonoBehaviour
{
    Animator anim;

    Rigidbody rb;

    Vector3 jump;
    public float distanceEntreLigne = 2.0f;
    public enum PositionX{Gauche,Milieu,Droite}
    PositionX currentSidepos = PositionX.Milieu;
    float moveUp = 10f;
    public float forwardSpeed = 3f;

    bool isGrounded;
    void Start()
    {   
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 1.0f, 0.0f);
    }

    void OnCollisionStay()
    {
        isGrounded = true;
        anim.SetBool("isGrounded", isGrounded); 
    }

    // Update is called once per frame
    void Update()

    {   
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        anim.SetBool("isGrounded", isGrounded);
        
        // Animation de course
        anim.SetBool("isRunning", true);  

         
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * moveUp, ForceMode.Impulse);
            anim.SetTrigger("jump");
        }


        float sideMovement = 0;

        // Si on appuye sur Q
        if (Input.GetKeyDown(KeyCode.A) && currentSidepos!=PositionX.Gauche)
        {
            sideMovement = -distanceEntreLigne;
            currentSidepos--;
        }

        // Si on appuye sur D
        if (Input.GetKeyDown(KeyCode.D) && currentSidepos!=PositionX.Droite)
        {
            sideMovement = distanceEntreLigne;
            currentSidepos++;
        }

        Vector3 side = new Vector3(sideMovement, 0, 0);

        // On applique tout les vecteurs à la position
        transform.position += Vector3.forward * Time.deltaTime * forwardSpeed + side;
    }


    // Getters and Setters de vitesse (vroom)
    public float GetSpeed()
    {
        return forwardSpeed;
    }
    public void SetSpeed(float newSpeed)
    {
        forwardSpeed = newSpeed;
    }
}