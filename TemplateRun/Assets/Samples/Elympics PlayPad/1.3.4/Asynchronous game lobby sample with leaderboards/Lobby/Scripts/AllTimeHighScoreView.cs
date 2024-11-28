using UnityEngine;
using ElympicsPlayPad.Leaderboard;
using ElympicsPlayPad.ExternalCommunicators;
using ElympicsPlayPad.ExternalCommunicators.Leaderboard;
using TMPro;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class AllTimeHighScoreView : MonoBehaviour
    {
        private const string NoBestScoreFallback = "N/A";

        [SerializeField] private string highScoreTextFormat = "Your best score: {0}";

        [SerializeField] private TextMeshProUGUI highScoreValue;

        private IExternalLeaderboardCommunicator LeaderboardCommunicator => PlayPadCommunicator.Instance.LeaderboardCommunicator;

        public void OnStart()
        {
            LeaderboardCommunicator.UserHighScoreUpdated += UpdateHighScore;

            UpdateHighScore(LeaderboardCommunicator.UserHighScore?.Points.ToString() ?? NoBestScoreFallback);
        }

        private void OnDestroy()
        {
            LeaderboardCommunicator.UserHighScoreUpdated -= UpdateHighScore;
        }

        private void UpdateHighScore(UserHighScoreInfo info) => UpdateHighScore(info.Points.ToString());
        private void UpdateHighScore(string points) => highScoreValue.text = string.Format(highScoreTextFormat, points);
    }
}
