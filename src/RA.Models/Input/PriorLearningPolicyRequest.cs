// <copyright file="PriorLearningPolicyRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing a PriorLearningPolicy
	/// </summary>
	public class PriorLearningPolicyRequest : BaseRequest
	{
		/// <summary>
		/// Authoritative statement of rules, principles, or procedures adopted by an organization to govern decisions and actions within the scope of prior learning.
		/// </summary>
		public PriorLearningPolicy PriorLearningPolicy { get; set; } = new PriorLearningPolicy();
	}
}
