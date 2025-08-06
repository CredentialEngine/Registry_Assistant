// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

namespace RA.Models.Input
{
	/// <summary>
	/// ScheduledOffering Request
	/// TBD - handling a list?
	/// </summary>
	public class ScheduledOfferingRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public ScheduledOfferingRequest()
		{
			ScheduledOffering = new ScheduledOffering();
		}

		/// <summary>
		/// ScheduledOffering Input Class
		/// </summary>
		public ScheduledOffering ScheduledOffering { get; set; }
	}
}
