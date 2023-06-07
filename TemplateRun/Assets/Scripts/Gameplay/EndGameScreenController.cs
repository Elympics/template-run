using Elympics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class EndGameScreenController : ElympicsMonoBehaviour, IInitializable
{
    private const int MainMenuSceneIndex = 0;

    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject endGameScreenObject;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private string matchmakingQueueSolo = "Solo";      // name defined in the panel https://www.elympics.cc/
    [SerializeField] private LeaderboardsDisplayer leaderboardsDisplayer;
    [SerializeField] private GameObject[] highscoreIndicators;

    public void Initialize()
    {
        gameStateSynchronizer.SubscribeToGameStateChange(AdjustToGameState);
        leaderboardsDisplayer.OnCurrentPlayerEntrySet += TryDisplayHighScoreEffects;
    }

    private void AdjustToGameState(int previousState, int newState)
    {
        if ((GameState)newState == GameState.GameEnded)
        {
            scoreText.text = scoreManager.GetDisplayableScore();
            endGameScreenObject.SetActive(true);
            leaderboardsDisplayer.InitializeAndRun();
        }
    }

    public void OnBackToMenuClicked()
    {
        SceneManager.LoadScene(MainMenuSceneIndex);
    }

    public void OnPlayAgainClicked()
    {
        int randomSeed = UnityEngine.Random.Range(0, 1000000);

        var closestRegion = FindObjectOfType<RegionManager>().closestRegion;
        ElympicsLobbyClient.Instance.PlayOnlineInRegion(closestRegion.Region, null, BitConverter.GetBytes(randomSeed), $"{matchmakingQueueSolo}");
        Debug.Log($"Connected to region {closestRegion.Region} with ping {closestRegion.LatencyMs}");
        playAgainButton.enabled = false;
    }

    private void TryDisplayHighScoreEffects(LeaderboardEntry leaderboardEntry)
    {
        if (scoreManager.Score >= leaderboardEntry.Score)
        {
            foreach (var indicator in highscoreIndicators)
            {
                indicator.SetActive(true);
            }
        }
    }    
}
