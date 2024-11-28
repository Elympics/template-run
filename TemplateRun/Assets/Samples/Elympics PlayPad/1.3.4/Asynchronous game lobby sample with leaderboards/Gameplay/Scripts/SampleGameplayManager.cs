using Elympics;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using System;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class SampleGameplayManager : ElympicsMonoBehaviour, IInitializable, IUpdatable
    {
        [SerializeField] private int secondsToEndGameAutomatically = 30;
        [SerializeField] private ViewManager viewManager;

        private readonly ElympicsInt remainingSecondsToEndGame = new ElympicsInt();
        private readonly ElympicsInt points = new ElympicsInt();

        private bool pointsBumpRequested = false;
        private bool endGameRequested = false;

        private Guid matchId;

        private int CurrentGameTimeInSeconds => Mathf.FloorToInt(Elympics.Tick * Elympics.TickDuration);

        private void Awake()
        {
            // Remembering matchId to display respect at the end

            if (ElympicsLobbyClient.Instance == null)
                return;

            var joinedRooms = ElympicsLobbyClient.Instance.RoomsManager.ListJoinedRooms();
            matchId = joinedRooms.Count > 0 ? (joinedRooms[0].State.MatchmakingData?.MatchData?.MatchId ?? Guid.Empty) : Guid.Empty;
        }

        public void Initialize()
        {
            remainingSecondsToEndGame.ValueChanged += (_, newValue) => viewManager.UpdateTimer(newValue);
            points.ValueChanged += (_, newValue) => viewManager.UpdatePoints(newValue);

            remainingSecondsToEndGame.Value = secondsToEndGameAutomatically;
            points.Value = 0;
        }

        public void ElympicsUpdate()
        {
            remainingSecondsToEndGame.Value = Mathf.Max(0, secondsToEndGameAutomatically - CurrentGameTimeInSeconds);

            if (remainingSecondsToEndGame.Value == 0)
            {
                EndGameClient();
                EndGameServer();
                return;
            }

            if (!Elympics.IsClient)
                return;

            if (pointsBumpRequested)
            {
                pointsBumpRequested = false;
                points.Value++;
                RpcBumpPoints();
            }

            if (endGameRequested)
            {
                endGameRequested = false;
                EndGameClient();
                RpcEndGame();
            }
        }


        [UsedImplicitly] // by EndGameButton
        public void RequestGameEnd() => endGameRequested = true;

        [ElympicsRpc(ElympicsRpcDirection.PlayerToServer)]
        private void RpcEndGame() => EndGameServer();

        private void EndGameServer()
        {
            if (!Elympics.IsServer)
                return;

            Elympics.EndGame(
                new ResultMatchPlayerDatas(
                    new List<ResultMatchPlayerData>
                    {
                        new ResultMatchPlayerData { MatchmakerData = new float[1] { points.Value } }
                    }));

        }

        private void EndGameClient()
        {
            if (!Elympics.IsClient)
                return;

            viewManager.ShowGameEndedView(matchId);
        }


        // Please note it is only possible outside of ElympicsUpdate because Prediction in the ElympicsGameConfig was set to False
        [UsedImplicitly] // by BumpPointsButton
        public void RequestBumpPoints() => pointsBumpRequested = true;

        // Do not let players influencne their points directly like this in your game!
        // This implementation is only for testing and learning about Elympics SDK
        // But generally it would make cheating quite easy, so please ensure server authority and indirect points evaluation in your scoring system
        [ElympicsRpc(ElympicsRpcDirection.PlayerToServer)]
        private void RpcBumpPoints() => points.Value++;
    }
}
