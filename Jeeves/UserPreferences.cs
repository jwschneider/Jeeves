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

		public string TimeZone { get; set; }
		public TimeSpan SchedulingFidelity { get; set; }
		public TimeSpan WorkdayStart { get; set; }
		public TimeSpan WorkdayLength { get; set; }
		public int DaysInInterval { get; set; }
		public TimeSpan RestTime
		{
			get
			{
				return (WorkdayStart - (WorkdayStart + WorkdayLength).mod(day())).mod(day());
			}
		}

		public TimeZoneInfo GetTimeZone() =>
			TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
		// this all only works for intervals of less than 24 hours
		private bool withinWorkInterval(TimeSpan timeLocal) =>
			timeSinceWorkDayStart(timeLocal) <= WorkdayLength;
		private TimeSpan timeSinceWorkDayStart(TimeSpan timeLocal) =>
			(timeLocal - WorkdayStart).mod(new TimeSpan(24, 0, 0));
		private TimeSpan timeUntilWorkdayStart(TimeSpan timeLocal) =>
			(WorkdayStart - timeLocal).mod(new TimeSpan(24, 0, 0));
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

		private int scheduleWorkdayLength() =>
			(int)Math.Round(WorkdayLength / SchedulingFidelity);

		private TimeSpan day() =>
			new TimeSpan(24, 0, 0);

		public UserPreferences()
		{
		}
		// all public methods which reference time assume that time is given in UTC
		public bool WithinWorkInterval(DateTime time, DateTime now) =>
			time >= workIntervalStartUTC(now)
			&& time <= workIntervalStartUTC(now) + (WorkdayLength + RestTime) * (DaysInInterval - 1) + WorkdayLength;


		// all public facing methods of UserPreferences need to take in an argument 'DateTime now'
		// in order to define what the realtime workday is. 
		public int ToScheduleTime(DateTime time, DateTime now) =>
			ToScheduleDuration(time - workIntervalStartUTC(now));

		public DateTime FromScheduleTime(int time, DateTime now) =>
			workIntervalStartUTC(now)
				+ (Math.Min(time / scheduleWorkdayLength(), DaysInInterval-1) * (WorkdayLength + RestTime))
				+ (time / scheduleWorkdayLength() >= DaysInInterval ? 1 : 0) * (WorkdayLength)
				+ (time % scheduleWorkdayLength()) * SchedulingFidelity;

		public int ToScheduleDuration(TimeSpan duration) =>
			Math.Min(
				scheduleWorkdayLength() * (int)Math.Floor(duration / (WorkdayLength + RestTime))
					+ (int)Math.Round(TimeExtensions.min(WorkdayLength, duration.mod(WorkdayLength + RestTime)) / SchedulingFidelity),
				scheduleWorkdayLength() * DaysInInterval);

		public int ValueByCategory(string category, bool isDaily, DateTime created, DateTime? lastCompleted, DateTime now) =>
			isDaily ?
				10 + Math.Min((now - (lastCompleted.HasValue? (DateTime)lastCompleted : DateTime.MinValue)).Days, 10) :
				category switch
				{
					"Sim / Flight" => 100,
					_ => 1 + (now - created).Days
				};



	}
	public static class TimeExtensions
    {
		public static TimeSpan mod(this TimeSpan a, TimeSpan b) =>
			new TimeSpan(a.Ticks - (b.Ticks * (long)Math.Floor((double)a.Ticks / b.Ticks)));
		public static TimeSpan min(this TimeSpan a, TimeSpan b) =>
			new TimeSpan(Math.Min(a.Ticks, b.Ticks));
		public static TimeSpan clamp(this TimeSpan a, TimeSpan minValue, TimeSpan maxValue) =>
			new TimeSpan(Math.Clamp(a.Ticks, minValue.Ticks, maxValue.Ticks));
		public static TimeSpan abs(this TimeSpan time) =>
			new TimeSpan(Math.Abs(time.Ticks));
		public static DateTime ToLocal(this DateTime time, TimeZoneInfo timeZone) =>
			TimeZoneInfo.ConvertTimeFromUtc(time, timeZone);
	}
}

