using System;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Jeeves
{
	public class CloudInstance
	{
		private MSGraphInstance graph;
		private string[] scope = { "Task.ReadWrite" };

		public CloudInstance()
		{
			graph = new MSGraphInstance(scope);
		}

		public async Task<bool> DetectChangesAsync() =>
			await graph.DetectChangesAsync();
		public async Task<IEnumerable<Job>> PullIncompleteJobsFromCloudAsync() =>
			await graph.GetIncompleteJobsAsync();
	}
}

