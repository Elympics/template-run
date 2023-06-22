using Elympics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using JetBrains.Annotations;

public class EndGameScreenController : ElympicsMonoBehaviour, IInitializable
{
    private const int MainMenuSceneIndex = 0;

    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LeaderboardsDisplayer leaderboardsDisplayer;

    [SerializeField] private GameObject endGameScreenObject;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject[] highscoreIndicators;

    public void Initialize()
    {
        gameStateSynchronizer.SubscribeToGameStateChange(DisplayAtGameEnded);
        leaderboardsDisplayer.OnCurrentPlayerEntrySet += TryDisplayHighScoreEffects;
    }

    private void DisplayAtGameEnded(int previousState, int newState)
    {
        if ((GameState)newState == GameState.GameEnded)
        {
            scoreText.text = scoreManager.GetDisplayableScore();
            endGameScreenObject.SetActive(true);
            leaderboardsDisplayer.InitializeAndRun();
        }
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

    [UsedImplicitly]
    public void GoBackToMenu()
    {
        LoadingScreenManager.Instance.SetSliderOpen(false);

        SceneManager.LoadScene(MainMenuSceneIndex);
    }
}
