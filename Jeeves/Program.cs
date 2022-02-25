using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jeeves
{
    class Program
    {
        static void Main(string[] args)
        {
            UpdateAsync().Wait();
        }
        static async Task UpdateAsync()
        {
            List<Job> jobs = await PullJobsFromDatabase();
            jobs = await LogAndRemoveCompletedJobs(jobs);
            IEnumerable<(Job, int)> scheduledJobs = ScheduleJobs(jobs);
            await PushScheduleToDatabase(scheduledJobs);
        }
        private static Task<List<Job>> PullJobsFromDatabase()
        {
            return null;
        }
        private static Task<List<Job>> LogAndRemoveCompletedJobs(List<Job> jobs)
        {
            return null;
        }
        private static IEnumerable<(Job, int)> ScheduleJobs(List<Job> jobs)
        {
            return Scheduler.Schedule(jobs, null);
        }
        private static Task PushScheduleToDatabase(IEnumerable<(Job, int)> jobs)
        {
            return null;
        }
    }
}
