using System;

namespace Jeeves
{
	public class UserPreferences
	{
		public string TimeZone { get; set; }
		public DateTime WorkdayStart { get; set; }
		public DateTime WorkdayEnd { get; set; }
		public TimeSpan SchedulingFidelity { get; set; }
		public UserPreferences()
		{
		}

		public int ToScheduleTime(DateTime time) =>
			(int)Math.Round(
				(timeSinceWorkdayStart(time)
					+ maxOneDayDuration(time - DateTime.Today) * workdayLength())
				/ SchedulingFidelity);
		public DateTime FromScheduleTime(int time) =>
			DateTime.Today + scheduleTimeDaysOnly(time) + scheduleTimeTimeOnly(time) + workdayStartUTC().TimeOfDay;
		public int ToScheduleDuration(TimeSpan duration) =>
			(int)Math.Round(
				(maxOneDayDuration(duration) * workdayLength() + timeOnlyDuration(duration))
				/ SchedulingFidelity
				);
		public int ValueByCategory(string category, bool isDaily, DateTime created, DateTime lastCompleted) =>
			isDaily ?
				10 + (DateTime.Now - lastCompleted).Days :
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
			time.TimeOfDay - workdayStartUTC().TimeOfDay;
		private TimeSpan timeOnlyDuration(TimeSpan duration) =>
			new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);
	}
}

