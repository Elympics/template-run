using Elympics;
using System;

public static class ExternalBackendClient
{
	private static readonly string BaseUrl = "https://os-templaterun-api.elympics.cc/api/v1/elympics/";
	private static readonly string GetNicknamePath = "players/get_nickname";
	private static readonly string SetNicknamePath = "players/set_nickname";
	private static readonly string NicknamesFromIdsPath = "players/get_nicknames";
	private static readonly string GetCurrentLeaderboardPropertiesPath = "leaderboard/";

	private static string ElympicsToken => ElympicsLobbyClient.Instance.AuthData?.JwtToken;
	private static string ElympicsAuth => $"Bearer {ElympicsToken}";

	private static BackendRequestModel CreateBackendModel(string nickname = null, string[] elympicsIds = null) =>
		new BackendRequestModel { Nickname = nickname, ElympicsUserIds = elympicsIds };

	public static void GetNickname(Action<Result<IdNicknamePair, Exception>> callback) =>
		ElympicsWebClient.SendPostRequest(string.Concat(BaseUrl, GetNicknamePath), CreateBackendModel(), ElympicsAuth, callback);

	public static void SetNickname(Action<Result<IdNicknamePair, Exception>> callback, string nickname) =>
		ElympicsWebClient.SendPutRequest(string.Concat(BaseUrl, SetNicknamePath), CreateBackendModel(nickname), ElympicsAuth, callback);

	public static void GetNicknamesFromIds(Action<Result<IdNicknamePairs, Exception>> callback, string[] elympicsIds) =>
		ElympicsWebClient.SendPostRequest(string.Concat(BaseUrl, NicknamesFromIdsPath), CreateBackendModel(null, elympicsIds), ElympicsAuth, callback);

	public static void GetCurrentLeaderboard(Action<Result<LeaderboardRequestModel, Exception>> callback) =>
		ElympicsWebClient.SendGetRequest(string.Concat(BaseUrl, GetCurrentLeaderboardPropertiesPath), null, ElympicsAuth, callback);
}
