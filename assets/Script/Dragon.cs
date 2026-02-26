using UnityEngine;
using System.Collections;


public class Dragon : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float distanceSol = 3f;

    public float distJoueur = 60f;

    public float rayon = 360f;

    public Transform player;

    public Rigidbody fireball;


    Animator anim;
    Rigidbody rb;

    float xJ, yJ, zJ;

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
        // Récupérer les coordonnées du Joueur 
        RecupererCoordJoueur();

        // Détecter la présence du sol
        if (!DetecterSol()){
            Pivoter();
            Debug.Log("Changer direction");
        }
            
        // Animation de vol
        anim.SetTrigger("isFlying");

        if (JoueurDetecte() && !estEnAttaque)
        {   
            Debug.Log("Joueur détecté");

            LancerAttaque();

            // Déplacer le dragon pour qu'il soit le plus possible face au joueur
            MoveDragonAttaque();            
        }
        else
        {
            // Avance automatique selon l'axe x
            transform.position += Vector3.right * Time.deltaTime * moveSpeed * direction;
            Debug.Log("Je vole");
        }

        
    }


    IEnumerator TirerApresXFrame(float nombreFrame)
    {
        for (int i = 0; i < nombreFrame; i++) yield return null; // attendre 1 frame

        // Créer une boule de feu
        Rigidbody p = Instantiate(fireball, transform.position+ new Vector3(0,1,0), transform.rotation);
        p.linearVelocity = transform.forward * (moveSpeed+2f) ;
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

    public float Hypo()
    {
        return (Mathf.Sqrt(Mathf.Pow(transform.position.x - xJ,2) +Mathf.Pow(transform.position.z - zJ,2)));
    }

    public float Angle(float hyp)
    {
        float dz = transform.position.z - zJ;

        return Mathf.Asin(dz/hyp)* Mathf.Rad2Deg;;
    }

    public void LancerAttaque()
    {   
        // Récupérer la distance entre le joueur et le dragon
        float hyp = Hypo();

        // Pivoter le dragon de sorte qu'il soit orienté vers le joueur
        transform.rotation= Quaternion.Euler(0,90+ Angle(hyp),0);

        // Lancer l'animation d'attaque
        anim.SetTrigger("isAttacking");

        estEnAttaque = true;

        // Attendre 130 frames pour créer les boules de feu
        StartCoroutine(TirerApresXFrame(100));

        

    }

    public void MoveDragonAttaque()
    {

            if(xJ > transform.position.x)
            {   
                Debug.Log("Vers la droite");
                direction = 1;
                transform.position += Vector3.right * Time.deltaTime * moveSpeed * direction;
            }
            if(xJ < transform.position.x)
            {   
                Debug.Log("Vers la gauche");
                direction = -1;
                transform.position += Vector3.right * Time.deltaTime * moveSpeed * direction;
            }
        
    }

    void RecupererCoordJoueur(){
        Vector3 pos = player.position;
        xJ = pos.x;
        yJ = pos.y;
        zJ = pos.z;

    }

    public bool JoueurDetecte()
    {   
        // Distance selon z entre le dragon et le joueur
        float dz = transform.position.z - zJ;

        // Joueur est détecté si il n'a pas encore dépassé le dragon    
        return dz>0 && dz< distJoueur;
    }


}
