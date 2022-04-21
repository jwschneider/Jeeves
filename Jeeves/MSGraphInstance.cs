using System;
using Microsoft.Identity.Client;
using Azure.Identity;
using Microsoft.Graph;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Jeeves
{
	public class MSGraphInstance : ICloudInstance
	{
		public MSGraphConfig config { get; set; }
		public GraphServiceClient client { get; set; }

		public MSGraphInstance(string[] scope)
        {
			IAuthenticationProvider provider = new MSGraphAuth(scope);
			client = new GraphServiceClient(provider);
        }

        public async Task<bool> DetectChangesAsync()
        {
			return false;
        }
		public async Task<IEnumerable<ITask>> PullIncompleteTasksFromCloudAsync() =>
			throw new NotImplementedException();
		public async Task PushScheduleToCloudAsync(IEnumerable<(ITask, DateTime)> schedule) =>
			throw new NotImplementedException();
		public async Task<SetupTime> GenerateSetupTimeAsync(IEnumerable<ITask> tasks) =>
			throw new NotImplementedException();
		public Job ToScheduleJob(ITask task, UserPreferences preferences, DateTime now) =>
			throw new NotImplementedException();


		private async Task<TodoTaskList> GetTaskList(string listName)
        {
			return null;
        }
	}
}

