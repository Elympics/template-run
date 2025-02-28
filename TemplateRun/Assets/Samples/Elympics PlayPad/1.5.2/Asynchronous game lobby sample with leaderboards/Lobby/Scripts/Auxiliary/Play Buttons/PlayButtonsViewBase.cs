using ElympicsPlayPad.ExternalCommunicators.GameStatus.Models;
using ElympicsPlayPad.Tournament.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public abstract class PlayButtonsViewBase : MonoBehaviour
    {
        [SerializeField] protected Button playButton;
        [SerializeField] protected TextMeshProUGUI playButtonText;
        [SerializeField] protected Image playButtonImage;

        /// <summary>
        /// These sprites will be determined by the playpad PlayStatusInfo and should be changed.
        /// These are currently assigned in TournamentPlayButton via the inspector to avoid assigning the in each different play button view, 
        /// but if you want different sprites for each play button please override the UpdatePlayButton() method and add your own sprites to the new PlayButtonsViewBase child class
        /// </summary>
        private Sprite playAvailableSprite, userActionRequiredSprite, playBlockedSprite;

        /// <summary>
        /// OnDestroy Monobehaviour will be called for all buttons, each button should override the RemoveCallbacks method to handle itself correctly OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            RemoveCallbacks();
        }

        public abstract void OnTournamentViewUpdated(TournamentInfo info);

        /// <summary>
        /// Method to check if this specific PlayButtonView should be shown, this will also be used to determine which play button is currently selected <br></br>
        /// IMPORTANT: These conditions should be exclusive to avoid selecting multiple play buttons.
        /// </summary>
        public abstract bool ShouldUseThisPlayButtonView(TournamentInfo info);

        /// <summary>
        /// Subscribe all buttons to the correct action. Example: OnClickTournamentPlay should be used by all PlayButtons, whereas OnClickTrainingPlay should only be used by Train buttons
        /// </summary>
        public virtual void SubscribePlayButtonCallback(UnityAction OnClickTournamentPlay, UnityAction OnClickTrainingPlay)
        {
            //Remove all listeners first
            RemoveCallbacks();

            playButton.onClick.AddListener(OnClickTournamentPlay);
        }

        public virtual void AssignPlayButtonSprites(Sprite playAvailableSprite, Sprite userActionRequiredSprite, Sprite playBlockedSprite)
        {
            this.playAvailableSprite = playAvailableSprite;
            this.userActionRequiredSprite = userActionRequiredSprite;
            this.playBlockedSprite = playBlockedSprite;
        }

        public virtual void UpdatePlayButton(PlayStatusInfo info)
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

        /// <summary>
        /// This method should be overriden by the derived classes in order to correctly remove the callbacks from the appropiate buttons.
        /// </summary>
        protected virtual void RemoveCallbacks()
        {
            playButton.onClick.RemoveAllListeners();
        }
    }
}