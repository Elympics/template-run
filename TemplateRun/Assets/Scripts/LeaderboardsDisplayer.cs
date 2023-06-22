using UnityEngine;
using Elympics;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

public class LeaderboardsDisplayer : MonoBehaviour
{
    private const int RecordsToFetch = 3;

    [SerializeField] private CurrentEventDisplayer currentEventDisplayer;
    [SerializeField] private LeaderboardEntryUI[] leaderboardVisualEntries;
    [SerializeField] private int delayMs = 500;

    private LeaderboardClient leaderboardClient;

    private LeaderboardEntry[] storedEntries;
    private LeaderboardEntry currentPlayerEntry;
    public event Action<LeaderboardEntry> OnCurrentPlayerEntrySet;

    public bool FetchAlreadyStarted { get; private set; } = false;

    public void InitializeAndRun()
    {
        FetchAlreadyStarted = true;
        ExternalBackendClient.GetCurrentLeaderboard(HandleRequest);
    }

    private async void HandleRequest(Result<LeaderboardRequestModel, Exception> result)
    {
        if (result.IsFailure)
            return;

        if (currentEventDisplayer != null)
            currentEventDisplayer.TimeTo = DateTime.Parse(result.Value.DateTo);

        var timeScope = new LeaderboardTimeScope(DateTimeOffset.Parse(result.Value.DateFrom), DateTimeOffset.Parse(result.Value.DateTo));
        leaderboardClient = new LeaderboardClient(RecordsToFetch, timeScope, QueueDict.MatchmakingQueueSolo, Enum.Parse<LeaderboardGameVersion>(result.Value.LeaderboardGameVersion));

        storedEntries = new LeaderboardEntry[leaderboardVisualEntries.Length];

        await Task.Delay(delayMs);

        if (ElympicsLobbyClient.Instance.IsAuthenticated)
            FetchTopThree();
        else
            ElympicsLobbyClient.Instance.AuthenticationSucceeded += FetchTopThree;
    }

    private void OnDestroy()
    {
        ElympicsLobbyClient.Instance.AuthenticationSucceeded -= FetchTopThree;
    }

    private void FetchTopThree(Elympics.Models.Authentication.AuthData _ = null) => leaderboardClient.FetchFirstPage(HandleFirstPageFetch, OmitFailure);
    private void FetchSecondTopThree() => leaderboardClient.FetchNextPage(HandleSecondTopThreeFetch, (_) => FetchNicknames());
    private void FetchUserPage() => leaderboardClient.FetchPageWithUser(HandleUserPageFetch, (_) => FetchSecondTopThree());

    private void HandleFirstPageFetch(LeaderboardFetchResult fetchResult)
    {
        StoreEntries(fetchResult.Entries);

        int playerIndexInResult = FindCurrentPlayerInEntries(fetchResult.Entries);


        if (playerIndexInResult != -1)
        {
            FetchSecondTopThree();
            SetCurrentPlayerEntry(fetchResult.Entries[playerIndexInResult]);
        }
        else
            FetchUserPage();
    }

    private void HandleSecondTopThreeFetch(LeaderboardFetchResult fetchResult)
    {
        StoreEntries(fetchResult.Entries, RecordsToFetch);
        FindCurrentPlayerInEntries(fetchResult.Entries, RecordsToFetch);

        FetchNicknames();
    }

    private void HandleUserPageFetch(LeaderboardFetchResult fetchResult)
    {
        int playerIndexInResult = FindCurrentPlayerInEntries(fetchResult.Entries, shouldHighlightCurrentPlayer: false);
        SetCurrentPlayerEntry(fetchResult.Entries[playerIndexInResult]);

        if ((playerIndexInResult > 0 && playerIndexInResult < fetchResult.Entries.Count - 1) || fetchResult.Entries[playerIndexInResult].Position == RecordsToFetch + 1)
        {
            leaderboardVisualEntries[RecordsToFetch + playerIndexInResult].HighlightEntry();
            StoreEntries(fetchResult.Entries, RecordsToFetch);

            FetchNicknames();
        }
        else if (playerIndexInResult == 0)
        {
            leaderboardVisualEntries[RecordsToFetch + 1].HighlightEntry();
            StoreEntries(fetchResult.Entries.GetRange(0, fetchResult.Entries.Count - 1), RecordsToFetch + 1);

            leaderboardClient.FetchPreviousPage(x => HandleMissingNeighbourRecord(x, RecordsToFetch, x.Entries.Count - 1), OmitFailure);
        }
        else
        {
            leaderboardVisualEntries[RecordsToFetch + RecordsToFetch - 2].HighlightEntry();
            StoreEntries(fetchResult.Entries.GetRange(1, fetchResult.Entries.Count - 1), RecordsToFetch);

            leaderboardClient.FetchNextPage(x => HandleMissingNeighbourRecord(x, leaderboardVisualEntries.Length - 1, 0), OmitFailure);
        }
    }

    private void HandleMissingNeighbourRecord(LeaderboardFetchResult fetchResult, int storeIdx, int entryIdx)
    {
        storedEntries[storeIdx] = fetchResult.Entries[entryIdx];

        FetchNicknames();
    }

    private void OmitFailure(LeaderboardFetchError fetchError) { }

    private void StoreEntries(List<LeaderboardEntry> entries, int storeIndexOffset = 0)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            storedEntries[storeIndexOffset + i] = entries[i];
        }
    }

    private int FindCurrentPlayerInEntries(List<LeaderboardEntry> entries, int visualEntriesIndexOffset = 0, bool shouldHighlightCurrentPlayer = true)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (ContainsCurrentUser(entries[i]))
            {
                if (shouldHighlightCurrentPlayer)
                    leaderboardVisualEntries[visualEntriesIndexOffset + i].HighlightEntry(entries[i]);

                return i;
            }
        }

        return -1;
    }

    private void SetCurrentPlayerEntry(LeaderboardEntry value)
    {
        currentPlayerEntry = value;
        OnCurrentPlayerEntrySet?.Invoke(currentPlayerEntry);
    }

    private static bool ContainsCurrentUser(LeaderboardEntry entry)
    {
        var playerId = ElympicsLobbyClient.Instance.UserGuid.ToString();

        return entry != null && entry.UserId.Equals(playerId);
    }

    private void FetchNicknames() => ExternalBackendClient.GetNicknamesFromIds(DisplayEntries, storedEntries.Select(x => x != null ? x.UserId : string.Empty).ToArray());

    private void DisplayEntries(Result<IdNicknamePairs, Exception> result)
    {
        for (int i = 0; i < leaderboardVisualEntries.Length; i++)
        {
            string nickname = null;

            if (result.IsSuccess)
            {
                foreach (var pair in result.Value.Players)
                {
                    if (storedEntries[i] != null && storedEntries[i].UserId.Equals(pair.ElympicsUserId))
                    {
                        nickname = pair.Nickname;
                        break;
                    }
                }
            }

            leaderboardVisualEntries[i].SetValues(storedEntries[i], nickname);
        }
    }
}
