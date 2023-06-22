using System.Collections.Generic;

[System.Serializable]
public class BackendRequestModel
{
    public string Nickname;
    public string[] ElympicsUserIds;
}

[System.Serializable]
public class IdNicknamePair
{
    public string ElympicsUserId;
    public string Nickname;

    public override string ToString() => $"{Nickname} - {ElympicsUserId}";
}

[System.Serializable]
public class IdNicknamePairs
{
    public List<IdNicknamePair> Players;
};

[System.Serializable]
public class LeaderboardRequestModel
{
    public string GameId;
    public string GameVersion;
    public string LeaderboardGameVersion;
    public string QueueName;
    public string TimeScope;
    public string DateFrom;
    public string DateTo;
}

