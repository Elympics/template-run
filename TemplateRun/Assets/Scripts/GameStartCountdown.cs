using Elympics;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameStartCountdown : ElympicsMonoBehaviour, IUpdatable, IInitializable
{
    [SerializeField] private WaitingServerHandler serverHandler;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int secondsToStartAfterConnection = 3;

    private bool countdownStarted = false;
    private readonly ElympicsFloat timeToStart = new ElympicsFloat();

    private void Awake()
    {
        Assert.IsNotNull(serverHandler);
        Assert.IsNotNull(gameStateSynchronizer);
        Assert.IsNotNull(countdownText);
        Assert.IsFalse(secondsToStartAfterConnection < 0);
    }

    public void Initialize()
    {
        serverHandler.OnGameReady += StartCountdown;
    }

    private void StartCountdown()
    {
        timeToStart.Value = secondsToStartAfterConnection;
        countdownStarted = true;
    }

    public void ElympicsUpdate()
    {
        if (!countdownStarted)
            return;

        timeToStart.Value -= Elympics.TickDuration;

        if (timeToStart.Value > 0f)
        {
            countdownText.text = Mathf.Ceil(timeToStart.Value).ToString();
        }
        else
        {
            // Game start
            countdownStarted = false;
            countdownText.gameObject.SetActive(false);
            gameStateSynchronizer.StartGame();
        }
    }
}
