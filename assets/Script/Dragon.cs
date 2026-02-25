using UnityEngine;
using System.Collections;


public class Dragon : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float distanceSol = 3f;

    public float distJoueur = 60f;

    public float rayon = 360f;

    public Rigidbody fireball;


    Animator anim;
    Rigidbody rb;

    bool isGrounded = true; 
    float direction = 1; // Vers la droite par défaut

    bool estEnAttaque = false;

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
            Debug.Log("Changer direction");
        }
            
        // Animation de vol
        anim.SetTrigger("isFlying");

        if (DetecterJoueur() && !estEnAttaque)
        {   
            Debug.Log("Joueur détecté");

            transform.rotation = Quaternion.Euler(0,180,0);
            
            anim.SetTrigger("isAttacking");

            estEnAttaque = true;

            StartCoroutine(attendreXFrame(130));

            Rigidbody p = Instantiate(fireball, transform.position, transform.rotation);
            p.linearVelocity = transform.forward * (moveSpeed+2f) ;
        }
        else
        {
            // Animation de vol
            anim.SetTrigger("isFlying");

            // Avance automatique selon l'axe x
            transform.position += Vector3.right * Time.deltaTime * moveSpeed * direction;
            Debug.Log("Je vole");
        }

        
    }

    IEnumerator attendreXFrame(float nombreFrame)
    {
        for (int i = 0; i < nombreFrame; i++) yield return null; // attendre 1 frame
    }

    bool DetecterSol(){

        // Prochaine position du dragon
        Vector3 prochainePosition = transform.position + Vector3.right * direction;

        // Infos de collision 
        RaycastHit hit;

        // Récupérer le layermask
        LayerMask mask = LayerMask.GetMask("sol");

        // Ray vers le sol 
        bool solTouche = Physics.Raycast(prochainePosition, Vector3.down, out hit, distanceSol+20, mask);
        Debug.DrawRay(prochainePosition, Vector3.down * (distanceSol+20), Color.yellow);
        Debug.Log("Hit");

        return solTouche;         

    }

    bool DetecterJoueur(){

        // Position du joueur en approche
        Vector3 positionJoueur = transform.position + Vector3.back;

        // Infos de collision 
        RaycastHit hit;

        // Ray vers le sol 
        bool joueurDetecte = Physics.SphereCast(positionJoueur, rayon, Vector3.back, out hit,  distJoueur+10);
        
        if (joueurDetecte && hit.collider.CompareTag("Player"))
        {   
            // Dragon pivote vers le joueur
            //transform.rotation = Quaternion.Euler(0,0,0);
            Debug.DrawRay(positionJoueur, Vector3.back * (distJoueur+10), Color.red);
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
        if (collision.gameObject.CompareTag("Player"))
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

    public void FinAttaque()
{
    estEnAttaque = false;
    transform.rotation = Quaternion.Euler(0,90,0);
    direction = 1; // Vers la droite
    Debug.Log("FinAttaque!");
}

}
