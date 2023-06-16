using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackendDependentObjects : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI errorLog;
	[SerializeField] private GameObject loginPanel;
	[SerializeField] private GameObject[] menuPanelObjects;
	[SerializeField] private LeaderboardsDisplayer leaderboardsDisplayer;

	[SerializeField] private GameObject requestLoginPanel;
	[SerializeField] private GameObject nicknamePanel;
	[SerializeField] private TextMeshProUGUI nicknameErrorLog;
	[SerializeField] private TMP_InputField nicknameInputField;
	[SerializeField] private Button submitNicknameButton;

	[SerializeField] private Button googleSignInButton;
	[SerializeField] private Button appleSignInButton;
	[SerializeField] private Button emailSignInButton;
	[SerializeField] private Button anonymousSignInButton;

	[SerializeField] private GameObject anonymousSignInLoading;

	public TextMeshProUGUI ErrorLog => errorLog;
	public TextMeshProUGUI NicknameErrorLog => nicknameErrorLog;
	public LeaderboardsDisplayer LeaderboardsDisplayer => leaderboardsDisplayer;

	public Button SubmitNicknameButton => submitNicknameButton;
	public TMP_InputField NicknameInputField => nicknameInputField;

	private void Start()
	{
		submitNicknameButton.onClick.AddListener(() => LoginManager.Instance.SubmitNickname(nicknameInputField.text));
	}

	public void ManageActiveState(bool needsAuthentication)
	{
		loginPanel.SetActive(needsAuthentication);
		requestLoginPanel.SetActive(needsAuthentication);

		foreach (var element in menuPanelObjects)
		{
			element.SetActive(!needsAuthentication);
		}

		nicknamePanel.SetActive(false);
		anonymousSignInLoading.SetActive(!needsAuthentication);
	}

	public void ChangeToNicknamePanel()
	{
		requestLoginPanel.SetActive(false);
		nicknamePanel.SetActive(true);
	}
}
