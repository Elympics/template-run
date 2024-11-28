using UnityEngine;
using TMPro;
using JetBrains.Annotations;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class ErrorPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI errorText;

        public void Show(string error)
        {
            gameObject.SetActive(true);

            errorText.text = error;
        }

        [UsedImplicitly]
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
