using Cysharp.Threading.Tasks;
using ElympicsPlayPad.ExternalCommunicators;
using ElympicsPlayPad.Session;
using UnityEngine;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private TournamentView tournamentView;
        [SerializeField] private LeaderboardView leaderboardView;
        [SerializeField] private AllTimeHighScoreView allTimeHighScoreView;
        [SerializeField] private TournamentPlayButton tournamentPlayButton;

        [SerializeField] private GameObject authenticationInProgressScreen;

        private void Start()
        {
            var sessionManager = FindObjectOfType<SessionManager>();
            OnLobbySceneLoaded(sessionManager).Forget();
        }

        private async UniTask OnLobbySceneLoaded(SessionManager sessionManager)
        {
            sessionManager.StartSessionInfoUpdate += SetAuthenticationScreenActive;
            sessionManager.FinishSessionInfoUpdate += SetAuthenticationScreenInactive;

            bool shouldHideSplashScreen = false;

            if (!sessionManager.ConnectedWithPlayPad)
            {
                await sessionManager.AuthenticateFromExternalAndConnect();
                shouldHideSplashScreen = true;
            }

            // Put here any further lobby scene configuration and UI adjustments
            tournamentView.OnStart();
            leaderboardView.OnStart();
            allTimeHighScoreView.OnStart();
            tournamentPlayButton.OnStart();

            if (shouldHideSplashScreen)
                PlayPadCommunicator.Instance.GameStatusCommunicator?.HideSplashScreen();
        }

        private void SetAuthenticationScreenActive() => authenticationInProgressScreen.SetActive(true);
        private void SetAuthenticationScreenInactive() => authenticationInProgressScreen.SetActive(false);

        private void OnDestroy()
        {
            var sessionManager = FindObjectOfType<SessionManager>();
            if (sessionManager == null)
                return;

            sessionManager.StartSessionInfoUpdate -= SetAuthenticationScreenActive;
            sessionManager.FinishSessionInfoUpdate -= SetAuthenticationScreenInactive;
        }
    }
}
