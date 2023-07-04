using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsManager : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    public static SoundSettingsManager Instance;

    [Header("Sound settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject soundOffIcon;
    [SerializeField] private GameObject soundOnIcon;
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject openSettingsButton;
    [SerializeField] private GameObject closeSettingsButton;
    [SerializeField] private GameObject soundOptions;
    [SerializeField] [Range(0f, 1f)] private float currentVolume = 0.5f;
    public float CurrentVolume => mute ? 0 : currentVolume;

    private bool mute;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeMute()
    {
        mute = !mute;
        soundOffIcon.SetActive(mute);
        soundOnIcon.SetActive(!mute);
        soundManager.SetMusicVolume(CurrentVolume);
    }

    public void UpdateToSlider()
    {
        if (volumeSlider.value == 0 && !mute) ChangeMute();
        else
        {
            currentVolume = volumeSlider.value;
            if (mute) ChangeMute();

            soundManager.SetMusicVolume(CurrentVolume);
        }
    }

    public void ChangeMenuActive(bool active)
    {
        openSettingsButton.SetActive(!active);
        backGround.SetActive(active);
        soundOptions.SetActive(active);
        closeSettingsButton.SetActive(active);
    }

}
