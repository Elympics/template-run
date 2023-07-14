using System;
using UnityEngine;

public static class AudioProperties
{
    private const string PreferredVolumeKey = "PreferredVolume";
    private const string ShouldBeMutedKey = "ShouldBeMuted";
    private const float DefaultVolume = 1f;

    private static bool _muted = false;
    private static float _realPreferredVolume;
    private static float _effectivePreferredVolume;

    public static event Action OnEffectiveVolumeChanged;


    public static float EffectivePreferredVolume
    {
        get => _effectivePreferredVolume;
        private set { _effectivePreferredVolume = value; OnEffectiveVolumeChanged?.Invoke(); }
    }

    public static float RealPreferredVolume
    {
        get => _realPreferredVolume;
        set { _realPreferredVolume = Mathf.Clamp(value, 0, 2); EffectivePreferredVolume = _realPreferredVolume; }
    }

    public static bool Muted
    {
        get => _muted;
        set { _muted = value; EffectivePreferredVolume = _muted ? 0f : RealPreferredVolume; }
    }


    public static void Init()
    {
        RealPreferredVolume = PlayerPrefs.GetFloat(PreferredVolumeKey, DefaultVolume);
        Muted = PlayerPrefs.GetInt(ShouldBeMutedKey, 0) == 1;    // 0 - not muted, 1 - muted
    }

    public static void Serialize()
    {
        PlayerPrefs.SetFloat(PreferredVolumeKey, RealPreferredVolume);
        PlayerPrefs.SetInt(ShouldBeMutedKey, Muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ToggleMute()
    {
        Muted = !Muted;
    }
}
