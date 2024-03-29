using UnityEngine;
using Elympics;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

public class LeaderboardsDisplayer : MonoBehaviour
{
    private static readonly int RecordsToFetch = 3;

    [SerializeField] private CurrentEventDisplayer currentEventDisplayer;
    [SerializeField] private LeaderboardEntryUI[] leaderboardVisualEntries;
    [SerializeField] private int fetchDelayMs = 500;

    private LeaderboardClient leaderboardClient;

    private LeaderboardEntry[] storedEntries;
    private LeaderboardEntry currentPlayerEntry;

    public event Action<LeaderboardEntry> OnCurrentPlayerEntrySet;
    public bool FetchAlreadyStarted { get; private set; } = false;

    public void InitializeAndRun()
    {
        if (ElympicsLobbyClient.Instance == null)
        {
            Debug.LogWarning("Leaderboards won't work unless you start from the menu scene.");
            return;
        }

        FetchAlreadyStarted = true;
        ExternalBackendClient.GetCurrentLeaderboardProperties(HandleLeaderboardRequest);
    }

    private async void HandleLeaderboardRequest(Result<LeaderboardRequestModel, Exception> result)
    {
        if (result.IsFailure)
            return;

        storedEntries = new LeaderboardEntry[leaderboardVisualEntries.Length];

        if (currentEventDisplayer != null)
            currentEventDisplayer.TimeTo = DateTime.Parse(result.Value.DateTo);

        var timeScope = new LeaderboardTimeScope(DateTimeOffset.Parse(result.Value.DateFrom), DateTimeOffset.Parse(result.Value.DateTo));
        leaderboardClient = new LeaderboardClient(RecordsToFetch, timeScope, result.Value.QueueName, Enum.Parse<LeaderboardGameVersion>(result.Value.LeaderboardGameVersion));

        await Task.Delay(fetchDelayMs);

        if (ElympicsLobbyClient.Instance.IsAuthenticated)
            FetchTopThree();
        else
            ElympicsLobbyClient.Instance.AuthenticationSucceeded += FetchTopThree;
    }

    private void OnDestroy()
    {
        if (ElympicsLobbyClient.Instance == null)
            return;

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
                    leaderboardVisualEntries[visualEntriesIndexOffset + i].HighlightEntry(entries[i].Position <= 3);

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
        if (result.IsFailure)
            return;

        for (int i = 0; i < leaderboardVisualEntries.Length; i++)
        {
            if (storedEntries[i] == null)
                return;

            string nickname = result.Value.Players.Where(pair => storedEntries[i].UserId.Equals(pair.ElympicsUserId)).FirstOrDefault()?.Nickname;

            leaderboardVisualEntries[i].SetValues(storedEntries[i], nickname);
        }
    }
}
