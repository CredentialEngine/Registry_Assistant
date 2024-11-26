using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Credentialing Action
    /// Action taken by an agent affecting the status of an object entity.
    /// </summary>
    public class CredentialingAction
	{
		/// <summary>
		/// Type of credentialing action
		/// One of
		/// ceterms:AccreditAction 
		/// ceterms:AdvancedStandingAction 
		/// ceterms:ApproveAction 
		/// ceterms:OfferAction 
		/// ceterms:RecognizeAction 
		/// ceterms:RegistrationAction 
		/// ceterms:RegulateAction 
		/// ceterms:RenewAction 
		/// ceterms:RevokeAction 
		/// ceterms:RightsAction
		/// ceterms:WorkforceDemandAction
		/// REQUIRED
		/// 
		/// NOTE:
		/// ceterms:CredentialingAction	is the super class and cannot be published.
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
		/// Action Name
		/// REQUIRED
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Action Description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// Description of a process by which a resource is administered.
        /// ceterms:administrationProcess
        /// </summary>
        public List<ProcessProfile> AdministrationProcess { get; set; }

        /// <summary>
        /// Evidence of Action
        /// Entity that proves that the action occured or that the action continues to be valid.
        /// The evidence verifies the information in the action and is particular to it. It is not a directory of such evidentiary entities or a description of how such verifications might generically be characterized.
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
		/// 24-07-30 Made Object a list of strings. May need to define as an object for initial testing - just in case 
		/// Object(s) upon which the action is carried out, whose state is kept intact or changed.
		/// A reference for Credentials, AssessmentProfile, any LearningOpportunity Profile type, or any organization type
		/// Input: a CTID or a blank node Id where the bnode will be added in the request class ReferenceObjects property.
		/// </summary>
        public List<string> Object { get; set; }

        /// <summary>
        /// Participant
        /// Co-agents that participated in the action indirectly.
        /// Provide the CTID for a participant in the Credential Registry or provide minimum data where not in the registry.
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


		/// <summary>
		/// Outcome produced in the action.
		/// Domain: only ceterms:WorkforceDemandAction
		/// Range: ceterms:CredentialAlignmentObject
		/// NOT CURRENTLY USED
		/// </summary>
		public List<CredentialAlignmentObject> Result { get; set; }

		/// <summary>
		/// Image URL
		/// </summary>
		public string Image { get; set; }
	}
}
