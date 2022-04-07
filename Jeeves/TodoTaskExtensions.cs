using System;
using Microsoft.Graph;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Jeeves
{
	public static class TodoTaskExtensions
	{
		private static TodoTask Clone(this TodoTask task)
		{
			string json = JsonConvert.SerializeObject(task, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			return JsonConvert.DeserializeObject<TodoTask>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
		}
		private static TodoTask SetExtensionProperty(this TodoTask task, string property, object value)
		{
			task.taskProperties().AdditionalData[property] = value;
			return task;
		}
		// copies the task and changes the release date
		public static TodoTask SetReleaseDate(this TodoTask task, DateTime releaseDate) =>
			task.Clone().SetExtensionProperty("releaseDate", releaseDate.FromUTC());
		public static TodoTask SetDueDate(this TodoTask task, DateTime dueDate)
		{
			TodoTask copy = task.Clone();
			copy.DueDateTime = dueDate.FromUTC();
			return copy;
		}
		public static TodoTask SetDeadline(this TodoTask task, DateTime deadline) =>
			task.Clone().SetExtensionProperty("deadline", deadline.FromUTC());

		public static TodoTask IncrementByInterval(this TodoTask task, TimeSpan interval) =>
			task.SetReleaseDate(task.ReleaseDate() + interval)
				.SetDueDate(task.DueDate() + interval)
				.SetDeadline(task.Deadline() + interval);

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
			TimeSpan.Parse((string)task.extensionProperty("processTime"));
		public static DateTime DueDate(this TodoTask task) =>
			task.DueDateTime.ToUTC();
		public static DateTime Deadline(this TodoTask task) =>
			((DateTimeTimeZone)task.extensionProperty("deadline")).ToUTC();
		public static DateTime CreatedTime(this TodoTask task) =>
			task.CreatedDateTime.HasValue ?
				((DateTimeOffset)task.CreatedDateTime).ToUniversalTime().DateTime :
				DateTime.MinValue;
		public static DateTime? CompletedTime(this TodoTask task) =>
			task.CompletedDateTime is null ? null :
				task.CompletedDateTime.ToUTC();
		public static PatternedRecurrence Recurrence(this TodoTask task) =>
			task.Recurrence;
		public static bool IsDaily(this TodoTask task) =>
			task.Recurrence() != null;
		public static TimeSpan RecurrenceInterval(this TodoTask task)
        {
			if (task.Recurrence().Pattern.Type != RecurrencePatternType.Daily)
				throw new NotImplementedException($"Recurrence Pattern {task.Recurrence().Pattern.Type} not yet supported.");
			int? interval = task.Recurrence().Pattern.Interval;
			if (!interval.HasValue)
				throw new ArgumentException($"Daily recurrance requires interval.");
			return (int)interval * new TimeSpan(24, 0, 0);
        }

		// Assumes task has already been checked for good format and no null fields
		// todo returns an enumerable of Jobs because of repeatable tasks
		public static Job ToScheduleJob(this TodoTask task, UserPreferences preferences, DateTime now) =>
			new Job
			{
				Identity = task.Identity(),
				ReleaseTime = preferences.ToScheduleTime(task.ReleaseDate(), now),
				ProcessTime = preferences.ToScheduleDuration(task.ProcessTime()),
				DueDate = preferences.ToScheduleTime(task.DueDate(), now),
				Deadline = preferences.ToScheduleTime(task.Deadline(), now),
				Value = preferences.ValueByCategory(task.Category(), task.IsDaily(), task.CreatedTime(), task.CompletedTime(), now)
			};

		public static IEnumerable<Job> ToScheduleJobs(this TodoTask task, UserPreferences preferences, DateTime now) =>
			task.RepeatWithinWorkWindow(preferences, now)
				.Select(task => task.ToScheduleJob(preferences, now));

        public static IEnumerable<TodoTask> RepeatWithinWorkWindow(this TodoTask task, UserPreferences preferences, DateTime now)
        {
            if (!preferences.WithinWorkWindow(task.ReleaseDate(), now)) return new List<TodoTask>();
            else return RepeatWithinWorkWindow(
                task.IncrementByInterval(task.RecurrenceInterval()), preferences, now).Append(task);
        }

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
		public static DateTimeTimeZone FromUTC(this DateTime utcTime) =>
			new DateTimeTimeZone { DateTime = utcTime.ToString(), TimeZone = "UTC" };


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

