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
		private UserPreferences GetSampleUserPreferences() =>
			UserPreferences.UserPrefsFromFile("sampleUserPreferences.json");
		private DateTime SampleTimeBeforeWorkdayStart() =>
			new DateTime(2022, 03, 10, 12, 0, 0);
		private DateTime SampleTimeDuringWorkday() =>
			new DateTime(2022, 3, 10, 18, 0, 0);

		[TestMethod]
		public void RestTime_SamplePrefs_ReturnsEightHours()
		{
			UserPreferences preferences = GetSampleUserPreferences();
			TimeSpan restTime = preferences.RestTime;

			TimeSpan expected = new TimeSpan(8, 0, 0);

			Assert.AreEqual(expected, restTime);
		}

		[DataTestMethod]
		[DataRow(0, "2022-03-10T07:00:00", "Workday start")]
		[DataRow(1, "2022-03-10T07:15:00", "One schedule tick")]
		[DataRow(63, "2022-03-10T22:45:00", "One tick before workday end")]
		[DataRow(64, "2022-03-11T07:00:00", "Workday start next day")]
		[DataRow(128, "2022-03-11T23:00:00", "Next workday end")]
		public void FromScheduleTime(int time, string expected, string description)
        {
			var (preferences, now) = (GetSampleUserPreferences(), SampleTimeBeforeWorkdayStart());

			DateTime actual = preferences.FromScheduleTime(time, now);

			DateTime expectedLocal = DateTime.Parse(expected);
			DateTime expectedUTC = TimeZoneInfo.ConvertTimeToUtc(expectedLocal, preferences.GetTimeZone());
			Assert.AreEqual(expectedUTC, actual, description);
		}
		
		[DataTestMethod]
		[DataRow("2022-03-10T07:00:00", 0, "Workday start")]
		[DataRow("2022-03-10T07:15:00", 1, "One schedule tick")]
		[DataRow("2022-03-10T22:45:00", 63, "One tick before workday end")]
		[DataRow("2022-03-11T07:00:00", 64, "Workday start next day")]
		[DataRow("2022-03-10T06:00:00", 0, "Before workday start")]
		[DataRow("2022-03-11T06:00:00", 64, "Before workday start next day")]
		[DataRow("2022-03-11T23:00:00", 128, "Next workday end")]
		[DataRow("2022-03-12T00:00:00", 128, "Beyond next workday end")]
		[DataRow("2022-03-13T23:00:00", 128, "Well beyond next workday end")]
		public void ToScheduleTime_BeforeWorkday(string time, int expected, string description)
		{
			var (preferences, now) = (GetSampleUserPreferences(), SampleTimeBeforeWorkdayStart());
			DateTime timeLocal = DateTime.Parse(time);
			DateTime timeUTC = TimeZoneInfo.ConvertTimeToUtc(timeLocal, preferences.GetTimeZone());

			int actual = preferences.ToScheduleTime(timeUTC, now);
			Assert.AreEqual(expected, actual, description + ": " + time);
		}

		[DataTestMethod]
		[DataRow("2022-03-10T12:00:00", 0, "Now")]
		[DataRow("2022-03-10T12:15:00", 1, "Now plus one schedule tick")]
		[DataRow("2022-03-10T23:00:00", 44, "Workday End")]
		[DataRow("2022-03-11T07:00:00", 44, "Next workday start")]
		[DataRow("2022-03-11T23:00:00", 108, "Next workday end")]
		[DataRow("2022-03-10T11:00:00", 0, "After workday start before now")]
		[DataRow("2022-03-11T06:00:00", 44, "Before workday start next day")]
		[DataRow("2022-03-11T23:00:00", 108, "Next workday end")]
		[DataRow("2022-03-12T00:00:00", 108, "Beyond next workday end")]
		[DataRow("2022-03-13T23:00:00", 108, "Well beyond next workday end")]
		public void ToScheduleTime_DuringWorkday(string time, int expected, string description)
        {
			var (preferences, now) = (GetSampleUserPreferences(), SampleTimeDuringWorkday());
			DateTime timeLocal = DateTime.Parse(time);
			DateTime timeUTC = TimeZoneInfo.ConvertTimeToUtc(timeLocal, preferences.GetTimeZone());

			int actual = preferences.ToScheduleTime(timeUTC, now);
			Assert.AreEqual(expected, actual, description + ": " + time);
		}

		[DataTestMethod]
		[DataRow("2022-03-10T07:00:00", true)]
		[DataRow("2022-03-11T00:00:00", true)]
		[DataRow("2022-03-11T23:00:00", true)]
		[DataRow("2022-03-11T23:15:00", false)]
		[DataRow("2022-03-10T06:45:00", false)]
		public void WithinWorkInterval(string time, bool expected)
        {
			var (preferences, now) = (GetSampleUserPreferences(), SampleTimeBeforeWorkdayStart());
			DateTime timeLocal = DateTime.Parse(time);
			DateTime timeUTC = TimeZoneInfo.ConvertTimeToUtc(timeLocal, preferences.GetTimeZone());

			bool actual = preferences.WithinWorkInterval(timeUTC, now);
			Assert.AreEqual(expected, actual, time);
		}
	}
}

