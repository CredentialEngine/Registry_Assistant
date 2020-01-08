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

		/// <summary>
		/// PROTOTYPE - NOT ALLOWED IN PRODUCTION
		/// Allow publishing up to 10 credentials at a time. 
		/// Only used by the endpoint: bulkPublish
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

			EstimatedCost = new List<CostProfile>();
			EstimatedDuration = new List<DurationProfile>();
			RenewalFrequency = new DurationItem();
			
			Jurisdiction = new List<Jurisdiction>();
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
			Revocation = new List<RevocationProfile>();
			AdministrationProcess = new List<ProcessProfile>();
			DevelopmentProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();
			AppealProcess = new List<ProcessProfile>();
			ComplaintProcess = new List<ProcessProfile>();
			ReviewProcess = new List<ProcessProfile>();
			RevocationProcess = new List<ProcessProfile>();

			VersionIdentifier = new List<IdentifierValue>();
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
        /// AlternateName
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
        public List<Place> AvailableAt { get; set; }

		public string CodedNotation { get; set; }
		/// <summary>
		/// ISIC Revision 4 Code
		/// </summary>
		public string ISICV4 { get; set; }

		public OrganizationReference CopyrightHolder { get; set; }
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
		[Obsolete]
		public List<JurisdictionAssertedInProfile> JurisdictionAssertions { get; set; } = new List<JurisdictionAssertedInProfile>();

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
		public List<FrameworkItem> OccupationType { get; set; }
		public List<string> AlternativeOccupationType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeOccupationType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// </summary>
		public List<string> ONET_Codes { get; set; } = new List<string>();

		public List<FrameworkItem> IndustryType { get; set; }
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> Naics { get; set; }

		public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeInstructionalProgramType_Map { get; set; } = new LanguageMapList();
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
		//prototyping
		public List<FrameworkItem> NavyRating { get; set; } = new List<FrameworkItem>();

		#region Properties allowed only for degree types

		public List<string> DegreeConcentration { get; set; }
        public LanguageMapList DegreeConcentration_Map { get; set; } = new LanguageMapList();
        public List<string> DegreeMajor { get; set; }
        public LanguageMapList DegreeMajor_Map { get; set; } = new LanguageMapList();
        public List<string> DegreeMinor { get; set; }
        public LanguageMapList DegreeMinor_Map { get; set; } = new LanguageMapList();
		#endregion

		//external classes

		public List<string> CommonCosts { get; set; }
		public List<string> CommonConditions { get; set; }
		public List<CostProfile> EstimatedCost { get; set; }

		public List<DurationProfile> EstimatedDuration { get; set; }
		//[obsolete]
		//public List<FinancialAlignmentObject> FinancialAssistanceOLD { get; set; } = new List<FinancialAlignmentObject>();
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();

		/// <summary>
		/// Geographic or political region in which the credential is formally applicable or an organization has authority to act.
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; }
		public DurationItem RenewalFrequency { get; set; }
		public List<RevocationProfile> Revocation { get; set; }

		public List<IdentifierValue> VersionIdentifier { get; set; }
		

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
