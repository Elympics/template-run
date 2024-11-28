using Elympics;
using TMPro;
using UnityEngine;
using Elympics.Models.Authentication;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using System;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class EndGameView : MonoBehaviour
    {
        private readonly string formattedRespectMessage = "You got {0} respect";

        [SerializeField] private string lobbySceneName = "AsyncGameLobbyScene";
        [SerializeField] private TextMeshProUGUI respectText;

        public void Show(Guid matchId)
        {
            gameObject.SetActive(true);
            DisplayRespect(matchId).Forget();
        }

        [UsedImplicitly] // by BackToLobbyButton
        public void ReturnToLobby()
        {
            SceneManager.LoadScene(lobbySceneName);
        }

        private async UniTask DisplayRespect(Guid matchId)
        {
            var lobby = ElympicsLobbyClient.Instance;

            if (lobby == null
                || !lobby.IsAuthenticated
                || lobby.AuthData.AuthType is AuthType.None or AuthType.ClientSecret)
            {
                respectText.text = "Log in to earn respect";
            }
            else
            {
                var respectService = new RespectService(ElympicsLobbyClient.Instance, ElympicsConfig.Load());
                var respectValue = await respectService.GetRespectForMatch(matchId);

                respectText.text = string.Format(formattedRespectMessage, respectValue.Respect.ToString());
            }
        }
    }
}
