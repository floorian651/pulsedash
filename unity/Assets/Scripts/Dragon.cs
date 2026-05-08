using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class Dragon : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float distanceSol = 3f;
    public float distJoueur = 10f;
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
    float direction = 1;

    bool estEnAttaque = false;
    float timerAttaque = 0f;
    float cooldownAttaque = 3f;
    bool estMort = false;

    float compt_frames = 0f;
    float positionDragon;

    float timerReverse = 0f;
    public float cooldownReverse = 0.25f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        GameObject p = GameObject.Find("Player");

        if(p != null)
            player = p.transform;
        else 
            Debug.Log("Player null");

<<<<<<< HEAD
        anim.SetBool("modeStatic", modeStatic);
        positionDragon = transform.position.z;
=======
        positionDragon = transform.position.z;

>>>>>>> main
    }

    void Update()
    {
        compt_frames += Time.deltaTime;
        positionDragon = transform.position.z;

        if (!estMort && !modeStatic)
        {
            RecupererCoordJoueur();

            timerReverse -= Time.deltaTime;

            if (!DetecterSol() && timerReverse <= 0f)
            {
                Pivoter();
                timerReverse = cooldownReverse;
            }

            //anim.SetTrigger("isFlying");
            anim.SetBool("isFlying", true);

            timerAttaque -= Time.deltaTime;

            if (JoueurDetecte() && !estEnAttaque && timerAttaque <= 0f)
            {
                Debug.Log("Joueur détecté");

                RegarderJoueur();
                LancerAttaque();
            }
            else if(!estEnAttaque && !JoueurDetecte())
            {
                transform.position += Vector3.right * Time.deltaTime * moveSpeed * direction;
            }
                        
        }
        else if (modeStatic && compt_frames >= timer_tir && !estMort)
        {
            ModeStatic();
            compt_frames = 0f;
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

    void OnCollisionEnter(Collision collision)
    {   
        positionDragon = transform.position.z;

        // Mort si le joueur tombe dessus (normal.y < -0.5)
        if (!estMort &&
            collision.gameObject.CompareTag("Player") &&
            collision.contacts[0].normal.y < -0.5f)
        {
            Mourir();
        }
    }

    bool DetecterSol()
{
    LayerMask mask = LayerMask.GetMask("sol");

    //ol sous le dragon
    bool solSousPieds = Physics.Raycast(transform.position, Vector3.down, distanceSol, mask);

    // sol devant le dragon (détection de bord)
    Vector3 front = transform.position + Vector3.right * direction * 1.5f;
    //bool solDevant = Physics.Raycast(front, Vector3.down, distanceSol, mask);

    Debug.DrawRay(transform.position, Vector3.down * distanceSol, Color.green);
    //Debug.DrawRay(front, Vector3.down * distanceSol, Color.red);

    // logique : on continue seulement si il y a du sol devant
    return  solSousPieds; //solDevant &&
}

    void Pivoter()
    {
        rb.linearVelocity = Vector3.zero;

        direction *= -1;

        if (direction == 1)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else
            transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    public void RegarderJoueur()
{
    transform.rotation = Quaternion.Euler(0, 180, 0);
    // Stopper le mouvement immédiatement
    rb.linearVelocity = Vector3.zero;
    
}

    public void LancerAttaque()
{
    Debug.Log(gameObject.name + " attaque appelée");

    anim.SetTrigger("isAttacking");

    estEnAttaque = true;
    timerAttaque = cooldownAttaque;

    StartCoroutine(TirerApresXFrame(25));
}

    IEnumerator TirerApresXFrame(float frames)
    {
        for (int i = 0; i < frames; i++)
            yield return null;

        Rigidbody p = Instantiate(
            fireball,
            transform.position + new Vector3(0, 0.5f, 0),
            transform.rotation
        );

        p.linearVelocity = transform.forward * (moveSpeed + 2f);

        estEnAttaque = false;
    }

    
    void RecupererCoordJoueur()
    {
        Vector3 pos = player.position;
        xJ = pos.x;
        yJ = pos.y;
        zJ = pos.z;
    }

    public bool JoueurDetecte()
    {
        float dz = transform.position.z - zJ;
        Debug.Log("Joueur est à "+ dz);
        return dz > 0 && dz < distJoueur;
    }

    public void FinAttaque()
    {
        estEnAttaque = false;
        transform.rotation = Quaternion.Euler(0, 90, 0);
        direction = 1;
    }

    void ModeStatic()
    {
        anim.SetTrigger("isAttacking");
        StartCoroutine(TirerApresXFrame(25));
    }

    void Mourir()
    {   
    
        estMort = true;

        // Jouer l’animation de mort
        anim.SetTrigger("mort");

        // Désactiver le collider après la mort
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        // Mettre des bonus
        for (int i = ((int) positionDragon)+5; i <= ((int) positionDragon)+ 55; i += 10){
            Debug.Log(positionDragon);
            Rigidbody p = Instantiate(bonus, new Vector3(0,0.7f,1*i),  Quaternion.Euler(-90,0,0));
        }
    }
}