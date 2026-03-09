using UnityEngine;

public class PlayerSubWaySurfer : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float sideSpeed = 5f;
    public float jumpForce = 7f;
    public float maxSideSpeed = 5f;

    Animator anim;
    Rigidbody rb;
    AudioSource audioSource;

    bool isGrounded = true; 

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Récupérer la musique sélectionnée dans la scene d'avant
        if(SessionData.Instance.audioSource !=null){
            Debug.Log("AudioSource chargé");
            SessionData.Instance.audioSource.Play();
        } 
        
    }

    void Update()
    {
        // Animation de course
        anim.SetBool("isRunning", true);


        // Avance automatique
        transform.position += Vector3.forward * Time.deltaTime * forwardSpeed;

        // Saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("jump");

            isGrounded = false;
            anim.SetBool("isGrounded", isGrounded);

        }
    }

    void FixedUpdate()
    {
        // Déplacement latéral instantané
        float moveX = Input.GetAxisRaw("Horizontal") * sideSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveX;

        // Limite la vitesse latérale
        rb.linearVelocity = Vector3.ClampMagnitude(velocity, maxSideSpeed);
    }

    void OnCollisionStay(Collision collision)
    {
        // Détection sol
        if (collision.gameObject.CompareTag("sol"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", isGrounded);

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Mort si obstacle
        if (collision.gameObject.CompareTag("obstacle"))
        {
            Mourir();
        }
    }

    public void Mourir()
    {
        anim.SetTrigger("mort");
        forwardSpeed = 0;
        sideSpeed = 0;
        rb.linearVelocity = Vector3.zero;
    }
}
