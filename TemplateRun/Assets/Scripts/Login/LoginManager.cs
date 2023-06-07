using UnityEngine;
using UnityEngine.SceneManagement;
using Elympics;
using System;

public class LoginManager : MonoBehaviour
{
	private BackendDependentObjects sceneObjects;
	public static LoginManager Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		SetUiObjects();

		DontDestroyOnLoad(gameObject);

		ElympicsLobbyClient.Instance.AuthenticationSucceeded += _ => AdjustToUserAuthentication();

		SceneManager.sceneLoaded += RefreshMainMenu;
		RefreshMainMenu(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}


	public void SignOut()
	{
		sceneObjects.ChangeToNicknamePanel();
	}

	public void SubmitNickname(string nickname) => BackendWebClient.SetNickname(HandleNicknameSet, nickname);

	private void SetUiObjects()
	{
		sceneObjects = FindObjectOfType<BackendDependentObjects>();
	}

	private void RefreshMainMenu(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (scene.buildIndex != 0)
			return;

		SetUiObjects();
		AdjustToUserAuthentication();
	}

	private void AdjustToUserAuthentication()
	{
		if (!ElympicsLobbyClient.Instance.IsAuthenticated)
		{
			sceneObjects.ManageActiveState(true);
			return;
		}

		BackendWebClient.GetNickname(HandleNicknameGet);
		sceneObjects.LeaderboardsDisplayer.InitializeAndRun();
	}


	private void HandleNicknameGet(Result<IdNicknamePair, Exception> result)
	{
		if (result.IsFailure)
		{
			sceneObjects.ErrorLog.text = "User not fully authenticated - application restart may help";
			return;
		}

		if (string.IsNullOrEmpty(result.Value.Nickname))
		{
			sceneObjects.ChangeToNicknamePanel();
		}
		else
		{
			sceneObjects.ManageActiveState(false);
		}
	}

	private void HandleNicknameSet(Result<IdNicknamePair, Exception> result)
	{
		if (result.IsFailure)
		{
			sceneObjects.NicknameErrorLog.text = "Nickname is incorrect or already taken by another user";
			return;
		}

		sceneObjects.ManageActiveState(false);
	}
}
