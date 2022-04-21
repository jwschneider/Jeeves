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
            ICloudInstance cloud = new MSGraphInstance(GetScope());
            PeriodicUpdateAsync(cloud).Wait();
        }
        public static async Task PeriodicUpdateAsync(ICloudInstance cloud)
        {
            if (await cloud.DetectChangesAsync())
            {
                UserPreferences preferences = GetUserPreferences();
                DateTime now = DateTime.UtcNow;
                var tasks = await cloud.PullIncompleteTasksFromCloudAsync();
                Func<ITask, Job> toJob = (ITask task) => cloud.ToScheduleJob(task, preferences, now);
                Func<int, DateTime> fromScheduleTime = (int time) => preferences.FromScheduleTime(time, now);
                SetupTime setup = await cloud.GenerateSetupTimeAsync(tasks);
                var schedule = tasks.ScheduleTasks(toJob, fromScheduleTime, setup);
                await cloud.PushScheduleToCloudAsync(schedule);
            }
        }
        public static async Task DailyUpdateAsync(ICloudInstance cloud)
        {

        }
        public static string[] GetScope() =>
            throw new NotImplementedException();
        public static UserPreferences GetUserPreferences() =>
            throw new NotImplementedException();
        // todo test this
        public static IEnumerable<(ITask, DateTime)> ScheduleTasks(this IEnumerable<ITask> tasks, Func<ITask, Job> toJob, Func<int, DateTime> fromScheduleTime, SetupTime setup)
        {
            var map = tasks.ToDictionary(task => task.Identity());
            return Scheduler.Schedule(tasks.Select(task => toJob(task)), setup)
                .Select(x =>
                {
                    (Job j, int time) = x;
                    return (map[j.Identity], fromScheduleTime(time));
                }
                );
        }
    }
}
