using UnityEngine;

public class Dragon : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float rotationSpeed = 3f;

    public float distanceSol = 3f;


    Animator anim;
    Rigidbody rb;

    bool isGrounded = true; 
    float direction = 1; // Vers la droite par défaut

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Point de départ
        transform.position = new Vector3(0,distanceSol,0);
    }

    void Update()
    {   
        // Détecter la présence du sol
        if (!DetecterSol()){
            Pivoter();
        }
            
        // Animation de vol
        anim.SetBool("isFlying", true);

        // Avance automatique selon l'axe x
        transform.position += Vector3.right * Time.deltaTime * moveSpeed * direction;

        
    }

    bool DetecterSol(){

        // Prochaine position du dragon
        Vector3 prochainePosition = transform.position + Vector3.right * direction;

        // Infos de collision 
        RaycastHit hit;

        // Ray vers le sol 
        bool solTouche = Physics.Raycast(prochainePosition, Vector3.down, out hit, distanceSol);

        if (solTouche && hit.collider.CompareTag("sol"))
        {      
            Debug.DrawRay(prochainePosition, Vector3.down * distanceSol, Color.yellow);
            Debug.Log("Hit");
            return true;
            
        }
        else {
            return false;
        }

    }

    void Pivoter(){
        rb.linearVelocity = Vector3.zero;

        if (direction==1){
            direction = -1; // Vers la gauche
            transform.rotation = Quaternion.Euler(0,-90,0);}
        else {
            direction = 1; // Vers la droite
            transform.rotation= Quaternion.Euler(0,90,0);}
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
        // Mort si joueur
        if (collision.gameObject.CompareTag("joueur"))
        {
            Mourir();
        }
    }

    public void Mourir()
    {
        anim.SetTrigger("mort");
        moveSpeed = 0;
        rb.linearVelocity = Vector3.zero;
    }
}
