using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2.QData
{
	/// <summary>
	/// DataSet Profile
	/// Particular characteristics or properties of a data set and its records.
	/// qdata:DataSetProfile
	/// <see href="https://credreg.net/qdata/terms/DataSetProfile"/>
	/// </summary>
	public class DataSetProfile : BaseResourceDocument
	{
		[JsonProperty( "@type" )]
		public string Type { get; set; } = "qdata:DataSetProfile";

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Subject matter of the resource.
		/// Means to point to a credential (etc.) where data is published by a third party.
		/// CTID/URI
		/// </summary>
		[JsonProperty( PropertyName = "schema:about" )]
		public List<string> About { get; set; }

		/// <summary>
		/// RelevantDataSetFor is obsolete. 24-09-02 MAYBE NOT
		/// Range
		///     ceterms:Credential (and subclasses)
		///     ceterms:LearningOpportunityProfile( and subclasses )
		///     ceterms:Organization( and subclasses )
		///     ceterms:Industry
		///     ceterms:Occupation
		///     ceterms:AssessmentProfile
		///     ceterms:Pathway
		///     ceterms:Job
		/// The propery will not be assigned by the API. However the import may encounter very old data that includes this property.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:relevantDataSetFor" )]
		public List<string> RelevantDataSetFor { get; set; }

		/// <summary>
		/// Description of a process by which a resource is administered.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:administrationProcess" )]
		public List<ProcessProfile> AdministrationProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		/// <summary>
		/// Instructional Program Type
		/// Type of instructional program; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:instructionalProgramType" )]
		public List<CredentialAlignmentObject> InstructionalProgramType { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// Credentialing organization or a third party providing the data.
		/// URI
		/// </summary>
		[JsonProperty( PropertyName = "qdata:dataProvider" )]
		public string DataProvider { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		/// <summary>
		/// Effective date of this resource's content.
		/// </summary>
		[JsonProperty( "ceterms: dateEffective" )]
		public string DateEffective { get; set; }

		/// <summary>
		///  Description of a process by which a resource was created.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		/// Data Set Temporal Coverage
		/// Time period covered by the data.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:dataSetTemporalCoverage" )]
		public DataSetTimeFrame DataSetTemporalCoverage { get; set; }

		/// <summary>
		/// Data Set Time Period
		/// Short- and long-term post-award reporting intervals including start and end dates.
		/// NOTE: This property is replaced by DataSetTemporalCoverage
		/// NOTE: after official implementation, only one time period will be allowed
		///     Also any data attributes may be ignored, although attempts may be made to convert them to the new structure.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:dataSetTimePeriod" )]
		public List<DataSetTimeFrame> DataSetTimePeriod { get; set; }

		/// <summary>
		/// Location or geographic area for a data set. 
		/// </summary>
		[JsonProperty( PropertyName = "qdata:dataSetSpatialCoverage" )]
		public List<Place> DataSetSpatialCoverage { get; set; }

		/// <summary>
		/// Data Suppression Policy
		/// Description of a data suppression policy for earnings and employment data when cell size is below a certain threshold to ensure an individual's privacy and security.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:dataSuppressionPolicy" )]
		public LanguageMap DataSuppressionPolicy { get; set; }

		/// <summary>
		/// Distribution File
		/// Downloadable form of this dataset, at a specific location, in a specific format.
		/// URL
		/// </summary>
		[JsonProperty( PropertyName = "qdata:distributionFile" )]
		public List<string> DistributionFile { get; set; }

		/// <summary>
		/// A representation of the data set. 
		/// </summary>
		[JsonProperty( "qdata:hasDataSetDistribution" )]
		public List<DataSetDistribution> HasDataSetDistribution { get; set; }

		/// <summary>
		/// A means of accessing one or more data sets or data processing functions. 
		/// </summary>
		[JsonProperty( "qdata:hasDataSetService" )]
		public List<DataSetService> HasDataSetService { get; set; }

		/// <summary>
		/// What the observations measure and details of the method used.
		/// </summary>
		[JsonProperty( "qdata:hasMetric" )]
		public List<string> HasMetric { get; set; } = null;

		/// <summary>
		/// The recorded data.
		/// </summary>
		[JsonProperty( "qdata:hasObservation" )]
		public List<string> HasObservation { get; set; }

		/// <summary>
		/// A set of resources or concepts to which the metric is applied or which affect the observation of the metric. 
		/// </summary>
		[JsonProperty( "qdata:hasDimension" )]
		public List<string> HasDimension { get; set; }

		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue 
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( "ceasn:license" )]
		public string License { get; set; }

		/// <summary>
		/// Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// The publication status of the resource.
		/// </summary>
		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// URL
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:source" )]
		public string Source { get; set; }

		/// <summary>
		/// Identification of data point(s) in the data set that describe personal subject attribute(s) used to uniquely identify a subject for the purpose of matching records and an indication of level of confidence in the accuracy of the match.
		/// </summary>
		[JsonProperty( PropertyName = "qdata:subjectIdentification" )]
		public LanguageMap SubjectIdentification { get; set; }
	}
}
