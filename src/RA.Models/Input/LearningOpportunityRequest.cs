using System;
using System.Collections.Generic;

namespace RA.Models.Input
{
	public class LearningOpportunityRequest : BaseRequest
	{
		public LearningOpportunityRequest()
		{
			LearningOpportunity = new LearningOpportunity();
		}

		public LearningOpportunity LearningOpportunity { get; set; }
	}
	public class LearningProgramRequest : LearningOpportunityRequest
	{
	}
	/// <summary>
	/// A course will have all of the properties of LearningOpportunityProfile, except:
	/// - ceterms:instructionalProgramType 
	/// </summary>
	public class CourseRequest : LearningOpportunityRequest
	{
	}
	public class LearningOpportunity : BaseRequestClass
	{
		public LearningOpportunity()
		{
			Subject = new List<string>();
			Keyword = new List<string>();
			DeliveryType = new List<string>();
			LearningMethodType = new List<string>();
			EstimatedCost = new List<CostProfile>();
			//Region = new List<GeoCoordinates>();

			AudienceType = new List<string>();
			AudienceType = new List<string>();
			AvailabilityListing = new List<string>();
			AvailableOnlineAt = new List<string>();

			AccreditedBy = new List<Input.OrganizationReference>();
			ApprovedBy = new List<Input.OrganizationReference>();
			RecognizedBy = new List<Input.OrganizationReference>();
			RegulatedBy = new List<Input.OrganizationReference>();

			//JurisdictionAssertions = new List<JurisdictionAssertedInProfile>();

			//Corequisite = new List<ConditionProfile>();
			//Recommends = new List<ConditionProfile>();
			//Requires = new List<ConditionProfile>();
			//EntryCondition = new List<ConditionProfile>();

			Teaches = new List<CredentialAlignmentObject>();

			AdvancedStandingFrom = new List<ConnectionProfile>();
			IsAdvancedStandingFor = new List<ConnectionProfile>();
			PreparationFrom = new List<ConnectionProfile>();
			IsPreparationFor = new List<ConnectionProfile>();
			IsRecommendedFor = new List<ConnectionProfile>();
			IsRequiredFor = new List<ConnectionProfile>();
			InLanguage = new List<string>();
			AvailableAt = new List<Place>();
			CommonConditions = new List<string>();
			CommonCosts = new List<string>();

			HasPart = new List<EntityReference>();
			IsPartOf = new List<EntityReference>();
			VersionIdentifier = new List<IdentifierValue>();
		}
		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "LearningOpportunityProfile";

		#region *** Required Properties ***
		/// <summary>
		/// Name or title of the resource.
		/// Required
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = null;
		/// <summary>
		/// Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = null;

		/// <summary>
		/// Webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Credential Identifier
		/// format: 
		/// ce-UUID (guid)
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages; e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// List of language codes. ex: en, es
		/// </summary>
		public List<string> InLanguage { get; set; }


        /// <summary>
        /// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
        /// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
        /// Required
        /// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
        /// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
        /// </summary>
        public string LifeCycleStatusType { get; set; } = "lifeCycle:Active";

        #region at least one of

        /// <summary>
        /// Organization that owns this resource
        /// </summary>
        public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();
		//OR
		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; } = new List<Input.OrganizationReference>();
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
		/// Provide credit information in a ValueProfile value
		/// A credit-related value.
		/// 21-07-19 - updating Creditvalue to also allow a list. It is defined as an object. The API will accept either a ValueProfile object or List of ValueProfiles
		/// 21-08-18 - Changing permantly to the List, as only existing use was from the publisher (and the latter is updated to use the list)
		/// </summary>
		public List<ValueProfile> CreditValue { get; set; } = new List<ValueProfile>();
		//public object CreditValue { get; set; }   


		/// <summary>
		/// Competency that the learning opportunity is intended to teach.
		/// List of CredentialAlignmentObjects where TargetNodeName is the only required property. 
		/// </summary>
		public List<CredentialAlignmentObject> Teaches { get; set; }

        /// <summary>
        /// Teaches Competency Framework - Helper property
        /// A list of CTIDs for frameworks, where this resource teaches all competencies in a framework.
        /// API will look up framework, get all competencies and add as Teaches for this resource. 
        /// Only provide Teaches or TeachesCompetencyFramework, but not both (will result in an error).
        /// </summary>
        public List<string> TeachesCompetencyFramework { get; set; }

        /// <summary>
        /// Types of methods used to conduct the learning opportunity; 
        /// Concepts: Applied, Gaming, Laboratory, Lecture, Prerecorded, SelfPaced, Seminar, WorkBased
        /// ConceptScheme: <see href="https://credreg.net/ctdl/terms/LearningMethod">LearningMethod</see>
        /// </summary>
        public List<string> LearningMethodType { get; set; }


		/// <summary>
		/// Learning Method Description 
		///  Description of the learning methods for a resource.
		/// 
		/// </summary>
		public string LearningMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap LearningMethodDescription_Map { get; set; } = null;

		/// <summary>
		/// Type of means by which a learning opportunity or assessment is delivered to credential seekers and by which they interact; select from an existing enumeration of such types.
		/// deliveryType:BlendedDelivery deliveryType:InPerson deliveryType:OnlineOnly
		/// <see href="https://credreg.net/ctdl/terms/Delivery"></see>
		/// </summary>
		public List<string> DeliveryType { get; set; }

		/// <summary>
		/// Detailed description of the delivery type of an assessment or learning opportunity.
		/// </summary>
		public string DeliveryTypeDescription { get; set; }
		public LanguageMap DeliveryTypeDescription_Map { get; set; } = null;
		#endregion

		//=========== optional ================================
		/// <summary>
		/// List of Alternate Names for this learning opportunity
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = null;

		#region Proposed collection helpers
		/// <summary>
		/// One or more collections of which this resource is to be a member.
		/// The owner of the collection must be the same as the publisher of this resource. 
		/// Actions
		/// Proposed: 
		///		if this resource includes at least one of DateEffective or ExpirationDate, then a CollectionMember will be added to the collection, otherwise just the URI will be added to Collection.HasMember.
		///	Response:
		///		NO, it is believed that we cannot arbitrarily take action based on the dates
		/// </summary>
		public List<string> IsMemberOfCollection { get; set; } = new List<string>();
		/// <summary>
		/// List of collection members for a collection where this learning opportunity is to be added.
		/// The CollectionCTID is required. 
		/// The ProxyFor property should be empty or contain the same CTID as this resource.
		///		- actually it will just be ignored
		///	TODO - can we get away with just one property?
		/// </summary>
		public List<CollectionMember> IsCollectionMemberOfCollection { get; set; } = new List<CollectionMember>();

		/// <summary>
		/// List of Collections from which to remove the current resource
		/// </summary>
		public List<string> RemoveCollectionMember{ get; set; } = new List<string>();

		#endregion

		/// <summary>
		///  Resource containing summary/statistical employment outcome, earnings, and/or holders information.
		///  For deeper information, include qdata:DataSetProfile.
		/// </summary>
		public List<AggregateDataProfile> AggregateData { get; set; } = new List<AggregateDataProfile>();

		/// <summary>
		///  Competency evaluated through the learning opportunity.		  
		/// </summary>
		public List<CredentialAlignmentObject> Assesses { get; set; } = new List<CredentialAlignmentObject>();

		/// <summary>
		/// Assessment Method Description 
		/// Description of the assessment methods for a resource.
		/// </summary>
		public string AssessmentMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap AssessmentMethodDescription_Map { get; set; } = null;

		/// <summary>
		/// Type of method used to conduct an assessment; select from an existing enumeration of such types.
		/// assessMethod:Artifact, assessMethod:Exam, assessMethod:Performance
		/// <see href="https://credreg.net/ctdl/terms/AssessmentMethod"></see>
		/// </summary>
		public List<string> AssessmentMethodType { get; set; } = new List<string>();

		/// <summary>
		/// The type of credential seeker for whom the entity is applicable; select from an existing enumeration of such types.
		/// audience:Citizen audience:CurrentMilitary audience:CurrentMilitaryDependent audience:CurrentMilitarySpouse audience:CurrentStudent audience:FormerMilitary audience:FormerMilitaryDependent audience:FormerMilitarySpouse audience:FormerStudent audience:FullTime audience:Member audience:NonCitizen audience:NonMember audience:NonResident audience:PartTime audience:PrivateEmployee audience:PublicEmployee audience:Resident
		/// <see href="https://credreg.net/ctdl/terms/Audience"></see>
		/// </summary>
		public List<string> AudienceType { get; set; }
		/// <summary>
		/// Type of level indicating a point in a progression through an educational or training context, for which the credential is intended; select from an existing enumeration of such types.
		/// audLevel:AdvancedLevel audLevel:AssociatesDegreeLevel audLevel:BachelorsDegreeLevel audLevel:BeginnerLevel audLevel:DoctoralDegreeLevel audLevel:GraduateLevel audLevel:IntermediateLevel audLevel:LowerDivisionLevel audLevel:MastersDegreeLevel audLevel:PostSecondaryLevel audLevel:ProfessionalLevel audLevel:SecondaryLevel audLevel:UndergraduateLevel audLevel:UpperDivisionLevel
		/// <see href="https://credreg.net/ctdl/terms/AudienceLevel"></see>
		/// </summary>
		public List<string> AudienceLevelType { get; set; } = new List<string>();

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies an item and supports its discovery and use.
		/// ceterms:codedNotation
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization.
		/// Set of costs maintained at an organizational or sub-organizational level, which apply to this learning opportunity.
		/// </summary>
		public List<string> CommonCosts { get; set; }


		/// <summary>
		/// Detailed description of credit unit. 
		/// Recommendation is to use CreditValue rather than this property.
		/// </summary>
		public string CreditUnitTypeDescription { get; set; }
		/// <summary>
		/// LanguageMap for CreditUnitTypeDescription
		/// </summary>
		public LanguageMap CreditUnitTypeDescription_Map { get; set; } = null;

		/// <summary>
		/// Start Date of the Learning opportunity
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
		/// End date of the learning opportunity if applicable
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();

        /// <summary>
        ///  Indicates an offering and typical schedule.
        ///  NOTE: Only use this property when it is necessary and useful to provide data about specific offerings of a learning opportunity or assessment, such as particular combinations of schedule, location, and delivery.
        /// </summary>
        public List<string> HasOffering { get; set; } = new List<string>();

        /// <summary>
        ///  Indicates a separately identifiable and independently useful component of the entity.
        /// </summary>
        public List<EntityReference> HasPart { get; set; }

		/// <summary>
		/// Indicates a resource that acts as a stand-in for the resource. 
		/// full URL OR CTID (recommended)
		/// </summary>
		public string HasProxy { get; set; }

        /// <summary>
        /// Reference to a relevant support service.
		/// List of CTIDs that reference one or more published support services
        /// </summary>
        public List<string> HasSupportService { get; set; }

        /// <summary>
        /// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
        /// <see href="https://purl.org/ctdl/terms/identifier"></see>
        /// ceterms:identifier
        /// </summary>
        public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// Is Non-Credit
		/// Resource carries or confers no official academic credit towards a program or a credential.
		/// </summary>
		public bool? IsNonCredit { get; set; }

		/// <summary>
		/// Indicates another entity of which this entity is a component. That is of another Learning Opportunity
		/// Covers partitive notions such as "embedded".
		/// </summary>
		public List<EntityReference> IsPartOf { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"></see>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; } 

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; }
		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		public LanguageMapList Keyword_Map { get; set; } = null;


		/// <summary>
		/// Resource that is required as a prior condition to this resource.
		/// Only allowed for a course on a course
		/// </summary>
		public List<EntityReference> Prerequisite { get; set; } = new List<EntityReference>();

        /// <summary>
        /// Organization(s) that register this resource. 
		/// Typically used for Registered Apprenticeships
        /// </summary>
        public List<OrganizationReference> RegisteredBy { get; set; } = new List<Input.OrganizationReference>();

        /// <summary>
        /// Another source of information about the entity being described.
        /// HINT: If the SameAs target is a resource in the Credential Registry, just the CTID needs to be provided. 
        /// ceterms:sameAs
        /// </summary>
        public List<string> SameAs { get; set; } = new List<string>();
		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// </summary>
		public List<string> Subject { get; set; } = new List<string>();
		/// <summary>
		/// Language map list for Subject
		/// </summary>
		public LanguageMapList Subject_Map { get; set; } = null;



        /// <summary>
        /// Type of frequency at which a resource is offered; select from an existing enumeration of such types.
        /// ConceptScheme: ceterms:ScheduleFrequency
        /// scheduleFrequency:Annually scheduleFrequency:BiMonthly scheduleFrequency:EventBased scheduleFrequency:Irregular scheduleFrequency:Monthly scheduleFrequency:MultiplePerWeek scheduleFrequency:OnDemand scheduleFrequency:OpenEntryExit scheduleFrequency:Quarterly scheduleFrequency:SelfPaced scheduleFrequency:SemiAnnually scheduleFrequency:SingleInstance scheduleFrequency:Weekly
        /// </summary>
        public List<string> OfferFrequencyType { get; set; } = new List<string>();

        /// <summary>
        /// Type of frequency with which events typically occur; select from an existing enumeration of such types.
        /// ConceptScheme: ceterms:ScheduleFrequency
        /// scheduleFrequency:Annually scheduleFrequency:BiMonthly scheduleFrequency:EventBased scheduleFrequency:Irregular scheduleFrequency:Monthly scheduleFrequency:MultiplePerWeek scheduleFrequency:OnDemand scheduleFrequency:OpenEntryExit scheduleFrequency:Quarterly scheduleFrequency:SelfPaced scheduleFrequency:SemiAnnually scheduleFrequency:SingleInstance scheduleFrequency:Weekly
        /// </summary>
        public List<string> ScheduleFrequencyType { get; set; } = new List<string>();

        /// <summary>
        /// Type of time at which events typically occur; select from an existing enumeration of such types.
        /// ConceptScheme: ceterms:ScheduleTiming
        /// scheduleTiming:Daytime scheduleTiming:Evening scheduleTiming:Weekdays scheduleTiming:Weekends
        /// </summary>
        public List<string> ScheduleTimingType { get; set; } = new List<string>();
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
		public LanguageMapList AlternativeOccupationType_Map { get; set; } = null;
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
		public LanguageMapList AlternativeIndustryType_Map { get; set; } = null;
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
		public LanguageMapList AlternativeInstructionalProgramType_Map { get; set; } = null;
		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion

		//
		#region Conditions and connections
		//Connection Profiles are Condition Profiles but typically only a subject of the Condition Profile properties are used. 
		/// <summary>
		/// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
		/// Set constraints, prerequisites, entry conditions, or requirements that are shared across an organization, organizational subdivision, set of credentials, or category of entities and activities.
		/// </summary>
		public List<string> CommonConditions { get; set; }

		/// <summary>
		///  Resources that must be pursued concurrently.
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
		/// <summary>
		///  Resource that must be completed prior to, or pursued at the same time as, this resource.
		/// </summary>
		public List<ConditionProfile> CoPrerequisite { get; set; } = new List<ConditionProfile>();

		// =========== connections ===========

		/// <summary>
		/// Credential that has its time or cost reduced by another credential, assessment or learning opportunity.
		/// ceterms:advancedStandingFrom
		/// <seealso href="https://credreg.net/ctdl/terms/advancedStandingFrom"></seealso>
		/// </summary>
		public List<ConnectionProfile> AdvancedStandingFrom { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity reduces the time or cost required to earn or complete the referenced credential, assessment, or learning opportunity.
		/// <para>A third-party credential agent claiming that its credential provides advanced standing for the credential of another credential agent should consider using ceterms:recommendedFor instead of ceterms:advancedStandingFor in the absence of an explicit and verifiable agreement on advanced standing between the agents of the two credentials.</para>
		/// ceterms:isAdvancedStandingFor
		/// <seealso href="https://credreg.net/ctdl/terms/isAdvancedStandingFor">isAdvancedStandingFor</seealso>
		/// </summary>
		public List<ConnectionProfile> IsAdvancedStandingFor { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity provides preparation for the credential, assessment, or learning opportunity being referenced.
		/// ceterms:isPreparationFor
		/// <seealso href="https://credreg.net/ctdl/terms/isPreparationFor">isPreparationFor</seealso>
		/// </summary>
		public List<ConnectionProfile> IsPreparationFor { get; set; }

		/// <summary>
		/// It is recommended to earn or complete this credential, assessment, or learning opportunity before attempting to earn or complete the referenced credential, assessment, or learning opportunity.
		/// ceterms:isRecommendedFor
		/// <seealso href="https://credreg.net/ctdl/terms/isRecommendedFor">isRecommendedFor</seealso>
		/// </summary>
		public List<ConnectionProfile> IsRecommendedFor { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity must be earned or completed prior to attempting to earn or complete the referenced credential, assessment, or learning opportunity.
		/// ceterms:isRequiredFor
		/// <seealso href="https://credreg.net/ctdl/terms/isRequiredFor"></seealso>
		/// </summary>
		public List<ConnectionProfile> IsRequiredFor { get; set; }

		/// <summary>
		///  Another credential, learning opportunity or assessment that provides preparation for this credential, learning opportunity or assessment.
		/// ceterms:preparationFrom
		/// <seealso href="https://credreg.net/ctdl/terms/preparationFrom"></seealso>
		/// </summary>
		public List<ConnectionProfile> PreparationFrom { get; set; }
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
		//There are currently two separate approaches to publishing properties like assertedIn
		//- Publish all 'IN' properties using JurisdictionAssertions
		//- Publish using ehe separate specific properties like AccreditedIn, ApprovedIn, etc
		// 2010-01-06 The property JurisdictionAssertions may become obsolete soon. We recomend to NOT use this property.


		//JurisdictionAssertion
		//Each 'IN' property must include one or more organizations and a Main jurisdiction. Only one main jusrisdiction (and multiple exceptions) can be entered with each property.
		//Only use this property where the organization only makes the assertion for a specific jurisdiction. 
		//Use the 'BY' equivalent (ex. accreditedBy) where the organization makes a general assertion

		/// <summary>
		/// List of Organizations that accredit this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> AccreditedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that approve this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> ApprovedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that offer this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> OfferedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that recognize this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RecognizedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that regulate this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RegulatedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that revoke this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RevokedIn { get; set; } = new List<JurisdictionAssertion>();

		#endregion

		/// <summary>
		///  Resource that replaces this resource.
		///  full URL OR CTID (recommended)
		/// </summary>
		public string SupersededBy { get; set; }
		/// <summary>
		/// Resource that this resource replaces.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string Supersedes { get; set; }

		/// <summary>
		/// Assessment that provides direct, indirect, formative or summative evaluation or estimation of the nature, ability, or quality for an entity.
		/// </summary>
		public List<EntityReference> TargetAssessment { get; set; }

		/// <summary>
		/// Learning opportunity that is the focus of a condition, process or another learning opportunity.
		/// </summary>
		public List<EntityReference> TargetLearningOpportunity { get; set; }

		/// <summary>
		/// Learning object or resource that is used as part of an learning activity.
		/// URI
		/// </summary>
		public List<string> TargetLearningResource { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; }
		//public List<string> TargetPathway { get; set; } = new List<string>();

		/// <summary>
		/// School Courses for the Exchange of Data code for a course.
		/// It is preferable to record the whole 12 character alphanumeric code, however it is also valid to record just the five digit subject code + course number.
		/// Minimum of 5 characters and maximum of 14 characters for now. 
		/// COURSE ONLY
		/// </summary>
		public string SCED { get; set; }


		#region Properties allowed only for learning programs
		//these will be ignored for all other types
		/// <summary>
		/// Focused plan of study within a college or university degree such as a concentration in Aerospace Engineering within an Engineering degree.
		/// TODO: enable more detail by using a blank node in ReferenceObject. The latter would be a CredentialAlignmentObject. The Id would be a Guid in DegreeConcentration. Alternately a fully formed blank node id (	  _:(GUID)	)
		/// </summary>
		public List<string> DegreeConcentration { get; set; }
		/// <summary>
		/// Focused plan of study within a college or university degree such as a concentration in Aerospace Engineering within an Engineering degree.
		/// </summary>
		public LanguageMapList DegreeConcentration_Map { get; set; } = null;
        #endregion

    }
}
