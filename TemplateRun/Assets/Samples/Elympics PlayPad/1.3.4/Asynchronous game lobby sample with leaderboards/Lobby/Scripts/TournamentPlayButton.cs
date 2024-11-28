using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using Cysharp.Threading.Tasks;
using ElympicsPlayPad.ExternalCommunicators;
using ElympicsPlayPad.ExternalCommunicators.GameStatus;
using ElympicsPlayPad.ExternalCommunicators.GameStatus.Models;
using UnityEngine.UI;
using System;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class TournamentPlayButton : MonoBehaviour
    {
        [Header("Matchmaking")]
        [SerializeField] private string playQueue = "solo";
        [SerializeField] private GameObject matchmakingInProgressScreen;
        [SerializeField] private ErrorPopup errorScreen;

        [Header("Play button")]
        [SerializeField] private TextMeshProUGUI playButtonText;
        [SerializeField] private Button playButton;
        [SerializeField] private Image playButtonImage;
        [SerializeField] private Sprite playAvailableSprite;
        [SerializeField] private Sprite userActionRequiredSprite;
        [SerializeField] private Sprite playBlockedSprite;

        private IExternalGameStatusCommunicator PlayStatusCommunicator => PlayPadCommunicator.Instance.GameStatusCommunicator;

        public void OnStart()
        {
            PlayStatusCommunicator.PlayStatusUpdated += UpdatePlayButton;

            UpdatePlayButton(PlayStatusCommunicator.CurrentPlayStatus);
        }

        private void OnDestroy()
        {
            PlayStatusCommunicator.PlayStatusUpdated -= UpdatePlayButton;
        }

        private void UpdatePlayButton(PlayStatusInfo info)
        {
            playButtonText.text = info.LabelInfo;

            switch (info.PlayStatus)
            {
                case PlayStatus.Play:
                    playButtonImage.sprite = playAvailableSprite;
                    playButton.interactable = true;
                    break;
                case PlayStatus.UserActionRequired:
                    playButtonImage.sprite = userActionRequiredSprite;
                    playButton.interactable = true;
                    break;
                case PlayStatus.Blocked:
                    playButtonImage.sprite = playBlockedSprite;
                    playButton.interactable = false;
                    break;
                default:
                    throw new Exception("Unsupported PlayStatus detected");
            }
        }

        [UsedImplicitly]
        public void PlayGame() => PlayGameAsync().Forget();

        private async UniTask PlayGameAsync()
        {
            matchmakingInProgressScreen.SetActive(true);

            try
            {
                _ = await PlayStatusCommunicator.PlayGame(new PlayGameConfig { QueueName = playQueue });
            }
            catch (Exception e)
            {
                matchmakingInProgressScreen.SetActive(false);
                errorScreen.Show(e.ToString());
            }
        }
    }
}
