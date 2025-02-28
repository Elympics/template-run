using ElympicsPlayPad.ExternalCommunicators;
using System;
using ElympicsPlayPad.Leaderboard;
using ElympicsPlayPad.ExternalCommunicators.Leaderboard;

/// <summary>
/// This class helps keep track of the All Time best score of the player and their Tournament Best score to be displayed in the end game screen
/// </summary>
public class ElympicsBestScoreManager
{
    private static IExternalLeaderboardCommunicator _leaderboardCommunicator => PlayPadCommunicator.Instance.LeaderboardCommunicator;
    public static int AllTimeHighScore { get; private set; }
    public static int TournamentHighScore { get; private set; }

    /// <summary>
    /// Method to initialize the class
    /// </summary>
    public static void OnStart()
    {
        PlayPadCommunicator.Instance.ExternalAuthenticator.AuthenticationUpdated += AuthenticationUpdated;

        // Subscribe to callback for user high score & if high score exists already save it
        _leaderboardCommunicator.UserHighScoreUpdated += UserAllTimeHighScoreUpdated;
        if (_leaderboardCommunicator.UserHighScore.HasValue)
        {
            UserAllTimeHighScoreUpdated(_leaderboardCommunicator.UserHighScore.Value);
        }

        // Subscribe to callback for leaderboards & if leaderboards exist save high score for today
        _leaderboardCommunicator.LeaderboardUpdated += LeaderboardUpdated;
        if (_leaderboardCommunicator.Leaderboard.HasValue)
        {
            LeaderboardUpdated(_leaderboardCommunicator.Leaderboard.Value);
        }
    }

    /// <summary>
    /// Check if the current score is the new "Tournament high score". This is needed since the current score is not saved in the backend until the game server closes.
    /// </summary>
    public static bool CheckIsNewTournamentHighScore(int currentScore)
    {
        if (currentScore > TournamentHighScore)
        {
            TournamentHighScore = currentScore;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if the current score is the new high score. This is needed since the current score is not saved in the backend until the game server closes.
    /// </summary>
    public static bool CheckIsNewAllTimeHighScore(int currentScore)
    {
        if (currentScore > AllTimeHighScore)
        {
            AllTimeHighScore = currentScore;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Callback for when authentication is updated (We should update the scores)
    /// </summary>
    private static void AuthenticationUpdated(Elympics.Models.Authentication.AuthData obj)
    {
        if (_leaderboardCommunicator.UserHighScore.HasValue)
        {
            UserAllTimeHighScoreUpdated(_leaderboardCommunicator.UserHighScore.Value);
        }

        if (_leaderboardCommunicator.Leaderboard.HasValue)
        {
            LeaderboardUpdated(_leaderboardCommunicator.Leaderboard.Value);
        }
    }

    /// <summary>
    /// Callback for when leaderboard is updated
    /// </summary>
    private static void LeaderboardUpdated(LeaderboardStatusInfo obj)
    {
        if (obj.UserPlacement.HasValue)
        {
            TournamentHighScore = RoundFloatToInt(obj.UserPlacement.Value.Score);

            // Check if the latest high score is bigger than all time high score and set
            // Should be no use-cases for this but just in case
            if (TournamentHighScore > AllTimeHighScore)
            {
                AllTimeHighScore = TournamentHighScore;
            }
        }
        else
        {
            TournamentHighScore = 0;
        }
    }

    /// <summary>
    /// Callback for user alltime high score updated
    /// </summary>
    private static void UserAllTimeHighScoreUpdated(UserHighScoreInfo obj)
    {
        AllTimeHighScore = RoundFloatToInt(obj.Points);
    }

    #region Utils

    /// <summary>
    /// Since this game uses whole numbers for the score, we will always round the float to the nearest int. <br></br>
    /// Please adapt this to your game as needed.
    /// </summary>
    private static int RoundFloatToInt(float f)
    {
        return (int)Math.Round(f);
    }

    #endregion
}
