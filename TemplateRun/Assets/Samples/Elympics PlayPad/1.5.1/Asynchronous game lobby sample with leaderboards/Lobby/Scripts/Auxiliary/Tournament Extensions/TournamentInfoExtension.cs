using ElympicsPlayPad.Tournament.Data;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public static class TournamentInfoExtension
    {
        /// <summary>
        /// Casual tournaments are tournaments which are NOT Daily tournaments (created by users) & do not have a prize pool.
        /// This is a handy extension method to check if tournament is casual
        /// </summary>
        public static bool IsCasual(this TournamentInfo info) => !info.IsDaily && info.PrizePool == null;

        /// <summary>
        /// Competitive tournaments are tournaments which are NOT Daily tournaments (created by users) & have a prize pool.
        /// This is a handy extension method to check if tournament is competitive
        /// </summary>
        public static bool IsCompetitive(this TournamentInfo info) => !info.IsDaily && info.PrizePool != null;
    }
}
