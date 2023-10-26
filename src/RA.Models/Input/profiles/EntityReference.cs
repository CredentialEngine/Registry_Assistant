using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace RA.Models.Input
{

	/// <summary>
	/// An outline entity used to determine the type of a entity to be processed.
	/// </summary>
	public class BaseEntityReference
	{
		/// <summary>
		/// Id is a resovable URI
		/// If the entity exists in the registry, use the CTID property.
		/// NEW: Coming soon use of ReferenceObjects as a list resources to be blank nodes
		/// 
		/// If not sure of the exact URI, especially if just publishing the entity, then provide the CTID and the API will format the URI.
		/// Alterate URIs are under consideration. For example
		/// http://dbpedia.com/Stanford_University
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Optionally, a CTID can be entered instead of an Id. 
		/// A CTID is recommended for flexibility.
		/// Only enter Id or CTID, but not both.
		/// </summary>
		public string CTID { get; set; }

        //if there is no available Id/CTID, enter the following, where Type, Name, Description, and subjectwebpage would typically be required

        /// <summary>
        /// the type of the entity must be provided if the Id was not provided. examples
        /// ceterms:AssessmentProfile
        /// ceterms:LearningOpportunityProfile
        /// ceterms:ConditionManifest
        /// ceterms:CostManifest
        /// or the many credential subclasses!!
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Type  of CTDL object
        /// </summary>
        [JsonProperty( "@type" )]
        public string CdtlType { get; set; }

        /// <summary>
        /// Name of the entity (normally required)
        /// </summary>
        public string Name { get; set; }
        public string PrefLabel { get; set; }
        public string CompetencyText { get; set; }
        
        /// <summary>
        /// the input classes don't use ceterms, etc. Ensure that each controller sets the default type.
        /// This could be an issue with lists of objects like for TVP and collections.
        /// </summary>
        public bool IsAssessmentType
		{
			get
			{
				if ( Type == "ceterms:AssessmentProfile" || Type == "AssessmentProfile" || Type == "Assessment"
					|| CdtlType == "ceterms:AssessmentProfile" || CdtlType == "AssessmentProfile" || CdtlType == "Assessment"
					)
					return true;
				else
					return false;
			}
		}
		public bool IsCompetencyType
		{
			get
			{
				if ( Type == "ceasn:Competency" || Type == "Competency"
					|| CdtlType == "ceasn:Competency" || CdtlType == "Competency"
                    )
					return true;
				else
					return false;
			}
		}
		//public bool IsCredentialType
		//{
		//	get
		//	{
				
		//		if ( Type == "ceasn:Competency"
		//			|| Type == "Competency"
		//			)
		//			return true;
		//		else
		//			return false;
		//	}
		//}
		public bool IsLearningOpportunityType
		{
			get 
			{
				if ( Type == "ceterms:LearningOpportunityProfile"
					|| Type == "LearningOpportunityProfile"
					|| Type == "Course"
                    || Type == "ceterms:Course"
                    || Type == "LearningProgram"
                    || Type == "ceterms:LearningProgram"
                    )
					return true;
				else
					return false;
			}
		}

		public bool IsJobType
		{
			get
			{
				if ( Type == "Job"
					|| Type == "ceterms:Job"
					)
					return true;
				else
					return false;
			}
		}
		public bool IsOccupationType
		{
			get
			{
				if ( Type == "Occupation"
					|| Type == "ceterms:Occupation"
					)
					return true;
				else
					return false;
			}
		}
		public bool IsTaskType
		{
			get
			{
				if ( Type == "Task"
					|| Type == "ceterms:Task"
					)
					return true;
				else
					return false;
			}
		}
		public bool IsWorkRoleType
		{
			get
			{
				if ( Type == "WorkRole"
					|| Type == "ceterms:WorkRole"
					)
					return true;
				else
					return false;
			}
		}
	}

	/// <summary>
	/// Class for handling references to an entity such as an Assessment, Organization, Learning opportunity, or credential that may or may not be in the Credential Registry.
	/// Either the Id as an resolvable URL, a CTID where the document exists in the Credential Registry, or provide specific properities for the entity.
	/// If neither a CTID or Id is provided, a blank node will be added the @graph.
	/// </summary>
	public class EntityReference
	{
		/// <summary>
		/// Id is a resovable URI
		/// If the entity exists in the registry, provide the URI. 
		/// If not sure of the exact URI, especially if just publishing the entity, then provide the CTID and the API will format the URI.
		/// Alterate URIs are under consideration. For example
		/// http://dbpedia.com/Stanford_University
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Optionally, a CTID can be entered instead of an Id. 
		/// A CTID is recommended for flexibility.
		/// Only enter Id or CTID, but not both.
		/// </summary>
		public string CTID { get; set; }

		//if there is no available Id/CTID, enter the following, where Type, Name, Description, and subjectwebpage would typically be required

		/// <summary>
		/// the type of the entity must be provided if the Id was not provided. examples
		/// ceterms:AssessmentProfile
		/// ceterms:LearningOpportunityProfile
		/// ceterms:ConditionManifest
		/// ceterms:CostManifest
		/// or the many credential subclasses!!
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Name of the entity (required)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// If the entity described below does exist in the registry, use this SameAs property to relate the two. 
		/// Provide a CTID(recommended) or a URI to the thing in the credential registry.
		/// </summary>
		public string SameAs { get; set; }

		/// <summary>
		/// Subject webpage of the entity (required)
		/// This should be for the referenced entity. 
		/// For example, if the reference is for an organization, the subject webpage should be on the organization site.
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Description of the entity (optional)
		/// This should be the general description of the entity. 
		/// For example, for an organization, the description should be about the organization specifically not, how the organization is related to, or interacts with the refering entity. 
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();


		#region Assessment/Lopp related properties
		//2020-05-18 Additional properties have been added to the EntityReference. 
		/*Allowed properties for Assessment
		 * Assesses
		 * AssessmentMethodDescription
		 * AvailableAt
		 * CodedNotation
		 * Description
		 * EstimatedDuration
		 * Keyword
		 * IndustryType
		 * LearningMethodDescription
		 * LearningMethodType
		 * Name
		 * OccupationType
		 * OfferedBy
		 * OwnedBy
		 * Requires, Recommends, Corequisite
		 * Subject
		 * SubjectWebpage
		 * Version
		 */

		/*Allowed properties for LearningOpportunity
		* AssessmentMethodDescription
		* AvailableAt
		* Name
		* CodedNotation
		* CreditValue
		* Description
		* EstimatedDuration
		* Keyword
		* Identifier
		* IndustryType
		* LearningMethodDescription
		* Name
		* OccupationType
		* OfferedBy
		* OwnedBy
		* Requires, Recommends, Corequisite, EntryCondition
		* Subject
		* SubjectWebpage
		* Teaches
		* Version
		*/
		public List<CredentialAlignmentObject> Assesses { get; set; } = new List<CredentialAlignmentObject>();

		public List<string> AssessmentMethodType { get; set; } = new List<string>();

		/// <summary>
		/// Assessment Method Description 
		/// Description of the assessment methods for a resource.
		/// </summary>
		public string AssessmentMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap AssessmentMethodDescription_Map { get; set; } = new LanguageMap();
		//
		public List<Place> AvailableAt { get; set; } = new List<Place>();
		//
		public string CodedNotation { get; set; }
		//both
		public List<string> DeliveryType { get; set; } = new List<string>();
		//condition profiles
		public List<ConditionProfile> Requires { get; set; } = new List<ConditionProfile>();
		public List<ConditionProfile> Corequisite { get; set; } = new List<ConditionProfile>();
		public List<ConditionProfile> Recommends { get; set; } = new List<ConditionProfile>();
		public List<ConditionProfile> EntryCondition { get; set; } = new List<ConditionProfile>();
		//
		/// <summary>
		/// Assertions that recognize this entity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RecognizedIn { get; set; } = new List<JurisdictionAssertion>();
		//LearningOpportunity only
		public ValueProfile CreditValue { get; set; } //= new ValueProfile();
		//
		public List<DurationProfile> EstimatedDuration { get; set; } = new List<DurationProfile>();

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();


		public List<string> Keyword { get; set; } = new List<string>();
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Learning Method Description 
		///  Description of the learning methods for a resource.		/// 
		/// </summary>
		public string LearningMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap LearningMethodDescription_Map { get; set; } = new LanguageMap();
		public List<string> LearningMethodType { get; set; } = new List<string>();
		//
		/// <summary>
		/// Organization(s) that offer this resource
		/// /// Can't initialize this or will be loop
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }// = new List<OrganizationReference>();
		//
		/// <summary>
		/// Organization(s) that owns this resource
		/// Can't initialize this or will be loop
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }// = new List<OrganizationReference>();
																//
		public List<string> Subject { get; set; } = new List<string>();
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// For Learning Opportunities only
		/// Start Date of the Learning opportunity
		/// </summary>
		public string DateEffective { get; set; }
		/// <summary>
		/// For Learning Opportunities only
		/// Expiration date of the learning opportunity if applicable
		/// </summary>
		public string ExpirationDate { get; set; }
		//
		/// <summary>
		/// For Learning Opportunities only
		/// List of competencies being taught
		/// </summary>
		public List<CredentialAlignmentObject> Teaches { get; set; } = new List<CredentialAlignmentObject>();
		#endregion

		#region properties for OccupationType, IndustryType
		/*
		 * Name
		 * CodedNotation
		 * Description
		 * Keyword
		 * OccupationType
		 * IndustryType
		 * SubjectWebpage
		 * Version
		 */

		/// <summary>
		/// Update handle of FrameworkItem to be like EntityReference - all in one, rather than separate property for Alternate and codes
		/// </summary>
		public List<FrameworkItem> OccupationType { get; set; } = new List<FrameworkItem>();

		public List<FrameworkItem> IndustryType { get; set; } = new List<FrameworkItem>();


		/// <summary>
		/// Future use? - don't use yet
		/// </summary>
		public string Version { get; set; } //URL
		#endregion

		/// <summary>
		/// Check if all properties for a reference request are present
		/// 17-08-27 We do need a type if only providing reference data
		/// </summary>
		/// <returns></returns>
		public bool HasNecessaryProperties()
		{
			//	|| string.IsNullOrWhiteSpace( Description )
			if ( (string.IsNullOrWhiteSpace( Name ) || ( Name_Map == null || Name_Map?.Count == 0) )
				|| string.IsNullOrWhiteSpace( Type )
				//|| string.IsNullOrWhiteSpace( SubjectWebpage )		//TODO in some cases not requiring SWP. May want a type specific create
				)
				return false;
			else
				return true;
		}
		public virtual bool IsEmpty()
		{
			if ( string.IsNullOrWhiteSpace( Id )
				&& string.IsNullOrWhiteSpace( CTID )
				&& ( string.IsNullOrWhiteSpace( Name ) && ( Name_Map == null || Name_Map?.Count == 0 ) )
				&& ( string.IsNullOrWhiteSpace( Description ) && ( Description_Map == null || Description_Map.Count == 0 ) )
				//&& string.IsNullOrWhiteSpace( SubjectWebpage )
				)
				return true;
			else
				return false;
		}
	}
}
