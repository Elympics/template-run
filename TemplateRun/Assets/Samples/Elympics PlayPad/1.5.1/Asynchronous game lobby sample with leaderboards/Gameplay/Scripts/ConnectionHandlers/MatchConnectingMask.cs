using TMPro;
using UnityEngine;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class MatchConnectingMask : MonoBehaviour
    {
        [SerializeField] private GameObject matchConnectingObject;
        [SerializeField] private TextMeshProUGUI subText;

        public static MatchConnectingMask Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowOrUpdate(string subTextValue)
        {
            subText.text = subTextValue;
            matchConnectingObject.SetActive(true);
        }

        public void Hide()
        {
            subText.text = string.Empty;
            matchConnectingObject.SetActive(false);
        }
    }
}
