using UnityEngine;
using UnityEngine.UI;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class LeaderboardBackgroundHighlighter : MonoBehaviour, ILeaderboardEntryHighlighter
    {
        [SerializeField] private Image background;
        [SerializeField] private Color defaultBackgroundColor;
        [SerializeField] private Color highlightedBackgroundColor;

        public void Highlight()
        {
            background.color = highlightedBackgroundColor;
        }

        public void ResetHighlight()
        {
            background.color = defaultBackgroundColor;
        }
    }
}
