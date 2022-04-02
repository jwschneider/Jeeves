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
		public DateTime WorkdayStart
		{
			get { return new DateTime(DateTime.Today.Ticks, DateTimeKind.Unspecified) + _workdayStart.TimeOfDay; }
			set { _workdayStart = value; }
		}
		public DateTime WorkdayEnd
		{
			get { return new DateTime(DateTime.Today.Ticks, DateTimeKind.Unspecified) + _workdayEnd.TimeOfDay; }
			set { _workdayEnd = value; }
		}
		public TimeSpan SchedulingFidelity { get; set; }

		private DateTime _workdayStart;
		private DateTime _workdayEnd;

		public UserPreferences()
		{
		}

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
		public TimeZoneInfo GetTimeZone() =>
			TimeZoneInfo.FindSystemTimeZoneById(TimeZone);

		private DateTime workdayStartUTC() =>
			TimeZoneInfo.ConvertTimeToUtc(WorkdayStart, GetTimeZone());

		private DateTime workdayEndUTC() =>
			TimeZoneInfo.ConvertTimeToUtc(WorkdayEnd, GetTimeZone());

		private TimeSpan workdayLength() =>
			workdayEndUTC() - workdayStartUTC();

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

