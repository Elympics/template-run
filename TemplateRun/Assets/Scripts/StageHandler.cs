using UnityEngine;
using Elympics;
using System.Collections.Generic;

public class StageHandler : ElympicsMonoBehaviour, IUpdatable
{
    public Transform endPoint; //Point at which next stage will be instantiated
    protected float destroyX;
    protected MapManager mapManager;
    [SerializeField] private List<GameObject> coinList = new List<GameObject>();

    public void InitializeStage(Vector3 position, float stageDestroyX, MapManager givenMapManager, int randomInt)
    {
        transform.position = position;
        destroyX = stageDestroyX;
        mapManager = givenMapManager;
        if (coinList.Count != 0)
        {
            int activeCoinIndex = randomInt % coinList.Count;
            coinList[activeCoinIndex].SetActive(true);
        }
    }

    private void MoveStage(Vector2 moveVector)
    {
        transform.position = (transform.position + new Vector3(moveVector.x, moveVector.y, 0));
    }

    protected virtual void DestroyStage()
    {
        mapManager.UpdateStagesPassed();
        ElympicsDestroy(gameObject);
    }

    public void ElympicsUpdate()
    {
        if (mapManager == null) mapManager = FindObjectOfType<MapManager>(); //If the object was destroyed before reconciliation, it might be missing the reference during the resimulation
        if (!mapManager.IsRunning) return;
        MoveStage(mapManager.CurrentSpeed * Vector2.left * Elympics.TickDuration);
        if (endPoint.position.x < destroyX) DestroyStage();
    }
}
