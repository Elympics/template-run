using UnityEngine;
using ElympicsPlayPad.ExternalCommunicators;
using TMPro;
using ElympicsPlayPad.ExternalCommunicators.Tournament;
using ElympicsPlayPad.ExternalCommunicators.Ui;
using JetBrains.Annotations;
using ElympicsPlayPad.Tournament.Data;
using Cysharp.Threading.Tasks;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class TournamentView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tournamentTitle;

        [SerializeField] private TextMeshProUGUI tournamentTimer;
        [SerializeField] private TextMeshProUGUI prizePoolSummary;

        private TournamentTimer tournamentTimerLogic;

        private IExternalTournamentCommunicator TournamentCommunicator => PlayPadCommunicator.Instance.TournamentCommunicator;
        private IExternalUiCommunicator UiCommunicator => PlayPadCommunicator.Instance.ExternalUiCommunicator;

        public void OnStart()
        {
            TournamentCommunicator.TournamentUpdated += UpdateTournamentView;

            UpdateTournamentView(TournamentCommunicator.CurrentTournament ?? default);
        }

        private void OnDestroy()
        {
            TournamentCommunicator.TournamentUpdated -= UpdateTournamentView;
        }

        private void UpdateTournamentView(TournamentInfo info)
        {
            tournamentTitle.text = info.Name;
            tournamentTimerLogic = new TournamentTimer(info.StartDate, info.EndDate);
            // COMING SOON: prizePoolSummary.text = . . .
        }

        private void Update()
        {
            if (tournamentTimerLogic != null)
                tournamentTimer.text = tournamentTimerLogic.GetTimer();
        }

        [UsedImplicitly]
        public void ShowSwitchTournamentPlayPadView() => UiCommunicator.DisplayTournamentsListing().Forget();

        [UsedImplicitly]
        public void ShowTournamentRewardsPlayPadView() => UiCommunicator.DisplayTournamentRewards().Forget();
    }
}
