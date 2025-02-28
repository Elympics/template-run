using Elympics;
using ElympicsPlayPad.Samples.AsyncGame;
using UnityEngine;
using UnityEngine.Assertions;

public class GameStateSynchronizer : ElympicsMonoBehaviour, IInitializable
{
    [SerializeField] private GameStartCountdown gameStartCountdown;
    [SerializeField] private EndGameTrigger endGameTrigger;

    private readonly ElympicsInt gameState = new ElympicsInt((int)GameState.Initialization);

    public GameState GameState => (GameState)gameState.Value;

    public void Initialize()
    {
        Assert.IsNotNull(gameStartCountdown);
        Assert.IsNotNull(endGameTrigger);

        gameStartCountdown.OnCountdownEnded += StartGame;
        endGameTrigger.OnEndGameTriggered += FinishGame;
    }

    public void SubscribeToGameStateChange(ElympicsVar<int>.ValueChangedCallback action)
    {
        gameState.ValueChanged += action;
    }

    private void StartGame() => SetGameState(GameState.Gameplay);

    private void FinishGame()
    {
        SetGameState(GameState.GameEnded);

        if (Elympics.IsServer)
        {
            var gameEnder = FindObjectOfType<MatchEnder>();
            Assert.IsNotNull(gameEnder);
            gameEnder.EndMatch();
        }
    }

    private void SetGameState(GameState newState)
    {
        gameState.Value = (int)newState;
    }
}
