using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Accredit Action
	/// Action by an independent, neutral, and authoritative agent that certifies an entity as meeting a prescribed set of standards.
	/// <seealso cref="https://credreg.net/ctdl/terms"/>
	/// </summary>
	public class AccreditAction
	{
		/*
			ceterms:actionStatusType	
				ActionStatus->CAO
				actionStat:ActiveActionStatus 
				actionStat:CompletedActionStatus 
				actionStat:FailedActionStatus 
				actionStat:PotentialActionStatus
			ceterms:object
			ceterms:resultingAward		???????????
		 */


		/// <summary>
		/// Action Status
		/// Types of current status of an action.
		/// Available statuses include ActiveActionStatus, CompletedActionStatus, FailedActionStatus, PotentialActionStatus.
		/// </summary>
		public string ActionStatusType { get; set; }

		/// <summary>
		/// Acting Agent
		/// Provide the CTID for a participant in the Credential Registry or provide minimum data where not in the registry.
		/// </summary>
		public OrganizationReference ActingAgent { get; set; } = new OrganizationReference();

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
		/// </summary>
		public EntityReference Instrument { get; set; } = new EntityReference();

		/// <summary>
		/// Object
		/// Object upon which the action is carried out, whose state is kept intact or changed.
		/// An EntityReference for Credentials, AssessmentProfile, or LearningOpportunity Profile
		/// </summary>
		public EntityReference Object { get; set; } = new EntityReference();

		/// <summary>
		/// Participant
		/// Co-agents that participated in the action indirectly.
		/// Provide the CTID for a participant in the Credential Registry or provide minimum data where not in the registry.
		/// </summary>
		public OrganizationReference Participant { get; set; } = new OrganizationReference();

		/// <summary>
		/// Resulting Award
		/// Awarded credential resulting from an action.
		/// Range: ceterms:CredentialAssertion
		/// ???		https://www.imsglobal.org/sites/default/files/Badges/OBv2p0Final/index.html#Assertion
		/// </summary>
		public string ResultingAward { get; set; }

		/// <summary>
		/// Date this action starts.
		/// </summary>
		public string StartDate { get; set; }

		/// <summary>
		/// Date this action ends.
		/// </summary>
		public string EndDate { get; set; }

	}
}
