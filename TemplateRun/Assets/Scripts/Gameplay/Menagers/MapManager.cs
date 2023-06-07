using System.Collections.Generic;
using UnityEngine;
using Elympics;

public class MapManager : ElympicsMonoBehaviour, IUpdatable, IInitializable
{

    [SerializeField] private float destroyStageX;  //if the end point of a stage passes this point, the stage will be destroyed
    [SerializeField] private float spawnStageX; //if the end point of the newest stage passes this point, the new stage will be spawned
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ElympicsGameObject lastStage = new ElympicsGameObject();
    [SerializeField] private RandomManager randomManager;
    private readonly ElympicsInt stagesPassed = new ElympicsInt(0);
    public float CurrentSpeed { get; private set; }
    public bool IsRunning { get; private set; } = false;


    public void Initialize()
    {
        gameStateSynchronizer.SubscribeToGameStateChange(AdjustToGameState);
    }

    public void UpdateStagesPassed()
    {
        stagesPassed.Value++;
        difficultyManager.UpdateDifficulty(stagesPassed.Value);
    }

    public void ElympicsUpdate()
    {
        randomManager.ResetRandom((int)Elympics.Tick); //Using the base seed and a tick we create a temporary predictable seed

        if (!IsRunning)
            return;

        CurrentSpeed = difficultyManager.GetSpeed();
        scoreManager.AddToScore(CurrentSpeed * Elympics.TickDuration);
        if (GetLastStageEndPosition().x < spawnStageX) //If the last stage passed a certain point, we want to generate next stage
            InstantiateNewStage();
    }

    private void InstantiateNewStage()
    {
        Vector3 lastStageEndPosition = GetLastStageEndPosition();
        lastStage.Value = ElympicsInstantiate(difficultyManager.GetRandomStage().name, ElympicsPlayer.All).GetComponent<ElympicsBehaviour>(); //We remember the last created stage in a synchronized variable, so in case of reconciliation the game can resimulate the spawning properly
        StageHandler newStage = lastStage.Value.GetComponent<StageHandler>();
        newStage.InitializeStage(lastStageEndPosition, destroyStageX, this, randomManager.InitializedRandom.Next());
        newStage.ElympicsUpdate(); //The first ElympicsUpdate of the object will trigger in the next tick, so we trigger it manually, to move the new stage consistently with the rest of the map
    }

    private Vector3 GetLastStageEndPosition()
    {
        return lastStage.Value.GetComponent<StageHandler>().endPoint.position;
    }

    private void AdjustToGameState(int previousState, int newState)
    {
        IsRunning = (GameState)newState == GameState.Gameplay;
    }
}