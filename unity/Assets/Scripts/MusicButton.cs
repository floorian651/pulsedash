using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel.Design;

public class MusicButton : MonoBehaviour
{
    //private bool estEnLecture = false;
    private TMP_Text texteBouton;
    private Context Context;

    void Start()
    {
        Context = Object.FindObjectOfType<Context>();

        // Récupérer le texte du bouton
        texteBouton = GetComponentInChildren<TMP_Text>();

        // Texte initial
        texteBouton.text = "Jouer";

        // Ajouter l'action
        GetComponent<Button>().onClick.AddListener(ToggleMusic);
    }

    void Update()
    {   
        if (Context == null || !Context.TryGetAudioSource(out AudioSource source)) return;

        if (source.isPlaying)
        {
            texteBouton.text = "Pause";
        }
        else
        {
            texteBouton.text = "Jouer";
        }
    }

    void ToggleMusic()
    {   
            if (Context == null || !Context.TryGetAudioSource(out AudioSource source)) return;
        
            // Si la musique n'est pas en train de jouer
            if (!source.isPlaying){
                Debug.Log("Jouer le son!");

                // Si la musique avait déjà commencé on reprend où elle avait été arrêtée
                if (source.time > 0f){
                    source.UnPause();}   

                // Lancer la musique 
                else {
                    source.Play();
            }
                
                texteBouton.text = "Pause";}
            else{
                source.Pause();
                texteBouton.text = "Jouer";}

    }
    }

    

