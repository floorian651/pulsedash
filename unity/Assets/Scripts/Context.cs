using TMPro;
using UnityEngine;

public class Context : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private SliderMusique sliderMusique;
    [SerializeField] private GameObject playPauseButton;

    public AudioSource AudioSource => audioSource;
    public TextMeshProUGUI MessageText => messageText;
    public SliderMusique SliderMusique => sliderMusique;
    public GameObject PlayPauseButton => playPauseButton;

    public static Context Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }


    public void Initialize(AudioSource source, TextMeshProUGUI text)
    {
        audioSource = source;
        messageText = text;
    }

    public void SetSliderMusique(SliderMusique slider)
    {
        sliderMusique = slider;
    }

    public void SetPlayPauseButton(GameObject button)
    {
        playPauseButton = button;
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

    public bool TryGetPlayPauseButton(out GameObject button)
    {
        button = playPauseButton;
        return button != null;
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

    public void SetPlayPauseVisible(bool visible)
    {
        if (playPauseButton != null)
        {
            playPauseButton.SetActive(visible);
        }
    }
}
