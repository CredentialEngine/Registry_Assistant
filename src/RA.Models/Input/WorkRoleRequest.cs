// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;

using MJ = RA.Models.JsonV2;

namespace RA.Models.Input
{
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
