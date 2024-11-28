using System.Collections.Generic;
using UnityEngine;
using ElympicsPlayPad.Leaderboard;
using ElympicsPlayPad.ExternalCommunicators;
using ElympicsPlayPad.ExternalCommunicators.Leaderboard;
using TMPro;

#nullable enable

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class LeaderboardView : MonoBehaviour
    {
        // Should reference exactly 6 entries
        [SerializeField] private List<LeaderboardEntryUi> displayedEntries;

        [SerializeField] private TextMeshProUGUI numberOfParticipants;

        private IExternalLeaderboardCommunicator LeaderboardCommunicator => PlayPadCommunicator.Instance!.LeaderboardCommunicator!;

        public void OnStart()
        {
            LeaderboardCommunicator.LeaderboardUpdated += UpdateLeaderboardView;

            UpdateLeaderboardView(LeaderboardCommunicator.Leaderboard ?? default);
        }

        private void OnDestroy()
        {
            LeaderboardCommunicator.LeaderboardUpdated -= UpdateLeaderboardView;
        }

        private void UpdateLeaderboardView(LeaderboardStatusInfo info)
        {
            DisplayLeaderboardEntries(info.Placements);
            numberOfParticipants.text = info.Participants.ToString();
        }

        private void DisplayLeaderboardEntries(Placement[]? placements)
        {
            var placementsCount = placements?.Length;

            for (int i = 0; i < displayedEntries.Count; i++)
            {
                if (i < placementsCount)
                {
                    displayedEntries[i].SetValues(placements![i]);
                    displayedEntries[i].SetEntrySeparator(!IsLastAvailablePlacement(i, placementsCount) && !ArePlacementsConsecutive(placements[i], placements[i + 1]));
                }
                else
                    displayedEntries[i].Clear();
            }
        }

        private static bool ArePlacementsConsecutive(Placement placement, Placement nextPlacement) => nextPlacement.Position - placement.Position == 1;
        private static bool IsLastAvailablePlacement(int i, int? placementsCount) => placementsCount - i == 1;
    }
}
