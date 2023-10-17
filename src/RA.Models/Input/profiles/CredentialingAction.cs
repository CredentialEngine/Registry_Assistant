using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Credentialing Action
    /// One of:
    /// ceterms:AccreditAction 
    /// ceterms:AdvancedStandingAction 
    /// ceterms:ApproveAction 
    /// ceterms:CredentialingAction 
    /// ceterms:OfferAction 
    /// ceterms:RecognizeAction 
    /// ceterms:RegulateAction 
    /// ceterms:RenewAction 
    /// ceterms:RevokeAction 
	/// ceterms:RightsAction
    /// ceterms:WorkforceDemandAction
    /// </summary>
    public class CredentialingAction
	{
		/// <summary>
		/// Type of credentialing action
		/// One of
		/// ceterms:AccreditAction 
		/// ceterms:AdvancedStandingAction 
		/// ceterms:ApproveAction 
		/// ceterms:CredentialingAction 
		/// ceterms:OfferAction 
		/// ceterms:RecognizeAction 
		/// ceterms:RegulateAction 
		/// ceterms:RenewAction 
		/// ceterms:RevokeAction 
		/// ceterms:RightsAction
		/// ceterms:WorkforceDemandAction
		/// REQUIRED
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// REQUIRED
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Action Status
		/// Types of current status of an action.
		/// REQUIRED
		/// Available statuses include:
		///		ActiveActionStatus, 
		///		CompletedActionStatus, 
		///		FailedActionStatus, 
		///		PotentialActionStatus.
		/// <see href="https://credreg.net/ctdl/terms/ActionStatus">ActionStatus</see>
		/// </summary>
		public string ActionStatusType { get; set; }

		/// <summary>
		/// Acting Agent
		/// REQUIRED
		/// Provide the CTID for a participant in the Credential Registry or provide minimum data where not in the registry.
		/// </summary>
		public List<OrganizationReference> ActingAgent { get; set; } 

		/// <summary>
		/// Accredit Action Description
		/// REQUIRED
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Evidence of Action
		/// Entity that proves that the action occured or that the action continues to be valid.
		/// URI
		/// </summary>
		public string EvidenceOfAction { get; set; }

		/// <summary>
		/// Instrument
		/// Object that helped the agent perform the action. e.g. John wrote a book with a pen.
		/// A credential or other instrument whose criteria was applied in executing the action.
		/// Provide the CTID for a credential in the Credential Registry or provide minimum data for a credential not in the registry.
		/// TODO - implement blank node process using ReferenceObjects
		/// </summary>
		public List<string> Instrument { get; set; } = new List<string>();

		/// <summary>
		/// Object
		/// Object upon which the action is carried out, whose state is kept intact or changed.
		/// An EntityReference for Credentials, AssessmentProfile, or LearningOpportunity Profile
		/// TODO - handle coded notation in a blank node
		/// TODO - implement blank node process using ReferenceObjects
		/// </summary>
		public string Object { get; set; } 

		/// <summary>
		/// Participant
		/// Co-agents that participated in the action indirectly.
		/// Provide the CTID for a participant in the Credential Registry or provide minimum data where not in the registry.
		/// LIST????
		/// </summary>
		public List<OrganizationReference> Participant { get; set; } = new List<OrganizationReference>();


		/// <summary>
		/// Date this action starts.
		/// Full date is required
		/// xsd:date
		/// </summary>
		public string StartDate { get; set; }

		/// <summary>
		/// Date this action ends.
		/// Full date is required
		/// xsd:date
		/// </summary>
		public string EndDate { get; set; }


        /// <summary>
        /// Jurisdiction Profile
		/// ONLY valid for WorkforceDemandAction
        /// Geo-political information about applicable geographic areas and their exceptions.
        /// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"></see>
        /// </summary>
        public List<JurisdictionProfile> Jurisdiction { get; set; }



		/// <summary>
		/// Resulting Award
		/// Awarded credential resulting from an action.
		/// Domain: AccreditAction, RenewAction
		/// Range: ceterms:CredentialAssertion - Representation of a credential awarded to a person.
		/// ???		https://www.imsglobal.org/sites/default/files/Badges/OBv2p0Final/index.html#Assertion
		/// SO A URI???
		/// NOT IMPLEMENTED
		/// </summary>
		public string ResultingAward { get; set; }
	}
}
