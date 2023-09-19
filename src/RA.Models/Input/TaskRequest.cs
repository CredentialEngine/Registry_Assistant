using System.Collections.Generic;

namespace RA.Models.Input
{

	/// <summary>
	/// Request class for publishing a Task
	/// </summary>
	public class TaskRequest : BaseRequest
	{
		/// <summary>
		/// Specific activity, typically related to performing a function or achieving a goal.
		/// </summary>
		public Task Task { get; set; } = new Task();

	}
	/// <summary>
	/// Request class for publishing a list of Tasks
	/// </summary>
	public class TaskListRequest : BaseRequest
	{
		/// <summary>
		/// List of specific activity, typically related to performing a function or achieving a goal.
		/// </summary>
		public List<object> TaskList { get; set; } = new List<object>();
		/// <summary>
		/// HasLanguageMaps
		/// If false, will format input using the plain Task classes otherwise the JSON-LD class
		/// </summary>
		public bool HasLanguageMaps { get; set; }
	}

}
