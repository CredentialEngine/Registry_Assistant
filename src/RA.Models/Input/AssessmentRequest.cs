using System;
using System.Collections.Generic;

namespace RA.Models.Input
{
	public class AssessmentRequest : BaseRequest
	{
		public AssessmentRequest()
		{
			Assessment = new Assessment();
		}

		public Assessment Assessment { get; set; }

	}
	/// <summary>
	/// Assessment request input class
	/// History
	/// 2021-05-31 CodedNotation is no longer part of the AssessmentProfile class. The property will be left in the input class until later this year and then removed. If provided, the data will be moved to the Identifier property and a warning will be returned. 
	/// </summary>
	public class Assessment
	{
		public Assessment()
		{
			Subject = new List<string>();
			Keyword = new List<string>();

			AssessmentMethodType = new List<string>();
			AudienceType = new List<string>();
			AssessmentUseType = new List<string>();

			AvailabilityListing = new List<string>();
			AvailableOnlineAt = new List<string>();
			Jurisdiction = new List<Input.Jurisdiction>();
			//JurisdictionAssertions = new List<JurisdictionAssertedInProfile>();
			DeliveryType = new List<string>();

			EstimatedCost = new List<CostProfile>();
			EstimatedDuration = new List<DurationProfile>();
			//
			ScoringMethodType = new List<string>();

			AccreditedBy = new List<Input.OrganizationReference>();
			ApprovedBy = new List<Input.OrganizationReference>();
			OfferedBy = new List<Input.OrganizationReference>();
			RecognizedBy = new List<Input.OrganizationReference>();
			RegulatedBy = new List<Input.OrganizationReference>();

			Corequisite = new List<ConditionProfile>();
			Recommends = new List<ConditionProfile>();
			Requires = new List<ConditionProfile>();
			EntryCondition = new List<ConditionProfile>();

			AdministrationProcess = new List<ProcessProfile>();
			DevelopmentProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();

			Assesses = new List<CredentialAlignmentObject>();
			//RequiresCompetency = new List<CredentialAlignmentObject>();

			AvailableAt = new List<Place>();

			AdvancedStandingFrom = new List<Connections>();
			IsAdvancedStandingFor = new List<Connections>();
			PreparationFrom = new List<Connections>();
			IsPreparationFor = new List<Connections>();
			IsRecommendedFor = new List<Connections>();
			IsRequiredFor = new List<Connections>();

			ExternalResearch = new List<string>();
			InLanguage = new List<string>();
			CommonConditions = new List<string>();
			CommonCosts = new List<string>();
			VersionIdentifier = new List<IdentifierValue>();

		}
		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "AssessmentProfile";

		#region *** Required Properties ***
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Assessment Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Credential Identifier
		/// format: 
		/// ce-UUID (guid)
		/// Required
		/// </summary>
		public string CTID { get; set; }
		//original API used the following property. Both are supported but of course only one should be provided. CTID will take precedence. 
		public string Ctid { get; set; }

		public string SubjectWebpage { get; set; } //URL

		#region at least one of

		/// <summary>
		/// Organization that owns this resource
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();
		//OR
		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }
		#endregion

		#region at least one of the following
		public List<string> AvailableOnlineAt { get; set; } //URL
		public List<string> AvailabilityListing { get; set; } //URL
		public List<Place> AvailableAt { get; set; }
		#endregion

		#endregion

		#region *** Recommended Benchmark ***
		/// <summary>
		///  Competency evaluated through the assessment.
		///  
		/// </summary>
		public List<CredentialAlignmentObject> Assesses { get; set; }

		/// <summary>
		/// Type of method used to conduct an assessment; select from an existing enumeration of such types.
		/// assessMethod:Artifact, assessMethod:Exam, assessMethod:Performance
		/// <see cref="https://credreg.net/ctdl/terms/AssessmentMethod"/>
		/// </summary>
		public List<string> AssessmentMethodType { get; set; } = new List<string>();
		/// <summary>
		/// Type of means by which a learning opportunity or assessment is delivered to credential seekers and by which they interact; select from an existing enumeration of such types.
		/// deliveryType:BlendedDelivery deliveryType:InPerson deliveryType:OnlineOnly
		/// <see cref="https://credreg.net/ctdl/terms/Delivery"/>
		/// </summary>
		public List<string> DeliveryType { get; set; }
		/// <summary>
		/// Detailed description of the delivery type of an assessment or learning opportunity.
		/// </summary>
		public string DeliveryTypeDescription { get; set; }
		public LanguageMap DeliveryTypeDescription_Map { get; set; } = new LanguageMap();
		#endregion

		//=========== optional ================================

		/// <summary>
		/// Assessment Method Description 
		/// Description of the assessment methods for a resource.
		/// </summary>
		public string AssessmentMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap AssessmentMethodDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Learning Method Description 
		///  Description of the learning methods for a resource.
		/// 
		/// </summary>
		public string LearningMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap LearningMethodDescription_Map { get; set; } = new LanguageMap();


		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Expiration date of the assessment if applicable
		/// </summary>
		public string ExpirationDate { get; set; }

		//List of language codes. ex: en, es
		public List<string> InLanguage { get; set; }

		public List<DurationProfile> EstimatedDuration { get; set; }
		public List<ConditionProfile> Requires { get; set; }

		/// <summary>
		/// Provide credit information in a ValueProfile value
		/// A credit-related value.
		/// 21-07-19 - updating Creditvalue to also allow a list. It is defined as an object. The API will accept either a ValueProfile object or List of ValueProfiles
		/// 12-08-18 - Changing permantly to the List, as only existing use was from the publisher (and the latter is updated to use the list)
		/// </summary>
		//public object CreditValue { get; set; } 
		public List<ValueProfile> CreditValue { get; set; } = new List<ValueProfile>();

		//
		/// <summary>
		/// Detailed description of credit unit. 
		/// Recommendation is to use CreditValue rather than this property.
		/// </summary>
		public string CreditUnitTypeDescription { get; set; }
		public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();

		//
		public List<string> Keyword { get; set; }
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();
		public List<string> Subject { get; set; }
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

		public string AssessmentExample { get; set; }
		public List<string> AssessmentUseType { get; set; }
	

		/// <summary>
		/// Type of official status of the TransferProfile; select from an enumeration of such types.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies an item and supports its discovery and use.
		/// ceterms:codedNotation
		/// </summary>
		[Obsolete]
		public string CodedNotation { get; set; }

		public string AssessmentExampleDescription { get; set; }
		public LanguageMap AssessmentExampleDescription_Map { get; set; } = new LanguageMap();
		public string AssessmentOutput { get; set; }
		public LanguageMap AssessmentOutput_Map { get; set; } = new LanguageMap();

		public List<Jurisdiction> Jurisdiction { get; set; }


		public string ProcessStandards { get; set; }

		public string ProcessStandardsDescription { get; set; }
		public LanguageMap ProcessStandardsDescription_Map { get; set; } = new LanguageMap();

		#region Occupations, Industries, and instructional programs
		//=====================================================================
		//List of occupations from a published framework, that is with a web URL
		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net. 
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		/// </summary>
		public List<FrameworkItem> OccupationType { get; set; }
		/// <summary>
		/// AlternativeOccupationType
		/// Occupations that are not found in a formal framework can be still added using AlternativeOccupationType. 
		/// Any occupations added using this property will be added to or appended to the OccupationType output.
		/// </summary>
		public List<string> AlternativeOccupationType { get; set; } = new List<string>();
		/// <summary>
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// The API will validate and format the ONet codes as Occupations
		/// </summary>
		public List<string> ONET_Codes { get; set; } = new List<string>();

		//=============================================================================
		/// <summary>
		/// IndustryType
		/// Type of industry; select from an existing enumeration of such types such as the SIC, NAICS, and ISIC classifications.
		/// Best practice in identifying industries for U.S. credentials is to provide the NAICS code using the ceterms:naics property. 
		/// Other credentials may use the ceterms:industrytype property and any framework of the class ceterms:IndustryClassification.
		/// </summary>
		public List<FrameworkItem> IndustryType { get; set; }
		/// <summary>
		/// AlternativeIndustryType
		/// Industries that are not found in a formal framework can be still added using AlternativeIndustryType. 
		/// Any industries added using this property will be added to or appended to the IndustryType output.
		/// </summary>
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid NAICS codes. These will be mapped to industry type
		/// See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> NaicsList { get; set; } = new List<string>();

		//=============================================================================
		/// <summary>
		/// InstructionalProgramType
		/// Type of instructional program; select from an existing enumeration of such types.
		/// </summary>
		public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();

		/// <summary>
		/// AlternativeInstructionalProgramType
		/// Programs that are not found in a formal framework can be still added using AlternativeInstructionalProgramType. 
		/// Any programs added using this property will be added to or appended to the InstructionalProgramType output.
		/// </summary>
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();

		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion
		//
		public bool? IsProctored { get; set; }
		public bool? HasGroupEvaluation { get; set; }
		public bool? HasGroupParticipation { get; set; }

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();
		//external classes
		public List<CostProfile> EstimatedCost { get; set; }

		/// <summary>
		/// The type of credential seeker for whom the entity is applicable; select from an existing enumeration of such types.
		/// audience:Citizen audience:CurrentMilitary audience:CurrentMilitaryDependent audience:CurrentMilitarySpouse audience:CurrentStudent audience:FormerMilitary audience:FormerMilitaryDependent audience:FormerMilitarySpouse audience:FormerStudent audience:FullTime audience:Member audience:NonCitizen audience:NonMember audience:NonResident audience:PartTime audience:PrivateEmployee audience:PublicEmployee audience:Resident
		/// <see cref="https://credreg.net/ctdl/terms/Audience"/>
		/// </summary>
		public List<string> AudienceType { get; set; }
		/// <summary>
		/// Type of level indicating a point in a progression through an educational or training context, for which the credential is intended; select from an existing enumeration of such types.
		/// audLevel:AdvancedLevel audLevel:AssociatesDegreeLevel audLevel:BachelorsDegreeLevel audLevel:BeginnerLevel audLevel:DoctoralDegreeLevel audLevel:GraduateLevel audLevel:IntermediateLevel audLevel:LowerDivisionLevel audLevel:MastersDegreeLevel audLevel:PostSecondaryLevel audLevel:ProfessionalLevel audLevel:SecondaryLevel audLevel:UndergraduateLevel audLevel:UpperDivisionLevel
		/// <see cref="https://credreg.net/ctdl/terms/AudienceLevel"/>
		/// </summary>
		public List<string> AudienceLevelType { get; set; } = new List<string>();
		/// <summary>
		/// Another source of information about the entity being described.
		/// HINT: If the SameAs target is a resource in the Credential Registry, just the CTID needs to be provided. 
		/// ceterms:sameAs
		/// </summary>
		public List<string> SameAs { get; set; } = new List<string>();

		public string ScoringMethodDescription { get; set; }
		public LanguageMap ScoringMethodDescription_Map { get; set; } = new LanguageMap();
		public string ScoringMethodExample { get; set; }
		public string ScoringMethodExampleDescription { get; set; }
		public LanguageMap ScoringMethodExampleDescription_Map { get; set; } = new LanguageMap();
		public List<string> ScoringMethodType { get; set; }

		//Connections
		public List<Connections> AdvancedStandingFrom { get; set; }
		public List<Connections> IsAdvancedStandingFor { get; set; }
		public List<Connections> PreparationFrom { get; set; }
		public List<Connections> IsPreparationFor { get; set; }
		public List<Connections> IsRecommendedFor { get; set; }
		public List<Connections> IsRequiredFor { get; set; }

		#region -- Quality Assurance BY --
		public List<OrganizationReference> AccreditedBy { get; set; }
		public List<OrganizationReference> ApprovedBy { get; set; }

		public List<OrganizationReference> RecognizedBy { get; set; }
		public List<OrganizationReference> RegulatedBy { get; set; }
		#endregion

		#region Quality Assurance IN - Jurisdiction based Quality Assurance  (INs)


		/// <summary>
		/// List of Organizations that accredit this assessment in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> AccreditedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that approve this assessment in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> ApprovedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that offer this assessment in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> OfferedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that recognize this assessment in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RecognizedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that regulate this assessment in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RegulatedIn { get; set; } = new List<JurisdictionAssertion>();

		#endregion

		//conditions
		public List<ConditionProfile> Corequisite { get; set; }
		public List<ConditionProfile> Recommends { get; set; }
		public List<ConditionProfile> EntryCondition { get; set; }

		//Processes
		public List<ProcessProfile> AdministrationProcess { get; set; }
		public List<ProcessProfile> DevelopmentProcess { get; set; }
		public List<ProcessProfile> MaintenanceProcess { get; set; }
		public List<string> ExternalResearch { get; set; }


		//required competencies are handled with condition profiles
		//public List<CredentialAlignmentObject> RequiresCompetency { get; set; }

		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization.
		/// Set of costs maintained at an organizational or sub-organizational level, which apply to this assessment.
		/// </summary>
		public List<string> CommonCosts { get; set; }
		/// <summary>
		/// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
		/// Set constraints, prerequisites, entry conditions, or requirements that are shared across an organization, organizational subdivision, set of credentials, or category of entities and activities.
		/// </summary>
		public List<string> CommonConditions { get; set; }
		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();

		/// <summary>
		/// Assessment that provides direct, indirect, formative or summative evaluation or estimation of the nature, ability, or quality for an entity.
		/// </summary>
		public List<EntityReference> TargetAssessment { get; set; }
		/// <summary>
		/// Learning object or resource that is used as part of an learning activity.
		/// </summary>
		public List<EntityReference> TargetLearningResource1 { get; set; } = new List<EntityReference>();
		public List<string> TargetLearningResource { get; set; } = new List<string>();
		//public List<string> TargetPathway { get; set; } = new List<string>();
		/// <summary>
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; }
	}
}
