using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementE5 : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;

    Vector3 jump;
    float moveRight = 2f;
    float moveUp = 30f;
    float forwardSpeed = 3f;
    float maxSpeed = 5f;
    float currentLeftSpeed;
    float currentRightSpeed;

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
        anim.SetBool("isGrounded", isGrounded); // ligne ajoutée 
    }

    // Update is called once per frame
    void Update()

    {   // Animation de course
        anim.SetBool("isRunning", true);  // ligne ajoutée 

        transform.position += Vector3.forward * Time.deltaTime * forwardSpeed;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Jumping
            rb.AddForce(jump * moveUp, ForceMode.Impulse);
            anim.SetTrigger("jump");  // ligne ajoutée 

            isGrounded = false;
            anim.SetBool("isGrounded", isGrounded);  // ligne ajoutée 
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal") * moveRight;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveX;

        rb.linearVelocity = Vector3.ClampMagnitude(velocity, maxSpeed);
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