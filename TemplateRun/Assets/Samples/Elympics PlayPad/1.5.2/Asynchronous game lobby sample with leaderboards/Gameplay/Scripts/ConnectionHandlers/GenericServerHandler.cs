using System;
using Elympics;
using ElympicsPlayPad.ExternalCommunicators.Tournament.Utility;
using UnityEngine;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    [RequireComponent(typeof(MatchEnder))]
    public class GenericServerHandler : DefaultServerHandlerr, IUpdatable
    {
        [Tooltip("Viable only when " + nameof(autoTerminationOnLeft) + " is set to None.")]
        [SerializeField] private float rejoiningTimeoutInSeconds = 120f;

        private bool _gameJustStarted = false;
        private bool _anyPlayerJustRejoined = false;

        private MatchEnder _matchEnder;
        [SerializeField] private SynchronizedRandomizerBase synchronizedRandomizer;


        public event Action GameJustStarted;
        // Following events are meant for freezing game logic when waiting for the player to rejoin (if such a feature is enabled - with autoTerminationOnLeft set to None)
        public event Action PlayerRejoined_ClientCallIncluded;
        public event Action PlayerDisconnected_ServerOnly;


        private void Awake()
        {
            _matchEnder = GetComponent<MatchEnder>();

            if (synchronizedRandomizer == null)
                throw new NullReferenceException($"Make sure that your randomization system inherits from {nameof(SynchronizedRandomizerBase)} and is assigned to the {nameof(GenericServerHandler)} component");
        }

        public override void OnServerInit(InitialMatchPlayerDatasGuid initialMatchPlayerDatas)
        {
            var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue); // random seed for no tournament, editor testing
            if (initialMatchPlayerDatas.CustomMatchmakingData != null && initialMatchPlayerDatas.CustomMatchmakingData.TryGetValue(TournamentConst.TournamentIdKey, out var tournamentId))
            {
                // existing tournament, online play
                seed = tournamentId.GetHashCode();
            }

            synchronizedRandomizer.InitializeRandomization(seed);

            base.OnServerInit(initialMatchPlayerDatas);
        }

        public override void OnPlayerDisconnected(ElympicsPlayer player)
        {
            base.OnPlayerDisconnected(player);

            PlayerDisconnected_ServerOnly?.Invoke();
        }

        protected override void CloseMatch() => _matchEnder.EndMatch();
        protected override void OnPlayerRejoined() => _anyPlayerJustRejoined = true;
        protected override void OnGameStarted()
        {
            base.OnGameStarted();
            _gameJustStarted = true;
        }

        public void ElympicsUpdate()
        {
            // happens only on the server instance
            if (!Elympics.IsServer)
                return;

            if (_gameJustStarted)
            {
                _gameJustStarted = false;
                StartGameplay();
                StartGameplayAtClient(synchronizedRandomizer.InitialSeed);
            }

            if (_anyPlayerJustRejoined)
            {
                _anyPlayerJustRejoined = false;
                HandleRejoin();
                HandleRejoinAtClient(synchronizedRandomizer.InitialSeed);
            }

            if (PlayersConnected.Count == 0 && GameStarted && autoTerminationOnLeft == TerminationOption.None)
            {
                rejoiningTimeoutInSeconds -= Elympics.TickDuration;

                if (rejoiningTimeoutInSeconds <= 0)
                {
                    Debug.LogWarning($"[GenericServerHandler] - Rejoining timeout {rejoiningTimeoutInSeconds}s reached. Forcing game server to quit because one of the players has disconnected. Current score saved as the results");
                    CloseMatch();
                }
            }
        }

        private void HandleRejoin()
        {
            Debug.Log("[GenericServerHandler] - HandleRejoin");
            PlayerRejoined_ClientCallIncluded?.Invoke();
        }

        [ElympicsRpc(ElympicsRpcDirection.ServerToPlayers)]
        private void HandleRejoinAtClient(int initialSeed)
        {
            synchronizedRandomizer.InitializeRandomization(initialSeed);
            HandleRejoin();
        }

        private void StartGameplay()
        {
            Debug.Log("[GenericServerHandler] - StartGameplay");
            GameJustStarted?.Invoke();
        }

        [ElympicsRpc(ElympicsRpcDirection.ServerToPlayers)]
        private void StartGameplayAtClient(int initialSeed)
        {
            synchronizedRandomizer.InitializeRandomization(initialSeed);
            StartGameplay();
        }
    }
}
