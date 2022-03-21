using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jeeves;
using System.Collections.Generic;
using System.Linq;

namespace JeevesTest
{
    [TestClass]
    public class SchedulerTests
    {
        [TestMethod]
        public void ExampleSchedule1()
        {
            //job = {identifier, value, releaseTime, processTime, dueDate, deadline}
            int[,] jobMatrix =
            {
                {1, 2, 0, 3, 7, 8},
                {2, 3, 2, 2, 9, 10},
                {3, 4, 3, 3, 8, 10},
                {4, 2, 6, 2, 13, 14}
            };
            List<Job> jobs = Job.CreateJobsFromMatrix(jobMatrix);
            int[,] setupMatrix =
            {
                {1, 1, 1, 1, 2},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 2, 1, 2, 1}
            };
            SetupTime setupTime = (Job i, Job j) => setupMatrix[i == null ? 0 : int.Parse(i.Identity), int.Parse(j.Identity)];
            List<(Job, int)> scheduledJobs = Scheduler.Schedule(jobs, setupTime).ToList<(Job, int)>();

            List<(Job, int)> solution = new List<(Job, int)>(new (Job, int)[]
            {
                (jobs[1], 2),
                (jobs[2], 5),
                (jobs[3], 9)
            });
            CollectionAssert.AreEqual(scheduledJobs, solution);
        }
        [TestMethod]
        public void CreatePotentialSchedulesTest1_1()
        {
            Job j = Job.CreateJobFromArray(new int[] { 1, 1, 0, 1, 1, 3 });
            IEnumerable<ScheduleState> states = Scheduler.CreatePotentialSchedules(
                new DominatingPriorityQueue<ScheduleState>(ScheduleState.init(), (s1, s2) => s1.TimeScheduled - s2.TimeScheduled),
                new List<Job>().Append<Job>(j),
                (x, y) => 0
                );
            Assert.AreEqual(1, states.Count());
            ScheduleState s = states.First();
            Assert.AreEqual(j, s.LastScheduled);
            Assert.AreEqual(1, s.Exclude.Count());
            Assert.AreEqual(j, s.Exclude.First());
            Assert.AreEqual(0, s.TimeScheduled);
            Assert.AreEqual(1, s.Value);
        }
        [TestMethod]
        public void CreatePotentialSchedulesTest1_2()
        {
            Job j = Job.CreateJobFromArray(new int[] { 1, 1, 0, 1, 1, 3 });
            List<(Job, int)> scheduledJobs = Scheduler.Schedule(new List<Job>().Append(j), (x, y) => 0).ToList();
            List<(Job, int)> solution = new List<(Job, int)>(new (Job, int)[] { (j, 0) });
            CollectionAssert.AreEqual(solution, scheduledJobs);
        }
        [TestMethod]
        public void CreatePotentialSchedulesTest2_1()
        {
            List<Job> jobs = Job.CreateJobsFromMatrix(
                new int[,] { { 1, 1, 0, 1, 7, 8},
                             { 2, 1, 2, 1, 7, 8}});
            SetupTime setup = (i, j) => i == null ? 0 : 1;
            IEnumerable<ScheduleState> states = Scheduler.CreatePotentialSchedules(
                new DominatingPriorityQueue<ScheduleState>(ScheduleState.init(), (s1, s2) => s1.TimeScheduled - s2.TimeScheduled, Scheduler.ScheduleStateDominator(setup)),
                jobs,
                setup);
            Assert.AreEqual(1, states.Count());
            ScheduleState s = states.First();
            Assert.AreEqual(jobs[1], s.LastScheduled, $"Last scheduled Job should have been 2 but was {s.LastScheduled.Identity}.");
            Assert.AreEqual(2, s.TimeScheduled);
            Assert.AreEqual(2, s.Value);
        }
    }
}
