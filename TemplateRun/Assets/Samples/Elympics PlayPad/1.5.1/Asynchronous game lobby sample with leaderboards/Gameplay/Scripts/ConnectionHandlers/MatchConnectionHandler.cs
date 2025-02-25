using System;
using System.Collections;
using Elympics;
using MatchTcpClients.Synchronizer;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class MatchConnectionHandler : ElympicsMonoBehaviour, IClientHandlerGuid
    {
        private const string MatchConnectionSubText = "Establishing connection with the server...";

        [SerializeField] private int ReconnectingPopupTimeout = 2;
        [SerializeField] private int DefiniteDisconnectionTimeout = 8;

        private MatchDisconnectionMask disconnectionMask;

        private float secondsWithoutConnection = -1;
        private Coroutine waitingForDefiniteDisconnection = null;
        private bool matchEnded = false;


        private void Awake()
        {
            Assert.IsNotNull(MatchConnectingMask.Instance);

            disconnectionMask = FindObjectOfType<MatchDisconnectionMask>(true);
            Assert.IsNotNull(disconnectionMask);

            if (ElympicsLobbyClient.Instance != null)
                ElympicsLobbyClient.Instance.WebSocketSession.Disconnected += WebSocketSession_Disconnected;
        }

        private void OnDestroy()
        {
            if (ElympicsLobbyClient.Instance != null)
                ElympicsLobbyClient.Instance.WebSocketSession.Disconnected -= WebSocketSession_Disconnected;
        }

        #region happy path
        public void OnStandaloneClientInit(InitialMatchPlayerDataGuid data)
        {
            Debug.Log($"[{nameof(MatchConnectionHandler)}.{nameof(OnStandaloneClientInit)}]");

            MatchConnectingMask.Instance.ShowOrUpdate(MatchConnectionSubText);
        }

        public void OnConnected(TimeSynchronizationData data)
        {
            Debug.Log($"[{nameof(MatchConnectionHandler)}.{nameof(OnConnected)}]");

            secondsWithoutConnection = 0;

            MatchConnectingMask.Instance.Hide();
        }

        public void OnMatchEnded(Guid matchId)
        {
            Debug.Log($"[{nameof(MatchConnectionHandler)}.{nameof(OnMatchEnded)}]");

            matchEnded = true;
        }
        #endregion

        #region continuous connection checking
        public void OnSynchronized(TimeSynchronizationData data)
        {
            secondsWithoutConnection = 0;

            if (waitingForDefiniteDisconnection != null)
            {
                StopCoroutine(waitingForDefiniteDisconnection);
                waitingForDefiniteDisconnection = null;

                disconnectionMask.Hide();
            }
        }

        private void Update()
        {
            if (Elympics.IsServer || secondsWithoutConnection < 0)
                return;

            secondsWithoutConnection += Time.deltaTime;

            if (secondsWithoutConnection >= ReconnectingPopupTimeout && !matchEnded && waitingForDefiniteDisconnection == null)
            {
                Debug.LogWarning($"[{nameof(MatchConnectionHandler)}] - Lost connection, trying to reconnect");
                disconnectionMask.Show();

                waitingForDefiniteDisconnection = StartCoroutine(HandleDefiniteDisconnection());
            }
        }

        private IEnumerator HandleDefiniteDisconnection()
        {
            // there is still a chance for reconnection
            yield return new WaitForSeconds(DefiniteDisconnectionTimeout);

            Debug.LogError($"[{nameof(MatchConnectionHandler)}] - Definite Disconnection");
            secondsWithoutConnection = -1;
            disconnectionMask.ShowDefiniteDisconnection();
        }
        #endregion

        #region failing paths
        private void WebSocketSession_Disconnected(DisconnectionData data)
        {
            // Necesary check so that the popup doesn't show up when the user is connecting wallet via external applications or other intended web socket disconnects
            if (data.Reason is DisconnectionReason.ClientRequest or DisconnectionReason.ApplicationShutdown)
            {
                Debug.Log($"Web socket disconnected: {data.Reason}");
                return;
            }

            Debug.LogError($"Web socket disconnected: {data.Reason}");

            secondsWithoutConnection = -1;
            disconnectionMask.ShowDefiniteDisconnection($"Web socket disconnected: {data.Reason}. Check your Internet connection and reload the game.");
        }

        public void OnConnectingFailed()
        {
            Debug.LogError($"[{nameof(MatchConnectionHandler)}.{nameof(OnConnectingFailed)}] - Couldn't connect to the server. Check your Internet connection and reload the game.");

            secondsWithoutConnection = -1;
            disconnectionMask.ShowDefiniteDisconnection("Couldn't connect to the server. Check your Internet connection and reload the game.");

            MatchConnectingMask.Instance.Hide();
        }

        public void OnAuthenticatedFailed(string errorMessage)
        {
            Debug.LogError($"[{nameof(MatchConnectionHandler)}.{nameof(OnAuthenticatedFailed)}] - Couldn't connect to the server. Check your Internet connection and reload the game - {errorMessage}");

            secondsWithoutConnection = -1;
            disconnectionMask.ShowDefiniteDisconnection("Couldn't authenticate. Check your Internet connection and reload the game.");

            MatchConnectingMask.Instance.Hide();
        }

        public void OnMatchJoinedFailed(string errorMessage)
        {
            Debug.LogError($"[{nameof(MatchConnectionHandler)}.{nameof(OnMatchJoinedFailed)}] - Couldn't connect to the server. Check your Internet connection and reload the game - {errorMessage}");

            secondsWithoutConnection = -1;
            disconnectionMask.ShowDefiniteDisconnection("Couldn't connect to the match. Check your Internet connection and reload the game.");

            MatchConnectingMask.Instance.Hide();
        }
        #endregion

        #region diagnosis
        public void OnDisconnectedByServer()
        {
            Debug.Log($"[{nameof(MatchConnectionHandler)}.{nameof(OnDisconnectedByServer)}]");
        }

        public void OnDisconnectedByClient()
        {
            Debug.Log($"[{nameof(MatchConnectionHandler)}.{nameof(OnDisconnectedByClient)}]");
        }
        #endregion
    }
}
