using ElympicsPlayPad.Tournament.Data;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class CasualTournamentPlayButtonsView : PlayButtonsViewBase
    {
        public override void OnTournamentViewUpdated(TournamentInfo info)
        {
            bool shouldShow = ShouldUseThisPlayButtonView(info);
            gameObject.SetActive(shouldShow);
        }

        public override bool ShouldUseThisPlayButtonView(TournamentInfo info)
        {
            //Show this play button if the tournament is casual or Daily
            return info.IsCasual() || info.IsDaily;
        }
    }
}