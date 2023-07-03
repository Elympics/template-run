using Elympics;

public class ScoreManager : ElympicsMonoBehaviour
{
    private readonly ElympicsFloat score = new ElympicsFloat();

    public float Score => score.Value;

    public void AddToScore(float addedScore) => score.Value += addedScore;

    public string GetDisplayableScore() => score.Value.ToString("0");

    public void SubscribeToScoreChange(ElympicsVar<float>.ValueChangedCallback action)
    {
        score.ValueChanged += action;
    }
}
