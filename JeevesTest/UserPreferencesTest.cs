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
	public class TimeExtensionsTest
    {
		[TestMethod]
		public void Mod_NegativeNumerator_ReturnsPositive()
        {
			TimeSpan negative = new TimeSpan(-1, 0, 0);
			TimeSpan actual = negative.mod(new TimeSpan(24, 0, 0));
			TimeSpan expected = new TimeSpan(23, 0, 0);
			Assert.AreEqual(expected, actual);
        }
		[TestMethod]
		public void Mod_PositiveLessThanModulus_ReturnsSame()
        {
			TimeSpan a = new TimeSpan(1, 0, 0);
			TimeSpan b = new TimeSpan(24, 0, 0);
			TimeSpan actual = a.mod(b);
			TimeSpan expected = new TimeSpan(1, 0, 0);
			Assert.AreEqual(expected, actual);
        }
		[TestMethod]
		public void Mod_PositiveGreaterThanModulus_ReturnsModulo()
        {
			TimeSpan a = new TimeSpan(25, 0, 0);
			TimeSpan b = new TimeSpan(24, 0, 0);
			TimeSpan actual = a.mod(b);
			TimeSpan expected = new TimeSpan(1, 0, 0);
			Assert.AreEqual(expected, actual);
		}
    }

	[TestClass]
	public class UserPreferencesTest
	{
		public static UserPreferences preferences { get; set; }
		public static DateTime now { get; set; }
		[ClassInitialize]
		public static void InitUserPrefs(TestContext context)
        {
			preferences = UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");
			now = new DateTime(2022, 3, 10, 18, 0, 0);
        }
		[TestMethod]
		public void RestTime_SamplePrefs_ReturnsEightHours()
		{
			TimeSpan restTime = preferences.RestTime;

			TimeSpan expected = new TimeSpan(8, 0, 0);

			Assert.AreEqual(expected, restTime);
		}


		// todo write better tests ffs
		[TestMethod]
		public void ScheduleTimeToRealTimeTest0()
        {
			int time = 0;
			DateTime expected = DateTime.Today +
				(DateTime.Today.IsDaylightSavingTime() ?
					TimeSpan.Parse("12:00:00") :
					TimeSpan.Parse("13:00:00"));
			DateTime actual = preferences.FromScheduleTime(time, now);
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
			DateTime actual = preferences.FromScheduleTime(time, now);
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
			DateTime actual = preferences.FromScheduleTime(time, now);
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
			DateTime actual = preferences.FromScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
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
			int actual = preferences.ToScheduleTime(time, now);
			Assert.AreEqual(expected, actual);
		}
	}
}

