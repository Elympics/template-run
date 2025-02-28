using System;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class TournamentTimer
    {
        private const string UpcomingLabelText = "Starts in";
        private const string EndedLabelText = "Ended";
        private const string OngoingLabelText = "Time left";

        private readonly DateTimeOffset _startDate;
        private readonly DateTimeOffset _endDate;

        public bool IsTournamentUpcoming => _startDate > DateTimeOffset.Now;
        public bool IsTournamentFinished => DateTimeOffset.Now > _endDate;
        public bool IsTournamentOngoing => !IsTournamentUpcoming && !IsTournamentFinished;

        public TournamentTimer(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }

        public (string timer, string label) GetTimerAndLabel()
        {
            var timerState = GetTimerState();
            var timeSpan = GetTimeDifference(timerState);
            var formattedTimer = FormatTimer(timeSpan);

            return timerState switch
            {
                TimerState.Upcoming => (formattedTimer, UpcomingLabelText),
                TimerState.Finished => ("---", EndedLabelText),
                TimerState.Ongoing => (formattedTimer, OngoingLabelText),
                _ => (null, null),
            };
        }

        private TimerState GetTimerState() =>
            IsTournamentUpcoming ? TimerState.Upcoming : IsTournamentFinished ? TimerState.Finished : TimerState.Ongoing;

        private TimeSpan GetTimeDifference(TimerState timerState) => timerState switch
        {
            TimerState.Upcoming => _startDate - DateTimeOffset.Now,
            TimerState.Ongoing => _endDate - DateTimeOffset.Now,
            TimerState.Finished => DateTimeOffset.Now - _endDate
        };

        /// <summary>
        /// Checks remaining time and returns correct format
        /// </summary>
        private string FormatTimer(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{DoubleToString(timeSpan.Days)}d {DoubleToString(timeSpan.Hours)}h";
            else if (timeSpan.TotalHours >= 1)
                return $"{DoubleToString(timeSpan.Hours)}h {DoubleToString(timeSpan.Minutes)}m";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{DoubleToString(timeSpan.Minutes)}m {DoubleToString(timeSpan.Seconds)}s";
            else
                return $"{DoubleToString(timeSpan.Seconds)}s";
        }

        private string DoubleToString(double d) => d.ToString("F0");

        private enum TimerState
        {
            Upcoming,
            Ongoing,
            Finished
        }
    }
}
