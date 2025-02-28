using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class MatchDisconnectionMask : MonoBehaviour
    {
        private const string DefiniteDisconnectionDefaultText = "You have been disconnected from the server. Check your connection and reload the game.";

        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private GameObject errorPopup;
        [SerializeField] private GameObject reconnectionPanel;

        public void Show()
        {
            gameObject.SetActive(true);
            reconnectionPanel.SetActive(true);
            errorPopup.SetActive(false);
        }

        public void ShowDefiniteDisconnection(string errorMessage = null)
        {
            errorText.text = errorMessage ?? DefiniteDisconnectionDefaultText;

            gameObject.SetActive(true);
            reconnectionPanel.SetActive(false);
            errorPopup.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            reconnectionPanel.SetActive(false);
            errorPopup.SetActive(false);
        }
    }
}
