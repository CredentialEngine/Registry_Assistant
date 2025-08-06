// <copyright file="MetricManager.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Monetary value or range.
	/// </summary>
	public class MonetaryAmount
	{
		/// <summary>
		/// Currency abbreviation (e.g., USD).
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// Value of a monetary amount or a quantitative value.
		/// </summary>
		public decimal? Value { get; set; }

		/// <summary>
		/// Lower value of some characteristic or property.
		/// </summary>
		public decimal? MinValue { get; set; }

		/// <summary>
		/// Upper value of some characteristic or property.
		/// </summary>
		public decimal? MaxValue { get; set; }

		/// <summary>
		/// Description of this record
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = null;

		/// <summary>
		/// Word or phrase indicating the unit of measure - mostly if the use of Currency is not sufficient
		/// </summary>
		public string UnitText { get; set; }

		/// <summary>
		/// Type of suppression, masking, or other modification made to the data to protect the identities of its subjects.
		/// concept from: qdata:DataWithholdingCategory
		/// </summary>
		public string DataWithholdingType { get; set; }
	}

	/// <summary>
	/// Statistical distribution of monetary amounts.
	/// https://credreg.net/qdata/terms/MonetaryAmountDistribution#MonetaryAmountDistribution
	/// </summary>
	public class MonetaryAmountDistribution
	{
		/// <summary>
		/// Currency abbreviation (e.g., USD).
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// Median salary value.
		/// </summary>
		public decimal? Median { get; set; }

		/// <summary>
		/// 10th percentile salary value.
		/// </summary>
		public decimal? Percentile10 { get; set; }

		/// <summary>
		/// 25th percentile salary value.
		/// </summary>
		public decimal? Percentile25 { get; set; }

		/// <summary>
		/// 75th percentile salary value.
		/// </summary>
		public decimal? Percentile75 { get; set; }

		/// <summary>
		/// 90th percentile salary value.
		/// </summary>
		public decimal? Percentile90 { get; set; }

		/// <summary>
		/// Type of suppression, masking, or other modification made to the data to protect the identities of its subjects.
		/// concept from: qdata:DataWithholdingCategory
		/// </summary>
		public string DataWithholdingType { get; set; }
	}
}
