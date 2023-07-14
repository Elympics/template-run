using UnityEngine;

public class GameplaySoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private ManagedAudioSource jumpSound;
    [SerializeField] private ManagedAudioSource landSound;
    [SerializeField] private ManagedAudioSource coinSound;

    private void OnEnable()
    {
        var jumpManager = FindObjectOfType<JumpManager>();
        jumpManager.OnJumped += () => jumpSound.AudioSource.Play();
        jumpManager.OnLanded += () => landSound.AudioSource.Play();

        var coinCollector = FindObjectOfType<CoinCollector>();
        coinCollector.OnCoinPickedUp += () => coinSound.AudioSource.Play();

        var gameStateSynchronizer = FindObjectOfType<GameStateSynchronizer>();
        gameStateSynchronizer.SubscribeToGameStateChange(AdjustToGameState);
    }

    private void AdjustToGameState(int oldState, int newState)
    {
        if ((GameState)newState == GameState.GameEnded)
            PersistentEffectsManager.Instance.PlayGameOverSoundEffects();
    }
}
