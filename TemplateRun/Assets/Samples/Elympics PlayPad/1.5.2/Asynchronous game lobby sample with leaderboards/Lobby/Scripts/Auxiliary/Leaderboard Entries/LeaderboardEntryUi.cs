using UnityEngine;
using TMPro;
using ElympicsPlayPad.Leaderboard;
using Elympics;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class LeaderboardEntryUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI position;
        [SerializeField] private TextMeshProUGUI nickname;
        [SerializeField] private TextMeshProUGUI score;

        [Header("Top 3 badges")]
        [SerializeField] private Image badgeImage;
        [SerializeField] private Sprite goldBadge;
        [SerializeField] private Sprite silverBadge;
        [SerializeField] private Sprite bronzeBadge;

        private ILeaderboardEntryHighlighter playerHighlighter;

        private void Awake()
        {
            playerHighlighter = GetComponent<ILeaderboardEntryHighlighter>();
            Assert.IsNotNull(playerHighlighter);
        }

        public void SetValues(Placement leaderboardPlacement)
        {
            gameObject.SetActive(true);

            position.text = $"{leaderboardPlacement.Position}.";
            nickname.text = leaderboardPlacement.Nickname;
            // If user id is empty then it is an empty cell and score should be displayed as empty.
            score.text = string.IsNullOrEmpty(leaderboardPlacement.UserId) ? string.Empty : leaderboardPlacement.Score.ToString();

            HighlightCurrentPlayer(leaderboardPlacement);
            UpdateBadgeImage(leaderboardPlacement.Position);
        }

        private void HighlightCurrentPlayer(Placement leaderboardPlacement)
        {
            bool isCurrentPlayer = leaderboardPlacement.UserId.Equals(ElympicsLobbyClient.Instance.UserGuid.ToString());

            if (isCurrentPlayer)
                playerHighlighter.Highlight();
            else
                playerHighlighter.ResetHighlight();
        }

        private void UpdateBadgeImage(int placement)
        {
            if (placement == 1) badgeImage.sprite = goldBadge;
            else if (placement == 2) badgeImage.sprite = silverBadge;
            else if (placement == 3) badgeImage.sprite = bronzeBadge;
            else
            {
                badgeImage.gameObject.SetActive(false);
                return;
            }

            badgeImage.gameObject.SetActive(true);
        }
    }
}
