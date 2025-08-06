using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.Input.profiles.QData
{
	/// <summary>
	/// DataSet Profile
	/// Particular characteristics or properties of a data set and its records.
	/// 
	/// NEW 24-09-30
	/// Required: 
	///     ceterms:ctid, qdata:dataProvider, 
	///     ceterms:description, ceterms:name, 
	///     qdata:dataSetTemporalCoverage, 
	///     qdata:hasMetric, qdata:hasObservation
	///     
	/// qdata:DataSetProfile
	/// <see href="https://credreg.net/qdata/terms/DataSetProfile"/>
	/// </summary>
	public class DataSetProfile
	{

		#region Required

		/// <summary>
		/// CTID - Required 
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name of dataset profile
		/// Required
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///  LanguageMap for Name
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// LanguageMap for Description
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Credentialing organization or a third party providing the data.
		/// Required when publishing the DataSetProfile directly. Otherwise derived from owner of related resource.
		/// </summary>
		public OrganizationReference DataProvider { get; set; } = new OrganizationReference();

		/// <summary>
		/// Data Set Temporal Coverage
		/// Time period covered by the data. 
		/// This property replaces DataSetTimePeriod
		/// Required with new dataSetProfiles after 24-09-30
		/// </summary>
		public DataSetTimeFrame DataSetTemporalCoverage { get; set; }

		/// <summary>
		/// What the observations measure and details of the method used.
		/// A CTID or a blank node
		/// Required with new dataSetProfiles after 24-09-30
		/// </summary>
		public List<string> HasMetric { get; set; }
		// public List<Metric> HasMetricFull { get; set; }

		/// <summary>
		/// The recorded data.
		/// Required with new dataSetProfiles after 24-09-30
		/// </summary>
		public List<string> HasObservation { get; set; }

		#endregion

		#region Recommended

		/// <summary>
		/// Data Set Time Period
		/// Short- and long-term post-award reporting intervals including start and end dates.
		/// NOTE: This property is replaced by DataSetTemporalCoverage
		/// NOTE: after official implementation, only one time period will be allowed
		///     Also any data attributes may be ignored, although attempts may be made to convert them to the new structure.
		/// </summary>
		public List<DataSetTimeFrame> DataSetTimePeriod { get; set; }

		/// <summary>
		/// Data Suppression Policy
		/// Description of a data suppression policy for earnings and employment data when cell size is below a certain threshold to ensure an individual's privacy and security.
		/// </summary>
		public string DataSuppressionPolicy { get; set; }

		[JsonProperty( PropertyName = "ceterms:dataSuppressionPolicy" )]
		public LanguageMap DataSuppressionPolicy_Map { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// URL 
		/// </summary>
		public string Source { get; set; }
		#endregion

		/// <summary>
		/// Data set for the entity being referenced.
		/// Input is typically one or more CTIDs for the related resource, such as a Credential
		/// REQUIRED when dataSetProfile published separately.
		/// Inverse property - points back to the parent resource (to which the dataSetProfile pertains)
		/// 21-02-19 mparsons -	Removing these from range: HoldersProfile, EarningsProfile, EmploymentOutlook
		///					  - adding credential, assessment, and lopp
		/// 21-05-10 mparsons - effectively obsolete outside of HoldersProfile, EarningsProfile, EmploymentOutlook
		///                     and the latter are moving to be obsolete
		///		              - Replaced by About
		/// 24-09-02 THIS IS NOW THE PRIMARY PROPERTY! - it is replacing the use of About!
		/// Range
		///     ceterms:AssessmentProfile
		///     ceterms:Credential (and subclasses)
		///     ceterms:LearningOpportunityProfile( and subclasses )
		///     ceterms:Organization( and subclasses )
		///     ceterms:Industry
		///     ceterms:Job
		///     ceterms:Occupation
		///     ceterms:Pathway
		/// The propery will not be assigned by the API. However the import may encounter very old data that includes this property.
		/// </summary>
		public List<string> RelevantDataSetFor { get; set; }

		/// <summary>
		/// Subject matter of the resource.
		/// Means to point to a credential or other resource where data is published by a third party. 
		/// Allowed: 
		/// - any credential
		/// - assessment profile
		/// - any learning opportunity
		/// - scheduled offering
		/// - 23-10-23 - any organization
		/// REQUIRED when dataSetProfile published separately.
		/// CTID/URI
		/// schema:about
		/// </summary>
		[Obsolete( "24-11-01 About is being replaced by RelevantDataSetFor" )]
		public List<EntityReference> About { get; set; } = new List<EntityReference>();

		/// <summary>
		/// Description of a process by which a resource is administered.
		/// </summary>
		public List<ProcessProfile> AdministrationProcess { get; set; }

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; }

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateNameLangMap { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
		/// URI to a concept(based on the ONet work activities example) or to a blank node in RA.Models.Input.BaseRequest.ReferenceObjects
		/// ceterms:classification
		/// </summary>
		public List<string> Classification { get; set; } = new List<string>();

		/// <summary>
		/// Location or geographic area for a data set. 
		/// This will allow partial addresses. For example a state/region with a country
		/// </summary>
		public List<Place> DataSetSpatialCoverage { get; set; }

		/// <summary>
		/// Effective date of this resource's content.
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		///  Description of a process by which a resource was created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		/// A representation of the data set. 
		/// </summary>
		public List<DataSetDistribution> HasDataSetDistribution { get; set; }

		/// <summary>
		/// A means of accessing one or more data sets or data processing functions.  
		/// </summary>
		public List<DataSetService> HasDataSetService { get; set; }

		/// <summary>
		/// A set of resources or concepts to which the metric is applied or which affect the observation of the metric. 
		/// </summary>
		public List<string> HasDimension { get; set; }

		#region Instruction program and helpers

		/// <summary>
		/// Instructional Program Type
		/// Type of instructional program; select from an existing enumeration of such types.
		/// </summary>
		[Obsolete( "InstructionalProgramType is being deprecated." )]
		public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();

		[Obsolete( "InstructionalProgramType is being deprecated." )]
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();

		/// <summary>
		/// Helper property - provided a list of CIP codes and API will validate and format 
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		[Obsolete( "InstructionalProgramType (and helper properties) is being deprecated." )]
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see href="https://credreg.net/ctdl/terms/identifier">Identifier</see>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// xsd:anyURI
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// The publication status of the of this resource.
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		public string Rights { get; set; }

		public LanguageMap Rights_Map { get; set; }

		/// <summary>
		/// Identification of data point(s) in the data set that describe personal subject attribute(s) used to uniquely identify a subject for the purpose of matching records and an indication of level of confidence in the accuracy of the match.
		/// </summary>
		public string SubjectIdentification { get; set; }

		public LanguageMap SubjectIdentification_Map { get; set; }
	}
}
