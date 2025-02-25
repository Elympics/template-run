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

        [SerializeField] private GameObject separator;

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
            separator.SetActive(false);

            var placementsCount = placements?.Length;

            // Needed in case there are less than 6 positions sent by server, but the user is last place. 
            int lastPlacementPosition = 0;

            for (int i = 0; i < displayedEntries.Count; i++)
            {
                if (i < placementsCount)
                {
                    displayedEntries[i].SetValues(placements![i]);
                    lastPlacementPosition = placements![i].Position;

                    // Only check if seperator is not active
                    if (!separator.activeSelf)
                    {
                        // Check if index + 1 is within list
                        if (i + 1 < placementsCount)
                        {
                            if (!ArePlacementsConsecutive(placements![i], placements![i + 1])) separator.SetActive(true);
                        }
                    }
                }
                else
                {
                    lastPlacementPosition++;

                    Placement emptyCell = new()
                    {
                        UserId = string.Empty,
                        Position = lastPlacementPosition,
                    };
                    displayedEntries[i].SetValues(emptyCell);
                }
            }
        }

        private static bool ArePlacementsConsecutive(Placement placement, Placement nextPlacement) => nextPlacement.Position - placement.Position == 1;
    }
}
