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
            CloudInstance cloud = new CloudInstance();
            PeriodicUpdateAsync(cloud).Wait();

        }
        static async Task PeriodicUpdateAsync(CloudInstance cloud)
        {
            if (await cloud.DetectChangesAsync())
            {
                (await cloud.PullIncompleteJobsFromCloudAsync())
                    .ScheduleJobs()
                    .PushScheduleToCloudAsync();
            }
        }

        static async Task DailyUpdateAsync(CloudInstance cloud)
        {

        }

        private static Task<List<Job>> LogAndRemoveCompletedJobs(List<Job> jobs)
        {
            return null;
        }
        private static IEnumerable<(Job, int)> ScheduleJobs(this IEnumerable<Job> jobs) =>
            Scheduler.Schedule(jobs, null);
        private static async Task PushScheduleToCloudAsync(this IEnumerable<(Job, int)> jobs)
        {
            
        }
    }
}
