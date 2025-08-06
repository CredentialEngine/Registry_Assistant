// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

namespace RA.Models.Input
{
	public class VerificationServiceProfileRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public VerificationServiceProfileRequest()
		{
			VerificationServiceProfile = new VerificationServiceProfile();
		}

		/// <summary>
		/// VerificationServiceProfile Input Class
		/// </summary>
		public VerificationServiceProfile VerificationServiceProfile { get; set; }
	}
}
