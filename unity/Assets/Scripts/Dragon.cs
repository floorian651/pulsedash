using UnityEngine;
using System.Collections;


public class Dragon : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float fallSpeed = 20f;

    public float distanceSol = 3f;

    public float distJoueur = 60f;

    public Transform player;

    public Rigidbody fireball;

    public Rigidbody bonus;

    public bool modeStatic = true;

    public float timer_tir = 5f;

    public float positionX = 0;


    Animator anim;
    Rigidbody rb;

    float xJ, yJ, zJ;

    bool isGrounded = true; 
    float direction = 1; // Vers la droite par défaut

    bool estEnAttaque = false;

    bool estMort = false;

    float compt_frames = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Point de départ
        if(modeStatic){
            transform.position = new Vector3(positionX,distanceSol,0);
            
        }
        else {
            transform.position = new Vector3(0,distanceSol,0);
        }
             

        // Forcer la position du dragon par rapport au sol
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        if(modeStatic){
            rb.constraints |= RigidbodyConstraints.FreezePositionX;
        
        }

        anim.SetBool("modeStatic",modeStatic);

    }


     void Update()
    {   
        // incrémenter le compteur de frames
        compt_frames += Time.deltaTime;

        if (!estMort && !modeStatic){
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

                //LancerAttaque();

                // Déplacer le dragon pour qu'il soit le plus possible face au joueur
                MoveDragonAttaque();

                LancerAttaque();            
            }
            else
            {
                // Avance automatique selon l'axe x
                transform.position += Vector3.right * Time.deltaTime * moveSpeed * direction;
                Debug.Log("Je vole");
            }}
        else if (modeStatic && compt_frames>= timer_tir && !estMort){

            ModeStatic();
            compt_frames = 0f;
        }
        else if (estMort)
        {
            Debug.Log("Dragon mort");
            
        }


        
    }

    void LateUpdate()
{
    if (modeStatic)
    {
        Vector3 cible = new Vector3(positionX, distanceSol, 0);
        transform.position = Vector3.Lerp(transform.position, cible, 0.2f);
    }
}


    public void ModeStatic(){

        // Lancer l'animation d'attaque
        anim.SetTrigger("isAttacking");
        Debug.Log("Animation d'attaque");
        StartCoroutine(TirerApresXFrame(25));    
    }

    IEnumerator TirerApresXFrame(float nombreFrame)
    {
        for (int i = 0; i < nombreFrame; i++) yield return null; // attendre 1 frame
        float x, z;
        // Créer une boule de feu
        if(!modeStatic){
            x =0;
            z = -0.05f;
        }
        else {
            x = 0.05f;
            z = 0;
        }
        Rigidbody p = Instantiate(fireball, transform.position+ new Vector3(x,0.5f,z), transform.rotation);

        p.linearVelocity = transform.forward * (moveSpeed+2f);
        Debug.Log("Boule de feu tirée");
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
        Debug.Log("Hit sol");

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


        if((collision.contacts[0].normal.y < -0.5) && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Dragon doit mourir car ce prend quelque par le haut");
            Mourir();
        }

        foreach (ContactPoint contact in collision.contacts)
    {
            Debug.Log("Point : " + contact.point);
            Debug.Log("Normal : " + contact.normal);
    }

    }

    IEnumerator SupprimerCollider(){
        // Enlever contrainte de distance par rapport au sol
        rb.constraints = RigidbodyConstraints.None;
        Debug.Log("Commencer la chute");
        while(transform.position.y > 0.5f){
            transform.position += new Vector3(0,-1,0) * Time.deltaTime * fallSpeed;
            yield return null;}
        Debug.Log("Suppression du collider");
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    public void Mourir()
    {   
        estMort = true; 
        
        StartCoroutine(SupprimerCollider());

        anim.SetTrigger("mort");

        // Mettre des bonus
        for (int i = 0; i < 5; i++){
            Rigidbody p = Instantiate(bonus, new Vector3(0,5,10*i),  Quaternion.Euler(-90,0,0));
        }
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
        if (direction == 1)
        {   Debug.Log("Joueur à droite");
            // Si le joueur est à droite sur l'écran
            transform.rotation= Quaternion.Euler(0,90+ Angle(hyp),0);
        }
        else
        {   
            Debug.Log("Joueur à gauche");
            // Si le joueur est à gauche sur l'écran
            transform.rotation= Quaternion.Euler(0,90+ Angle(hyp)+90,0);
        }
        
        Debug.Log("Dragon pivote de "+Angle(hyp));

        // Lancer l'animation d'attaque
        anim.SetTrigger("isAttacking");
        Debug.Log("Animation d'attaque");

        estEnAttaque = true;

        // Attendre 1000 frames pour créer les boules de feu
        StartCoroutine(TirerApresXFrame(25));    

        

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
