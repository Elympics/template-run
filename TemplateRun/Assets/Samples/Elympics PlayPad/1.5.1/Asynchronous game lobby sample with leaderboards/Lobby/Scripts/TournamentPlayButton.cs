using UnityEngine;
using Cysharp.Threading.Tasks;
using ElympicsPlayPad.ExternalCommunicators;
using ElympicsPlayPad.ExternalCommunicators.GameStatus;
using ElympicsPlayPad.ExternalCommunicators.GameStatus.Models;
using System;
using UnityEngine.Assertions;
using Elympics;
using ElympicsPlayPad.ExternalCommunicators.Tournament;
using ElympicsPlayPad.Tournament.Data;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class TournamentPlayButton : MonoBehaviour
    {
        private const string ConnectingSubText = "Preparing the match...";

        [Header("Matchmaking")]
        [SerializeField] private string playQueue = "solo";
        [SerializeField] private ErrorPopup errorScreen;

        [Header("Play button")]
        [SerializeField] private Sprite playAvailableSprite;
        [SerializeField] private Sprite userActionRequiredSprite;
        [SerializeField] private Sprite playBlockedSprite;
        [SerializeField] private PlayButtonsViewBase[] playButtonViews;

        private PlayButtonsViewBase currentPlayButtonView;

        private IExternalGameStatusCommunicator PlayStatusCommunicator => PlayPadCommunicator.Instance.GameStatusCommunicator;
        private IExternalTournamentCommunicator TournamentCommunicator => PlayPadCommunicator.Instance.TournamentCommunicator;

        private void Awake()
        {
            Assert.IsNotNull(errorScreen);
            Assert.IsNotNull(playAvailableSprite);
            Assert.IsNotNull(userActionRequiredSprite);
            Assert.IsNotNull(playBlockedSprite);
        }

        public void OnStart()
        {
            //Subscribe callbacks for play button & train button, each play button will handle it as needed
            for (int i = 0; i < playButtonViews.Length; i++)
            {
                playButtonViews[i].SubscribePlayButtonCallback(PlayTournament, PlayTraining);
                playButtonViews[i].AssignPlayButtonSprites(playAvailableSprite, userActionRequiredSprite, playBlockedSprite);
            }

            TournamentCommunicator.TournamentUpdated += UpdateTournamentView;
            UpdateTournamentView(TournamentCommunicator.CurrentTournament.Value);

            PlayStatusCommunicator.PlayStatusUpdated += UpdatePlayButton;
            UpdatePlayButton(PlayStatusCommunicator.CurrentPlayStatus);
        }

        private void OnDestroy()
        {
            PlayStatusCommunicator.PlayStatusUpdated -= UpdatePlayButton;
            TournamentCommunicator.TournamentUpdated -= UpdateTournamentView;
        }

        private void UpdateTournamentView(TournamentInfo info)
        {
            //Check and select new current play button view & enable / disable all play buttons depending on tournament type
            for (int i = 0; i < playButtonViews.Length; i++)
            {
                playButtonViews[i].OnTournamentViewUpdated(info);

                if (playButtonViews[i].ShouldUseThisPlayButtonView(info))
                    currentPlayButtonView = playButtonViews[i];
            }
        }

        private void UpdatePlayButton(PlayStatusInfo info)
        {
            currentPlayButtonView.UpdatePlayButton(info);
        }

        private void PlayTournament()
        {
            UniTask<IRoom> roomTask = PlayStatusCommunicator.PlayGame(new PlayGameConfig { QueueName = playQueue });
            PlayGameAsync(roomTask).Forget();
        }

        private void PlayTraining()
        {
            UniTask<IRoom> roomTask = ElympicsLobbyClient.Instance.RoomsManager.StartQuickMatch(playQueue);
            PlayGameAsync(roomTask).Forget();
        }

        private async UniTask PlayGameAsync(UniTask<IRoom> roomTask)
        {
            Assert.IsNotNull(MatchConnectingMask.Instance);
            MatchConnectingMask.Instance.ShowOrUpdate(ConnectingSubText);

            try
            {
                _ = await roomTask;
            }
            catch (Exception e)
            {
                if (PlayStatusCommunicator.CurrentPlayStatus.PlayStatus == PlayStatus.UserActionRequired)
                {
                    Debug.Log($"[PlayGameAsync]: {e.Message}");
                }
                else
                {
                    Debug.LogError($"[PlayGameAsync]: {e}");
                    errorScreen.Show($"Connecting to match failed. {e.Message}");
                }

                MatchConnectingMask.Instance.Hide();
            }
        }
    }
}
