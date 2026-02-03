using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicButton : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;

    private bool estEnLecture = false;
    private TMP_Text texteBouton;

    void Start()
    {
        // Récupérer le texte du bouton
        texteBouton = GetComponentInChildren<TMP_Text>();

        // Texte initial
        texteBouton.text = "Jouer";

        // Ajouter l'action
        GetComponent<Button>().onClick.AddListener(ToggleMusic);
    }

    void ToggleMusic()
    {
        if (!estEnLecture)
        {
            // Si la musique n'est pas en train de jouer
            if (!source.isPlaying){
                Debug.Log("Jouer le son!");
                source.Play();}
            else
                source.UnPause();

            texteBouton.text = "Pause";
        }
        else
        {
            source.Pause();
            texteBouton.text = "Jouer";
        }

        estEnLecture = !estEnLecture;
    }
}
