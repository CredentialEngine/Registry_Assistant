﻿using System.Collections.Generic;

namespace RA.Models.Input
{
	/// <summary>
	/// Class used with a Credential format or publish request
	/// June 24, 2020 - added BaseSalary (Monetary Amount)
	/// </summary>
	public class CredentialRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public CredentialRequest()
		{
			Credential = new Credential();
		}
		/// <summary>
		/// Credential Input Class
		/// </summary>
		public Credential Credential { get; set; }
	}

	/// <summary>
	/// PROTOTYPE - NOT ALLOWED IN PRODUCTION
	/// Allow publishing up to 50 credentials at a time. 
	/// Only used by the endpoint: bulkPublish
	/// </summary>
	public class BulkCredentialRequest : BaseRequest
	{
		public BulkCredentialRequest()
		{
		}

		/// <summary>
		/// List of credentials to publish
		/// </summary>
		public List<Credential> Credentials { get; set; } = new List<Credential>();
	}

	/// <summary>
	/// Credential input class
	/// </summary>
	public class Credential : BaseRequestHelper
	{
		/// <summary>
		/// constructor
		/// </summary>
		public Credential()
		{
			Type = "Credential";
			AudienceLevelType = new List<string>();
			AudienceType = new List<string>();
			Subject = new List<string>();
			OccupationType = new List<FrameworkItem>();
			IndustryType = new List<FrameworkItem>();
			Naics = new List<string>();
			Keyword = new List<string>();
			DegreeConcentration = new List<string>();
			DegreeMajor = new List<string>();
			DegreeMinor = new List<string>();

			OwnedBy = new List<OrganizationReference>();

			AccreditedBy = new List<Input.OrganizationReference>();
			ApprovedBy = new List<Input.OrganizationReference>();
			OfferedBy = new List<Input.OrganizationReference>();
			RecognizedBy = new List<Input.OrganizationReference>();
			RegulatedBy = new List<Input.OrganizationReference>();
			RenewedBy = new List<Input.OrganizationReference>();
			RevokedBy = new List<Input.OrganizationReference>();


			Corequisite = new List<ConditionProfile>();
			Recommends = new List<ConditionProfile>();
			Requires = new List<ConditionProfile>();

			CommonConditions = new List<string>();
			CommonCosts = new List<string>();

			HasPart = new List<EntityReference>();
			IsPartOf = new List<EntityReference>();

			AdvancedStandingFrom = new List<ConditionProfile>();
			IsAdvancedStandingFor = new List<ConditionProfile>();
			PreparationFrom = new List<ConditionProfile>();
			IsPreparationFor = new List<ConditionProfile>();
			IsRecommendedFor = new List<ConditionProfile>();
			IsRequiredFor = new List<ConditionProfile>();
			InLanguage = new List<string>();
			AvailableAt = new List<Place>();
			Renewal = new List<ConditionProfile>();

			AdministrationProcess = new List<ProcessProfile>();
			DevelopmentProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();
			AppealProcess = new List<ProcessProfile>();
			ComplaintProcess = new List<ProcessProfile>();
			ReviewProcess = new List<ProcessProfile>();
			RevocationProcess = new List<ProcessProfile>();

		}
		/// <summary>
		/// Helper property for use with blank nodes
		/// This is a 'broad' type. CredentialType is still required.
		/// </summary>
		public string Type { get; set; } = "Credential";


		#region *** Required Properties ***

		/// <summary>
		/// The credential type as defined in CTDL
		/// <see href="https://credreg.net/page/typeslist"/>
		/// Required
		/// NOTE: The following types are 'top level' types that may not be published:
		///		Credential, Degree
		/// Only the sub-types under the latter may be used in publishing
		/// </summary>
		public string CredentialType { get; set; }

		/// <summary>
		/// Name of this credential
		/// Required
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Credential description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Organization that owns this credential
		/// Required
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		/// <summary>
		/// The status type of this credential. 
		/// Required
		/// The default is Active. 
		/// </summary>
		public string CredentialStatusType { get; set; }
		/// <summary>
		/// Credential Identifier
		/// format: 
		/// ce-UUID (guid)
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// Required
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages; e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// Required
		/// List of language codes. ex: en, es
		/// </summary>
		public List<string> InLanguage { get; set; }
		#endregion


		/// <summary>
		/// List of Alternate Names for this credential
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();



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
		/// Delivery type for the assessment for the credential.
		/// deliveryType:BlendedDelivery deliveryType:InPerson deliveryType:OnlineOnly
		/// <see href="https://credreg.net/ctdl/terms/Delivery"></see>
		/// </summary>
		public List<string> AssessmentDeliveryType { get; set; } = new List<string>();
        /// <summary>
        /// AvailableOnlineAt URL
        /// </summary>
        public List<string> AvailableOnlineAt { get; set; }
        /// <summary>
        /// AvailabilityListing URL
        /// </summary>
        public List<string> AvailabilityListing { get; set; } 
		/// <summary>
		/// List of Addresses for this credential, using Place
		/// </summary>
		public List<Place> AvailableAt { get; set; }

		/// <summary>
		/// Person or organization holding the rights in copyright to entities such as credentials, learning opportunities, assessments, competencies or concept schemes.
		/// </summary>
		public OrganizationReference CopyrightHolder { get; set; }
		/// <summary>
		/// Credential Identifier
		/// Globally unique identifier by which the creator, owner or provider of a credential recognizes that credential in transactions with the external environment (e.g., in verifiable claims involving the credential).
		/// NOTE: Acronyns should not be used here. Use AlternateName for providing acronyms.
		/// </summary>
		public string CredentialId { get; set; }

		///// <summary>
		///// Provide credit information in a ValueProfile value
		///// A credit-related value.
		///// 22-08-30 - NEW ==> actually not approved yet
		///// </summary>
		//public List<ValueProfile> CreditValue { get; set; } = new List<ValueProfile>();

		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// Previously earned, completed, or attained resources may still be valid even if they are no longer offered.
		/// </summary>
		public string ExpirationDate { get; set; }


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
		/// List of Organizations that offer this credential
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }
		/// <summary>
		/// List of Organizations that recognize this credential
		/// </summary>
		public List<OrganizationReference> RecognizedBy { get; set; }
		/// <summary>
		/// Organization(s) that register this resource. 
		/// Only allowed for Apprenticeship related certificates. Currently:
		/// ceterms:ApprenticeshipCertificate, ceterms:JourneymanCertificate, ceterms:MasterCertificate, ceterms:PreApprenticeshipCertificate
		/// </summary>
		public List<OrganizationReference> RegisteredBy { get; set; } = new List<Input.OrganizationReference>();

		/// <summary>
		/// List of Organizations that regulate this credential
		/// </summary>
		public List<OrganizationReference> RegulatedBy { get; set; }
		/// <summary>
		/// List of Organizations that renew this credential
		/// </summary>
		public List<OrganizationReference> RenewedBy { get; set; }
		/// <summary>
		/// List of Organizations that can revoke this credential
		/// </summary>
		public List<OrganizationReference> RevokedBy { get; set; }
		#endregion


		#region Quality Assurance IN - Jurisdiction based Quality Assurance  (INs)

		/// <summary>
		/// List of Organizations that accredit this credential in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> AccreditedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that approve this credential in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> ApprovedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that offer this credential in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> OfferedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that recognize this credential in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RecognizedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that regulate this credential in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RegulatedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that renew this credential in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RenewedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that revoke this credential in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RevokedIn { get; set; } = new List<JurisdictionAssertion>();

		#endregion


		#region Costs, duration, assistance
		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization.
		/// Set of costs maintained at an organizational or sub-organizational level, which apply to this credential.
		/// </summary>
		public List<string> CommonCosts { get; set; }
		/// <summary>
		/// The salary value or range associated with this credential.
		/// PROPOSED - NOT VALID FOR PRODUCTION YET
		/// </summary>
		public MonetaryAmount BaseSalary { get; set; } 

		/// <summary>
		/// List of cost profiles for this credential
		/// </summary>
		public List<CostProfile> EstimatedCost { get; set; } = new List<CostProfile>();

		/// <summary>
		/// Estimated time it will take to complete an activity, event, or resource.
		/// List of duration profiles expressing one or more duration profiles to attain this resource.
		/// </summary>
		public List<DurationProfile> EstimatedDuration { get; set; } 

		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; }
		#endregion

		/// <summary>
		/// List of credentials that are part of this credential
		/// </summary>
		public List<EntityReference> HasPart { get; set; }

		/// <summary>
		/// Rubric related to this resource.
		/// <see cref="https://credreg.net/ctdl/terms/hasRubric"/>
		/// ceterms:hasRubric
		/// </summary>
		public List<string> HasRubric { get; set; } = new List<string>();
		/// <summary>
		/// Reference to a relevant support service available for this resource.
		/// List of CTIDs for published resources
		/// </summary>
		public List<string> HasSupportService { get; set; } = new List<string>();
        //

        /// <summary>
        /// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
        /// <see href="https://credreg.net/ctdl/terms/identifier">Identifier</see>
        /// ceterms:identifier
        /// </summary>
        public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// Image URL
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		public string InCatalog { get; set; }

		/// <summary>
		/// ISIC Revision 4 Code
		/// </summary>
		public string ISICV4 { get; set; }

		/// <summary>
		/// Is Non-Credit
		/// Resource carries or confers no official academic credit towards a program or a credential.
		/// </summary>
		public bool? IsNonCredit { get; set; }

		/// <summary>
		/// List of credentials where this credential is a part of.
		/// </summary>
		public List<EntityReference> IsPartOf { get; set; }
		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile">JurisdictionProfile</see>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; } = new List<JurisdictionProfile>();
		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; }
		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Delivery type for the learning opportunity for the credential.
		/// deliveryType:BlendedDelivery deliveryType:InPerson deliveryType:OnlineOnly
		/// <see href="https://credreg.net/ctdl/terms/Deliver">Deliver</see>
		/// </summary>
		public List<string> LearningDeliveryType { get; set; } = new List<string>();

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

		#region Version related properties
		//
		/// <summary>
		/// Latest version of the credential.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string LatestVersion { get; set; }
		/// <summary>
		/// Version of the resource that immediately precedes this version.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string PreviousVersion { get; set; } 
		/// <summary>
		/// Version of the resource that immediately follows this version.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string NextVersion { get; set; } 
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
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();

		#endregion

		/// <summary>
		/// Frequency with which the credential needs to be renewed.
		/// A single DurationItem, using Years, Months, etc. or a combination.
		/// Alternately the actual ISO8601 format may be provided (using Duration_ISO8601)
		/// </summary>
		public DurationItem RenewalFrequency { get; set; }

		/// <summary>
		/// Revocation Profile
		/// Entity describing conditions and methods by which a credential can be removed from a holder.
		/// </summary>
		public List<RevocationProfile> Revocation { get; set; } = new List<RevocationProfile>();

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
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();


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
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> Naics { get; set; }

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
		//
		//Navy


		#region Properties allowed only for degree types

		/// <summary>
		/// Focused plan of study within a college or university degree such as a concentration in Aerospace Engineering within an Engineering degree.
		/// TODO: enable more detail by using a blank node in ReferenceObject. The latter would be a CredentialAlignmentObject. The Id would be a Guid in DegreeConcentration. Alternately a fully formed blank node id (	  _:(GUID)	)
		/// </summary>
		public List<string> DegreeConcentration { get; set; }
		/// <summary>
		/// Focused plan of study within a college or university degree such as a concentration in Aerospace Engineering within an Engineering degree.
		/// </summary>
		public LanguageMapList DegreeConcentration_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// LanguageMapList for Primary field of study of a degree-seeking student.
		/// </summary>
		public List<string> DegreeMajor { get; set; }
		/// <summary>
		/// LanguageMapList for Primary field of study of a degree-seeking student.
		/// </summary>
		public LanguageMapList DegreeMajor_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// Optional, secondary field of study of a degree-seeking student.
		/// </summary>
		public List<string> DegreeMinor { get; set; }
		/// <summary>
		/// LanguageMapList for Optional, secondary field of study of a degree-seeking student.
		/// </summary>
		public LanguageMapList DegreeMinor_Map { get; set; } = new LanguageMapList();
		#endregion

		#region Properties allowed only for a Quality Assurance Credential

		/// <summary>
		/// Only valid with a Quality Assurance credential. ("ceterms:QualityAssuranceCredential")
		/// List of CTIDs for credentials, or assessments, or learning opportunities that are 'members' of this quality assurance credential (essentially approved by the owner of the QA credential)
		/// PROPOSED - NOT VALID FOR PRODUCTION YET
		/// </summary>
		public List<string> HasETPLResource { get; set; } = new List<string>();

		#endregion
		//

		#region Outcome data 
		/// <summary>
		///  Resource containing summary/statistical employment outcome, earnings, and/or holders information.
		///  For deeper information, include qdata:DataSetProfile.
		/// </summary>
		public List<AggregateDataProfile> AggregateData { get; set; } = new List<AggregateDataProfile>();

		#endregion

        /// <summary>
        /// Uses Verification Service
		/// Reference to a service that is used to verify this Credential.
		/// Range: ceterms:VerificationServiceProfile
		/// List of CTIDs that reference a VerificationServiceProfile in the registry
        /// </summary>
        public List<string> UsesVerificationService { get; set; } = new List<string>();


        #region -- Condition Profiles --
        /// <summary>
        /// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
        /// </summary>
        public List<string> CommonConditions { get; set; }
		/// <summary>
		/// Requirement or set of requirements for this resource
		/// </summary>
		public List<ConditionProfile> Requires { get; set; }
		/// <summary>
		/// Recommended resources for this resource.
		/// </summary>
		public List<ConditionProfile> Recommends { get; set; }
		/// <summary>
		///  Resources that must be pursued concurrently.
		/// </summary>
		public List<ConditionProfile> Corequisite { get; set; }

		/// <summary>
		///  Resource that must be completed prior to, or pursued at the same time as, this resource.
		/// </summary>
		public List<ConditionProfile> CoPrerequisite { get; set; } = new List<ConditionProfile>();

		/// <summary>
		/// Requirements for entry into a credentialing program, a learning opportunity, or an assessment, including credentials, transcripts, records of previous experience, or other forms of entry documentation.
		/// </summary>
		public List<ConditionProfile> EntryCondition { get; set; } = new List<ConditionProfile>();


		/// <summary>
		/// Entity describing the constraints, prerequisites, entry conditions, or requirements necessary to maintenance and renewal of an awarded credential.
		/// </summary>
		public List<ConditionProfile> Renewal { get; set; }
		#endregion


		#region -- Connections Profiles --
		//Connection Profiles are Condition Profiles but typically only a subject of the Condition Profile properties are used. 
		/// <summary>
		/// Credential that has its time or cost reduced by another credential, assessment or learning opportunity.
		/// ceterms:advancedStandingFrom
		/// <seealso href="https://credreg.net/ctdl/terms/advancedStandingFrom"></seealso>
		/// </summary>
		public List<ConditionProfile> AdvancedStandingFrom { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity reduces the time or cost required to earn or complete the referenced credential, assessment, or learning opportunity.
		/// <para>A third-party credential agent claiming that its credential provides advanced standing for the credential of another credential agent should consider using ceterms:recommendedFor instead of ceterms:advancedStandingFor in the absence of an explicit and verifiable agreement on advanced standing between the agents of the two credentials.</para>
		/// ceterms:isAdvancedStandingFor
		/// <seealso href="https://credreg.net/ctdl/terms/isAdvancedStandingFor">isAdvancedStandingFor</seealso>
		/// </summary>
		public List<ConditionProfile> IsAdvancedStandingFor { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity provides preparation for the credential, assessment, or learning opportunity being referenced.
		/// ceterms:isPreparationFor
		/// <seealso href="https://credreg.net/ctdl/terms/isPreparationFor">isPreparationFor</seealso>
		/// </summary>
		public List<ConditionProfile> IsPreparationFor { get; set; }

		/// <summary>
		/// It is recommended to earn or complete this credential, assessment, or learning opportunity before attempting to earn or complete the referenced credential, assessment, or learning opportunity.
		/// ceterms:isRecommendedFor
		/// <seealso href="https://credreg.net/ctdl/terms/isRecommendedFor">isRecommendedFor</seealso>
		/// </summary>
		public List<ConditionProfile> IsRecommendedFor { get; set; }

		/// <summary>
		/// This credential, assessment, or learning opportunity must be earned or completed prior to attempting to earn or complete the referenced credential, assessment, or learning opportunity.
		/// ceterms:isRequiredFor
		/// <seealso href="https://credreg.net/ctdl/terms/isRequiredFor"></seealso>
		/// </summary>
		public List<ConditionProfile> IsRequiredFor { get; set; }

		/// <summary>
		///  Another credential, learning opportunity or assessment that provides preparation for this credential, learning opportunity or assessment.
		/// ceterms:preparationFrom
		/// <seealso href="https://credreg.net/ctdl/terms/preparationFrom"></seealso>
		/// </summary>
		public List<ConditionProfile> PreparationFrom { get; set; }
        #endregion

        #region -- Process Profiles --
        /// <summary>
        /// Entity describing the process by which a credential, assessment, organization, or aspects of it, are administered.
        /// ceterms:administrationProcess
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
		/// Formal process for objecting to decisions of the organization regarding credentials, assessments or processes.
		/// </summary>
		public List<ProcessProfile> AppealProcess { get; set; }
		/// <summary>
		/// Process for handling complaints about a credential, or aspects of it including related learning opportunities and assessments.
		/// </summary>
		public List<ProcessProfile> ComplaintProcess { get; set; }
		/// <summary>
		/// Entity that describes the process by which the credential, or aspects of it, are reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }
		/// <summary>
		/// Entity describing the process by which the credential is revoked.
		/// </summary>
		public List<ProcessProfile> RevocationProcess { get; set; }
		#endregion

		/// <summary>
		/// HasRating - Navy
		/// Rating related to this resource.
		/// URI to a Rating
		/// </summary>
		public List<string> HasRating { get; set; } = new List<string>();

		/// <summary>
		/// Action related to the credential
		/// This may end up being a list of CTIDs?
		/// PROPOSED - NOT VALID FOR PRODUCTION YET
		/// </summary>
		public List<CredentialingAction> RelatedAction { get; set; } = new List<CredentialingAction>();

		#region OBSOLETE


		#endregion
		///// <summary>
		///// List of CTIDs for a published pathway.
		///// Blank nodes are not supported/relevent.
		///// NOTE: the TargetPathway is an inverse property for a credential. That is, it will not be published with the credential, and is instead derived via a PathwayComponent
		///// </summary>
		//public List<string> TargetPathway { get; set; } = new List<string>();


		#region Alignments - Not relevent for the Credential Registry Profile
		// <summary>
		// Item that covers all of the relevant concepts in the item being described as well as additional relevant concepts.
		// ceterms:broadAlignment
		// List of CTIDs. The referenced object must exist in the registry
		// </summary>
		//public List<string> BroadAlignment { get; set; } = new List<string>();
		#endregion

	}
}
