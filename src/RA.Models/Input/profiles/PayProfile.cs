// <copyright file="PayProfile.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Entity that describes the monetary compensation for work.
	/// </summary>
	public class PayProfile
	{

		/// <summary>
		///  Statement, characterization or account of the entity.
		///  Optional
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately use a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		/// <summary>
		/// Duration to which a monetary amount applies.
		/// xsd:duration
		/// </summary>
		public DurationItem BasePeriod { get; set; }

		/// <summary>
		/// Type of pay band indicating a range or classification of monetary compensation associated with work; select from an existing enumeration of such types.
		/// Concept - no specific CTDL concept scheme
		/// </summary>
		public List<string> PayBandType { get; set; }

		/// <summary>
		/// Type of pattern for payments, indicating how often a payment is made; select from an existing enumeration of such types.
		/// Concept - in concept scheme: ceterms:ScheduleFrequency
		/// </summary>
		public string PaymentPatternType { get; set; }

		/// <summary>
		/// Approximate date after which this resource is available or applicable.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		public string ApproximateDateEffective { get; set; }

		/// <summary>
		/// Approximate date after which this resource is no longer offered or available.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		public string ApproximateExpirationDate { get; set; }

		/// <summary>
		/// A currency (ISO_4217) code, for example USD
		/// Currency in which the monetary amount is expressed in 3-letter ISO 4217 format such as "USD".
		/// Optional
		/// https://en.wikipedia.org/wiki/ISO_4217#List_of_ISO_4217_currency_codes
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// Date after which this resource is available or applicable.
		/// xsd:date
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// xsd:date
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Minimum value for this purpose. If provided, a maximum value must also be provided
		/// </summary>
		public decimal? MinValue { get; set; }

		/// <summary>
		/// Maximum value for this purpose.
		/// </summary>
		public decimal? MaxValue { get; set; }

		/// <summary>
		/// A single value for this purpose.
		/// Do not use if providing a minimum and maximum value.
		/// </summary>
		public decimal? Value { get; set; }

		/// <summary>
		///  Requirement or set of requirements for this resource.
		/// </summary>
		public List<ConditionProfile> Requires { get; set; } = new List<ConditionProfile>();
	}
}
