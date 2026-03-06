// <copyright file="TransferAgreementRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing a TransferAgreement
	/// </summary>
	public class TransferAgreementRequest : BaseRequest
	{
		/// <summary>
		/// Formal agreement between two or more parties that specifies how prior learning is recognized, including its transfer and applicability for credit.
		/// </summary>
		public TransferAgreement TransferAgreement { get; set; } = new TransferAgreement();
	}
}
