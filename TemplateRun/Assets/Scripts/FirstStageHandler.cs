using UnityEngine;
public class FirstStageHandler : StageHandler
{
    [SerializeField] private float assignedDestroyX;
    [SerializeField] private MapManager assignedMapManager;

    private void Start()
    {
        destroyX = assignedDestroyX;
        mapManager = assignedMapManager;
    }

    protected override void DestroyStage()
    {
        gameObject.SetActive(false); //Synchronized stages cannot be destroyed if they weren't created via ElympicsInstantiate so we deactivate them instead
    }
}
