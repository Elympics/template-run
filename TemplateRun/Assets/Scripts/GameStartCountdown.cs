using Elympics;
using ElympicsPlayPad.Samples.AsyncGame;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameStartCountdown : ElympicsMonoBehaviour, IUpdatable, IInitializable
{
    [SerializeField] private GenericServerHandler serverHandler;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int secondsToStartAfterConnection = 3;

    private bool countdownStarted = false;
    private readonly ElympicsFloat timeToStart = new ElympicsFloat();

    public event System.Action OnCountdownEnded;

    private void Awake()
    {
        Assert.IsNotNull(serverHandler);
        Assert.IsNotNull(countdownText);
        Assert.IsFalse(secondsToStartAfterConnection < 0);
    }

    public void Initialize()
    {
        serverHandler.GameJustStarted += StartCountdown;
    }

    private void StartCountdown()
    {
        Assert.IsNotNull(PersistentEffectsManager.Instance);

        PersistentEffectsManager.Instance.PlayGameplayMusic();

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
            OnCountdownEnded?.Invoke();
        }
    }
}
