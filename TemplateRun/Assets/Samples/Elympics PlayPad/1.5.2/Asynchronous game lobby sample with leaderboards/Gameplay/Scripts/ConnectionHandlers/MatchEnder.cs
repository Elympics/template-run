using Elympics;
using System.Linq;
using UnityEngine;
using System;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class MatchEnder : ElympicsMonoBehaviour
    {
        [SerializeField] private bool shouldZeroScoreBeInterpretedAsNotPlayedMatch = true;

        [SerializeField] private ScoreProviderBase scoreProvider = null;

        private void Awake()
        {
            if (scoreProvider == null)
                throw new NullReferenceException($"Make sure that your score management script inherits from {nameof(ScoreProviderBase)} and is assigned to the {nameof(MatchEnder)} component");
        }

        public void EndMatch()
        {
            if (!Elympics.IsServer)
                return;

            var scores = scoreProvider.Scores;

            if (shouldZeroScoreBeInterpretedAsNotPlayedMatch && scores.All(x => x == 0f))
            {
                Debug.Log($"[{nameof(MatchEnder)}] - ending the match interpreting 0 score as a not played one");
                Elympics.EndGame();
            }
            else
            {
                Debug.Log($"[{nameof(MatchEnder)}] - ending the match with current score results");
                Elympics.EndGame(new ResultMatchPlayerDatas(scores.Select(x => new ResultMatchPlayerData { MatchmakerData = new float[1] { x } }).ToList()));
            }
        }
    }
}
