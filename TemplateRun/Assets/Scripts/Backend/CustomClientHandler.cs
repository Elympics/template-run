using UnityEngine;
using System;
using Elympics;
using MatchTcpClients.Synchronizer;

public class CustomClientHandler : ElympicsMonoBehaviour, IClientHandlerGuid
{
    [SerializeField] private RandomManager randomManager;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private int timeoutSeconds = 5;

    private DateTime? timeoutThreshold;

    public void OnConnected(TimeSynchronizationData data)
    {
        if (Elympics.IsClient)
        {
            LoadingScreenManager.Instance.SetSliderOpen(true);
            SoundManager.Instance.SetUpMusic();
        }
    }

    public void OnSynchronized(TimeSynchronizationData data)
    {
        timeoutThreshold = data.UnreliableLastReceivedPingDateTime + new TimeSpan(0, 0, timeoutSeconds);
    }

    public void OnStandaloneClientInit(InitialMatchPlayerDataGuid data)
    {
        randomManager.SetSeed(BitConverter.ToInt32(data.GameEngineData));
    }

    private void Update()
    {
        if (timeoutThreshold != null && timeoutThreshold < DateTime.Now && gameStateSynchronizer.GameState != GameState.GameEnded)
        {
            Elympics.Disconnect();
            errorPanel.SetActive(true);
        }
    }

    public void OnConnectingFailed()
    {
        errorPanel.SetActive(true);
    }

    #region Unused
    public void OnDisconnectedByClient() { }
    public void OnClientsOnServerInit(InitialMatchPlayerDatasGuid data) { }
    public void OnDisconnectedByServer() { }
    public void OnAuthenticated(Guid userId) { }
    public void OnAuthenticatedFailed(string errorMessage) { }
    public void OnMatchJoined(Guid matchId) { }
    public void OnMatchJoinedFailed(string errorMessage) { }
    public void OnMatchEnded(Guid matchId) { }
    #endregion
}