using UnityEngine;
using System.Collections; // Pour IEnumerator et WaitForSeconds (coroutines)

public class Trigger : MonoBehaviour
{
    public AudioSource musicSource; // Source audio de la musique
    private bool musicStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!musicStarted && other.CompareTag("Player"))
        {
            musicStarted = true;
            StartCoroutine(jouerMusiqueAvecDelai());
        }
    }

    private IEnumerator jouerMusiqueAvecDelai()
    {
        // Pause de 2 secondes avant de lancer la musique
        yield return new WaitForSeconds(2f);
        
        // Lancer la musique
        musicSource.Play();
    }
}
