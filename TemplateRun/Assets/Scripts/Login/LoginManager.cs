using UnityEngine;
using Elympics;
using System;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private LoginPanel loginPanel;
    [SerializeField] private LeaderboardsDisplayer leaderboardsDisplayer;

    private void Awake()
    {
        ElympicsLobbyClient.Instance.AuthenticationFailed += OnAuthenticationFailed;
        ElympicsLobbyClient.Instance.AuthenticationSucceeded += _ => AdjustToUserAuthentication();

        // In case we are already authenticated (event above wouldn't then happen)
        AdjustToUserAuthentication();
    }

    private void OnAuthenticationFailed(string error) => loginPanel.DisplayBlockingError(error);

    private void AdjustToUserAuthentication()
    {
        if (ElympicsLobbyClient.Instance == null || !ElympicsLobbyClient.Instance.IsAuthenticated)
            return;

        if (!leaderboardsDisplayer.FetchAlreadyStarted)
            leaderboardsDisplayer.InitializeAndRun();

        if (!string.IsNullOrEmpty(UserData.Nickname))
        {
            loginPanel.Hide();
            menuPanel.SetActive(true);
        }
        else
        {
            ExternalBackendClient.GetNickname(HandleNicknameGet);
        }
    }

    private void HandleNicknameGet(Result<IdNicknamePair, Exception> result)
    {
        if (result.IsFailure)
        {
            loginPanel.DisplayBlockingError(result.Error.ToString());
            return;
        }

        if (string.IsNullOrEmpty(result.Value.Nickname))
        {
            loginPanel.DisplayNicknamePanel(this);
        }
        else
        {
            UserData.Nickname = result.Value.Nickname;
            AdjustToUserAuthentication();
        }
    }

    public void HandleNicknameSet(Result<IdNicknamePair, Exception> result)
    {
        if (result.IsFailure)
        {
            loginPanel.DisplayNicknameError();
            return;
        }

        UserData.Nickname = result.Value.Nickname;
        AdjustToUserAuthentication();
    }
}
