using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2.QData
{
	/// <summary>
	/// What is being measured and the method of measurement used for observations within a data set. 
	/// Required: ceterms:ctid, ceasn:publisher
	/// However if the Metric is a blank node, then publisher (and of course CTID) are not required.
	/// </summary>
	public class Metric
	{
		[JsonProperty( "@type" )]
		public string Type { get; set; } = "qdata:Metric";

		[JsonProperty( "@id" )]
		public string Id { get; set; }

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// Recommended, but can be a blank node.
		/// <see cref="https://credreg.net/ctdl/terms/ctid"/>
		/// ceterms:ctid
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		/// <summary>
		/// Name of this Resource
		/// Required
		/// ceterms:name
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// An agent responsible for making this resource available.
		/// Required where CTID is present
		/// ceasn:publisher 
		/// </summary>
		[JsonProperty( PropertyName = "ceasn:publisher" )]
		public List<string> Publisher { get; set; }

		/// <summary>
		/// Describes whether and how the Observations have been adjusted for relevant factors.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:adjustment" )]
		public LanguageMap Adjustment { get; set; }

		/// <summary>
		/// Type of record used to source the data. 
		/// Examples include various official records and self-reporting via a survey.
		/// concept in conceptScheme that doesn't have to be in the registry.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:recordType" )]
		public string RecordType { get; set; }

		/// <summary>
		/// The entity being described has been modified, extended or refined from the referenced resource.
		/// Range:
		/// ceasn:Competency 
		/// ceasn:CompetencyFramework 
		/// ceasn:Rubric 
		/// ceterms:TransferValueProfile  
		/// qdata:Metric - realistically only metric
		/// </summary>
		[JsonProperty( PropertyName = "ceasn:derivedFrom" )]
		public string DerivedFrom { get; set; }

		/// <summary>
		/// Definition of "earnings" used by the data source.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:earningsDefinition" )]
		public LanguageMap EarningsDefinition { get; set; }

		/// <summary>
		/// Statement of any work time or earnings threshold used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the time period of the data set.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:earningsThreshold" )]
		public LanguageMap EarningsThreshold { get; set; }

		/// <summary>
		/// Definition of employment used by the data source.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:employmentDefinition" )]
		public LanguageMap EmploymentDefinition { get; set; }

		/// <summary>
		/// Mechanism by which income is determined; i.e., actual or annualized earnings; select from an enumeration of such types.
		/// Concept in conceptScheme: qdata:IncomeDeterminationMethod
		/// </summary>
		[JsonProperty( PropertyName = "qdata:incomeDeterminationType" )]
		public string IncomeDeterminationType { get; set; }

		/// <summary>
		/// Type of phenomenon being measured, select from an existing enumeration of such types.
		/// Concept in ConceptScheme: qdata:MetricCategory
		/// </summary>
		[JsonProperty( PropertyName = "qdata:metricType" )]
		public List<string> MetricType { get; set; }

		/// <summary>
		/// Links an individual to an individual; such an owl:sameAs statement indicates that two URI references actually refer to the same thing: the individuals have the same "identity".
		/// NOTE: sameAs applies to every resource and as such will not (likely) be an explicit property!
		/// owl:sameAs
		/// </summary>
		[JsonProperty( PropertyName = "owl:sameAs" )]
		public List<string> SameAs { get; set; }
	}
}
