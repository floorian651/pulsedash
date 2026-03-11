using UnityEngine;
public class TestMusique : MonoBehaviour
{
    void Start()
    {
        if(SessionData.Instance.audioSource !=null){
            Debug.Log("AudioSource chargé");
            SessionData.Instance.audioSource.Play();
        } 
    }
}