// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using MJ = RA.Models.JsonV2;

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
}
