using UnityEngine;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.UI;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class EndGameView : MonoBehaviour
    {
        [SerializeField] private string lobbySceneName = "AsyncGameLobbyScene";
        [SerializeField] private TextMeshProUGUI currentScoreText, tournamentBestScoreText, allTimeBestScoreText;
        [SerializeField] private Image tournamentBestScoreRibbon, allTimeBestScoreRibbon;

        [Header("Score Formats")]
        [SerializeField] private string currentScoreStringFormat = "Score: {0}";
        [SerializeField] private string tournamentBestScoreStringFormat = "Tournament Best Score: {0}";
        [SerializeField] private string allTimeBestScoreStringFormat = "All Time Best Score: {0}";

        public void Show(int points)
        {
            // Display the current score
            currentScoreText.text = string.Format(currentScoreStringFormat, points);

            // Please note that the "Check" methods are updating Tournament & All Time High Scores. These scores will automatically be updated after server closure, but at this point server is still open.
            UpdateHighScoreRibbonAndText(ElympicsBestScoreManager.CheckIsNewTournamentHighScore(points), ElympicsBestScoreManager.TournamentHighScore, tournamentBestScoreRibbon, tournamentBestScoreText, tournamentBestScoreStringFormat);
            UpdateHighScoreRibbonAndText(ElympicsBestScoreManager.CheckIsNewAllTimeHighScore(points), ElympicsBestScoreManager.AllTimeHighScore, allTimeBestScoreRibbon, allTimeBestScoreText, allTimeBestScoreStringFormat);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Reusable method to update the best scores ribbons 
        /// </summary>
        private void UpdateHighScoreRibbonAndText(bool ribbonActive, int score, Image ribbon, TextMeshProUGUI text, string scoreFormat)
        {
            ribbon.gameObject.SetActive(ribbonActive);
            text.text = string.Format(scoreFormat, score);
        }


        [UsedImplicitly] // by BackToLobbyButton
        public void ReturnToLobby()
        {
            SceneManager.LoadScene(lobbySceneName);
        }
    }
}
