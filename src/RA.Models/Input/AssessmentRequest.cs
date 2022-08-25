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
	public class Assessment : BaseRequestClass
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
		/// <summary>
		/// Name or title of the resource.
		/// Required
		/// </summary>
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

		/// <summary>
		/// Webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }
		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages; e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// List of language codes. ex: en, es
		/// </summary>
		public List<string> InLanguage { get; set; }
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

		/// <summary>
		/// Online location where the credential, assessment, or learning opportunity can be pursued.
		/// URL
		/// </summary>
		public List<string> AvailableOnlineAt { get; set; } 

		/// <summary>
		/// Listing of online and/or physical locations where a credential can be pursued.
		/// URL
		/// </summary>
		public List<string> AvailabilityListing { get; set; } 


		/// <summary>
		/// Physical location where the credential, assessment, or learning opportunity can be pursued.
		/// Place
		/// </summary>
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
		/// Example assessment or assessment item.
		/// URI
		/// </summary>
		public string AssessmentExample { get; set; }

		/// <summary>
		/// Text of an example assessment or assessment item.
		/// </summary>
		public string AssessmentExampleDescription { get; set; }
		/// <summary>
		/// Language map for Text of an example assessment or assessment item.
		/// </summary>
		public LanguageMap AssessmentExampleDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Description of the assessment artifact, performance or examination.
		/// </summary>
		public string AssessmentOutput { get; set; }
		/// <summary>
		/// Language map for Description of the assessment artifact, performance or examination.
		/// </summary>
		public LanguageMap AssessmentOutput_Map { get; set; } = new LanguageMap();


		/// <summary>
		/// Type of intended use of the assessment; select from an existing enumeration of such types.
		/// Concepts
		///  <see href="https://credreg.net/ctdl/terms/AssessmentUse">ceterms:AssessmentUse</see>
		/// </summary>
		public List<string> AssessmentUseType { get; set; }

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
		//

		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization.
		/// Set of costs maintained at an organizational or sub-organizational level, which apply to this assessment.
		/// </summary>
		public List<string> CommonCosts { get; set; }

		/// <summary>
		/// Detailed description of credit unit. 
		/// Recommendation is to use CreditValue rather than this property.
		/// </summary>
		public string CreditUnitTypeDescription { get; set; }
		/// <summary>
		/// Language map for CreditUnitTypeDescription
		/// </summary>
		public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Provide credit information in a ValueProfile value
		/// A credit-related value.
		/// 21-07-19 - updating Creditvalue to also allow a list. It is defined as an object. The API will accept either a ValueProfile object or List of ValueProfiles
		/// 21-08-18 - Changing permantly to the List, as only existing use was from the publisher (and the latter is updated to use the list)
		/// </summary>
		public List<ValueProfile> CreditValue { get; set; } = new List<ValueProfile>();

	
		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Estimated cost of a credential, learning opportunity or assessment.
		/// </summary>
		public List<CostProfile> EstimatedCost { get; set; }

		/// <summary>
		/// Estimated time it will take to complete a credential, learning opportunity or assessment.
		/// </summary>
		public List<DurationProfile> EstimatedDuration { get; set; }

		/// <summary>
		/// Expiration date of the assessment if applicable
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Research that supports or validates one or more aspects of the entity.
		/// </summary>
		public List<string> ExternalResearch { get; set; }

		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();

		/// <summary>
		/// Whether or not the assessment activity is scored as a group.
		/// </summary>
		public bool? HasGroupEvaluation { get; set; }
		/// <summary>
		/// Whether or not two or more participants are required to complete the assessment activity.
		/// </summary>
		public bool? HasGroupParticipation { get; set; }
		//
		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// Is Non-Credit
		/// Resource carries or confers no official academic credit towards a program or a credential.
		/// </summary>
		public bool? IsNonCredit { get; set; }

		/// <summary>
		/// Whether or not the assessment is supervised or monitored by an agent.
		/// </summary>
		public bool? IsProctored { get; set; }
		//
		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"></see>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; }

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; }
		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Learning Method Description 
		///  Description of the learning methods for a resource.
		/// </summary>
		public string LearningMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap LearningMethodDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Required
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; } = "lifeCycle:Active";


		/// <summary>
		/// Webpage or online document that describes the criteria, standards, and/or requirements used with a process.
		/// </summary>
		public string ProcessStandards { get; set; }
		/// <summary>
		/// Textual description of the criteria, standards, and/or requirements used with a process
		/// </summary>
		public string ProcessStandardsDescription { get; set; }
		/// <summary>
		/// Language map - Textual description of the criteria, standards, and/or requirements used with a process
		/// </summary>
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
		/// Language map list for AlternativeOccupationType
		/// </summary>
		public LanguageMapList AlternativeOccupationType_Map { get; set; } = new LanguageMapList();
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
		/// Language map list for AlternativeInstructionalProgramType
		/// </summary>
		public LanguageMapList AlternativeInstructionalProgramType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion


		/// <summary>
		/// Another source of information about the entity being described.
		/// HINT: If the SameAs target is a resource in the Credential Registry, just the CTID needs to be provided. 
		/// ceterms:sameAs
		/// </summary>
		public List<string> SameAs { get; set; } = new List<string>();

		/// <summary>
		/// Textual description of the method used to score the assessment.
		/// </summary>
		public string ScoringMethodDescription { get; set; }
		/// <summary>
		/// Language map for ScoringMethodDescription
		/// </summary>
		public LanguageMap ScoringMethodDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Webpage or online document providing an example of the method or tool used to score the assessment.
		/// </summary>
		public string ScoringMethodExample { get; set; }
		/// <summary>
		/// Textual example of the method or tool used to score the assessment.
		/// </summary>
		public string ScoringMethodExampleDescription { get; set; }
		/// <summary>
		/// Language map for ScoringMethodExampleDescription
		/// </summary>
		public LanguageMap ScoringMethodExampleDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Type of method used to score the assessment; select from an existing enumeration of such types.
		/// Concept
		/// <see href="https://credreg.net/ctdl/terms/ScoringMethod">ceterms:ScoringMethod</see>
		/// </summary>
		public List<string> ScoringMethodType { get; set; }

		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// </summary>
		public List<string> Subject { get; set; } = new List<string>();
		/// <summary>
		/// Language map list for Subject
		/// </summary>
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();


		#region Conditions and connections
		//Connection Profiles are Condition Profiles but typically only a subject of the Condition Profile properties are used. 
		/// <summary>
		/// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
		/// Set constraints, prerequisites, entry conditions, or requirements that are shared across an organization, organizational subdivision, set of credentials, or category of entities and activities.
		/// </summary>
		public List<string> CommonConditions { get; set; }

		/// <summary>
		///  Credentials that must be pursued concurrently.
		/// </summary>
		public List<ConditionProfile> Corequisite { get; set; }

		/// <summary>
		/// Prerequisites for entry into a credentialing program, a learning opportunity or an assessment including transcripts, records of previous experience, and lower-level learning opportunities.
		/// </summary>
		public List<ConditionProfile> EntryCondition { get; set; }

		/// <summary>
		/// Recommended resources for this resource.
		/// </summary>
		public List<ConditionProfile> Recommends { get; set; }

		/// <summary>
		///  Requirement or set of requirements for this resource.
		/// </summary>
		public List<ConditionProfile> Requires { get; set; }

		// =========== connections ===========

		/// <summary>
		/// Credential that has its time or cost reduced by another credential, assessment or learning opportunity.
		/// ceterms:advancedStandingFrom
		/// <seealso href="https://credreg.net/ctdl/terms/advancedStandingFrom"></seealso>
		/// </summary>
		public List<Connections> AdvancedStandingFrom { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity reduces the time or cost required to earn or complete the referenced credential, assessment, or learning opportunity.
		/// <para>A third-party credential agent claiming that its credential provides advanced standing for the credential of another credential agent should consider using ceterms:recommendedFor instead of ceterms:advancedStandingFor in the absence of an explicit and verifiable agreement on advanced standing between the agents of the two credentials.</para>
		/// ceterms:isAdvancedStandingFor
		/// <seealso href="https://credreg.net/ctdl/terms/isAdvancedStandingFor">isAdvancedStandingFor</seealso>
		/// </summary>
		public List<Connections> IsAdvancedStandingFor { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity provides preparation for the credential, assessment, or learning opportunity being referenced.
		/// ceterms:isPreparationFor
		/// <seealso href="https://credreg.net/ctdl/terms/isPreparationFor">isPreparationFor</seealso>
		/// </summary>
		public List<Connections> IsPreparationFor { get; set; }

		/// <summary>
		/// It is recommended to earn or complete this credential, assessment, or learning opportunity before attempting to earn or complete the referenced credential, assessment, or learning opportunity.
		/// ceterms:isRecommendedFor
		/// <seealso href="https://credreg.net/ctdl/terms/isRecommendedFor">isRecommendedFor</seealso>
		/// </summary>
		public List<Connections> IsRecommendedFor { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity must be earned or completed prior to attempting to earn or complete the referenced credential, assessment, or learning opportunity.
		/// ceterms:isRequiredFor
		/// <seealso href="https://credreg.net/ctdl/terms/isRequiredFor"></seealso>
		/// </summary>
		public List<Connections> IsRequiredFor { get; set; }

		/// <summary>
		///  Another credential, learning opportunity or assessment that provides preparation for this credential, learning opportunity or assessment.
		/// ceterms:preparationFrom
		/// <seealso href="https://credreg.net/ctdl/terms/preparationFrom"></seealso>
		/// </summary>
		public List<Connections> PreparationFrom { get; set; }
		#endregion

		#region -- Quality Assurance BY --
		/// <summary>
		/// List of Organizations that accredit this credential
		/// </summary>
		public List<OrganizationReference> AccreditedBy { get; set; }

		/// <summary>
		/// List of Organizations that approve this credential
		/// </summary>
		public List<OrganizationReference> ApprovedBy { get; set; }
		/// <summary>
		/// List of Organizations that recognize this credential
		/// </summary>
		public List<OrganizationReference> RecognizedBy { get; set; }
		/// <summary>
		/// List of Organizations that regulate this credential
		/// </summary>
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

		//Processes
		/// <summary>
		/// Entity describing the process by which a credential, assessment, organization, or aspects of it, are administered.
		/// </summary>
		public List<ProcessProfile> AdministrationProcess { get; set; }
		/// <summary>
		/// Entity describing the process by which a credential, or aspects of it, were created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }
		/// <summary>
		/// Entity describing the process by which the credential is maintained including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// Assessment that provides direct, indirect, formative or summative evaluation or estimation of the nature, ability, or quality for an entity.
		/// </summary>
		public List<EntityReference> TargetAssessment { get; set; }
		/// <summary>
		/// Learning object or resource that is used as part of an learning activity.
		/// URL
		/// </summary>
		public List<string> TargetLearningResource { get; set; } = new List<string>();
		//public List<string> TargetPathway { get; set; } = new List<string>();
		/// <summary>
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; }
	}
}
