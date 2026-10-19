// <copyright file="RegulatoryInformationRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing a RegulatoryInformation
	/// </summary>
	public class RegulatoryInformationRequest : BaseRequest
	{
		/// <summary>
		/// Authoritative statement of rules, principles, or procedures adopted by an organization to govern decisions and actions within the scope of prior learning.
		/// </summary>
		public RegulatoryInformation RegulatoryInformation { get; set; } = new RegulatoryInformation();
	}
}
