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
		[TestMethod]
		public void MSGraphGetSampleDataTest0()
        {
			string json = System.IO.File.ReadAllText("testSampleTask.json");
			TodoTask task = JsonConvert.DeserializeObject<TodoTask>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Assert.IsNotNull(task);
        }
		[TestMethod]
		public void MSGraphGetSampleDataTest1()
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
		public void MSGraphGetSampleDataTest2()
        {
			IEnumerable<TodoTask> pool = GetSampleTasks(GetSampleTaskLists(), "The Pool");
			Assert.IsTrue(pool.Where(task => String.Equals(task.Title, "SampleChore1")).Any());
        }
		[TestMethod]
		public void MSGraphGetSampleDataTest3()
        {
			TodoTask chore1 = GetSampleTasks(GetSampleTaskLists(), "The Pool")
								.Where(task => String.Equals(task.Title, "SampleChore1"))
								.FirstOrDefault();
			Assert.IsNotNull(chore1);
			DateTimeTimeZone expected = new DateTimeTimeZone
			{
				DateTime = "2022-03-12T23:00:00",
				TimeZone = "America/Chicago"
			};
			DateTimeTimeZone actual = chore1.Deadline();
			Assert.IsTrue(DateTimeTimeZone.Equals(expected, actual), $"expected time {expected} but got {actual}");
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

