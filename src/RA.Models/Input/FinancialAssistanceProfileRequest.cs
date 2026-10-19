// <copyright file="MetricManager.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing an FinancialAssistanceProfile
	/// Required
	/// - CTID 
	/// - Name
	/// - OfferedBy
	/// </summary>
	public class FinancialAssistanceProfileRequest : BaseRequest
	{
		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public FinancialAssistanceProfileEntity FinancialAssistanceProfile { get; set; } = new FinancialAssistanceProfileEntity();
	}

	/// <summary>
	/// CTID and OfferedBy are required in this context
	/// </summary>
	public class FinancialAssistanceProfileEntity : FinancialAssistanceProfile
	{
		/// <summary>
		/// Globally unique Credential Transparency Identifier
		/// format:
		/// ce-UUID (guid)
		/// Required for standalone publishing, not used for embedded resources
		/// </summary>
		public string CTID { get; set; }

	}
}
