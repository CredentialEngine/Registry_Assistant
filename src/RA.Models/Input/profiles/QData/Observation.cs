using Newtonsoft.Json;
using System.Collections.Generic;


namespace RA.Models.Input.profiles.QData
{

    /// <summary>
    /// Data within a dataset defined by specific values for all relevant dimensions and associated with measured values.
    /// Required:
    ///     IsObservationOf 
    /// And One Of: 
    ///     schema:value, schema:minValue, schema:maxValue, qdata:percentage, qdata:minPercentage, qdata:maxPercentage
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
        public string BlankNodeId { get; set; }

        /// <summary>
        /// A point in a Dimension that represents a resource or factor relevant to this Observation.
        /// If a CTID is provided, it will be formatted for the target registry.
        /// Blank nodes can be included by adding the bnode identifier here, and adding the bnode class to the request property: ReferenceObjects
        /// Range Includes:	rdfs:Resource (URI)
        /// qdata:atPoint
        /// </summary>
        public List<string> AtPoint { get; set; }

        /// <summary>
        /// Supplemental text provided by the promulgating body that clarifies the nature, scope or use of this resource.
        /// ceasn:comment
        /// </summary>
        public string Comment { get; set; }

        public LanguageMap Comment_Map { get; set; }
        /// <summary>
        /// Currency abbreviation (e.g., USD).
        /// schema:currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Type of suppression, masking, or other modification made to the data to protect the identities of its subjects.
        /// qdata:dataWithholdingType
        /// Concept in qdata:DataWithholdingCategory
        /// ConceptScheme
        ///      https://credreg.net/qdata/terms/DataWithholdingCategory/json
        ///  Example Concept
        ///      https://credreg.net/qdata/vocabs/dataWithholding/DataMasked/json
        /// </summary>
        public string DataWithholdingType { get; set; }

        /// <summary>
        /// Indicates the metric used for this Observation.
        /// This is required.
        /// qdata:isObservationOf
        /// Range: qdata:Metric
        /// </summary>
        public string IsObservationOf { get; set; }

        /// <summary>
        /// The lower percentage of some characteristic or property.
        /// qdata:minPercentage
        /// </summary>
        public decimal? MinPercentage { get; set; }

        /// <summary>
        /// The upper percentage of some characteristic or property.
        /// qdata:maxPercentage
        /// </summary>
        public decimal? MaxPercentage { get; set; }

        /// <summary>
        /// Lower value of some characteristic or property.
        /// schema:minValue
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Upper value of some characteristic or property.
        /// schema:maxValue
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Mean value.
        /// qdata:mean
        /// </summary>
        public decimal? Mean { get; set; }

        /// <summary>
        /// Median salary value.
        /// qdata:median
        /// </summary>
        public decimal? Median { get; set; }

        /// <summary>
        /// Quotient of two values of the data set, expressed as a percentage.
        /// qdata:percentage
        /// </summary>
        public decimal? Percentage { get; set; }

        /// <summary>
        /// 10th percentile salary value.
        /// qdata:percentile10
        /// </summary>
        public decimal? Percentile10 { get; set; }

        /// <summary>
        /// 25th percentile salary value.
        /// qdata:percentile25
        /// </summary>
        public decimal? Percentile25 { get; set; }

        /// <summary>
        /// 75th percentile salary value.
        /// qdata:percentile75
        /// </summary>
        public decimal? Percentile75 { get; set; }

        /// <summary>
        /// 90th percentile salary value.
        /// qdata:percentile90
        /// </summary>
        public decimal? Percentile90 { get; set; }

        /// <summary>
        /// Number of subjects from the population for which data was obtained to calculate the observed value.
        /// qdata:sizeOfData
        /// </summary>
        public int? SizeOfData { get; set; }

        /// <summary>
        ///  Number of subjects in the sample for which no data was obtained.
        /// qdata:sizeOfNoData
        /// </summary>
        public int? SizeOfNoData { get; set; }

        /// <summary>
        ///  The total number of subjects in the relevant population.
        /// qdata:sizeOfPopulation
        /// </summary>
        public int? SizeOfPopulation { get; set; }

        /// <summary>
        ///  Standard deviation about the mean.
        /// qdata:standardDeviation
        /// </summary>
        public decimal? StandardDeviation { get; set; }

        /// <summary>
        /// Value of a monetary amount or a quantitative value.
        /// schema:value
        /// </summary>
        public decimal? Value { get; set; }

    }
}
