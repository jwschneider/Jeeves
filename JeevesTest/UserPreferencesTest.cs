using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jeeves;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace JeevesTest
{
	[TestClass]
	public class UserPreferencesTest
	{
		public static UserPreferences preferences { get; set; }
		[ClassInitialize]
		public static void InitUserPrefs(TestContext context)
        {
			string json = System.IO.File.ReadAllText("sampleUserPreferences.json");
			preferences = JsonConvert.DeserializeObject<UserPreferences>(json);
        }
		[TestMethod]
		public void ScheduleTimeToRealTimeTest0()
        {
			int time = 0;
			DateTime expected = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("12:00:00") :
					TimeSpan.Parse("13:00:00"));
			DateTime actual = preferences.FromScheduleTime(time);
			Assert.AreEqual(expected, actual);
        }
		[TestMethod]
		public void ScheduleTimeToRealTimeTest1()
		{
			int time = 4;
			DateTime expected = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("13:00:00") :
					TimeSpan.Parse("14:00:00"));
			DateTime actual = preferences.FromScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ScheduleTimeToRealTimeTest2()
		{
			int time = 64;
			DateTime expected = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("1.12:00:00") :
					TimeSpan.Parse("1.13:00:00"));
			DateTime actual = preferences.FromScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
	}
}

