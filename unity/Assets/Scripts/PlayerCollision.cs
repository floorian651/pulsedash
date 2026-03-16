using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public EnergyBar energyBar;

    void Start()
    {
        energyBar = FindAnyObjectByType<EnergyBar>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detectee avec " + collision.gameObject.name + ", de tag " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("obstacle"))
        {
            energyBar.Damage(10f);
            Debug.Log("10 de degats");
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "obstacle") { 
    //         // If colliding with obstacle, execute this block
    //         energyBar.Damage (10);
    //         Debug.Log("10 de degats");
    //     }
    //     // if (!invul && collision.gameObject.tag == "obstacle") { 
    //     //     // If colliding with obstacle, execute this block
    //     //     energyBar.Damage (10);
    //     //     invul = true; //player is invulnerable
    //     //     StartCoroutine (InvulWait ());
    //     // }
    // }
    // // private IEnumerator InvulWait() {
    // // 	animator.SetTrigger ("Hit"); //play hit animation
    // // 	yield return new WaitForSeconds (1); //invul time
    // // 	invul = false;
    // // }
}
