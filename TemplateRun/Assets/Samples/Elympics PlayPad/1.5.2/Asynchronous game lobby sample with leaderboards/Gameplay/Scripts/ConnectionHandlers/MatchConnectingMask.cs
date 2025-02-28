using TMPro;
using UnityEngine;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class MatchConnectingMask : MonoBehaviour
    {
        [SerializeField] private GameObject matchConnectingObject;
        [SerializeField] private TextMeshProUGUI subText;

        public static MatchConnectingMask Instance { get; protected set; } = null;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public virtual void ShowOrUpdate(string subTextValue)
        {
            subText.text = subTextValue;
            matchConnectingObject.SetActive(true);
        }

        public virtual void Hide()
        {
            subText.text = string.Empty;
            matchConnectingObject.SetActive(false);
        }
    }
}
