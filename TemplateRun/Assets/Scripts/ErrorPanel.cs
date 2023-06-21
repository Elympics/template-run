using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ErrorPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private Button tryAgainButton;

    public void Display(string errorMessage, bool matchmakingError)
    {
        Debug.LogError(errorMessage);

        gameObject.SetActive(true);
        errorMessageText.text = errorMessage;

        if (matchmakingError)
        {
            tryAgainButton.gameObject.SetActive(true);
            LoadingScreenManager.Instance.SetSliderOpen(true);
        }
        else
        {
            tryAgainButton.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
