using System;
using Microsoft.Graph;
using System.Globalization;
using System.Linq;

namespace Jeeves
{
	public static class TodoTaskExtensions
	{
		// all extensions assume task is not null
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

