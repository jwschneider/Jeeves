using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Jeeves
{
    static class Program
    {
        static void Main(string[] args)
        {
            PeriodicUpdateAsync().Wait();
        }
        static async Task PeriodicUpdateAsync()
        {
            if (await DetectChangesAsync())
            {
                (await PullIncompleteJobsFromDatabaseAsync())
                    .ScheduleJobs()
                    .PushScheduleToDatabaseAsync();
            }
        }

        static async Task DailyUpdateAsync()
        {

        }

        public static Task<bool> DetectChangesAsync()
        {
            return Task.Run(() => false);
        }
        private static async Task<IEnumerable<Job>> PullIncompleteJobsFromDatabaseAsync()
        {
            return null;
        }
        private static Task<List<Job>> LogAndRemoveCompletedJobs(List<Job> jobs)
        {
            return null;
        }
        private static IEnumerable<(Job, int)> ScheduleJobs(this IEnumerable<Job> jobs) =>
            Scheduler.Schedule(jobs, null);
        private static async Task PushScheduleToDatabaseAsync(this IEnumerable<(Job, int)> jobs)
        {
            
        }
    }
}
