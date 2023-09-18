using Elympics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WaitingServerHandler : ElympicsMonoBehaviour, IServerHandlerGuid, IUpdatable
{
    [SerializeField] private float timeForPlayersToConnect = 30f;
    [SerializeField] private float connectingTimeoutCheckDelta = 5f;
    [SerializeField] private bool shouldGameEndAfterAnyDisconnect = false;
    [SerializeField] private RandomManager randomManager;

    private readonly HashSet<ElympicsPlayer> _playersConnected = new HashSet<ElympicsPlayer>();
    private int _playersNumber;
    private int _humanPlayersNumber;

    private bool _gameCancelled = false;
    private bool _isReadyLocally = false;
    private readonly ElympicsBool _gameReady = new ElympicsBool(false);

    public event Action OnGameReady;
    public bool IsGameReady => _gameReady.Value;

    private void Awake()
    {
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

        InitializeRandomnessSeed(initialMatchPlayerDatas);

        StartCoroutine(WaitForClientsToConnect());
    }

#warning Potential vulnerability: Currently, seed handling is implemented in such a way that player's GameEngineData is taken into account if available. Otherwise it is randomized by the server. First solution is only secure if proper External Game Backend is provided. If not, user could send prepared seed manually. In such case it is recommended to remove checking and just apply server generated seed directly.
    private void InitializeRandomnessSeed(InitialMatchPlayerDatasGuid initialMatchPlayerDatas)
    {
        byte[] data = initialMatchPlayerDatas[0].GameEngineData;
        int seed;

        try
        {
            // Available when our External Game Backend is connected to the game
            // or when there is no EGB connected but a player supplies valid Game Engine data
            seed = BitConverter.ToInt32(data);
        }
        catch (Exception)
        {
            // Alternatively, seed is just randomized by the server
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        randomManager.SetSeed(seed);
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

    public void ElympicsUpdate()
    {
        if (_isReadyLocally)
            return;

        if (!_gameReady.Value)
            return;

        OnGameReady?.Invoke();
        _isReadyLocally = true;
    }
}
