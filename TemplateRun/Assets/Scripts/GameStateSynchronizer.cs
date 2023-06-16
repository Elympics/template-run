using Elympics;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSynchronizer : ElympicsMonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    private readonly ElympicsInt gameState = new ElympicsInt((int)GameState.Initialization);
    public GameState GameState => (GameState)gameState.Value;

    public void SubscribeToGameStateChange(ElympicsVar<int>.ValueChangedCallback action)
    {
        gameState.ValueChanged += action;
    }

    public void StartGame() => SetGameState(GameState.Gameplay);

    public void FinishGame()
    {
        SetGameState(GameState.GameEnded);

        if (Elympics.IsServer)
        {
            Elympics.EndGame(new ResultMatchPlayerDatas(new List<ResultMatchPlayerData> { new ResultMatchPlayerData { MatchmakerData = new float[1] { scoreManager.Score } } }));
        }
    }

    public void SetGameState(GameState newState)
    {
        gameState.Value = (int)newState;
    }
}
