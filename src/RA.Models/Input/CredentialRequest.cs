using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models;

namespace RA.Models.Input
{
	/// <summary>
	/// Class used with a CredentiAL format or publish request
	/// </summary>
	public class CredentialRequest
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
		/// API key for the requesting partner - required for publishing
		/// May ultimately be passed in the header
		/// </summary>
		public string APIKey { get; set; }

		/// <summary>
		/// Envelope Identifier
		/// Previously required to update an existing document
		/// </summary>
		public string RegistryEnvelopeId { get; set; }

	}

	//public class CredentialRequestHelper
	//{
	//	public CredentialRequestHelper( CredentialRequest request)
	//	{
	//		Request = request;
	//		RequestStatus = new RequestStatus();
	//	}
	//	//public CredentialRequestHelper()
	//	//{
	//	//	Request = new CredentialRequest();
	//	//	RequestStatus = new RequestStatus();
	//	//}
	//	/// <summary>
	//	/// Credential Input Class
	//	/// </summary>
	//	public CredentialRequest Request { get; set; }


	//	public string Payload { get; set; }

	//	public RequestStatus RequestStatus { get; set; }
	//}

	public class Credential
	{
		public Credential()
		{
			AudienceLevelType = new List<string>();
			Subject = new List<string>();
			OccupationType = new List<FrameworkItem>();
			IndustryType = new List<FrameworkItem>();
			Keyword = new List<string>();
			DegreeConcentration = new List<string>();
			DegreeMajor = new List<string>();
			DegreeMinor = new List<string>();

			EstimatedCost = new List<CostProfile>();
			EstimatedDuration = new List<DurationProfile>();

			Jurisdiction = new List<Jurisdiction>();
			// Region = new List<GeoCoordinates>();
			OwnedBy = new OrganizationReference();
			CopyrightHolder = new OrganizationReference();

			HasPart = new List<EntityReference>();

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
			CommonConditions = new List<EntityReference>();
			CommonCosts = new List<EntityReference>();
			HasPart = new List<EntityReference>();

			FinancialAssistance = new List<Input.FinancialAlignmentObject>();
			AdvancedStandingFrom = new List<Connections>();
			IsAdvancedStandingFor = new List<Connections>();
			PreparationFrom = new List<Connections>();
			IsPreparationFor = new List<Connections>();
			IsRecommendedFor = new List<Connections>();
			IsRequiredFor = new List<Connections>();
			InLanguage = new List<string>();
			AvailableAt = new List<PostalAddress>();
			CodedNotation = new List<string>();
			Renewal = new List<ConditionProfile>();
			Revocation = new List<RevocationProfile>();
			AdministrationProcess = new List<ProcessProfile>();
			DevelopmentProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();
			AppealProcess = new List<ProcessProfile>();
			ComplaintProcess = new List<ProcessProfile>();
			ReviewProcess = new List<ProcessProfile>();
			RevocationProcess = new List<ProcessProfile>();
		}

		/// <summary>
		/// Name of this credential
		/// </summary>
		public string Name { get; set; }
		public string Description { get; set; }
		public string DateEffective { get; set; }
		/// <summary>
		/// Organization that owns this credential
		/// </summary>
		public OrganizationReference OwnedBy { get; set; }

		/// <summary>
		/// CTID - unique identifier
		/// If not provided, will be set to ce-UUID
		/// ex: ce-F22CA1DC-2D2E-49C0-9E72-0457AD348873
		/// It will be the primary key for retrieving this entity from the registry. 
		/// Also it must be provided 
		/// </summary>
		public string Ctid { get; set; }

		public string AlternateName { get; set; }
		/// <summary>
		/// Image URL
		/// </summary>
		public string Image { get; set; }
		/// <summary>
		/// SubjectWebpage URL
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// The credential type as defined in CTDL
		/// </summary>
		public string CredentialType { get; set; }

		/// <summary>
		/// AvailableOnlineAt URL
		/// </summary>
		public string AvailableOnlineAt { get; set; }

		public List<EntityReference> HasPart { get; set; }

		public List<PostalAddress> AvailableAt { get; set; }

		public OrganizationReference CopyrightHolder { get; set; }
		/// <summary>
		/// could use general input or specific
		/// general has more opportunity for errors
		/// </summary>
		//public List<OrgRole> AgentRoles { get; set; }
		/*
		 * ceterms:accreditedBy
			ceterms:approvedBy
			ceterms:offeredBy
			ceterms:recognizedBy
			ceterms:regulatedBy
			ceterms:renewedBy
			ceterms:revokedBy
		 * 
		 * 
		 * 
		 * 
		 */
		public List<OrganizationReference> AccreditedBy { get; set; }
		public List<OrganizationReference> ApprovedBy { get; set; }
		public List<OrganizationReference> OfferedBy { get; set; }
		public List<OrganizationReference> RecognizedBy { get; set; }
		public List<OrganizationReference> RegulatedBy { get; set; }
		public List<OrganizationReference> RenewedBy { get; set; }
		public List<OrganizationReference> RevokedBy { get; set; }

		#region INs
		/// <summary>
		/// Handling assertions in jurisdictions
		/// The current approach is to use a one record per asserting organization. The JurisdictionAssertedInProfile has a list of boolean properties where the assertion(s) can be selected
		/// </summary>
		public List<JurisdictionAssertedInProfile> JurisdictionAssertions { get; set; }

	
		#endregion

		/// <summary>
		/// AvailabilityListing URL
		/// </summary>
		public string AvailabilityListing { get; set; }
		public string CredentialId { get; set; }
		public string VersionIdentifier { get; set; }
		public List<string> CodedNotation { get; set; }
		public List<string> InLanguage { get; set; }
		public string ProcessStandards { get; set; }
		public string ProcessStandardsDescription { get; set; }

		public string LatestVersion { get; set; }
		public string PreviousVersion { get; set; }
		public List<string> Subject { get; set; }
		public List<string> AudienceLevelType { get; set; }
		public List<FrameworkItem> OccupationType { get; set; }
		public List<FrameworkItem> IndustryType { get; set; }
		public List<string> Keyword { get; set; }

		public List<string> DegreeConcentration { get; set; }
		public List<string> DegreeMajor { get; set; }
		public List<string> DegreeMinor { get; set; }
		//external classes
		public List<CostProfile> EstimatedCost { get; set; }

		public List<DurationProfile> EstimatedDuration { get; set; }

		public List<Jurisdiction> Jurisdiction { get; set; }
		// public List<GeoCoordinates> Region { get; set; }

		/// <summary>
		/// Container for all condition profiles for entity
		/// </summary>        
		//public ConditionProfiles ConditionProfiles { get; set; }

		public List<ConditionProfile> Requires { get; set; }
		public List<ConditionProfile> Recommends { get; set; }
		public List<ConditionProfile> Corequisite { get; set; }
		public List<ConditionProfile> Renewal { get; set; }

		public List<EntityReference> CommonCosts { get; set; }
		public List<EntityReference> CommonConditions { get; set; }

		public List<Connections> AdvancedStandingFrom { get; set; }
		public List<Connections> IsAdvancedStandingFor { get; set; }
		public List<Connections> IsPreparationFor { get; set; }
		public List<Connections> IsRecommendedFor { get; set; }
		public List<Connections> IsRequiredFor { get; set; }
		public List<Connections> PreparationFrom { get; set; }

		public List<ProcessProfile> AdministrationProcess { get; set; }
		public List<ProcessProfile> DevelopmentProcess { get; set; }
		public List<ProcessProfile> MaintenanceProcess { get; set; }
		public List<ProcessProfile> AppealProcess { get; set; }
		public List<ProcessProfile> ComplaintProcess { get; set; }
		public List<ProcessProfile> ReviewProcess { get; set; }
		public List<ProcessProfile> RevocationProcess { get; set; }
		public List<FinancialAlignmentObject> FinancialAssistance { get; set; }

		public string CredentialStatusType { get; set; }
		public List<RevocationProfile> Revocation { get; set; }
		
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
			RevocationCriteria = new List<string>();
		}
		public string Description { get; set; }
		//public List<string> CredentialProfiled { get; set; }
		public string DateEffective { get; set; }
		public List<Jurisdiction> Jurisdiction { get; set; }
		public List<string> RevocationCriteria { get; set; }
		public string RevocationCriteriaDescription { get; set; }

	}
}
