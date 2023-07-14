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

    [SerializeField] private float activationDelay = 1.5f;


    public void Initialize()
    {
        gameStateSynchronizer.SubscribeToGameStateChange(DisplayAtGameEnded);
        leaderboardsDisplayer.OnCurrentPlayerEntrySet += TryDisplayHighScoreEffects;
    }

    private void DisplayAtGameEnded(int previousState, int newState)
    {
        if ((GameState)newState == GameState.GameEnded)
        {
            leaderboardsDisplayer.InitializeAndRun();
            Invoke(nameof(ActivateEndGameScreen), activationDelay);
        }
    }

    private void ActivateEndGameScreen()
    {
        scoreText.text = scoreManager.GetDisplayableScore();
        endGameScreenObject.SetActive(true);
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
        PersistentEffectsManager.Instance.SetLoadingScreenSliderOpen(false);

        SceneManager.LoadScene(MainMenuSceneIndex);
    }
}
