using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private WaitingIndicator waitingIndicator;
    [SerializeField] private TextMeshProUGUI blockingErrorLog;
    [SerializeField] private GameObject loginFrame;

    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TextMeshProUGUI nicknameErrorLog;
    [SerializeField] private TMP_InputField nicknameInputField;

    private LoginManager loginManager;

    private void ChangeToLoginFrame()
    {
        waitingIndicator.gameObject.SetActive(false);
        loginFrame.SetActive(true);
    }

    public void DisplayBlockingError(string error)
    {
        ChangeToLoginFrame();

        blockingErrorLog.text = string.Concat("Check your Internet connection and try restartig the game.\n", error);
    }

    public void Hide()
    {
        waitingIndicator.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    public void DisplayNicknamePanel(LoginManager loginManager)
    {
        ChangeToLoginFrame();

        this.loginManager = loginManager;
        nicknamePanel.SetActive(true);
    }

    [UsedImplicitly] public void SubmitNickname() => ExternalBackendClient.SetNickname(loginManager.HandleNicknameSet, nicknameInputField.text);

    public void DisplayNicknameError()
    {
        nicknameErrorLog.text = "Nickname is incorrect or already taken by another user";
    }
}
