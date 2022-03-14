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


		[TestMethod]
		public void MSGraphGetSampleDataTest0_JsonToTask()
        {
			string json = System.IO.File.ReadAllText("testSampleTask.json");
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
									DateTime = "2022-03-12T23:00:00",
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
			DateTimeTimeZone expected = new DateTimeTimeZone
			{
				DateTime = DateTime.Parse("2022-03-12T23:00:00").ToString(),
				TimeZone = "America/Chicago"
			};
			DateTimeTimeZone actual = chore1.Deadline();
			Assert.IsTrue(DateTime.Equals(expected.DateTime, actual.DateTime), $"expected time {expected.DateTime.ToString()} but got {actual.DateTime.ToString()}");
			Assert.IsTrue(TimeZone.Equals(expected.TimeZone, actual.TimeZone), $"expected timezone {expected.TimeZone.ToString()} but got {actual.TimeZone.ToString()}");
        }
		[TestMethod]
		public void MSGraphGetSampleDataTest4_CreateTime()
        {
			TodoTask chore1 = GetTaskByName("The Pool", "SampleChore1");
			Assert.IsNotNull(chore1);
			DateTimeOffset? actual = chore1.CreatedTime();
			DateTimeOffset expected = DateTimeOffset.Parse("2022-03-05 12:00:00 -5:00");
			if (actual is DateTimeOffset)
				Assert.AreEqual(0, expected.CompareTo((DateTimeOffset)actual));
			else
				Assert.Fail($"Created time is type {actual.GetType().ToString()}");
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
	}
}

