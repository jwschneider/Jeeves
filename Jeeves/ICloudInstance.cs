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
		public SetupTime GenerateSetupTime(IEnumerable<ITask> tasks);
	}
}

