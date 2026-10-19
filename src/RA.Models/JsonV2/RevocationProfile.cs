// <copyright file="RevocationProfile.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class RevocationProfile
	{
		public RevocationProfile()
		{
			Type = "ceterms:RevocationProfile";
			Jurisdiction = new List<JurisdictionProfile>();
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Approximate date after which this resource is available or applicable.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		[JsonProperty( "ceterms:approximateDateEffective" )]
		public string ApproximateDateEffective { get; set; }

		/// <summary>
		/// Approximate date after which this resource is no longer offered or available.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		[JsonProperty( "ceterms:approximateExpirationDate" )]
		public string ApproximateExpirationDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// </summary>
		[JsonProperty( "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:revocationCriteria" )]
		public string RevocationCriteria { get; set; }

		[JsonProperty( PropertyName = "ceterms:revocationCriteriaDescription" )]
		public LanguageMap RevocationCriteriaDescription { get; set; }
	}
}
