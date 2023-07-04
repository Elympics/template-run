using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sound settings")]
    [SerializeField] private SoundSettingsManager soundSettings;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] menuMusic;
    [SerializeField] private AudioSource[] gameplayMusic;
    [SerializeField] private AudioSource[] resultsMusic;
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource landSound;
    [SerializeField] private AudioSource coinSound;

    [Header("LoopingMusic")]
    [SerializeField] private float menuLoopDuration;
    [SerializeField] private float gameplayLoopDuration;
    [SerializeField] private float resultsLoopDuration;

    private float loopedMusicTimer = 0;
    private bool playingALoop = true;

    private enum MusicGameState { menu, gameplay, results }
    private MusicGameState gameState = MusicGameState.menu;

    private JumpManager jumpManager = null;
    private bool wasGrounded;

    #region defaultSoundValues
    private static readonly float DefaultVolume = 0.5f;
    private float menuDefaultVolume;
    private float gameplayDefaultVolume;
    private float resultsDefaultVolume;
    private float jumpDefaultVolume;
    private float landDefaultVolume;
    private float coinDefaultVolume;
    private float gameoverDefaultVolume;
    #endregion

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

    public void SetUpOnAwake()
    {
        menuMusic[0].Play();
        SaveDefaultVolumes();
    }
    private void SaveDefaultVolumes()
    {
        menuDefaultVolume = menuMusic[0].volume / DefaultVolume;
        gameplayDefaultVolume = gameplayMusic[0].volume / DefaultVolume;
        resultsDefaultVolume = resultsMusic[0].volume / DefaultVolume;
        jumpDefaultVolume = jumpSound.volume / DefaultVolume;
        landDefaultVolume = landSound.volume / DefaultVolume;
        coinDefaultVolume = coinSound.volume / DefaultVolume;
        landDefaultVolume = landSound.volume / DefaultVolume;
        gameoverDefaultVolume = gameOverSound.volume / DefaultVolume;
    }

    private void Update()
    {
        ManagedLoopedMusic();

        if (gameState == MusicGameState.menu) return;
        ManageGameplaySounds();
    }

    private void ManagedLoopedMusic()
    {
        loopedMusicTimer += Time.deltaTime;
        float durationToCheck = 0;
        switch (gameState)
        {
            case MusicGameState.menu:
                durationToCheck = menuLoopDuration;
                break;
            case MusicGameState.gameplay:
                durationToCheck = gameplayLoopDuration;
                break;
            case MusicGameState.results:
                durationToCheck = resultsLoopDuration;
                break;
        }

        if (loopedMusicTimer >= durationToCheck)
        {
            loopedMusicTimer = 0;
            int indexToPlay = playingALoop ? 1 : 0;

            switch (gameState)
            {
                case MusicGameState.menu:
                    menuMusic[indexToPlay].Play();
                    break;
                case MusicGameState.gameplay:
                    gameplayMusic[indexToPlay].Play();
                    break;
                case MusicGameState.results:
                    resultsMusic[indexToPlay].Play();
                    break;
            }
            playingALoop = !playingALoop;
        }
    }

    private void ManageGameplaySounds()
    {
        if (gameState == MusicGameState.results) return;
        if (jumpManager.IsGrounded())
        {
            if (!wasGrounded)
            {
                landSound.volume = soundSettings.CurrentVolume * landDefaultVolume;
                landSound.Play();
            }
            wasGrounded = true;
        }
        else
        {
            if (wasGrounded && jumpManager.PlayerRigidbody.velocity.y > 0)
            {
                jumpSound.volume = soundSettings.CurrentVolume * jumpDefaultVolume;
                jumpSound.Play();
            }
            wasGrounded = false;
        }
    }

    public void SetMusicVolume(float newVolume)
    {
        menuMusic[0].volume = menuDefaultVolume * newVolume;
        menuMusic[1].volume = menuDefaultVolume * newVolume;
        gameplayMusic[0].volume = gameplayDefaultVolume * newVolume;
        gameplayMusic[1].volume = gameplayDefaultVolume * newVolume;
        resultsMusic[0].volume = resultsDefaultVolume * newVolume;
        resultsMusic[1].volume = resultsDefaultVolume * newVolume;
    }

    public void AdjustMusicToScene(Scene newScene)
    {
        bool isOnMenuScene = newScene.buildIndex == 0;

        if (isOnMenuScene)
        {
            gameState = MusicGameState.menu;
            PlayAndStopOthers(menuMusic[0]);
        }
        else
        {
            gameState = MusicGameState.gameplay;
            jumpManager = FindObjectOfType<JumpManager>();
            FindObjectOfType<GameStateSynchronizer>().SubscribeToGameStateChange(AdjustToGameState);
            FindObjectOfType<CoinCollector>().SubscribeToCoinPickedUp(PlayCoinSound);
        }
    }

    public void SetUpMusic()
    {
        if (gameState == MusicGameState.menu) PlayAndStopOthers(menuMusic[0]);
        else PlayAndStopOthers(gameplayMusic[0]);
    }

    private void PlayAndStopOthers(AudioSource sourceToPlay)
    {
        menuMusic[0].Stop();
        menuMusic[1].Stop();
        resultsMusic[0].Stop();
        resultsMusic[1].Stop();
        gameplayMusic[0].Stop();
        gameplayMusic[1].Stop();
        gameOverSound.Stop();
        sourceToPlay.Play();
        playingALoop = true;
        loopedMusicTimer = 0;
    }

    private void PlayGameOverSound()
    {
        gameState = MusicGameState.results;
        gameplayMusic[0].Stop();
        gameplayMusic[1].Stop();
        gameOverSound.volume = soundSettings.CurrentVolume * gameoverDefaultVolume;
        gameOverSound.Play();
        resultsMusic[0].PlayDelayed(gameOverSound.clip.length);
        loopedMusicTimer = -gameOverSound.clip.length;
        playingALoop = true;
    }

    private void AdjustToGameState(int oldState, int newState)
    {
        if ((GameState)newState == GameState.GameEnded) PlayGameOverSound();
        else gameState = MusicGameState.gameplay;
    }

    private void PlayCoinSound()
    {
        coinSound.volume = soundSettings.CurrentVolume * coinDefaultVolume;
        coinSound.Play();
    }
}
