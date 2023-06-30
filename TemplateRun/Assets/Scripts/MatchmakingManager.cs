using Elympics;
using UnityEngine;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;

public class MatchmakingManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private ErrorPanel errorPanel;
    [SerializeField] private RegionData regionData;

    private void Start()
    {
        ControlPlayAccess(ElympicsLobbyClient.Instance.IsAuthenticated);
        ElympicsLobbyClient.Instance.AuthenticationSucceeded += (_) => ControlPlayAccess(true);

        ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed += OnMatchmakingFailed;
    }

    private void OnDestroy()
    {
        ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed -= OnMatchmakingFailed;
    }

    private void ControlPlayAccess(bool allowToPlay) => playButton.interactable = allowToPlay;
    private void OnMatchmakingFailed((string error, Guid _) result) => errorPanel.Display(result.error, true);

    [UsedImplicitly]
    public async void PlayOnline()
    {
        PersistentManagers.Instance.SetLoadingScreenSliderOpen(false);
        ControlPlayAccess(false);

        var (Region, LatencyMs) = await regionData.ClosestRegion();
        ElympicsLobbyClient.Instance.PlayOnlineInRegion(Region, null, null, QueueDict.MatchmakingQueueSolo);

        Debug.Log($"Connecting to region {Region} with ping {LatencyMs}");
    }
}
