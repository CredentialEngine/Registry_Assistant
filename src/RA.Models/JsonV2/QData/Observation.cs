using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2.QData
{

	/// <summary>
	/// Data within a dataset defined by specific values for all relevant dimensions and associated with measured values.
	/// </summary>
	public class Observation
	{
		public Observation()
		{
			Type = "qdata:Observation";
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// An identifier for use with blank nodes. 
		/// It will be ignored if included with a primary resource
		/// </summary>
		[JsonProperty( "@id" )]
		public string Id { get; set; }

		/// <summary>
		///  A point in a Dimension that represents a resource or factor relevant to this Observation.
		/// Range Includes:	rdfs:Resource (URI)
		/// qdata:atPoint
		/// </summary>
		[JsonProperty( "qdata:atPoint" )]
		public List<string> AtPoint { get; set; }

		/// <summary>
		/// Supplemental text provided by the promulgating body that clarifies the nature, scope or use of this resource.
		/// ceasn:comment
		/// </summary>
		[JsonProperty( "ceasn:comment" )]
		public LanguageMap Comment { get; set; }

		/// <summary>
		/// Currency abbreviation (e.g., USD).
		/// </summary>
		[JsonProperty( "schema:currency" )]
		public string Currency { get; set; }

		/// <summary>
		/// Type of suppression, masking, or other modification made to the data to protect the identities of its subjects.
		/// qdata:dataWithholdingType
		/// Concept in qdata:DataWithholdingCategory
		/// </summary>
		[JsonProperty( "qdata:dataWithholdingType" )]
		public string DataWithholdingType { get; set; }

		/// <summary>
		/// Indicates the metric used for this Observation.
		/// qdata:isObservationOf
		/// Range: qdata:Metric
		/// </summary>
		[JsonProperty( "qdata:isObservationOf" )]
		public string IsObservationOf { get; set; }

		/// <summary>
		/// Mean value.
		/// </summary>
		[JsonProperty( "qdata:mean" )]
		public decimal? Mean { get; set; }

		/// <summary>
		/// Median salary value.
		/// </summary>
		[JsonProperty( "qdata:median" )]
		public decimal? Median { get; set; }

		/// <summary>
		/// The lower percentage of some characteristic or property.
		/// </summary>
		[JsonProperty( "qdata:minPercentage" )]
		public decimal? MinPercentage { get; set; }

		/// <summary>
		/// The upper percentage of some characteristic or property.
		/// </summary>
		[JsonProperty( "qdata:maxPercentage" )]
		public decimal? MaxPercentage { get; set; }

		/// <summary>
		/// Lower value of some characteristic or property.
		/// </summary>
		[JsonProperty( "schema:minValue" )]
		public decimal? MinValue { get; set; }

		/// <summary>
		/// Upper value of some characteristic or property.
		/// </summary>
		[JsonProperty( "schema:maxValue" )]
		public decimal? MaxValue { get; set; }

		/// <summary>
		/// Quotient of two values of the data set, expressed as a percentage.
		/// </summary>
		[JsonProperty( "qdata:percentage" )]
		public decimal? Percentage { get; set; }

		/// <summary>
		/// 10th percentile salary value.
		/// </summary>
		[JsonProperty( "qdata:percentile10" )]
		public decimal? Percentile10 { get; set; }

		/// <summary>
		/// 25th percentile salary value.
		/// </summary>
		[JsonProperty( "qdata:percentile25" )]
		public decimal? Percentile25 { get; set; }

		/// <summary>
		/// 75th percentile salary value.
		/// </summary>
		[JsonProperty( "qdata:percentile75" )]
		public decimal? Percentile75 { get; set; }

		/// <summary>
		/// 90th percentile salary value.
		/// </summary>
		[JsonProperty( "qdata:percentile90" )]
		public decimal? Percentile90 { get; set; }

		/// <summary>
		/// Number of subjects from the population for which data was obtained to calculate the observed value.
		/// qdata:sizeOfData
		/// </summary>
		[JsonProperty( "qdata:sizeOfData" )]
		public int? SizeOfData { get; set; }

		/// <summary>
		///  Number of subjects in the sample for which no data was obtained.
		/// qdata:sizeOfNoData
		/// </summary>
		[JsonProperty( "qdata:sizeOfNoData" )]
		public int? SizeOfNoData { get; set; }

		/// <summary>
		///  The total number of subjects in the relevant population.
		/// qdata:sizeOfPopulation
		/// </summary>
		[JsonProperty( "qdata:sizeOfPopulation" )]
		public int? SizeOfPopulation { get; set; }

		/// <summary>
		///  Standard deviation about the mean.
		/// qdata:standardDeviation
		/// </summary>
		[JsonProperty( "qdata:standardDeviation" )]
		public decimal? StandardDeviation { get; set; }

		/// <summary>
		/// Value of a monetary amount or a quantitative value.
		/// </summary>
		[JsonProperty( "schema:value" )]
		public decimal? Value { get; set; }
	}
}
