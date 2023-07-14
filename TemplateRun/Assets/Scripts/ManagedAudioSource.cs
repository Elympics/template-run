using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ManagedAudioSource : MonoBehaviour
{
    private AudioSource audioSource;
    private float volumeBase;

    public AudioSource AudioSource => audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        volumeBase = audioSource.volume;

        RecalculateVolume();
    }

    private void OnEnable()
    {
        AudioProperties.OnEffectiveVolumeChanged += RecalculateVolume;
    }

    private void OnDisable()
    {
        AudioProperties.OnEffectiveVolumeChanged -= RecalculateVolume;
    }

    private void RecalculateVolume()
    {
        audioSource.volume = AudioProperties.EffectivePreferredVolume * volumeBase;
    }
}
