using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementE5 : MonoBehaviour
{
    Rigidbody rb;

    Vector3 jump;
    float moveForward = 4f;
    float moveRight = 2f;
    float moveUp = 20f;
    float maxSpeed = 5f;
    float currentForwardSpeed;
    float currentBackwardSpeed;
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
        float moveZ = Input.GetAxis("Vertical") * moveForward;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveX;
        velocity.z = moveZ;

        rb.linearVelocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }
}