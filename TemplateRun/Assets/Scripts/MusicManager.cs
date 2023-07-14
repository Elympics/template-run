using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private enum MusicGameState { Menu, Gameplay, Results }

    [Header("Audio Sources")]
    [SerializeField] private ManagedAudioSource[] menuMusic;
    [SerializeField] private ManagedAudioSource[] gameplayMusic;
    [SerializeField] private ManagedAudioSource[] resultsMusic;
    [SerializeField] private ManagedAudioSource gameOverSound;

    [Header("LoopingMusic")]
    [SerializeField] private float menuLoopDuration;
    [SerializeField] private float gameplayLoopDuration;
    [SerializeField] private float resultsLoopDuration;

    private float loopedMusicTimer = 0;
    private bool playingALoop = true;

    private MusicGameState gameState = MusicGameState.Menu;

    public void SetUpOnAwake()
    {
        menuMusic[0].AudioSource.Play();
    }

    public void AdjustStateToScene(Scene newScene)
    {
        gameState = newScene.buildIndex == 0 ? MusicGameState.Menu : MusicGameState.Gameplay;

        if (gameState == MusicGameState.Menu)
            PlayMenuMusic();
        // In gameplay scene we play music manually to match its start with the beginning of countdown
    }

    private void Update()
    {
        ManageLoopedMusic();
    }

    private void ManageLoopedMusic()
    {
        float durationToCheck = 0;
        switch (gameState)
        {
            case MusicGameState.Menu:
                durationToCheck = menuLoopDuration;
                break;
            case MusicGameState.Gameplay:
                durationToCheck = gameplayLoopDuration;
                break;
            case MusicGameState.Results:
                durationToCheck = resultsLoopDuration;
                break;
        }

        loopedMusicTimer += Time.deltaTime;
        if (loopedMusicTimer >= durationToCheck)
        {
            loopedMusicTimer = 0;
            int indexToPlay = playingALoop ? 1 : 0;

            switch (gameState)
            {
                case MusicGameState.Menu:
                    menuMusic[indexToPlay].AudioSource.Play();
                    break;
                case MusicGameState.Gameplay:
                    gameplayMusic[indexToPlay].AudioSource.Play();
                    break;
                case MusicGameState.Results:
                    resultsMusic[indexToPlay].AudioSource.Play();
                    break;
            }
            playingALoop = !playingALoop;
        }
    }

    private void PlayMusicLoop(AudioSource sourceToPlay)
    {
        sourceToPlay.Play();
        playingALoop = true;
        loopedMusicTimer = 0;
    }

    private void PlayMenuMusic()
    {
        resultsMusic[0].AudioSource.Stop();
        resultsMusic[1].AudioSource.Stop();

        PlayMusicLoop(menuMusic[0].AudioSource);
    }

    public void PlayGameplayMusic()
    {
        menuMusic[0].AudioSource.Stop();
        menuMusic[1].AudioSource.Stop();

        PlayMusicLoop(gameplayMusic[0].AudioSource);
    }

    public void PlayGameOverSoundEffects()
    {
        gameState = MusicGameState.Results;

        gameplayMusic[0].AudioSource.Stop();
        gameplayMusic[1].AudioSource.Stop();

        gameOverSound.AudioSource.Play();
        var resultsMusicDelay = gameOverSound.AudioSource.clip.length;
        resultsMusic[0].AudioSource.PlayDelayed(resultsMusicDelay);

        loopedMusicTimer = -resultsMusicDelay;
        playingALoop = true;
    }
}
