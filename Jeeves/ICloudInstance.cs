using System;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Jeeves
{
	public interface ICloudInstance
	{

		public Task<bool> DetectChangesAsync();
		public Task<IEnumerable<Job>> PullIncompleteJobsFromCloudAsync();
	}
}

