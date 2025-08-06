// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

namespace RA.Models.Input
{
	public class VerifyRequest
	{
		/// <summary>
		/// Identifier for Organization which Owns the data being verified
		/// </summary>
		public string PublishForOrganizationIdentifier { get; set; }

		/// <summary>
		/// The CTID of the resource to be verified.
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// The community/private registry where the resource to be verified is located.
		/// Optional. If not provided, the default registry will be used.
		/// </summary>
		public string Community { get; set; }
	}
}
