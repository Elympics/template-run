using UnityEngine;
using TMPro;
public class RuntimeScoreDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;

    private void Start()
    {
        gameStateSynchronizer.SubscribeToGameStateChange(AdjustVisibilityToGameState);
        scoreManager.SubscribeToScoreChange(UpdateVisibleScore);
    }

    private void UpdateVisibleScore(float oldValue, float newValue)
    {
        scoreText.text = scoreManager.GetDisplayableScore();
    }

    private void AdjustVisibilityToGameState(int oldState, int newState)
    {
        scoreText.enabled = (GameState)newState == GameState.Gameplay;
    }
}
