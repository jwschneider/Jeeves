using System;

namespace Jeeves
{
	public class UserPreferences
	{
		public TimeZoneInfo TimeZone { get; set; }
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
		public DateTime FromScheduleTime(int time)
        {
			throw new NotImplementedException();
        }
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

		private DateTime workdayStartUTC() =>
			TimeZoneInfo.ConvertTimeToUtc(WorkdayStart, TimeZone);
		private DateTime workdayEndUTC() =>
			TimeZoneInfo.ConvertTimeToUtc(WorkdayEnd, TimeZone);
		private TimeSpan workdayLength() =>
			workdayEndUTC() - workdayStartUTC();
		private int maxOneDayDuration(TimeSpan duration) =>
			Math.Min(1, duration.Days);
		private TimeSpan timeSinceWorkdayStart(DateTime time) =>
			time.TimeOfDay - workdayStartUTC().TimeOfDay;
		private TimeSpan timeOnlyDuration(TimeSpan duration) =>
			new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);
	}
}

