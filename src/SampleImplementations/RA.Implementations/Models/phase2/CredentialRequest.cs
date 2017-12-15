using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;

namespace RA.Implementations.Models.phase1
{
	/// <summary>
	/// Class used with a Credential format or publish request
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


	}

	public class Credential
	{
		public Credential()
		{
			AudienceLevelType = new List<string>();
			Subject = new List<string>();
			Keyword = new List<string>();

			OwnedBy = new List<OrganizationReference>();
			OfferedBy = new List<OrganizationReference>();
		}


		#region *** Required Properties ***



		/// <summary>
		/// Name of this credential
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Credential description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// CTID - unique identifier
		/// If not provided, will be set to ce-UUID
		/// ex: ce-F22CA1DC-2D2E-49C0-9E72-0457AD348873
		/// It will be the primary key for retrieving this entity from the registry. 
		/// Also it must be provided 
		/// </summary>
		public string Ctid { get; set; }
		/// <summary>
		/// The credential type as defined in CTDL
		/// </summary>
		public string CredentialType { get; set; }

		/// <summary>
		/// SubjectWebpage URL
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		public string CredentialStatusType { get; set; }
		#endregion

		#region *** At least one of  ***
		//(more in next phase) 
		/// <summary>
		/// Organization(s) that own this credential
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		/// <summary>
		/// Organization(s) that offers this credential
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }
		#endregion

		#region *** Required if available Properties ***

		#endregion

		#region *** Recommended Properties ***
		//more in next phase
		public string DateEffective { get; set; }

		public List<string> Keyword { get; set; }
		public List<string> AudienceLevelType { get; set; }
		public List<FrameworkItem> OccupationType { get; set; }
		public List<FrameworkItem> IndustryType { get; set; }

		public List<CostProfile> EstimatedCost { get; set; }

		public List<DurationProfile> EstimatedDuration { get; set; }
		public List<Place> AvailableAt { get; set; }

		/// <summary>
		/// AvailabilityListing URL
		/// </summary>
		public string AvailabilityListing { get; set; }

		/// <summary>
		/// AvailableOnlineAt URL
		/// </summary>
		public string AvailableOnlineAt { get; set; }

		//the BYs
		public List<OrganizationReference> AccreditedBy { get; set; }
		public List<OrganizationReference> ApprovedBy { get; set; }
		//public List<OrganizationReference> OfferedBy { get; set; }
		public List<OrganizationReference> RecognizedBy { get; set; }
		public List<OrganizationReference> RegulatedBy { get; set; }
		public List<OrganizationReference> RenewedBy { get; set; }
		public List<OrganizationReference> RevokedBy { get; set; }

		//Conditions
		/// <summary>
		/// Requirement or set of requirements for this credential, learning opportunity, or assessment.
		/// </summary>
		public List<ConditionProfile> Requires { get; set; }
		/// <summary>
		/// Entity describing the constraints, prerequisites, entry conditions, or requirements necessary to maintenance and renewal of an awarded credential.
		/// </summary>
		public List<ConditionProfile> Renewal { get; set; }

		public List<ConditionProfile> Recommends { get; set; }
		public List<ConditionProfile> Corequisite { get; set; }

		/// <summary>
		/// Entity that describes the processes and criteria for ending (revoking) the validity or operation of an awarded credential.
		/// </summary>
		public List<RevocationProfile> Revocation { get; set; }
		#endregion


		public List<string> CommonCosts { get; set; }
		public List<string> CommonConditions { get; set; }

		public string AlternateName { get; set; }
		/// <summary>
		/// Image URL
		/// </summary>
		public string Image { get; set; }


		public List<EntityReference> HasPart { get; set; }
		public List<EntityReference> IsPartOf { get; set; }


		public string CredentialId { get; set; }

		public List<IdentifierValue> VersionIdentifier { get; set; }
		public List<string> CodedNotation { get; set; }
		public List<string> InLanguage { get; set; }
		public string ProcessStandards { get; set; }
		public string ProcessStandardsDescription { get; set; }

		public string LatestVersion { get; set; }
		public string PreviousVersion { get; set; }
		public List<string> Subject { get; set; }


		public List<string> Naics { get; set; }


		public List<string> DegreeConcentration { get; set; }
		public List<string> DegreeMajor { get; set; }
		public List<string> DegreeMinor { get; set; }

		public DurationItem RenewalFrequency { get; set; }

		public List<Jurisdiction> Jurisdiction { get; set; }



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




	}

	/// <summary>
	/// Revocation Profile
	/// The conditions and methods by which a credential can be removed from a holder.
	/// </summary>
	public class RevocationProfile
	{
		public RevocationProfile()
		{
			Jurisdiction = new List<Jurisdiction>();
			//RevocationCriteria = new List<string>();
		}
		public string Description { get; set; }
		//public List<string> CredentialProfiled { get; set; }
		public string DateEffective { get; set; }
		public List<Jurisdiction> Jurisdiction { get; set; }
		public string RevocationCriteria { get; set; }
		public string RevocationCriteriaDescription { get; set; }

	}
}
