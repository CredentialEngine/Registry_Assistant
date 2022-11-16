using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MJ=RA.Models.JsonV2;
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

		/// <summary>
		/// Occupation already formatted as JSON-LD
		/// ONLY USED WITH PUBLISH LIST
		/// </summary>
		public MJ.Occupation FormattedOccupation { get; set; } = new MJ.Occupation();
	}
	/// <summary>
	/// Request class for publishing a list of Occupations
	/// </summary>
	public class OccupationListRequest : BaseRequest
	{
		/// <summary>
		/// List of Occupations
		/// Using data type of object to allow handling plain requests or those with language maps
		/// </summary>
		public List<object> OccupationList { get; set; } = new List<object>();
		/// <summary>
		/// HasLanguageMaps
		/// If false, will format input using the plain Occupation classes otherwise the JSON-LD class
		/// </summary>
		public bool HasLanguageMaps { get; set; }
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


		/// <summary>
		/// Occupation already formatted as JSON-LD
		/// ONLY USED WITH PUBLISH LIST
		/// </summary>
		public MJ.Job FormattedJob { get; set; } = new MJ.Job();
	}
	/// <summary>
	/// Request class for publishing a list of Jobs
	/// </summary>
	public class JobListRequest : BaseRequest
	{
		/// <summary>
		/// List of Jobs
		/// /// Using data type of object to allow handling plain requests or those with language maps
		/// </summary>
		public List<object> JobList { get; set; } = new List<object>();
		/// <summary>
		/// HasLanguageMaps
		/// If false, will format input using the plain Job classes otherwise the JSON-LD class
		/// </summary>
		public bool HasLanguageMaps { get; set; }
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

		/// <summary>
		/// Task already formatted as JSON-LD
		/// ONLY USED WITH PUBLISH LIST
		/// </summary>
		public MJ.Task FormattedTask { get; set; } = new MJ.Task();

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

	/// <summary>
	/// Request class for publishing a WorkRole
	/// </summary>
	public class WorkRoleRequest : BaseRequest
	{
		/// <summary>
		/// Collection of tasks and competencies that embody a particular function in one or more jobs.
		/// </summary>
		public WorkRole WorkRole { get; set; } = new WorkRole();


		/// <summary>
		/// Task already formatted as JSON-LD
		/// ONLY USED WITH PUBLISH LIST
		/// </summary>
		public MJ.WorkRole FormattedWorkRole { get; set; } = new MJ.WorkRole();
	}

	/// <summary>
	/// Request class for publishing a list of WorkRoles
	/// </summary>
	public class WorkRoleListRequest : BaseRequest
	{
		/// <summary>
		/// List of specific activity, typically related to performing a function or achieving a goal.
		/// </summary>
		public List<object> WorkRoleList { get; set; } = new List<object>();
		/// <summary>
		/// HasLanguageMaps
		/// If false, will format input using the plain Work Role classes otherwise the JSON-LD class
		/// </summary>
		public bool HasLanguageMaps { get; set; }
	}
}
