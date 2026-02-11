using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementE5 : MonoBehaviour
{
    Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 1.0f, 0.0f);
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.forward * Time.deltaTime * forwardSpeed;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Jumping
            rb.AddForce(jump * moveUp, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal") * moveRight;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveX;

        rb.linearVelocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }
}