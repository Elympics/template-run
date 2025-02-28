using Elympics;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public abstract class ScoreProviderBase : ElympicsMonoBehaviour
    {
        /// <summary>
        /// Returns an array representing all players' score in playerId order
        /// </summary>
        public abstract float[] Scores { get; }
    }
}
