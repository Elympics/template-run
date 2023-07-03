using Elympics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameStateSynchronizer : ElympicsMonoBehaviour, IInitializable
{
    [SerializeField] private GameStartCountdown gameStartCountdown;
    [SerializeField] private EndGameTrigger endGameTrigger;
    [SerializeField] private ScoreManager scoreManager;

    private readonly ElympicsInt gameState = new ElympicsInt((int)GameState.Initialization);

    public GameState GameState => (GameState)gameState.Value;

    public void Initialize()
    {
        Assert.IsNotNull(gameStartCountdown);
        Assert.IsNotNull(endGameTrigger);
        Assert.IsNotNull(scoreManager);

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
            Elympics.EndGame(new ResultMatchPlayerDatas(new List<ResultMatchPlayerData> { new ResultMatchPlayerData { MatchmakerData = new float[1] { scoreManager.Score } } }));
        }
    }

    private void SetGameState(GameState newState)
    {
        gameState.Value = (int)newState;
    }
}
