using UnityEngine;
using UnityEngine.UI; //slider
using System.Collections; // IEnumerator
using TMPro;  // indispensable pour TextMeshProUGUI


public class ActionsBouton : MonoBehaviour
{
     public string pseudo;

    public TMP_InputField inputPseudo;
    public TMP_InputField inputMdp;

    public void Connexion()
    {
        string pseudo = inputPseudo.text;
        string mdp = inputMdp.text;

        Debug.Log(pseudo);
        Debug.Log(mdp);

        if (SessionData.Instance != null)
        {
            SessionData.Instance.playerName = pseudo;
        }

        // Vérifier avec la BDD que le compte existe
        if (true){
            PopupManager.Show("Connexion réussie !");
        }

        SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
        if (sceneloader != null)
        {
            sceneloader.LoadSceneByName("Platform_Streaming");
        }
    }

    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }

    public void Inscription()
    {
        string pseudo = inputPseudo.text;
        string mdp = inputMdp.text;

        Debug.Log(pseudo);
        Debug.Log(mdp);

        // Envoyer à la BDD le nouveau compte utilisateur
        if (true){
            PopupManager.Show("Compte créé ! Connectez-vous !");
            //Vider le champ mot de passe
            inputMdp.text = "";
        }

    }


}