using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System.Linq;

namespace Jeeves
{
    static class Program
    {
        static void Main(string[] args)
        {
            ICloudInstance cloud = new MSGraphInstance();
            PeriodicUpdateAsync(cloud).Wait();

        }
        static async Task PeriodicUpdateAsync(ICloudInstance cloud)
        {
            if (await cloud.DetectChangesAsync())
            {
                UserPreferences preferences;
                var tasks = (await cloud.PullIncompleteTasksFromCloudAsync())
                    .ToDictionary(task => task.Identity());
                var scheduledTasks = tasks.Values.Select(task => task.ToScheduleJob())
                    .ScheduleJobs()
                    .Select(x => (tasks[x.Item1.Identity], preferences.FromScheduleTime(x.Item2)));
                // todo
            }
        }

        static async Task DailyUpdateAsync(ICloudInstance cloud)
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
