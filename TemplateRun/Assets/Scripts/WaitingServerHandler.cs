using Elympics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WaitingServerHandler : ElympicsMonoBehaviour, IServerHandlerGuid, IInitializable
{
    [SerializeField] private float timeForPlayersToConnect = 30f;
    [SerializeField] private float connectingTimeoutCheckDelta = 5f;
    [SerializeField] private bool shouldGameEndAfterAnyDisconnect = false;
    [SerializeField] private RandomManager randomManager;

    private readonly HashSet<ElympicsPlayer> _playersConnected = new HashSet<ElympicsPlayer>();
    private int _playersNumber;
    private int _humanPlayersNumber;

    private bool _gameCancelled = false;
    private readonly ElympicsBool _gameReady = new ElympicsBool(false);

    public event Action OnGameReady;
    public bool IsGameReady => _gameReady.Value;

    public void Initialize()
    {
        // We set up this event here because it is also run on every Client, unlike OnServerInit
        _gameReady.ValueChanged += (_, _) => OnGameReady?.Invoke();
        OnGameReady += () => Debug.Log("All players are ready");
    }

    public void OnServerInit(InitialMatchPlayerDatasGuid initialMatchPlayerDatas)
    {
        if (!IsEnabledAndActive)
            return;

        // Ensure initial synchronization variables are correct
        Assert.IsFalse(timeForPlayersToConnect < 0f || connectingTimeoutCheckDelta < 0f || connectingTimeoutCheckDelta > timeForPlayersToConnect);

        _playersNumber = initialMatchPlayerDatas.Count;
        _humanPlayersNumber = initialMatchPlayerDatas.Count(x => !x.IsBot);
        Debug.Log($"Game initialized with {_humanPlayersNumber} human players and {initialMatchPlayerDatas.Count - _humanPlayersNumber} bots");

        byte[] data = initialMatchPlayerDatas[0].GameEngineData;
        Debug.Log("DATA: " + Convert.ToBase64String(data));
        int seed = BitConverter.ToInt32(data);
        randomManager.SetSeed(seed);
        Debug.Log("SET SEED TO: " + seed);

        StartCoroutine(WaitForClientsToConnect());
    }

    private IEnumerator WaitForClientsToConnect()
    {
        DateTime waitForPlayersFinishTime = DateTime.Now + TimeSpan.FromSeconds(timeForPlayersToConnect);

        while (DateTime.Now < waitForPlayersFinishTime && !GameStateAlreadyDetermined)
        {
            Debug.Log("Waiting for all players to connect...");
            yield return new WaitForSeconds(connectingTimeoutCheckDelta);
        }

        if (GameStateAlreadyDetermined)
            yield break;

        EndGameForcefully("Not all players have connected, therefore the game cannot start and so it ends");
    }
    
    public void OnPlayerDisconnected(ElympicsPlayer player)
    {
        if (!IsEnabledAndActive)
            return;

        Debug.Log($"Player {player} disconnected");
        _playersConnected.Remove(player);

        if (_gameCancelled)
            return;

        if (shouldGameEndAfterAnyDisconnect || NoHumanPlayersInGame)
            EndGameForcefully("Therefore, the game has ended");
    }


    public void OnPlayerConnected(ElympicsPlayer player)
    {
        if (!IsEnabledAndActive)
            return;

        Debug.Log($"Player {player} connected");
        _playersConnected.Add(player);

        if (NotAllPlayersConnected || GameStateAlreadyDetermined)
            return;

        BeginTheGame();
    }

    private bool GameStateAlreadyDetermined => _gameReady.Value || _gameCancelled;
    private bool NoHumanPlayersInGame => _playersConnected.Count == _playersNumber - _humanPlayersNumber;
    private bool NotAllPlayersConnected => _playersConnected.Count != _playersNumber;

    private void BeginTheGame()
    {
        _gameReady.Value = true;
    }

    private void EndGameForcefully(string message)
    {
        _gameCancelled = true;
        Debug.Log(message);
        Elympics.EndGame();
    }

    // This Unity event method is necessary for the script to have a checkbox in Inspector.
    // https://forum.unity.com/threads/why-do-some-components-have-enable-disable-checkboxes-in-the-inspector-while-others-dont.390770/#post-2547484
    // ReSharper disable once Unity.RedundantEventFunction
    private void Start()
    { }
}
