using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementE5 : MonoBehaviour
{   
    [SerializeField] private EnergyBar energyBar;

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
        
        if (energyBar.GetEnergy()==0){
            Mourir();
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        anim.SetBool("isGrounded", isGrounded);
        
        // Animation de course
        anim.SetBool("isRunning", true);  

         
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * moveUp, ForceMode.Impulse);
            anim.SetTrigger("jump");
        }


        float sideMovement = 0;

        // Si on appuye sur Q
        if (Input.GetKeyDown(KeyCode.A) && currentSidepos!=PositionX.Gauche)
        {
            currentSidepos--;
        }

        // Si on appuye sur D
        if (Input.GetKeyDown(KeyCode.D) && currentSidepos!=PositionX.Droite)
        {
            currentSidepos++;
        }

        UpdatePosition();
        // On applique tout les vecteurs à la position
        transform.position += Vector3.forward * Time.deltaTime * forwardSpeed;
    }

    // Update la position X en fonction de currentPosX
    private void UpdatePosition()
    {
        Vector3 pos = transform.position;

        if (currentSidepos == PositionX.Gauche)
        {
            pos.x = -distanceEntreLigne;
        }
        else if (currentSidepos == PositionX.Milieu)
        {
            pos.x = 0f;
        }
        else
        {
            pos.x = distanceEntreLigne;
        }

        transform.position = pos;
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

    public void Mourir()
    {
        anim.SetTrigger("mort");
        forwardSpeed = 0;
        rb.linearVelocity = Vector3.zero;
    }
}