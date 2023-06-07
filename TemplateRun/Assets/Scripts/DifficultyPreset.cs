using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/DifficultyPreset")]
public class DifficultyPreset : ScriptableObject //Scriptable object in which we can set how our game's difficulty will progress, by setting threshholds, speed and set of levels from which we randomize the map
{
    [System.Serializable]
    public struct StageAndWeightPair
    {
        public GameObject stage;
        public int weight;
    }
    [System.Serializable]
    public class DifficultyLevel
    {
        public int threshold;
        public float levelSpeed;
        public List<StageAndWeightPair> stageAndWeightPairs;
        public int WeightSum => stageAndWeightPairs.Sum(pair => pair.weight);
        public int coinValue;
    }

    public List<DifficultyLevel> difficultyLevels;
}
