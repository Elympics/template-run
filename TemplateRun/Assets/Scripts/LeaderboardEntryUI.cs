using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Elympics;


public class LeaderboardEntryUI : MonoBehaviour
{
    [Header("UI references")]
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image rankBackgroundImage;

    [Header("Current user highlight visuals")]
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

    public void HighlightEntry(bool topThree = false)
    {
        positionText.color = currentUserTextColor;
        nicknameText.color = currentUserTextColor;
        scoreText.color = currentUserTextColor;

        backgroundImage.sprite = currentUserBackground;
        if (!topThree)
            rankBackgroundImage.sprite = currentUserRankBackground;
    }
}
