using System;

namespace ElympicsPlayPad.Samples.AsyncGame
{
    public class TournamentTimer
    {
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

        public string GetTimer()
        {
            var timerState = GetTimerState();
            var timeSpan = GetTimeDifference(timerState);
            var timerFormat = GetTimeFormat(timerState);

            if (timeSpan.TotalDays >= 1)
                return string.Format(timerFormat, (int)timeSpan.TotalDays, "days");
            else if (timeSpan.TotalHours >= 1)
                return string.Format(timerFormat, (int)timeSpan.TotalHours, "hours");
            else if (timeSpan.TotalMinutes >= 1)
                return string.Format(timerFormat, (int)timeSpan.TotalMinutes, "minutes");
            else
                return string.Format(timerFormat, (int)timeSpan.TotalSeconds, "seconds");
        }

        private TimerState GetTimerState() =>
            IsTournamentUpcoming ? TimerState.Upcoming : IsTournamentFinished ? TimerState.Finished : TimerState.Ongoing;

        private TimeSpan GetTimeDifference(TimerState timerState) => timerState switch
        {
            TimerState.Upcoming => _startDate - DateTimeOffset.Now,
            TimerState.Ongoing => _endDate - DateTimeOffset.Now,
            TimerState.Finished => DateTimeOffset.Now - _endDate
        };

        private string GetTimeFormat(TimerState timerState) => timerState switch
        {
            TimerState.Upcoming => "Starts in: {0} {1}",
            TimerState.Ongoing => "Time left: {0} {1}",
            TimerState.Finished => "Ended {0} {1} ago"
        };

        private enum TimerState
        {
            Upcoming,
            Ongoing,
            Finished
        }
    }
}
