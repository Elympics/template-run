using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private DifficultyPreset difficultyPreset;
    [SerializeField] private RandomManager randomManager;
    private int currentDifficultyIndex = 0;
    private DifficultyPreset.DifficultyLevel CurrentDifficultyLevel => difficultyPreset.difficultyLevels[currentDifficultyIndex];

    public void UpdateDifficulty(int stagesPassed)
    {
        if (ReachedMaxDifficulty()) return;
        if (ReachedNextDifficultyThreshold(stagesPassed)) currentDifficultyIndex++;
    }

    private bool ReachedMaxDifficulty()
    {
        return difficultyPreset.difficultyLevels.Count <= currentDifficultyIndex + 1;
    }

    private bool ReachedNextDifficultyThreshold(int stagesPassed)
    {
        return stagesPassed > difficultyPreset.difficultyLevels[currentDifficultyIndex + 1].threshold;
    }

    public float GetSpeed()
    {
        return difficultyPreset.difficultyLevels[currentDifficultyIndex].levelSpeed;
    }

    public float GetCoinValue()
    {
        return difficultyPreset.difficultyLevels[currentDifficultyIndex].coinValue;
    }

    public GameObject GetRandomStage()
    {
        //Weighted randomization
        int randomWeightedIndex = randomManager.InitializedRandom.Next(0, CurrentDifficultyLevel.WeightSum) + 1;
        int stageIndex = 0;
        int currentWeightedSum = 0;
        while (stageIndex < CurrentDifficultyLevel.stageAndWeightPairs.Count)
        {
            currentWeightedSum += CurrentDifficultyLevel.stageAndWeightPairs[stageIndex].weight;
            if (currentWeightedSum >= randomWeightedIndex) return CurrentDifficultyLevel.stageAndWeightPairs[stageIndex].stage;
            else stageIndex++;
        }
        return CurrentDifficultyLevel.stageAndWeightPairs[stageIndex].stage;
    }
}
