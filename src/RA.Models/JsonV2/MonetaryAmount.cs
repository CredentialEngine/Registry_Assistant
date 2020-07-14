using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class MonetaryAmount
	{
		public MonetaryAmount()
		{
			Type = "schema:MonetaryAmount";
		}
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// Currency abbreviation (e.g., USD).
		/// </summary>
		[JsonProperty( "schema:currency" )]
		public string Currency { get; set; }

		[JsonProperty( "schema:value" )]
		public decimal Value { get; set; }

		[JsonProperty( "schema:minValue" )]
		public decimal MinValue { get; set; }

		[JsonProperty( "schema:maxValue" )]
		public decimal MaxValue { get; set; }

		/// <summary>
		/// Provide a valid concept from the CreditUnitType concept scheme, with or without the namespace. For example:
		/// creditUnit:ContinuingEducationUnit or ContinuingEducationUnit
		/// <see cref="https://credreg.net/ctdl/terms/creditUnitType"/> 
		/// </summary>
		[JsonProperty( "schema:unitText" )]
		public string UnitText { get; set; }
	}
}
