using UnityEngine;

public class Trigger : MonoBehaviour
{
    public AudioSource musicSource; // Source audio de la musique
    private bool musicStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!musicStarted && other.CompareTag("Player"))
        {
            musicSource.Play();
            musicStarted = true;
        }
    }
}
