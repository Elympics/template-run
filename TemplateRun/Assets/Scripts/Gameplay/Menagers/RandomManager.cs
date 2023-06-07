using UnityEngine;
using Elympics;
using System;
using MatchTcpClients.Synchronizer;

public class RandomManager : MonoBehaviour, IClientHandlerGuid
{
    [SerializeField] private ElympicsInt randomSeed = new ElympicsInt();
    public System.Random InitializedRandom { get; private set; }

    public void OnStandaloneClientInit(InitialMatchPlayerDataGuid data)
    {
        SetSeed(BitConverter.ToInt32(data.GameEngineData));
    }

    public void ResetRandom(int tick)
    {
        //We use system random with set seed to make sure, that while random, the stages we will be spawning will be the same on both server and client
        InitializedRandom = new System.Random(randomSeed.Value + tick);
    }

    public void SetSeed(int seed)
    {
        randomSeed.Value = seed;
    }

    public void OnAuthenticated(Guid userId) { }
    public void OnAuthenticatedFailed(string errorMessage) { }
    public void OnClientsOnServerInit(InitialMatchPlayerDatasGuid data) { }
    public void OnConnected(TimeSynchronizationData data) { }
    public void OnConnectingFailed() { }
    public void OnDisconnectedByClient() { }
    public void OnDisconnectedByServer() { }
    public void OnMatchEnded(Guid matchId) { }
    public void OnMatchJoined(Guid matchId) { }
    public void OnMatchJoinedFailed(string errorMessage) { }
    public void OnSynchronized(TimeSynchronizationData data) { }    
}