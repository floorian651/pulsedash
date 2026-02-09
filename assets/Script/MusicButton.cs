using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel.Design;

public class MusicButton : MonoBehaviour
{
    //private bool estEnLecture = false;
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

    void Update()
    {   
        if (MenuGenerator.audioSource == null) return;

        if (MenuGenerator.audioSource.isPlaying)
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
        
            // Si la musique n'est pas en train de jouer
            if (!MenuGenerator.audioSource.isPlaying){
                Debug.Log("Jouer le son!");

                // Si la musique avait déjà commencé on reprend où elle avait été arrêtée
                if (MenuGenerator.audioSource.time > 0f){
                    MenuGenerator.audioSource.UnPause();}   

                // Lancer la musique 
                else {
                    MenuGenerator.audioSource.Play();
            }
                
                texteBouton.text = "Pause";}
            else{
                MenuGenerator.audioSource.Pause();
                texteBouton.text = "Jouer";}

    }
    }

    

