using System.Collections.Generic;

namespace RA.Models.Input
{
	/// <summary>
	/// Profession, trade, or career field that may involve training and/or a formal qualification.
	/// requires
	/// - CTID
	/// - NAME
	/// - Description
	/// </summary>
	public class Occupation : BaseEmploymentToWorkObject
	{
		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "Occupation";

		#region Required
		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// Required
		/// <see cref="https://credreg.net/ctdl/terms/ctid"/>
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name of this Occupation
		/// Required
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; }

		/// <summary>
		/// Profile Description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; }


		/// <summary>
		/// Agent making a statement based on fact or belief.
		/// Required
		/// </summary>
		public List<OrganizationReference> AssertedBy { get; set; } = new List<OrganizationReference>();
		#endregion

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } 
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } 


		/// <summary>
		/// AbilityEmbodied
		/// Enduring attributes of the individual that influence performance are embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:abilityEmbodied
		/// </summary>
		public List<string> AbilityEmbodied { get; set; } 

		/// <summary>
		/// Category or classification of this resource.
		/// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
		/// URI to a concept(based on the ONet work activities example) or to a blank node in RA.Models.Input.BaseRequest.ReferenceObjects
		/// ceterms:classification
		/// </summary>
		public List<string> Classification { get; set; } 
		
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
		public List<string> Comment { get; set; } 
		public LanguageMapList Comment_map { get; set; } 

		/// <summary>
		/// Job related to this resource.
		/// CTID for an existing Job
		/// ceterms:hasJob
		/// </summary>
		public List<string> HasJob { get; set; }

		/// <summary>
		/// More specialized profession, trade, or career field that is encompassed by the one being described.
		/// CTID for an existing Occupation
		/// <see cref="https://credreg.net/ctdl/terms/hasSpecialization"/>
		/// ceterms:hasSpecialization
		/// </summary>
		public List<string> HasSpecialization { get; set; } 

		/// <summary>
		/// Task related to this resource.
		/// CTID for an existing Task
		/// </summary>
		public List<string> HasTask { get; set; } 

		/// <summary>
		/// Work Role related to this resource.
		/// CTID for an existing WorkRole
		/// ceterms:hasWorkRole
		/// </summary>
		public List<string> HasWorkRole { get; set; }

		/// <summary>
		/// Indicates the level of demand for a resource via a demand level action.
		/// Range Includes:	ceterms:WorkforceDemandAction
		/// </summary>
		public List<string> HasWorkforceDemand{ get; set; } 

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } 

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
		/// Less specialized profession, trade, or career field that encompasses the one being described.
		/// CTID for an existing Occupation
		/// ceterms:isSpecializationOf
		/// </summary>
		public List<string> IsSpecializationOf { get; set; }

		/// <summary>
		/// Body of information embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:knowledgeEmbodied
		/// </summary>
		public List<string> KnowledgeEmbodied { get; set; } 


		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// ceterms:keyword
		/// </summary>
		public List<string> Keyword { get; set; } 
		/// <summary>
		/// or provide via a LanguageMapList
		/// </summary>
		public LanguageMapList Keyword_Map { get; set; } 

		#region Occupation type and related helpers
		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net. 
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		///  ceterms:occupationType
		/// </summary>
		public List<FrameworkItem> OccupationType { get; set; } 
		/// <summary>
		/// AlternativeOccupationType
		/// Occupations that are not found in a formal framework can be still added using AlternativeOccupationType. 
		/// Any occupations added using this property will be added to or appended to the OccupationType output.
		/// </summary>
		public List<string> AlternativeOccupationType { get; set; } 
		/// <summary>
		/// Language map list for AlternativeOccupationType
		/// </summary>
		public LanguageMapList AlternativeOccupationType_Map { get; set; } 

		/// <summary>
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// The API will validate and format the ONet codes as Occupations
		/// </summary>
		public List<string> ONET_Codes { get; set; } 
		#endregion

		/// <summary>
		/// Requirement or set of requirements for this resource
		/// </summary>
		public List<ConditionProfile> Requires { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// HINT: If the SameAs target is a resource in the Credential Registry, just the CTID needs to be provided. 
		/// ceterms:sameAs
		/// </summary>
		public List<string> SameAs { get; set; } 

		/// <summary>
		///Ability to apply knowledge and use know-how to complete tasks and solve problems including types or categories of developed proficiency or dexterity in mental operations and physical processes is embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:skillEmbodied
		/// </summary>
		public List<string> SkillEmbodied { get; set; } 

		/// <summary>
		/// Subject Webpage
		/// URL
		/// ceterms:subjectWebpage
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// Environmental Hazard Type
		/// Type of condition in the physical work performance environment that entails risk exposures requiring mitigating processes; 
		/// select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> EnvironmentalHazardType { get; set; }

		/// <summary>
		/// Type of required or expected human performance level; select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> PerformanceLevelType { get; set; } 

		/// <summary>
		/// Type of physical activity required or expected in performance; select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> PhysicalCapabilityType { get; set; } 

		/// <summary>
		/// Type of required or expected sensory capability; select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> SensoryCapabilityType { get; set; } 
		/// <summary>
		/// This resource provides transfer value for the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		public List<string> ProvidesTransferValueFor { get; set; } 

		/// <summary>
		/// This resource receives transfer value from the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		public List<string> ReceivesTransferValueFrom { get; set; } 
	}
}
