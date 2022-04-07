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
		private int daysInInterval { get; set; }
		private TimeSpan restTime
		{
			get
			{
				return (workdayStart - (workdayStart + workdayLength).mod(day())).mod(day());
			}
		}

		public TimeZoneInfo GetTimeZone() =>
			TimeZoneInfo.FindSystemTimeZoneById(timeZone);
		// this all only works for intervals of less than 24 hours
		private bool withinWorkInterval(TimeSpan timeLocal) =>
			timeSinceWorkDayStart(timeLocal) <= workdayLength;
		private TimeSpan timeSinceWorkDayStart(TimeSpan timeLocal) =>
			(timeLocal - workdayStart).mod(new TimeSpan(24, 0, 0));
		private TimeSpan timeUntilWorkdayStart(TimeSpan timeLocal) =>
			(workdayStart - timeLocal).mod(new TimeSpan(24, 0, 0));
		// If 'now' is in a workday, returns the date and time of the start of that workday
		// If 'now' is outside of a workday, returns the date and time of the start of the next workday
		// argument 'now' is in UTC
		private DateTime workIntervalStartUTC(DateTime now) {
			DateTime nowLocal = TimeZoneInfo.ConvertTimeFromUtc(now, GetTimeZone());
			DateTime workdayStartLocal = withinWorkInterval(nowLocal.TimeOfDay) ?
				nowLocal - timeSinceWorkDayStart(nowLocal.TimeOfDay) :
				nowLocal + timeUntilWorkdayStart(nowLocal.TimeOfDay);
			return TimeZoneInfo.ConvertTimeToUtc(workdayStartLocal, GetTimeZone());
		}





		public UserPreferences()
		{
		}
		// all public methods which reference time assume that time is given in UTC
		public bool WithinWorkWindow(DateTime time) =>
			throw new NotImplementedException();


		// all public facing methods of UserPreferences need to take in an argument 'DateTime now'
		// in order to define what the realtime workday is. 
		public int ToScheduleTime(DateTime time, DateTime now) =>
			ToScheduleDuration(time - workIntervalStartUTC(now));

		public DateTime FromScheduleTime(int time, DateTime now) =>
			workIntervalStartUTC(now)
				+ ((time / scheduleWorkdayLength()) * (workdayLength + restTime))
				+ (time % scheduleWorkdayLength()) * schedulingFidelity;

		public int ToScheduleDuration(TimeSpan duration) =>
			scheduleWorkdayLength() * Math.Min(daysInInterval, (int)Math.Floor(duration / (workdayLength + restTime)))
				+ (int)Math.Round(TimeExtensions.min(workdayLength, duration.mod(workdayLength + restTime)) / schedulingFidelity);

		//todo
		public int ValueByCategory(string category, bool isDaily, DateTime created, DateTime? lastCompleted) =>
			isDaily ?
				10 + Math.Min((DateTime.Now - (lastCompleted.HasValue? (DateTime)lastCompleted : DateTime.MinValue)).Days, 10) :
				category switch
				{
					"Sim / Flight" => 100,
					_ => 1 + (DateTime.Now - created).Days
				};

		private int scheduleWorkdayLength() =>
			(int)Math.Round(workdayLength / schedulingFidelity);

		private TimeSpan day() =>
			new TimeSpan(24, 0, 0);

	}
	public static class TimeExtensions
    {
		public static TimeSpan mod(this TimeSpan a, TimeSpan b) =>
			new TimeSpan(a.Ticks % b.Ticks);
		public static TimeSpan min(this TimeSpan a, TimeSpan b) =>
			new TimeSpan(Math.Min(a.Ticks, b.Ticks));
		public static TimeSpan clamp(this TimeSpan a, TimeSpan minValue, TimeSpan maxValue) =>
			new TimeSpan(Math.Clamp(a.Ticks, minValue.Ticks, maxValue.Ticks));
	}
}

