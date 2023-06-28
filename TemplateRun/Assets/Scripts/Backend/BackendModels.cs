using System.Collections.Generic;

[System.Serializable]
public class SetNicknameRequestModel
{
    public string Nickname;

    public SetNicknameRequestModel(string nickname) => Nickname = nickname;
}

[System.Serializable]
public class GetNicknamesRequestModel
{
    public string[] ElympicsUserIds;

    public GetNicknamesRequestModel(string[] elympicsUserIds) => ElympicsUserIds = elympicsUserIds;
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
    public string LeaderboardGameVersion;
    public string QueueName;
    public string TimeScope;
    public string DateFrom;
    public string DateTo;
}
