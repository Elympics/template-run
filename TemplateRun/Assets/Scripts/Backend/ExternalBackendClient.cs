using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Elympics;

public static class ExternalBackendClient
{
    private static readonly string BaseUrl = "https://os-templaterun-api.elympics.cc/api/v1/elympics/";
    private static readonly string GetNicknameRoute = "players/get_nickname";
    private static readonly string SetNicknameRoute = "players/set_nickname";
    private static readonly string NicknamesFromIdsRoute = "players/get_nicknames";
    private static readonly string GetCurrentLeaderboardRoute = "leaderboard/";

    private static string ElympicsAuth => $"Bearer {ElympicsLobbyClient.Instance.AuthData?.JwtToken}";

    public static void GetNickname(Action<Result<IdNicknamePair, Exception>> callback) =>
        SendBackendRequest(UnityWebRequest.kHttpVerbPOST, GetNicknameRoute, callback);

    public static void SetNickname(Action<Result<IdNicknamePair, Exception>> callback, string nickname) =>
        SendBackendRequest(UnityWebRequest.kHttpVerbPUT, SetNicknameRoute, callback, new SetNicknameRequestModel(nickname));

    public static void GetNicknamesFromIds(Action<Result<IdNicknamePairs, Exception>> callback, string[] elympicsIds) =>
        SendBackendRequest(UnityWebRequest.kHttpVerbPOST, NicknamesFromIdsRoute, callback, new GetNicknamesRequestModel(elympicsIds));

    public static void GetCurrentLeaderboardProperties(Action<Result<LeaderboardRequestModel, Exception>> callback) =>
        SendBackendRequest(UnityWebRequest.kHttpVerbGET, GetCurrentLeaderboardRoute, callback);

    private static void SendBackendRequest<T>(string method, string route, Action<Result<T, Exception>> callback = null, object jsonBody = null) where T : class
    {
        string url = string.Concat(BaseUrl, route);
        var request = new UnityWebRequest(new Uri(url), method);
        request.downloadHandler = new DownloadHandlerBuffer();

        if (jsonBody != null)
        {
            var bodyString = JsonUtility.ToJson(jsonBody);
            var bodyRaw = Encoding.ASCII.GetBytes(bodyString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
        }

        request.SetRequestHeader("Authorization", ElympicsAuth);
        request.SetRequestHeader("Accept", "application/json");

        var asyncOperation = request.SendWebRequest();
        CallCallbackOnCompleted(asyncOperation, callback);
    }

    private static void CallCallbackOnCompleted<T>(UnityWebRequestAsyncOperation requestOp, Action<Result<T, Exception>> callback) where T : class
    {
        void RunCallback(Result<T, Exception> result)
        {
            callback?.Invoke(result);
            requestOp.webRequest.Dispose();
        }

        requestOp.completed += _ =>
        {
            if (requestOp.webRequest.responseCode != 200)
            {
                RunCallback(Result<T, Exception>.Failure(new ElympicsException($"{requestOp.webRequest.responseCode} - {requestOp.webRequest.error}\n{requestOp.webRequest.downloadHandler.text}")));
                return;
            }

            T response;
            try
            {
                response = JsonUtility.FromJson<T>(requestOp.webRequest.downloadHandler.text);
            }
            catch (Exception e)
            {
                RunCallback(Result<T, Exception>.Failure(new ElympicsException($"{requestOp.webRequest.responseCode} - {e.Message}\n{requestOp.webRequest.downloadHandler.text}\n{e}")));
                return;
            }
            RunCallback(Result<T, Exception>.Success(response));
        };
    }
}
