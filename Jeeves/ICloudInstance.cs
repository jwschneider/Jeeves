using System;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Jeeves
{
	public interface ICloudInstance
	{

		public Task<bool> DetectChangesAsync();
		public Task<IEnumerable<ITask>> PullIncompleteTasksFromCloudAsync();
		public Task PushScheduleToCloudAsync(IEnumerable<(ITask, DateTime)> schedule);
		public Task<SetupTime> GenerateSetupTimeAsync(IEnumerable<ITask> tasks);
		public Job ToScheduleJob(ITask task, UserPreferences preferences, DateTime now);
	}
}

