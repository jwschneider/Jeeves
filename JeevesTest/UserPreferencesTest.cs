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
			preferences = UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");
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
			int time = 63;
			DateTime expected = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("1.03:45:00") :
					TimeSpan.Parse("1.04:45:00"));
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
		[TestMethod]
		public void ScheduleTimeToRealTimeTest3()
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
		public void RealTimeToScheduleTimeTest0()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("1.03:45:00") :
					TimeSpan.Parse("1.04:45:00"));
			int expected = 63;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest1()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("12:00:00") :
					TimeSpan.Parse("13:00:00"));
			int expected = 0;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest2()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("1.12:00:00") :
					TimeSpan.Parse("1.13:00:00"));
			int expected = 64;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest3()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("13:00:00") :
					TimeSpan.Parse("14:00:00"));
			int expected = 4;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest4()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("11:00:00") :
					TimeSpan.Parse("12:00:00"));
			int expected = 0;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest5()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("1.11:00:00") :
					TimeSpan.Parse("1.12:00:00"));
			int expected = 64;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest6()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("2.04:00:00") :
					TimeSpan.Parse("2.05:00:00"));
			int expected = 128;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest7()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("2.05:00:00") :
					TimeSpan.Parse("2.06:00:00"));
			int expected = 128;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void RealTimeToScheduleTimeTest8()
		{
			DateTime time = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("1.04:00:00") :
					TimeSpan.Parse("1.05:00:00"));
			int expected = 64;
			int actual = preferences.ToScheduleTime(time);
			Assert.AreEqual(expected, actual);
		}
	}
}

