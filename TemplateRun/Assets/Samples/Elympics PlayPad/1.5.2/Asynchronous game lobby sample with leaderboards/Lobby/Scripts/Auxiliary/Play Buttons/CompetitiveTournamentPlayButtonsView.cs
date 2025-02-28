using ElympicsPlayPad.Tournament.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class CompetitiveTournamentPlayButtonsView : PlayButtonsViewBase
    {
        [SerializeField] private Button trainButton;

        public override void SubscribePlayButtonCallback(UnityAction OnClickTournamentPlay, UnityAction OnClickTrainingPlay)
        {
            base.SubscribePlayButtonCallback(OnClickTournamentPlay, OnClickTrainingPlay);

            trainButton.onClick.AddListener(OnClickTrainingPlay);
        }

        public override void OnTournamentViewUpdated(TournamentInfo info)
        {
            bool shouldShow = ShouldUseThisPlayButtonView(info);
            gameObject.SetActive(shouldShow);
        }

        public override bool ShouldUseThisPlayButtonView(TournamentInfo info)
        {
            //Only show this play button if is competitive tournament
            return info.IsCompetitive();
        }

        protected override void RemoveCallbacks()
        {
            base.RemoveCallbacks();
            trainButton.onClick.RemoveAllListeners();
        }
    }
}