using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing an Occupation
	/// </summary>
	public class OccupationRequest : BaseRequest
	{
		/// <summary>
		/// Profession, trade, or career field that may involve training and/or a formal qualification.
		/// </summary>
		public Occupation Occupation { get; set; } = new Occupation();

	}

	/// <summary>
	/// Request class for publishing a Job
	/// </summary>
	public class JobRequest : BaseRequest
	{
		/// <summary>
		/// Set of responsibilities based on work roles within an occupation as defined by an employer.
		/// </summary>
		public Job Job { get; set; } = new Job();

	}


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
	/// Request class for publishing a WorkRole
	/// </summary>
	public class WorkRoleRequest : BaseRequest
	{
		/// <summary>
		/// Collection of tasks and competencies that embody a particular function in one or more jobs.
		/// </summary>
		public WorkRole WorkRole { get; set; } = new WorkRole();

	}
}
