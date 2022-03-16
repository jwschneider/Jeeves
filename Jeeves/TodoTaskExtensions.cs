using System;
using Microsoft.Graph;
using System.Globalization;
using System.Linq;

namespace Jeeves
{
	public static class TodoTaskExtensions
	{
		// all extensions assume task is not null
		public static string Identity(this TodoTask task) =>
			task.Id;
		public static string Title(this TodoTask task) =>
			task.Title;
		public static string Category(this TodoTask task) =>
			(string)task.extensionProperty("category");
		public static string Location(this TodoTask task) =>
			(string)task.extensionProperty("location");
		public static DateTimeTimeZone ReleaseDate(this TodoTask task) =>
			(DateTimeTimeZone)task.extensionProperty("releaseDate");
		public static TimeSpan ProcessTime(this TodoTask task) =>
			(TimeSpan)task.extensionProperty("processTime");
		public static DateTimeTimeZone DueDate(this TodoTask task) =>
			task.DueDateTime;
		public static DateTimeTimeZone Deadline(this TodoTask task) =>
			(DateTimeTimeZone)task.extensionProperty("deadline");
		public static DateTimeOffset? CreatedTime(this TodoTask task) =>
			task.CreatedDateTime;
		public static DateTimeTimeZone CompletedTime(this TodoTask task) =>
			task.CompletedDateTime;
		public static PatternedRecurrence Recurrence(this TodoTask task) =>
			task.Recurrence;


		// Assumes task has already been checked for good format and no null fields
		public static Job ToScheduleJob(this TodoTask task, RealDateTimeConverter toScheduleTime, RealTimeSpanConverter toScheduleDuration) =>
			new Job
			{
				Identity = task.Identity(),
				ReleaseTime = toScheduleTime(task.ReleaseDate().ToUTC()),
				ProcessTime = toScheduleDuration(task.ProcessTime()),
				DueDate = toScheduleTime(task.DueDate().ToUTC()),
				Deadline = toScheduleTime(task.Deadline().ToUTC()),
				Value = task.Category() switch
                {
					_ => 1  // todo value based on category
                }
			};
		private static void CheckForNullJobField(this TodoTask task)
		{
			if (task.Category() is null)
				throw new ArgumentNullException("'Category'");
			if (task.Location() is null)
				throw new ArgumentNullException("'Location'");
			if (task.ReleaseDate() is null)
				throw new ArgumentNullException("'Release date'");
			if (task.ProcessTime().Equals(TimeSpan.Zero))
				throw new ArgumentNullException("'Process time'");
			if (task.DueDate() is null)
				throw new ArgumentNullException("'Due date'");
			if (task.Deadline() is null)
				throw new ArgumentNullException("'Deadline'");
			if (task.CreatedTime() is null)
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

