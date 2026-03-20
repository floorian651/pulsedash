using TMPro;
using UnityEngine;

public class Context : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI messageText;

    public AudioSource AudioSource => audioSource;
    public TextMeshProUGUI MessageText => messageText;

    public void Initialize(AudioSource source, TextMeshProUGUI text)
    {
        audioSource = source;
        messageText = text;
    }

    public bool TryGetAudioSource(out AudioSource source)
    {
        source = audioSource;
        return source != null;
    }

    public void SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    public string GetMessage()
    {
        if (messageText != null)
        {
            return messageText.text;
        }
        else return null;
    }
}
