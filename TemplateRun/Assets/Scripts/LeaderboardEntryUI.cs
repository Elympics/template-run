using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Elympics;


public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image rankBackgroundImage;

    [SerializeField] private Color currentUserTextColor;
    [SerializeField] private Sprite currentUserBackground;
    [SerializeField] private Sprite currentUserRankBackground;

    public void SetValues(LeaderboardEntry entry, string nickname)
    {
        if (entry == null)
            return;

        positionText.text = entry.Position.ToString();
        nicknameText.text = nickname ?? entry.UserId;
        scoreText.text = entry.Score.ToString("0");
    }

    public void HighlightEntry(LeaderboardEntry entry = null)
    {
        positionText.color = currentUserTextColor;
        nicknameText.color = currentUserTextColor;
        scoreText.color = currentUserTextColor;

        backgroundImage.sprite = currentUserBackground;
        if (entry == null || entry.Position > 3)
            rankBackgroundImage.sprite = currentUserRankBackground;
    }
}
