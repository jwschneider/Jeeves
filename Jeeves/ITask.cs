using System;
namespace Jeeves
{
	public interface ITask
	{
		public string Identity();

		public Job ToScheduleJob();
	}
}

