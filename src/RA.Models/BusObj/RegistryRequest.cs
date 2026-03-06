// <copyright file="RegistryRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.BusObj
{
	public class RegistryRequest
	{
		/// <summary>
		/// Requested registry Url
		/// </summary>
		public string RegistryUrl { get; set; }

		/// <summary>
		/// CDTL Type of resource retrieved from the registry
		/// </summary>
		public string CtdlType { get; set; }

		/// <summary>
		/// Payload returned from the registry.
		/// The content can vary depending on the request:
		/// - Resource
		/// - Graph
		/// - Envelope
		/// </summary>
		public string Payload { get; set; }



	}
}
