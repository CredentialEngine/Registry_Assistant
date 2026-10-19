using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class CostProfile
	{
		public CostProfile()
		{
			Type = "ceterms:CostProfile";
			Jurisdiction = new List<JurisdictionProfile>();
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( PropertyName = "ceterms:costDetails" )]
		public string CostDetails { get; set; }

		/// <summary>
		/// A currency code, for ex USD
		/// Currency in which the monetary amount is expressed in 3-letter ISO 4217 format such as "USD".
		/// Optional
		/// https://en.wikipedia.org/wiki/ISO_4217#List_of_ISO_4217_currency_codes
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:currency" )]
		public string Currency { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( "ceterms:endDate" )]
		public string EndDate { get; set; }

		/// <summary>
		/// Approximate date some event or activity ends.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approximateEndDate" )]
		public string ApproximateEndDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:startDate" )]
		public string StartDate { get; set; }

		/// <summary>
		/// Approximate date the validity or usefulness of the information in this resource begins.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approximateStartDate" )]
		public string ApproximateStartDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:condition" )]
		public LanguageMapList Condition { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:directCostType" )]
		public CredentialAlignmentObject DirectCostType { get; set; }

		[JsonProperty( PropertyName = "ceterms:residencyType" )]
		public List<CredentialAlignmentObject> ResidencyType { get; set; }

		[JsonProperty( PropertyName = "ceterms:audienceType" )]
		public List<CredentialAlignmentObject> AudienceType { get; set; }

		[DefaultValue( 0 )]
		[JsonProperty( PropertyName = "ceterms:price" )]
		public decimal? Price { get; set; }

		/// <summary>
		/// Applicable pattern for payments such as "per semester" and "every three months".
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:paymentPattern" )]
		public LanguageMap PaymentPattern { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }
	}
}
