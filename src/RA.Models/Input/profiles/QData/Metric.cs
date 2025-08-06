using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Input.profiles.QData
{
	/// <summary>
	/// What is being measured and the method of measurement used for observations within a data set. 
	/// Required: ceterms:ctid, ceasn:publisher
	/// However if the Metric is a blank node, then publisher (and of course CTID) are not required.
	/// </summary>
	public class Metric : BasePrimaryResource
	{
		public Metric()
		{
			Type = "qdata:Metric";
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// Recommended, but can be a blank node.
		/// <see cref="https://credreg.net/ctdl/terms/ctid"/>
		/// ceterms:ctid
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name of this Resource
		/// Optional
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Description
		/// OPTIONAL and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// LanguageMap for Description
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// An agent responsible for making this resource available.
		/// Required where CTID is present for Metric.
		/// Provide one or more CTIDs for an existing publisher resource in the registry. 
		/// ceasn:publisher 
		/// </summary>
		public List<string> Publisher { get; set; }

		/// <summary>
		/// Describes whether and how the Observations have been adjusted for relevant factors.
		/// qdata:adjustment
		/// </summary>
		public string Adjustment { get; set; }

		[JsonProperty( PropertyName = "qdata:adjustment" )]
		public LanguageMap AdjustmentLangMap { get; set; }

		/// <summary>
		/// The entity being described has been modified, extended or refined from the referenced resource.
		/// Range:
		///     ceasn:Competency 
		///     ceasn:CompetencyFramework 
		///     ceasn:Rubric 
		///     ceterms:TransferValueProfile  
		///     qdata:Metric
		/// ceasn:derivedFrom
		/// Concept
		/// </summary>
		public string DerivedFrom { get; set; }

		/// <summary>
		/// Definition of "earnings" used by the data source.
		/// qdata:earningsDefinition
		/// </summary>
		public string EarningsDefinition { get; set; }

		[JsonProperty( PropertyName = "qdata:earningsDefinition" )]
		public LanguageMap EarningsDefinitionLangMap { get; set; }

		/// <summary>
		/// Statement of any work time or earnings threshold used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the time period of the data set.
		/// qdata:earningsThreshold
		/// </summary>
		public string EarningsThreshold { get; set; }

		[JsonProperty( PropertyName = "qdata:earningsThreshold" )]
		public LanguageMap EarningsThresholdLangMap { get; set; }

		/// <summary>
		/// Definition of employment used by the data source.
		/// qdata:employmentDefinition
		/// </summary>
		public string EmploymentDefinition { get; set; }

		[JsonProperty( PropertyName = "qdata:employmentDefinition" )]
		public LanguageMap EmploymentDefinitionLangMap { get; set; }

		/// <summary>
		/// Mechanism by which income is determined; i.e., actual or annualized earnings; select from an enumeration of such types.
		/// Concept in conceptScheme: qdata:IncomeDeterminationMethod
		/// qdata:incomeDeterminationType
		/// </summary>
		public string IncomeDeterminationType { get; set; }

		/// <summary>
		/// Type of phenomenon being measured, select from an existing enumeration of such types.
		/// Concept in ConceptScheme: qdata:MetricCategory
		/// </summary>
		public List<string> MetricType { get; set; }

		/// <summary>
		/// Type of record used to source the data. 
		/// Examples include various official records and self-reporting via a survey.
		/// concept in conceptScheme that doesn't have to be in the registry.
		/// Handle a string that is a CTID, bnodeId, or URI (verification is TBD)
		/// qdata:recordType
		/// </summary>
		public string RecordType { get; set; }

		/// <summary>
		/// Links an individual to an individual; such an owl:sameAs statement indicates that two URI references actually refer to the same thing: the individuals have the same "identity".
		/// NOTE: sameAs applies to every resource and as such will not (likely) be an explicit property!
		/// owl:sameAs
		/// </summary>
		public List<string> SameAs { get; set; }
	}
}
