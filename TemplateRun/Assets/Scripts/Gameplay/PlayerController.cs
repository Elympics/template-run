using UnityEngine;
using Elympics;

public class PlayerController : ElympicsMonoBehaviour, IUpdatable, IInputHandler
{
    [SerializeField] JumpManager jumpManager;
    private bool localJumpInput;

    private void Update()
    {
        localJumpInput = Input.GetKey(KeyCode.Space) || Input.touchCount > 0 || localJumpInput;
    }

    public void ElympicsUpdate()
    {
        bool receivedJumpInput = false;
        if (ElympicsBehaviour.TryGetInput(PredictableFor, out IInputReader inputReader))
        {
            inputReader.Read(out receivedJumpInput);
        }

        jumpManager.ManageJump(receivedJumpInput);
    }

    public void OnInputForClient(IInputWriter inputSerializer)
    {
        inputSerializer.Write(localJumpInput);

        localJumpInput = false;
    }

    public void OnInputForBot(IInputWriter inputSerializer)
    {
        // Do nothing, game doesn't have bots
    }
}
