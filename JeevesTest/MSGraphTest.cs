using System;
using Jeeves;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;


namespace JeevesTest
{

	[TestClass]
	public class TodoTaskExtensionsTest
	{
		private JObject GetSampleTaskLists() =>
			JObject.Parse(System.IO.File.ReadAllText("sampleTasks.json"));
		private JObject GetSampleJobs() =>
			JObject.Parse(System.IO.File.ReadAllText("sampleJobs.json"));

		private IEnumerable<TodoTask> GetSampleTasks(JObject sampleTaskLists, string displayName) =>
			sampleTaskLists["taskLists"].Children()
				.Where(list => String.Equals(list["displayName"].ToObject<string>(), displayName))
				.FirstOrDefault()["tasks"]
				.Select(task => task.ToObject<TodoTask>(new JsonSerializer
				{
					TypeNameHandling = TypeNameHandling.Auto
				}));

		private UserPreferences GetSampleUserPreferences() =>
			UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");
		private DateTime SampleTimeNow() =>
			new DateTime(2022, 03, 10, 18, 0, 0);
		private TimeZoneInfo SampleDataTimeZone() =>
			GetSampleUserPreferences().GetTimeZone();
		private (UserPreferences, DateTime) PreferencesAndTime() =>
			(GetSampleUserPreferences(), SampleTimeNow());

		private Func<TodoTask, bool> TaskIsNamed(string name) =>
			(TodoTask task) => String.Equals(task.Title(), name);

		private TodoTask GetTaskByName(string list, string name) =>
			GetSampleTasks(GetSampleTaskLists(), list)
				.Where(TaskIsNamed(name))
				.FirstOrDefault();

		private Job GetJobByIdentity(string identity) =>
			GetSampleJobs()["jobs"]
				.Children()
				.Where(job => String.Equals(job["identity"].ToObject<string>(), identity))
				.Select(job => job.ToObject<Job>())
				.FirstOrDefault();


		public void InitGraphWithSampleData()
		{


		}

		


		[TestMethod]
		[TestCategory("SampleDataRetrieval")]
		public void DeserializeTask_SampleTaskTest_IsNotNull()
		{
			string json = System.IO.File.ReadAllText("sampleTaskTest.json");
			TodoTask task = JsonConvert.DeserializeObject<TodoTask>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Assert.IsNotNull(task);
		}
		[TestMethod]
		[TestCategory("SampleDataRetrieval")]
		public void SerializeTask_InlineTaskWithExtensions_IsNotNull()
		{
			TodoTask task = new TodoTask
			{
				Extensions = new TodoTaskExtensionsCollectionPage()
				{
					new OpenTypeExtension
					{
						ExtensionName = "testExtension",
						AdditionalData = new Dictionary<string, object>()
						{
							{"extensionProperty1", "property1value" },
							{"extensionProperty2", "property2value" },
							{"extensionDateTimeTimeZone", new DateTimeTimeZone
								{
									DateTime = "2022-03-10T23:00:00",
									TimeZone = "America/Chicago"
								}
							}
						}
					}

				}
			};
			string json = JsonConvert.SerializeObject(task, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Assert.IsNotNull(json);
		}

		[TestMethod]
		[TestCategory("SampleDataRetrieval")]
		public void GetSampleTasks_ThePool_ContainsSampleChore1()
		{
			IEnumerable<TodoTask> pool = GetSampleTasks(GetSampleTaskLists(), "The Pool");
			var containsSampleChore1 = pool.Where(TaskIsNamed("SampleChore1")).Any();

			Assert.IsTrue(containsSampleChore1);
		}

		[TestMethod]
		[TestCategory("SampleDataRetrieval")]
		public void GetTaskByName_ThePoolSampleChore1_ReturnsNotNull()
        {
			TodoTask chore1 = GetTaskByName("The Pool", "SampleChore1");

			Assert.IsNotNull(chore1);
		}

		[TestMethod]
		[TestCategory("TodoTaskExtensions")]
		public void Deadline_SampleChore1_WorkdayEndToday()
		{
			TodoTask chore1 = GetTaskByName("The Pool", "SampleChore1");

			DateTime actual = chore1.Deadline();

			DateTime expectedLocal = new DateTime(2022, 03, 10, 23, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, actual);
		}

		[TestMethod]
		[TestCategory("TodoTaskExtensions")]
		public void CreatedTime_SampleChore1_Chore1CreatedTime()
		{
			TodoTask chore1 = GetTaskByName("The Pool", "SampleChore1");

			DateTime actual = chore1.CreatedTime();

			DateTime expectedLocal = new DateTime(2022, 3, 3, 12, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, actual);
		}

		[TestMethod]
		[TestCategory("TodoTaskExtensions")]
		public void Recurrance_SampleDaily1_DailyRecurrance()
		{
			TodoTask chore1 = GetTaskByName("Daily", "SampleDaily1");

			PatternedRecurrence actual = chore1.Recurrence();

			Assert.AreEqual(RecurrencePatternType.Daily, actual.Pattern.Type);
			Assert.AreEqual(1, actual.Pattern.Interval);
		}

		[TestMethod]
		[TestCategory("TodoTaskExtensions")]
		public void Recurrance_SampleDaily1_1DayInterval()
		{
			TodoTask chore1 = GetTaskByName("Daily", "SampleDaily1");

			PatternedRecurrence actual = chore1.Recurrence();

			Assert.AreEqual(1, actual.Pattern.Interval);
		}

		[TestMethod]
		public void SetReleasedate_SampleDaily1_SetsTomorrow7am()
        {
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");

			DateTime newReleaseDate = daily1.ReleaseDate() + daily1.RecurrenceInterval();
			daily1 = daily1.SetReleaseDate(newReleaseDate);

			DateTime expectedLocal = new DateTime(2022, 3, 11, 7, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, daily1.ReleaseDate());
        }

		[TestMethod]
		public void SetDueDate_SampleDaily1_SetsTomorrowNoon()
		{
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");

			DateTime newDueDate = daily1.DueDate() + daily1.RecurrenceInterval();
			daily1 = daily1.SetDueDate(newDueDate);

			DateTime expectedLocal = new DateTime(2022, 3, 11, 12, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, daily1.DueDate());
		}

		[TestMethod]
		public void SetDeadline_SampleDaily1_SetsTomorrowWorkdayEnd()
		{
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");

			DateTime newDeadline = daily1.Deadline() + daily1.RecurrenceInterval();
			daily1 = daily1.SetDeadline(newDeadline);

			DateTime expectedLocal = new DateTime(2022, 3, 11, 23, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, daily1.Deadline());
		}

		[TestMethod]
		public void IncrementByInterval_SampleDaily1_IncrementsReleaseDateOneDay()
        {
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");

			daily1 = daily1.IncrementByInterval(daily1.RecurrenceInterval());

			DateTime expectedLocal = new DateTime(2022, 3, 11, 7, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, daily1.ReleaseDate());
		}

		[TestMethod]
		public void IncrementByInterval_SampleDaily1_IncrementsDueDateOneDay()
		{
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");

			daily1 = daily1.IncrementByInterval(daily1.RecurrenceInterval());

			DateTime expectedLocal = new DateTime(2022, 3, 11, 12, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, daily1.DueDate());
		}

		[TestMethod]
		public void IncrementByInterval_SampleDaily1_IncrementsDeadlineOneDay()
		{
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");

			daily1 = daily1.IncrementByInterval(daily1.RecurrenceInterval());

			DateTime expectedLocal = new DateTime(2022, 3, 11, 23, 0, 0);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, SampleDataTimeZone());
			Assert.AreEqual(expectedUTC, daily1.Deadline());
		}

		[TestMethod]
		public void RepeatWithinWorkInterval_SampleDaily1_RepeatsOnceOneDayApart()
		{
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");
			var (preferences, now) = PreferencesAndTime();

			IEnumerable<TodoTask> dailies = daily1.RepeatWithinWorkWindow(preferences, now);
			Assert.AreEqual(2, dailies.Count());
			TimeSpan actualInterval = dailies.ElementAt(1).ReleaseDate() - dailies.ElementAt(0).ReleaseDate();

			TimeSpan expectedInterval = new TimeSpan(24, 0, 0);
			Assert.AreEqual(expectedInterval, actualInterval);
		}

		[TestMethod]
		public void ToScheduleJob_SampleDaily1_jsonSerializationIsNotNull()
		{
			TodoTask daily1 = GetTaskByName("Daily", "SampleDaily1");
			var (preferences, now) = PreferencesAndTime();

			Job chore1Job = daily1.ToScheduleJob(preferences, now);
			string json = JsonConvert.SerializeObject(chore1Job);

			Assert.IsNotNull(json);
		}

		[DataTestMethod]
		[DataRow("Daily", "SampleDaily1", "1")]
		[DataRow("Daily", "SampleLunch", "2")]
		[DataRow("Daily", "SampleDinner", "3")]
		[DataRow("Daily", "SampleExercise", "4")]
		[DataRow("Daily", "SampleExecutiveTime", "5")]
		[DataRow("The Pool", "SampleFlight", "6")]
		[DataRow("The Pool", "SampleChore1", "7")]
		[DataRow("The Pool", "SampleChore2", "8")]
		[DataRow("The Pool", "SampleErrand1", "9")]
		[DataRow("The Pool", "SampleErrand2", "10")]
		public void ToScheduleJob(string taskList, string taskName, string identity)
		{
			TodoTask task = GetTaskByName(taskList, taskName);
			var (preferences, now) = PreferencesAndTime();

			Job jobActual = task.ToScheduleJob(preferences, now);
			Job jobExpected = GetJobByIdentity(identity);

			Assert.IsNotNull(jobExpected, taskName);
			string message;
			Assert.IsTrue(jobActual.Equals(jobExpected, out message), taskName + ": " + message);
		}

		[TestMethod]
		public void GenerateSampleSchedule()
		{
			UserPreferences prefs = UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");
			Assert.Fail();
		}


	}
	[TestClass]
	public class MSGraphTest
    {
		[TestMethod]
		[TestCategory("GraphIntegration")]
		public void MSGraphAuthenticationTest()
		{
			string[] scope = { "User.Read" };
			MSGraphAuth provider = new MSGraphAuth(scope);
			string accessToken = provider.GetTokenAsync(scope).Result;
			Assert.IsNotNull(accessToken);
		}
	}
}

