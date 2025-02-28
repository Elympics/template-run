using Elympics;
using UnityEngine;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;

public class MatchmakingManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private ErrorPanel errorPanel;

    private void Start()
    {
        if (ElympicsLobbyClient.Instance == null)
            return;

        ControlPlayAccess(ElympicsLobbyClient.Instance.IsAuthenticated);
        ElympicsLobbyClient.Instance.AuthenticationSucceeded += (_) => ControlPlayAccess(true);

        ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed += OnMatchmakingFailed;
    }

    private void OnDestroy()
    {
        if (ElympicsLobbyClient.Instance == null)
            return;

        ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed -= OnMatchmakingFailed;
    }

    private void ControlPlayAccess(bool allowToPlay) => Debug.Log("Play access");
    private void OnMatchmakingFailed((string error, Guid _) result) => errorPanel.Display(result.error, true);

    [UsedImplicitly]
    public async void PlayOnline()
    {
        if (ElympicsLobbyClient.Instance == null)
        {
            Debug.LogWarning("In order for this method to work you need to start from the menu scene. Method call skipped.");
            return;
        }

        PersistentEffectsManager.Instance.SetMatchLoadingScreenActive(true);
        ControlPlayAccess(false);

        var (Region, LatencyMs) = await ClosestRegionFinder.GetClosestRegion();
        ElympicsLobbyClient.Instance.PlayOnlineInRegion(Region, null, null, QueueDict.MatchmakingQueueSolo);

        Debug.Log($"Connecting to region {Region} with ping {LatencyMs}");
    }
}
