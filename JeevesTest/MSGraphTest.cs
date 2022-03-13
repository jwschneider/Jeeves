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

		public IEnumerable<TodoTask> GetSampleTasks(JObject sampleTaskLists, string displayName)
        {
			IEnumerable<JToken> taskLists = sampleTaskLists["taskLists"].Children();
			JToken filtered = taskLists.Where((list) => String.Equals(list["displayName"].ToObject<string>(), displayName)).FirstOrDefault();
			IEnumerable<JToken> tasks = filtered["tasks"];
			IEnumerable<TodoTask> ret = tasks.Select(task => task.ToObject<TodoTask>(new JsonSerializer
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
			// maybe need to recursively deserialize the internal json members
			return ret;
        }
			//=>
			//sampleTaskLists["taskLists"]
			//	.Where(list => list["displayName"].Equals(displayName))
			//	.FirstOrDefault()["tasks"]
			//	.Select(task => task.ToObject<TodoTask>());

		
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
							{"extensionProperty2", "property2value" }
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
		public void MSGraphGetSampleDataTest()
        {
			IEnumerable<TodoTask> pool = GetSampleTasks(GetSampleTaskLists(), "The Pool");
			Assert.IsTrue(pool.Where(task => String.Equals(task.Title, "SampleChore1")).Any());
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

