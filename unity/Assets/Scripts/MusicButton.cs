using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel.Design;

public class MusicButton : MonoBehaviour
{
    //private bool estEnLecture = false;
    //private TMP_Text texteBouton;
    private Context Context;
    public Image icon;            // L'image enfant
    public Sprite playSprite;     // PNG Play
    public Sprite pauseSprite;    // PNG Pause

    void Start()
    {
        Context = Object.FindObjectOfType<Context>();

        if (icon == null)
        {
            icon = GetComponentInChildren<Image>();
        }
        if (icon == null)
        {
            Debug.LogError("MusicButton: Image 'icon' manquante.");
            return;
        }

        Button btn = GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("MusicButton: composant Button manquant sur le prefab.");
            return;
        }
        // Sprite initial
        icon.sprite = playSprite;
        icon.color = Color.white;

        // Ajouter l'action
        btn.onClick.AddListener(ToggleMusic);
    }

    void Update()
    {   
        if (Context == null || !Context.TryGetAudioSource(out AudioSource source)) return;

        // Mettre à jour l'icône selon l'état
        icon.sprite = source.isPlaying ? pauseSprite : playSprite;
        icon.color = Color.white;

        // Cacher le bouton à la fin de la musique (pas en pause)
        if (source.clip != null && !source.isPlaying && source.time >= source.clip.length - 0.01f)
        {
            Context.SetPlayPauseVisible(false);
            Context.SetMessage("");
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
                    icon.sprite = pauseSprite;
            }
                
                //texteBouton.text = "||";
                }
            else{
                source.Pause();
                //texteBouton.text = ">";
                icon.sprite = playSprite;}

    }
    }

    

