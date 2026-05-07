using UnityEngine;

public class Trigger : MonoBehaviour
{
    public AudioSource musicSource; // Source audio de la musique
    private bool musicStarted = false;
    public float delaiAvantMusique = 2.0f;
    public string titre_musique;


    // A MODIFIER POUR LA BDD
    void OnEnable(){
        
         if (!musicStarted){
            titre_musique = SessionData.Instance.titre;
            Debug.Log("Récupérer la musique!");
            musicSource.clip = Resources.Load<AudioClip>("Musique/" + titre_musique);
         }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!musicStarted && other.CompareTag("Player"))
        {
            musicStarted = true;

            // On lit l'heure exacte de la carte son au moment du déclenchement
            double heureActuelle = AudioSettings.dspTime;

            // On calcule l'heure exacte à laquelle la musique DOIT démarrer
            double heureDepartMusique = heureActuelle + delaiAvantMusique;
            // Récupérer l'audio source 
            
            // On donne l'ordre à l'AudioSource de se lancer pile à cette heure-là
            musicSource.PlayScheduled(heureDepartMusique);

        }
    }
}