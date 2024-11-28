using UnityEngine;
using TMPro;
using ElympicsPlayPad.Leaderboard;
using System;
using Elympics;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class LeaderboardEntryUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI position;
        [SerializeField] private TextMeshProUGUI nickname;
        [SerializeField] private TextMeshProUGUI score;

        [SerializeField] private GameObject entrySeparator;

        private void Awake()
        {
            Clear();
        }

        public void Clear()
        {
            gameObject.SetActive(false);
        }

        public void SetValues(Placement leaderboardPlacement)
        {
            gameObject.SetActive(true);

            position.text = $"{leaderboardPlacement.Position}.";
            nickname.text = leaderboardPlacement.Nickname;
            score.text = leaderboardPlacement.Score.ToString();

            HighlightCurrentPlayer(leaderboardPlacement);
        }

        public void SetEntrySeparator(bool on) => entrySeparator.SetActive(on);

        private void HighlightCurrentPlayer(Placement leaderboardPlacement)
        {
            bool isCurrentPlayer = leaderboardPlacement.UserId.Equals(ElympicsLobbyClient.Instance.UserGuid.ToString());

            var style = isCurrentPlayer ? FontStyles.Bold : FontStyles.Normal;

            position.fontStyle = style;
            nickname.fontStyle = style;
            score.fontStyle = style;
        }
    }
}
