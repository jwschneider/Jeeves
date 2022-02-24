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
            //job = {identifier, value, releaseTime, processTime, dueDate, deadLine}
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
            SetupTime setupTime = (Job i, Job j) => setupMatrix[i.Identity, j.Identity];
            List<(Job, int)> scheduledJobs = Scheduler.Schedule(jobs, setupTime).ToList<(Job, int)>();

            List<(Job, int)> solution = new List<(Job, int)>(new (Job, int)[]
            {
                (jobs[1], 2),
                (jobs[2], 5),
                (jobs[3], 9)
            });
            CollectionAssert.AreEqual(scheduledJobs, solution);
        }
    }
}
