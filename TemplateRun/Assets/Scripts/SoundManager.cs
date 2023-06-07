using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject soundOffIcon;
    [SerializeField] private GameObject soundOnIcon;
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject openSettingsButton;
    [SerializeField] private GameObject closeSettingsButton;
    [SerializeField] private GameObject soundOptions;
    [SerializeField][Range(0f, 1f)] private float currentVolume = 0.5f;
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
    private float defaultVolume = 0.5f;
    private float loopedMusicTimer;
    bool playingALoop;
    bool mute;
    private bool isOnMenuScene = true;
    private bool gameOver;
    private bool wasGrounded;

    private float menuDefaultVolume;
    private float gameplayDefaultVolume;
    private float resultsDefaultVolume;
    private float jumpDefaultVolume;
    private float landDefaultVolume;
    private float coinDefaultVolume;
    private float gameoverDefaultVolume;



    private JumpManager jumpManager = null;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += AdjustMusicToScene;
        menuMusic[0].Play();
        loopedMusicTimer = 0;
        playingALoop = true;
        SaveDefaultVolumes();
    }

    private void SaveDefaultVolumes()
    {
        menuDefaultVolume = menuMusic[0].volume / defaultVolume;
        gameplayDefaultVolume = gameplayMusic[0].volume / defaultVolume;
        resultsDefaultVolume = resultsMusic[0].volume / defaultVolume;
        jumpDefaultVolume = jumpSound.volume / defaultVolume;
        landDefaultVolume = landSound.volume / defaultVolume;
        coinDefaultVolume = coinSound.volume / defaultVolume;
        landDefaultVolume = landSound.volume / defaultVolume;
        gameoverDefaultVolume = gameOverSound.volume / defaultVolume;
    }

    private void SetVolume(float newVolume)
    {
        menuMusic[0].volume = menuDefaultVolume * newVolume;
        menuMusic[1].volume = menuDefaultVolume * newVolume;
        gameplayMusic[0].volume = gameplayDefaultVolume * newVolume;
        gameplayMusic[1].volume = gameplayDefaultVolume * newVolume;
        resultsMusic[0].volume = resultsDefaultVolume * newVolume;
        resultsMusic[1].volume = resultsDefaultVolume * newVolume;
        jumpSound.volume = jumpDefaultVolume * newVolume;
        landSound.volume = landDefaultVolume * newVolume;
        coinSound.volume = coinDefaultVolume * newVolume;
        gameOverSound.volume = gameoverDefaultVolume * newVolume;
    }

    private void Update()
    {
        ManagedLoopedMusic();

        if (isOnMenuScene) return;
        ManageGameplaySounds();
    }

    private void ManagedLoopedMusic()
    {
        loopedMusicTimer += Time.deltaTime;
        int state = isOnMenuScene ? 0 : (gameOver ? 2 : 1);
        float durationToCheck = 0;
        switch (state){
            case 0:
                durationToCheck = menuLoopDuration;              
                break;
            case 1:
                durationToCheck = gameplayLoopDuration;
                break;
            case 2:
                durationToCheck = resultsLoopDuration;
                break;
        }

        if(loopedMusicTimer >= durationToCheck)
        {
            loopedMusicTimer = 0;
            int indexToPlay = playingALoop ? 1 : 0;
            
            switch (state)
            {
                case 0:
                    menuMusic[indexToPlay].Play();
                    break;
                case 1:
                    gameplayMusic[indexToPlay].Play();
                    break;
                case 2:
                    resultsMusic[indexToPlay].Play();
                    break;
            }
            playingALoop = !playingALoop;
        }
    }


    private void AdjustMusicToScene(Scene newScene, LoadSceneMode mode)
    {
        isOnMenuScene = newScene.buildIndex == 0;
        if (isOnMenuScene) SetupMenuMusic();
        else SetupGameplayMusic();
    }

    private void SetupMenuMusic()
    {
        menuMusic[0].Play();
        gameplayMusic[0].Stop();
        gameplayMusic[1].Stop();
        resultsMusic[0].Stop();
        resultsMusic[1].Stop();
        gameOverSound.Stop();
        coinSound.Stop();
        jumpSound.Stop();
        landSound.Stop();
        playingALoop = true;
        loopedMusicTimer = 0;
    }

    private void SetupGameplayMusic()
    {
        menuMusic[0].Stop();
        menuMusic[1].Stop();
        resultsMusic[0].Stop();
        resultsMusic[1].Stop();
        gameplayMusic[0].Play();
        playingALoop = true;
        loopedMusicTimer = 0;
        jumpManager = FindObjectOfType<JumpManager>();
        FindObjectOfType<GameStateSynchronizer>().SubscribeToGameStateChange(AdjustToGameState);
        FindObjectOfType<CoinCollector>().SubscribeToCoinPickedUp(PlayCoinSound);
    }


    private void ManageGameplaySounds()
    {
        if (gameOver) return;
        if (jumpManager.IsGrounded())
        {
            if (!wasGrounded) landSound.Play();
            wasGrounded = true;
        }
        else
        {
            if (wasGrounded && jumpManager.PlayerRigidbody.velocity.y > 0) jumpSound.Play();
            wasGrounded = false;
        }
    }

    private void PlayGameOverSound()
    {
        gameOver = true;
        gameplayMusic[0].Stop();
        gameplayMusic[1].Stop();
        gameOverSound.Play();
        resultsMusic[0].PlayDelayed(gameOverSound.clip.length);
        loopedMusicTimer = -gameOverSound.clip.length;
        playingALoop = true;
    }

    private void AdjustToGameState(int oldState, int newState)
    {
        if ((GameState)newState == GameState.GameEnded) PlayGameOverSound();
        else gameOver = false;
    }

    private void PlayCoinSound()
    {
        coinSound.Play();
    }

    public void ChangeMute()
    {
        mute = !mute;
        soundOffIcon.SetActive(mute);
        soundOnIcon.SetActive(!mute);
        if (mute) SetVolume(0);
        else SetVolume(currentVolume);
    }

    public void UpdateToSlider()
    {
        if(volumeSlider.value == 0 && !mute) ChangeMute();
        else
        {
            currentVolume = volumeSlider.value;
            if (mute) ChangeMute();
            else SetVolume(currentVolume);
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
