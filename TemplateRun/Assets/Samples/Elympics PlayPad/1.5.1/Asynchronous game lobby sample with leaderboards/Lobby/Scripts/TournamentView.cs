using UnityEngine;
using ElympicsPlayPad.ExternalCommunicators;
using TMPro;
using ElympicsPlayPad.ExternalCommunicators.Tournament;
using ElympicsPlayPad.ExternalCommunicators.Ui;
using JetBrains.Annotations;
using ElympicsPlayPad.Tournament.Data;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class TournamentView : MonoBehaviour
    {
        private static readonly string NullPrizePoolText = "Fame & Glory";

        [SerializeField] private TextMeshProUGUI tournamentTitle;

        [Header("Timer Text References")]
        [SerializeField] private TextMeshProUGUI tournamentTimerLabel;
        [SerializeField] private TextMeshProUGUI tournamentTimer;

        [Header("Prize Pool References")]
        [SerializeField] private TextMeshProUGUI prizePoolValue;
        [SerializeField] private Image prizePoolImage;

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

            if (info.PrizePool == null)
            {
                prizePoolValue.text = NullPrizePoolText;
                prizePoolImage.gameObject.SetActive(false);
                return;
            }

            // In case amount is not applicable, display the name
            prizePoolValue.text = info.PrizePool.Value.Amount > 0 ? info.PrizePool.Value.Amount.ToString() : info.PrizePool.Value.DisplayName;

            if (info.PrizePool.Value.Image != null)
            {
                prizePoolImage.gameObject.SetActive(true);
                prizePoolImage.sprite = Texture2DToSprite.Convert(info.PrizePool.Value.Image);
            }
            else
                prizePoolImage.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (tournamentTimerLogic != null)
            {
                var (timer, label) = tournamentTimerLogic.GetTimerAndLabel();
                tournamentTimerLabel.text = label;
                tournamentTimer.text = timer;
            }
        }

        [UsedImplicitly]
        public void ShowSwitchTournamentPlayPadView() => UiCommunicator.DisplayTournamentsListing().Forget();

        [UsedImplicitly]
        public void ShowTournamentRewardsPlayPadView() => UiCommunicator.DisplayTournamentRewards().Forget();
    }
}
