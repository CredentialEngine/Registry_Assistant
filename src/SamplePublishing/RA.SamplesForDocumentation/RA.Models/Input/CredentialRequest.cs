using System;
using System.Collections.Generic;
namespace RA.Models.Input
{
	/// <summary>
	/// Class used with a Credential format or publish request
	/// </summary>
	public class CredentialRequest : BaseRequest
	{
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
	/// Allow publishing up to 10 credentials at a time. 
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

	public class Credential
	{
		public Credential()
		{
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

			// Region = new List<GeoCoordinates>();
			OwnedBy = new List<OrganizationReference>();
			CopyrightHolder = new OrganizationReference();

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

			//ProcessProfile = new List<Input.ProcessProfile>();
			CommonConditions = new List<string>();
			CommonCosts = new List<string>();

			HasPart = new List<EntityReference>();
			IsPartOf = new List<EntityReference>();

			AdvancedStandingFrom = new List<Connections>();
			IsAdvancedStandingFor = new List<Connections>();
			PreparationFrom = new List<Connections>();
			IsPreparationFor = new List<Connections>();
			IsRecommendedFor = new List<Connections>();
			IsRequiredFor = new List<Connections>();
			InLanguage = new List<string>();
			AvailableAt = new List<Place>();
			//CodedNotation = new List<string>();
			Renewal = new List<ConditionProfile>();
			
			AdministrationProcess = new List<ProcessProfile>();
			DevelopmentProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();
			AppealProcess = new List<ProcessProfile>();
			ComplaintProcess = new List<ProcessProfile>();
			ReviewProcess = new List<ProcessProfile>();
			RevocationProcess = new List<ProcessProfile>();

		}


		#region *** Required Properties ***


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
		/// Required
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Organization that owns this credential
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		/// <summary>
		/// The credential type as defined in CTDL
		/// </summary>
		public string CredentialType { get; set; }

		/// <summary>
		/// The status type of this credential. 
		/// The default is Active. 
		/// </summary>
		public string CredentialStatusType { get; set; }
		/// <summary>
		/// CTID - unique identifier
		/// If not provided, will be set to ce-UUID
		/// ex: ce-F22CA1DC-2D2E-49C0-9E72-0457AD348873
		/// It will be the primary key for retrieving this entity from the registry. 
		/// Also it must be provided 
		/// </summary>
		public string Ctid { get; set; }

		/// <summary>
		/// SubjectWebpage URL
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		public List<string> InLanguage { get; set; }
		#endregion


		/// <summary>
		/// List of Alternate Names for this credential
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// Image URL
		/// </summary>
		public string Image { get; set; }



		/// <summary>
		/// AvailableOnlineAt URL
		/// </summary>
		public string AvailableOnlineAt { get; set; }
		/// <summary>
		/// AvailabilityListing URL
		/// </summary>
		public string AvailabilityListing { get; set; }
		/// <summary>
		/// List of Addresses for this credential, using Place
		/// </summary>
		public List<Place> AvailableAt { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies the credential and supports its discovery and use.
		/// </summary>
		public string CodedNotation { get; set; }
		/// <summary>
		/// ISIC Revision 4 Code
		/// </summary>
		public string ISICV4 { get; set; }

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

		public string DateEffective { get; set; }


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
		//There are currently two separate approaches to publishing properties like assertedIn
		//- Publish all 'IN' properties using JurisdictionAssertions
		//- Publish using ehe separate specific properties like AccreditedIn, ApprovedIn, etc
		// 2010-01-06 The property JurisdictionAssertions may become obsolete soon. We recomend to NOT use this property.

		/// <summary>
		/// Handling assertions in jurisdictions
		/// The property JurisdictionAssertions is a simple approach, using one record per asserting organization - where that organization will have multiple assertion types. 
		/// The JurisdictionAssertedInProfile has a list of boolean properties where the assertion(s) can be selected.
		/// This approach simplifies the input where the same organization asserts more than action. 
		/// 2010-01-06 TBD - this property will LIKELY be made obsolete once any partner who has been using it has been informed.
		/// </summary>
		//[Obsolete]
		//public List<JurisdictionAssertedInProfile> JurisdictionAssertions { get; set; } = new List<JurisdictionAssertedInProfile>();

		//JurisdictionAssertion
		//Each 'IN' property must include one or more organizations and a Main jurisdiction. Only one main jusrisdiction (and multiple exceptions) can be entered with each property.
		//Only use this property where the organization only makes the assertion for a specific jurisdiction. 
		//Use the 'BY' equivalent (ex. accreditedBy) where the organization makes a general assertion

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

		/// <summary>
		/// List of credentials that are part of this credential
		/// </summary>
		public List<EntityReference> HasPart { get; set; }

		/// <summary>
		/// List of credentials where this credential is a part of.
		/// </summary>
		public List<EntityReference> IsPartOf { get; set; }

		public string ProcessStandards { get; set; }
		public string ProcessStandardsDescription { get; set; }
		public LanguageMap ProcessStandardsDescription_Map { get; set; } = new LanguageMap();

		public List<string> Keyword { get; set; }
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		public string LatestVersion { get; set; } //URL
		public string PreviousVersion { get; set; } //URL
		public string NextVersion { get; set; } //URL
		public string SupersededBy { get; set; } //URL
		public string Supersedes { get; set; } //URL

		public List<string> Subject { get; set; }
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();
		public List<string> AudienceLevelType { get; set; }
		public List<string> AudienceType { get; set; }
		public List<string> AssessmentDeliveryType { get; set; } = new List<string>();
		public List<string> LearningDeliveryType { get; set; } = new List<string>();

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
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> Naics { get; set; }

		/// <summary>
		/// InstructionalProgramType
		/// Type of instructional program; select from an existing enumeration of such types.
		/// </summary>
		public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();

		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();

		//
		//Navy
		/// <summary>
		/// HasRating
		/// Rating related to this resource.
		/// URI to a Rating
		/// </summary>
		public List<string> HasRating { get; set; } = new List<string>();

		#region Properties allowed only for degree types

		public List<string> DegreeConcentration { get; set; }
		public LanguageMapList DegreeConcentration_Map { get; set; } = new LanguageMapList();
		public List<string> DegreeMajor { get; set; }
		public LanguageMapList DegreeMajor_Map { get; set; } = new LanguageMapList();
		public List<string> DegreeMinor { get; set; }
		public LanguageMapList DegreeMinor_Map { get; set; } = new LanguageMapList();
		#endregion

		//external classes
		/// <summary>
		/// List of CTIDs or full URLs for a CostManifest published by the owning organization
		/// </summary>
		public List<string> CommonCosts { get; set; }
		/// <summary>
		/// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
		/// </summary>
		public List<string> CommonConditions { get; set; }
		/// <summary>
		/// List of cost profiles for this credential
		/// </summary>
		public List<CostProfile> EstimatedCost { get; set; } = new List<CostProfile>();

		/// <summary>
		/// List of duration profiles expressing one or more duration profiles to attain this credential
		/// </summary>
		public List<DurationProfile> EstimatedDuration { get; set; } = new List<DurationProfile>();
		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();

		/// <summary>
		/// Geographic or political region in which the credential is formally applicable or an organization has authority to act.
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();

		/// <summary>
		/// Frequency with which the credential needs to be renewed.
		/// A single DurationItem, using Years, Months, etc. or a combination.
		/// Alternately the actual ISO8601 format may be provided (using Duration_ISO8601)
		/// </summary>
		public DurationItem RenewalFrequency { get; set; } = new DurationItem();

		/// <summary>
		/// Revocation Profile
		/// Entity describing conditions and methods by which a credential can be removed from a holder.
		/// </summary>
		public List<RevocationProfile> Revocation { get; set; } = new List<RevocationProfile>();

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();


		#region -- Condition Profiles --
		public List<ConditionProfile> Requires { get; set; }
		public List<ConditionProfile> Recommends { get; set; }
		public List<ConditionProfile> Corequisite { get; set; }
		public List<ConditionProfile> Renewal { get; set; }
		#endregion


		#region -- Connections Profiles --
		public List<Connections> AdvancedStandingFrom { get; set; }
		public List<Connections> IsAdvancedStandingFor { get; set; }
		public List<Connections> IsPreparationFor { get; set; }
		public List<Connections> IsRecommendedFor { get; set; }
		public List<Connections> IsRequiredFor { get; set; }
		public List<Connections> PreparationFrom { get; set; }
		#endregion

		#region -- Process Profiles --
		public List<ProcessProfile> AdministrationProcess { get; set; }
		public List<ProcessProfile> DevelopmentProcess { get; set; }
		public List<ProcessProfile> MaintenanceProcess { get; set; }
		public List<ProcessProfile> AppealProcess { get; set; }
		public List<ProcessProfile> ComplaintProcess { get; set; }
		public List<ProcessProfile> ReviewProcess { get; set; }
		public List<ProcessProfile> RevocationProcess { get; set; }
		#endregion



	}

	/// <summary>
	/// Revocation Profile
	/// The conditions and methods by which a credential can be removed from a holder.
	/// </summary>
	public class RevocationProfile
	{
		public RevocationProfile()
		{
			Jurisdiction = new List<Input.Jurisdiction>();
			//RevocationCriteria = new List<string>();
		}
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		//public List<string> CredentialProfiled { get; set; }
		public string DateEffective { get; set; }
		public List<Jurisdiction> Jurisdiction { get; set; }
		public string RevocationCriteria { get; set; }
		public string RevocationCriteriaDescription { get; set; }
		public LanguageMap RevocationCriteriaDescription_Map { get; set; } = new LanguageMap();

	}
}
