using System;
using Microsoft.Graph;
using System.Globalization;
using System.Linq;

namespace Jeeves
{
	public static class TodoTaskExtensions
	{
		// all extensions assume task is not null
		// all extensions return UTC
		public static string Identity(this TodoTask task) =>
			task.Id;
		public static string Title(this TodoTask task) =>
			task.Title;
		public static string Category(this TodoTask task) =>
			(string)task.extensionProperty("category");
		public static string Location(this TodoTask task) =>
			(string)task.extensionProperty("location");
		public static DateTime ReleaseDate(this TodoTask task) =>
			((DateTimeTimeZone)task.extensionProperty("releaseDate")).ToUTC();
		public static TimeSpan ProcessTime(this TodoTask task) =>
			(TimeSpan)task.extensionProperty("processTime");
		public static DateTime DueDate(this TodoTask task) =>
			task.DueDateTime.ToUTC();
		public static DateTime Deadline(this TodoTask task) =>
			((DateTimeTimeZone)task.extensionProperty("deadline")).ToUTC();
		public static DateTime CreatedTime(this TodoTask task) =>
			task.CreatedDateTime.HasValue ?
				((DateTimeOffset)task.CreatedDateTime).ToUniversalTime().DateTime :
				DateTime.MinValue;
		public static DateTime CompletedTime(this TodoTask task) =>
			task.CompletedDateTime.ToUTC();
		public static PatternedRecurrence Recurrence(this TodoTask task) =>
			task.Recurrence;
		public static bool IsDaily(this TodoTask task) =>
			task.Recurrence() != null;

		// Assumes task has already been checked for good format and no null fields
		public static Job ToScheduleJob(this TodoTask task, UserPreferences preferences) =>
			new Job
			{
				Identity = task.Identity(),
				ReleaseTime = preferences.ToScheduleTime(task.ReleaseDate()),
				ProcessTime = preferences.ToScheduleDuration(task.ProcessTime()),
				DueDate = preferences.ToScheduleTime(task.DueDate()),
				Deadline = preferences.ToScheduleTime(task.Deadline()),
				Value = preferences.ValueByCategory(task.Category(), task.IsDaily(), task.CreatedTime(), task.CompletedTime())
			};
		private static void CheckForNullJobField(this TodoTask task)
		{
			if (task.Category() is null)
				throw new ArgumentNullException("'Category'");
			if (task.Location() is null)
				throw new ArgumentNullException("'Location'");
			if (DateTime.Equals(task.ReleaseDate(), DateTime.MinValue))
				throw new ArgumentNullException("'Release date'");
			if (task.ProcessTime().Equals(TimeSpan.Zero))
				throw new ArgumentNullException("'Process time'");
			if (DateTime.Equals(task.DueDate(), DateTime.MinValue))
				throw new ArgumentNullException("'Due date'");
			if (DateTime.Equals(task.Deadline(), DateTime.MinValue))
				throw new ArgumentNullException("'Deadline'");
			if (DateTime.Equals(task.CreatedTime(), DateTime.MinValue))
				throw new ArgumentNullException("'Created time'");
		}

		public static DateTime ToUTC(this DateTimeTimeZone time) =>
			TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(time.DateTime), TimeZoneInfo.FindSystemTimeZoneById(time.TimeZone));


		public delegate int RealDateTimeConverter(DateTime realTime);
		public delegate int RealTimeSpanConverter(TimeSpan realTime);
		public delegate DateTime ScheduleTimeConverter(int scheduleTime);

		private static OpenTypeExtension taskProperties(this TodoTask task) =>
			task.Extensions.CurrentPage
				.Select(extension => (OpenTypeExtension)extension)
				.Where(extension => String.Equals(extension.ExtensionName, "taskProperties"))
				.FirstOrDefault();
		private static object extensionProperty(this TodoTask task, string property)
		{
			object ret;
			return task.taskProperties().AdditionalData.TryGetValue(property, out ret) ?
				ret :
				null;
		}
	}
}

