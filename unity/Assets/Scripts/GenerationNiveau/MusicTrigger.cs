using UnityEngine;

public class Trigger : MonoBehaviour
{
    public AudioSource musicSource; // Source audio de la musique
    private bool musicStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!musicStarted && other.CompareTag("Player"))
        {
            musicStarted = true;
            sleep(2000); // Pause de 2sec avant de lancer la musique
            musicSource.Play();
        }
    }
}
