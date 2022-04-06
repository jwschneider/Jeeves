using System;
using Newtonsoft.Json;

namespace Jeeves
{
	public class UserPreferences
	{
		public static UserPreferences UserPrefsFromFile(string filename)
		{
			string json = System.IO.File.ReadAllText(filename);
			return JsonConvert.DeserializeObject<UserPreferences>(json);
		}

		private string timeZone { get; set; }
		private TimeSpan schedulingFidelity { get; set; }
		private TimeSpan workdayStart { get; set; }
		private TimeSpan workdayLength { get; set; }

		public TimeZoneInfo GetTimeZone() =>
			TimeZoneInfo.FindSystemTimeZoneById(timeZone);
		// this all only works for intervals of less than 24 hours
		private bool withinWorkInterval(TimeSpan timeLocal) =>
			timeSinceWorkIntervalStart(timeLocal) <= workdayLength;
		private TimeSpan timeSinceWorkIntervalStart(TimeSpan timeLocal) =>
			(timeLocal - workdayStart).modTimeSpan(new TimeSpan(24, 0, 0));
		private TimeSpan timeUntilWorkIntervalStart(TimeSpan timeLocal) =>
			(workdayStart - timeLocal).modTimeSpan(new TimeSpan(24, 0, 0));
		// If 'now' is in a workday, returns the date and time of the start of that workday
		// If 'now' is outside of a workday, returns the date and time of the start of the next workday
		// argument 'now' is in UTC
		private DateTime workdayStartUTC(DateTime now) {
			DateTime nowLocal = TimeZoneInfo.ConvertTimeFromUtc(now, GetTimeZone());
			DateTime workdayStartLocal = withinWorkInterval(nowLocal.TimeOfDay) ?
				nowLocal - timeSinceWorkIntervalStart(nowLocal.TimeOfDay) :
				nowLocal + timeUntilWorkIntervalStart(nowLocal.TimeOfDay);
			return TimeZoneInfo.ConvertTimeToUtc(workdayStartLocal, GetTimeZone());
		}

		private DateTime workdayEndUTC(DateTime now) =>
			workdayStartUTC(now) + workdayLength;

		public UserPreferences()
		{
		}
		// all public methods which reference time assume that time is given in UTC
		public bool WithinWorkWindow(DateTime time) =>
			throw new NotImplementedException();


		// todo all public facing methods of UserPreferences need to take in an argument 'DateTime now'
		// in order to define what the realtime workday is. 
		public int ToScheduleTime(DateTime time) =>
			(int)Math.Round(
				(timeSinceWorkdayStart(time).modTimeSpan(day()).clampTimeSpan(TimeSpan.Zero, workdayLength())
					+ maxOneDayDuration(timeSinceWorkdayStart(time)) * workdayLength())
				/ SchedulingFidelity);
		public DateTime FromScheduleTime(int time) =>
			DateTime.Today + scheduleTimeDaysOnly(time) + scheduleTimeTimeOnly(time) + workdayStartUTC().TimeOfDay;
		public int ToScheduleDuration(TimeSpan duration) =>
			(int)Math.Round(
				(maxOneDayDuration(duration) * workdayLength() + timeOnlyDuration(duration))
				/ SchedulingFidelity
				);
		public int ValueByCategory(string category, bool isDaily, DateTime created, DateTime? lastCompleted) =>
			isDaily ?
				10 + Math.Min((DateTime.Now - (lastCompleted.HasValue? (DateTime)lastCompleted : DateTime.MinValue)).Days, 10) :
				category switch
				{
					"Sim / Flight" => 100,
					_ => 1 + (DateTime.Now - created).Days
				};


		private int scheduleWorkdayLength() =>
			(int)Math.Round(workdayLength() / SchedulingFidelity);

		private TimeSpan scheduleTimeDaysOnly(int time) =>
			new TimeSpan(time / scheduleWorkdayLength(), 0, 0, 0);

		private TimeSpan scheduleTimeTimeOnly(int time) =>
			(time % scheduleWorkdayLength()) * SchedulingFidelity;

		private int maxOneDayDuration(TimeSpan duration) =>
			Math.Min(1, duration.Days);

		private TimeSpan timeSinceWorkdayStart(DateTime time) =>
			time - workdayStartUTC();

		private TimeSpan timeOnlyDuration(TimeSpan duration) =>
			new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);

		private TimeSpan day() =>
			new TimeSpan(24, 0, 0);

	}
	public static class TimeExtensions
    {
		public static TimeSpan modTimeSpan(this TimeSpan a, TimeSpan b) =>
			new TimeSpan(a.Ticks % b.Ticks);
		public static TimeSpan clampTimeSpan(this TimeSpan a, TimeSpan minValue, TimeSpan maxValue) =>
			new TimeSpan(Math.Clamp(a.Ticks, minValue.Ticks, maxValue.Ticks));
	}
}

