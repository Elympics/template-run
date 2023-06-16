using Elympics;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuController : MonoBehaviour
{
	[SerializeField] private Button playButton;
	[SerializeField] private TMPro.TextMeshProUGUI errorMessage;
	[SerializeField] private GameObject errorPanel;

	private void Start()
	{
		ControlPlayAccess(ElympicsLobbyClient.Instance.IsAuthenticated);
		ElympicsLobbyClient.Instance.AuthenticationSucceeded += (_) => ControlPlayAccess(true);
		ElympicsLobbyClient.Instance.AuthenticationFailed += (error) => Debug.LogError(error);
		ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed += OnMatchmakingFailed;
	}

	private void OnDestroy()
	{
		ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed -= OnMatchmakingFailed;
	}

	private void OnMatchmakingFailed((string error, Guid _) result)
	{
		LoadingScreenManager.Instance.SetSliderOpen(true);

		errorPanel.SetActive(true);
		errorMessage.text = result.error;
	}

	private void ControlPlayAccess(bool allowToPlay)
	{
		playButton.interactable = allowToPlay;
	}

	public void OnPlaySoloClicked()
	{
		ControlPlayAccess(false);

		var closestRegion = FindObjectOfType<RegionManager>().closestRegion;
		ElympicsLobbyClient.Instance.PlayOnlineInRegion(closestRegion.Region, null, null, $"{QueueDict.MatchmakingQueueSolo}");

		Debug.Log($"Connected to region {closestRegion.Region} with ping {closestRegion.LatencyMs}");
	}

	public void PlayHalfRemote()
	{
		ElympicsLobbyClient.Instance.PlayHalfRemote(0);
	}

	public void StartHalfRemoteServer()
	{
		ElympicsLobbyClient.Instance.StartHalfRemoteServer();
	}
}
