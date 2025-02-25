using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class ViewManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timer;
        [SerializeField] private TextMeshProUGUI pointsDisplay;
        [SerializeField] private EndGameView gameEndedView;
        [SerializeField] private Camera mainCamera;

        private SynchronizedRandomizer randomizer;

        private void Awake()
        {
            randomizer = FindObjectOfType<SynchronizedRandomizer>();
            Assert.IsNotNull(randomizer);
        }

        public void UpdateTimer(int remainingSeconds) => timer.text = remainingSeconds.ToString();
        public void UpdatePoints(int pointsValue) => pointsDisplay.text = pointsValue.ToString();

        public void ShowGameEndedView(int points)
        {
            gameEndedView.Show(points);
        }

        public void RandomizeBackgroundColor(bool isTickDependent = false)
        {
            var random = isTickDependent ? randomizer.TickDependentRandomizer : randomizer.GlobalRandom;

            if (random == null)
            {
                Debug.LogWarning("Synchronized Randomizer is not yet ready");
                return;
            }

            mainCamera.backgroundColor = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }
    }
}
