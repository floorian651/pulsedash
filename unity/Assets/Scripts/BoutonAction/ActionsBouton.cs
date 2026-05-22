using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ActionsBouton : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputMdp;
    public TMP_InputField inputUsername; // inscription seulement (optionnel)

    private UserDAO userDAO;

    void Awake()
    {
        userDAO = GetComponent<UserDAO>();
    }

    void Start()
    {
        if (SessionData.Instance != null && !string.IsNullOrEmpty(SessionData.Instance.pendingMessage))
        {
            PopupManager.Show(SessionData.Instance.pendingMessage);
            SessionData.Instance.pendingMessage = null;
        }
    }

    public void Connexion()
    {
        userDAO.Login(inputEmail.text, inputMdp.text, success =>
        {
            if (success)
            {
                if (SessionData.Instance != null)
                    SessionData.Instance.playerName = inputEmail.text;

                PopupManager.Show("Connexion réussie !");

                SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
                if (sceneloader != null)
                    sceneloader.LoadSceneByName("Platform_Streaming");
            }
            else
            {
                PopupManager.Show("Email ou mot de passe incorrect.");
            }
        });
    }

    public void Inscription()
    {
        string username = (inputUsername != null && !string.IsNullOrEmpty(inputUsername.text))
            ? inputUsername.text
            : inputEmail.text;

        userDAO.Register(inputEmail.text, inputMdp.text, username, success =>
        {
            if (success)
            {
                PopupManager.Show("Compte créé ! Connectez-vous !");

                SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
                if (sceneloader != null)
                    sceneloader.LoadSceneByName("PageConnexion");
            }
            else
            {
                PopupManager.Show("Erreur lors de la création du compte.");
            }
        });
    }

    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }


    public void PageConnexion()
    {
        SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
        if (sceneloader != null)
            sceneloader.LoadSceneByName("PageConnexion");
    }

    public void PageInscription()
    {
        SceneLoader sceneloader = FindObjectOfType<SceneLoader>();
        if (sceneloader != null)
            sceneloader.LoadSceneByName("PageInscription");
    }
}