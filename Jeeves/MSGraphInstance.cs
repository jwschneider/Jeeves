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


		public async Task<IEnumerable<TodoTask>> GetTaskListByNameAsync(string name)
		{
			string id = await GetTaskListIdByNameAsync(name);
			return await GetTaskListByIdAsync(id);
		}
		


		public async Task<string> GetTaskListIdByNameAsync(string name) {
			var taskLists = await client.Me.Todo.Lists.Request()
				.Filter($"displayName eq '{name}'")
				.GetAsync();
			if (taskLists.Count < 1)
				throw new AggregateException($"No task list found with displayName {name}");
			return taskLists.First().Id;
		}

		public async Task UpdateTaskListAsync(string name, IEnumerable<TodoTask> tasks)
		{
			var currentTasks = await GetTaskListByNameAsync(name);
			var intersection = currentTasks.Intersect(tasks);
			var toDelete = currentTasks.Except(intersection);
			var toAdd = tasks.Except(intersection);
			// todo fire off all these requests asynchronously, then wait for them all to be complete
		}

		public async Task<IEnumerable<TodoTask>> GetTaskListByIdAsync(string id) =>
			(await client.Me.Todo.Lists[id].Tasks.Request().GetAsync()).AsEnumerable();

		public async Task CreateTaskByIdAsync(string listId, TodoTask task) =>
			await client.Me.Todo.Lists[listId].Tasks.Request().AddAsync(task);

		public async Task CreateTasksByIdAsync(string listId, IEnumerable<TodoTask> tasks) =>
			await Task.WhenAll(tasks.Select(task => CreateTaskByIdAsync(listId, task)));

		public async Task UpdateTaskByIdAsync(string listId, TodoTask task) =>
			await client.Me.Todo.Lists[listId].Tasks[task.Identity()].Request().UpdateAsync(task);

		public async Task UpdateTasksByIdAsync(string listId, IEnumerable<TodoTask> tasks) =>
			await Task.WhenAll(tasks.Select(task => UpdateTaskByIdAsync(listId, task)));

		public async Task DeleteTaskByIdAsync(string listId, string taskId) =>
			await client.Me.Todo.Lists[listId].Tasks[taskId].Request().DeleteAsync();

		public async Task DeleteTasksByIdAsync(string listId, IEnumerable<string> taskIds) =>
			await Task.WhenAll(taskIds.Select(taskId => DeleteTaskByIdAsync(listId, taskId)));
	}
}

