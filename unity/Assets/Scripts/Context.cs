using TMPro;
using UnityEngine;

public class Context : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private SliderMusique sliderMusique;

    public AudioSource AudioSource => audioSource;
    public TextMeshProUGUI MessageText => messageText;
    public SliderMusique SliderMusique => sliderMusique;

    public void Initialize(AudioSource source, TextMeshProUGUI text)
    {
        audioSource = source;
        messageText = text;
    }

    public void SetSliderMusique(SliderMusique slider)
    {
        sliderMusique = slider;
    }

    public bool TryGetAudioSource(out AudioSource source)
    {
        source = audioSource;
        return source != null;
    }

    public bool TryGetSliderMusique(out SliderMusique slider)
    {
        slider = sliderMusique;
        return slider != null;
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

    public void SetSliderVisible(bool visible)
    {
        if (sliderMusique != null)
        {
            sliderMusique.gameObject.SetActive(visible);
        }
    }
}
