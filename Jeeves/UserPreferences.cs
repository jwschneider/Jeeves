using System;

namespace Jeeves
{
	public class UserPreferences
	{
		public DateTime WorkdayStart { get; set; }
		public DateTime WorkdayEnd { get; set; }
		public TimeSpan SchedulingFidelity { get; set; }
		public UserPreferences()
		{
		}
		public int ToScheduleTime(DateTime time)
        {
			throw new NotImplementedException();
        }
		public DateTime FromScheduleTime(int time)
        {
			throw new NotImplementedException();
        }
		public int ToScheduleDuration(TimeSpan duration)
        {
			throw new NotImplementedException();
        }
		public int ValueByCategory(string category, bool isDaily, DateTime created, DateTime lastCompleted) =>
			isDaily ?
				10 + (DateTime.Now - lastCompleted).Days :
				category switch
				{
					"Sim / Flight" => 100,
					_ => 1 + (DateTime.Now - created).Days
				};
	}
}

