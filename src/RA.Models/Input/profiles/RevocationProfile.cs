// <copyright file="MetricManager.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Revocation Profile
	/// The conditions and methods by which a credential can be removed from a holder.
	/// </summary>
	public class RevocationProfile
	{
		public RevocationProfile()
		{
		}

		/// <summary>
		/// Profile Description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		/// <summary>
		/// Approximate date after which this resource is available or applicable.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// One of ApproximateDateEffective, or DateEffective is required
		/// </summary>
		public string ApproximateDateEffective { get; set; }

		/// <summary>
		/// Approximate date after which this resource is no longer offered or available.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		public string ApproximateExpirationDate { get; set; }

		/// <summary>
		/// Date after which this resource is available or applicable.
		/// One of ApproximateDateEffective, or DateEffective is required
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		/// Webpage or online document that provides information about the removal criteria for an awarded credential.
		/// URI
		/// </summary>
		public string RevocationCriteria { get; set; }

		/// <summary>
		/// Textual description providing information about the removal criteria for an awarded credential.
		/// Optional
		/// Minimum of 15 characters when present, but should be clear.
		/// </summary>
		public string RevocationCriteriaDescription { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:revocationCriteriaDescription" )]
		public LanguageMap RevocationCriteriaDescriptionLangMap { get; set; } = new LanguageMap();
	}
}
