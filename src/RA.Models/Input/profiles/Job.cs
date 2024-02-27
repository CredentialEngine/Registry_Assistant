using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Set of responsibilities based on work roles within an occupation as defined by an employer.
	/// </summary>
	public class Job : BaseEmploymentToWorkObject
	{
		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "Job";

		#region Required
		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID) by which the creator, owner or provider of a resource recognizes it in transactions with the external environment (e.g., in verifiable claims involving the resource).
		/// required
		/// - CTID
		/// - NAME
		/// <see cref="https://credreg.net/ctdl/terms/ctid"/>
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name of this Job
		/// Required
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Profile Description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Organization(s) that offer this resource
		/// Required
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }
		#endregion

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();


		/// <summary>
		/// AbilityEmbodied
		/// Enduring attributes of the individual that influence performance are embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:abilityEmbodied
		/// </summary>
		public List<string> AbilityEmbodied { get; set; } = new List<string>();

        /// <summary>
        /// Category or classification of this resource.
        /// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
        /// URI to a concept(based on the ONet work activities example)
        /// skos:Concept
        /// or Blank nodes!
        /// </summary>
        public List<string> Classification { get; set; } = new List<string>();

		/// <summary>
		/// Additional Classification
		/// List of concepts that don't exist in the registry. Will be published as blank nodes
		/// </summary>
		public List<CredentialAlignmentObject> AdditionalClassification { get; set; } = new List<CredentialAlignmentObject>();

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies an item and supports its discovery and use.
		/// ceterms:codedNotation
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Comment
		/// Definition:	en-US: Supplemental text provided by the promulgating body that clarifies the nature, scope or use of this competency.
		/// ceasn:comment
		/// </summary>
		public List<string> Comment { get; set; } = new List<string>();
		public LanguageMapList Comment_map { get; set; } = new LanguageMapList();

        /// <summary>
        /// Occupation related to this resource.
        /// CTID for an existing Occupation
		/// NEW - handle blank nodes
        /// ceterms:hasOccupation
        /// </summary>
        public List<string> HasOccupation { get; set; } = new List<string>();

		/// <summary>
		/// Rubric related to this resource.
		/// <see cref="https://credreg.net/ctdl/terms/hasRubric"/>
		/// ceterms:hasRubric
		/// </summary>
		public List<string> HasRubric { get; set; } = new List<string>();

        /// <summary>
        /// Reference to a relevant support service.
        /// List of CTIDs that reference one or more published support services
        /// </summary>
        public List<string> HasSupportService { get; set; }

		/// <summary>
		/// Task related to this resource.
		/// CTID for an existing Task
		/// NEW - handle blank nodes
		/// <see cref="https://credreg.net/ctdl/terms/hasTask"/>
		/// ceterms:hasTask
		/// </summary>
		public List<string> HasTask { get; set; } = new List<string>();

		/// <summary>
		/// Work Role related to this resource.
		/// CTID for an existing WorkRole
		/// NEW - handle blank nodes
		/// ceterms:hasWorkRole
		/// </summary>
		public List<string> HasWorkRole { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

        ///// <summary>
        ///// Is Member Of
        ///// Collection to which this resource belongs.
        ///// Inverse property that cannot be used here. 
        ///// </summary>
        //public List<string> IsMemberOf { get; set; } = new List<string>();

        #region Industry type and helpers
        /// <summary>
        /// IndustryType
        /// Type of industry; select from an existing enumeration of such types such as the SIC, NAICS, and ISIC classifications.
        /// Best practice in identifying industries for U.S. credentials is to provide the NAICS code using the ceterms:naics property. 
        /// Other credentials may use the ceterms:industrytype property and any framework of the class ceterms:IndustryClassification.
        /// ceterms:industryType
        /// </summary>
        public List<FrameworkItem> IndustryType { get; set; } = new List<FrameworkItem>();
		/// <summary>
		/// AlternativeIndustryType
		/// Industries that are not found in a formal framework can be still added using AlternativeIndustryType. 
		/// Any industries added using this property will be added to or appended to the IndustryType output.
		/// </summary>
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		/// <summary>
		/// Language map list for AlternativeIndustryType
		/// </summary>
		public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid NAICS codes. These will be mapped to industry type
		/// See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> NaicsList { get; set; } = new List<string>();
		#endregion

		/// <summary>
		/// Body of information embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:knowledgeEmbodied
		/// </summary>
		public List<string> KnowledgeEmbodied { get; set; } = new List<string>();


		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// ceterms:keyword
		/// </summary>
		public List<string> Keyword { get; set; } = new List<string>();
		/// <summary>
		/// or provide via a LanguageMapList
		/// </summary>
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		#region OccupationType and related helpers
		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net. 
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		///  ceterms:occupationType
		/// </summary>
		public List<FrameworkItem> OccupationType { get; set; } = new List<FrameworkItem>();
		/// <summary>
		/// AlternativeOccupationType
		/// Occupations that are not found in a formal framework can be still added using AlternativeOccupationType. 
		/// Any occupations added using this property will be added to or appended to the OccupationType output.
		/// </summary>
		public List<string> AlternativeOccupationType { get; set; } = new List<string>();
		/// <summary>
		/// Language map list for AlternativeOccupationType
		/// </summary>
		public LanguageMapList AlternativeOccupationType_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// The API will validate and format the ONet codes as Occupations
		/// </summary>
		public List<string> ONET_Codes { get; set; } = new List<string>();
		#endregion



        //Free floating Concepts 

        /// <summary>
        /// Environmental Hazard Type
        /// Type of condition in the physical work performance environment that entails risk exposures requiring mitigating processes; 
        /// select from an existing enumeration of such types.
        /// skos:Concept
        /// Blank nodes!
        /// </summary>
        public List<string> EnvironmentalHazardType { get; set; } = new List<string>();

        /// <summary>
        /// Type of required or expected human performance level; select from an existing enumeration of such types.
        /// skos:Concept
        /// Blank nodes!
        /// </summary>
        public List<string> PerformanceLevelType { get; set; } = new List<string>();

        /// <summary>
        /// Type of physical activity required or expected in performance; select from an existing enumeration of such types.
        /// skos:Concept
        /// Blank nodes!
        /// </summary>
        public List<string> PhysicalCapabilityType { get; set; } = new List<string>();

        /// <summary>
        /// Type of required or expected sensory capability; select from an existing enumeration of such types.
        /// skos:Concept
        /// Blank nodes!
        /// </summary>
        public List<string> SensoryCapabilityType { get; set; } = new List<string>();

        /// <summary>
        /// Requirement or set of requirements for this resource
        /// </summary>
        public List<ConditionProfile> Requires { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// ceterms:sameAs
		/// </summary>
		public List<string> SameAs { get; set; } = new List<string>();

		/// <summary>
		///Ability to apply knowledge and use know-how to complete tasks and solve problems including types or categories of developed proficiency or dexterity in mental operations and physical processes is embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:abilityEmbodied
		/// </summary>
		public List<string> SkillEmbodied { get; set; } = new List<string>();

		/// <summary>
		/// Subject Webpage
		/// URL
		/// Optional
		/// ceterms:subjectWebpage
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// ceterms:versionIdentifier
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();
		/// <summary>
		/// This resource provides transfer value for the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		public List<string> ProvidesTransferValueFor { get; set; } = new List<string>();

		/// <summary>
		/// This resource receives transfer value from the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		public List<string> ReceivesTransferValueFrom { get; set; } = new List<string>();


	}
}
