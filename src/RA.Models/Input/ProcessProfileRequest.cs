// <copyright file="MetricManager.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing an ProcessProfile
	/// Required
	/// CTID- 
	/// Name?
	/// - Description
	/// </summary>
	public class ProcessProfileRequest : BaseRequest
	{
		/// <summary>
		/// Entity describing the type, nature, and other relevant information about a process.
		/// </summary>
		public ProcessProfile ProcessProfile { get; set; } = new ProcessProfile();
	}
}
