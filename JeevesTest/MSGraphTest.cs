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
	public class MSGraphTest
	{
		public JObject GetSampleTaskLists() =>
			JObject.Parse(System.IO.File.ReadAllText("sampleTasks.json"));
		public JObject GetSampleJobs() =>
			JObject.Parse(System.IO.File.ReadAllText("sampleJobs.json"));

		public IEnumerable<TodoTask> GetSampleTasks(JObject sampleTaskLists, string displayName) =>
			sampleTaskLists["taskLists"].Children()
				.Where(list => String.Equals(list["displayName"].ToObject<string>(), displayName))
				.FirstOrDefault()["tasks"]
				.Select(task => task.ToObject<TodoTask>(new JsonSerializer
				{
					TypeNameHandling = TypeNameHandling.Auto
				}));


		public void InitGraphWithSampleData()
		{


		}
		public Func<TodoTask, bool> TaskIsNamed(string name) =>
			(TodoTask task) => String.Equals(task.Title(), name);

		public TodoTask GetTaskByName(string list, string name) =>
			GetSampleTasks(GetSampleTaskLists(), list)
				.Where(TaskIsNamed(name))
				.FirstOrDefault();

		public Job GetJobByIdentity(string identity) =>
			GetSampleJobs()["jobs"]
				.Children()
				.Where(job => String.Equals(job["identity"].ToObject<string>(), identity))
				.Select(job => job.ToObject<Job>())
				.FirstOrDefault();

		[TestMethod]
		public void MSGraphGetSampleDataTest0_JsonToTask()
		{
			string json = System.IO.File.ReadAllText("sampleTaskTest.json");
			TodoTask task = JsonConvert.DeserializeObject<TodoTask>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Assert.IsNotNull(task);
		}
		[TestMethod]
		public void MSGraphGetSampleDataTest1_TaskToJson()
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
		public void MSGraphGetSampleDataTest2_TaskFromJson()
		{
			IEnumerable<TodoTask> pool = GetSampleTasks(GetSampleTaskLists(), "The Pool");
			Assert.IsTrue(pool.Where(TaskIsNamed("SampleChore1")).Any());
		}
		[TestMethod]
		public void MSGraphGetSampleDataTest3_Deadline()
		{
			TodoTask chore1 = GetTaskByName("The Pool", "SampleChore1");
			Assert.IsNotNull(chore1);
			//DateTimeTimeZone expected = new DateTimeTimeZone
			//{
			//	DateTime = DateTime.Parse("2022-03-12T23:00:00").ToString(),
			//	TimeZone = "America/Chicago"
			//};
			DateTime expected = new DateTime(2022, 03, 11, 5, 0, 0);
			DateTime actual = chore1.Deadline();
			Assert.IsTrue(DateTime.Equals(expected, actual), $"expected time {expected} but got {actual}");
		}
		[TestMethod]
		public void MSGraphGetSampleDataTest4_CreateTime()
		{
			TodoTask chore1 = GetTaskByName("The Pool", "SampleChore1");
			Assert.IsNotNull(chore1);
			DateTime actual = chore1.CreatedTime();
			//DateTimeOffset expected = DateTimeOffset.Parse("2022-03-05 12:00:00 -5:00");
			DateTime expected = new DateTime(2022, 3, 3, 18, 0, 0);
			Assert.AreEqual(0, expected.CompareTo(actual));
		}
		[TestMethod]
		public void MSGraphGetSampleDataTest5_Recurrance()
		{
			TodoTask chore1 = GetTaskByName("Daily", "SampleDaily1");
			Assert.IsNotNull(chore1);
			PatternedRecurrence actual = chore1.Recurrence();
			Assert.AreEqual(RecurrencePatternType.Daily, actual.Pattern.Type);
			Assert.AreEqual(1, actual.Pattern.Interval);
		}

		[TestMethod]
		public void MSGraphAuthenticationTest()
		{
			string[] scope = { "User.Read" };
			MSGraphAuth provider = new MSGraphAuth(scope);
			string accessToken = provider.GetTokenAsync(scope).Result;
			Assert.IsNotNull(accessToken);
		}
		[TestMethod]
		public void TodoTaskToScheduleJobTest0()
		{
			TodoTask chore1 = GetTaskByName("Daily", "SampleDaily1");
			UserPreferences prefs = UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");
			Job chore1Job = chore1.ToScheduleJob(prefs);
			string json = JsonConvert.SerializeObject(chore1Job);
			Assert.IsNotNull(json);
		}
		[TestMethod]
		public void TodoTaskToScheduleJobTest1()
		{
			TodoTask chore1 = GetTaskByName("Daily", "SampleDaily1");
			UserPreferences prefs = UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");
			Job chore1actual = chore1.ToScheduleJob(prefs);
			Job chore1expected = GetJobByIdentity("1");
			Assert.IsNotNull(chore1expected);
			Assert.AreEqual(chore1expected, chore1actual);
		}

		[TestMethod]
		public void GenerateSampleSchedule()
		{
			UserPreferences prefs = UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");

		}

	}
}

