// <copyright file="CredentialingAction.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Credentialing Action
	/// Action taken by an agent affecting the status of an object entity.
	/// REQUIRED
	/// - Type
	/// - CTID
	/// - ActionStatusType
	/// - ActingAgent
	/// - Name
	/// - Description
	/// NOTE:
	/// ceterms:CredentialingAction	is the super class and cannot be published.
	/// </summary>
	public class CredentialingAction : BaseResourceDocument
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
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; } = "ceterms:CredentialingAction";

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; } = string.Empty;

		[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		/// <summary>
		/// Types of current status of an action.
		/// REQUIRED
		/// Available statuses include ActiveActionStatus, CompletedActionStatus, FailedActionStatus, PotentialActionStatus.
		/// <see cref="https://credreg.net/ctdl/terms/ActionStatus"/>
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:actionStatusType" )]
		public CredentialAlignmentObject ActionStatusType { get; set; }

		/// <summary>
		/// Acting Agent
		/// REQUIRED
		/// Provide the CTID for a participant in the Credential Registry or provide minimum data where not in the registry.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:actingAgent" )]
		public List<string> ActingAgent { get; set; }

		/// <summary>
		/// Action Name
		/// REQUIRED
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		/// <summary>
		/// Accredit Action Description
		/// REQUIRED
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:administrationProcess" )]
		public List<ProcessProfile> AdministrationProcess { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		/// <summary>
		/// Evidence of Action
		/// Entity that proves that the action occured or that the action continues to be valid.
		/// URI
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:evidenceOfAction" )]
		public string EvidenceOfAction { get; set; }

		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		/// <summary>
		/// Instrument
		/// REQUIRED
		/// Object that helped the agent perform the action. e.g. John wrote a book with a pen.
		/// A credential or other instrument whose criteria was applied in executing the action.
		/// Provide the CTID for a credential in the Credential Registry or provide minimum data for a credential not in the registry.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:instrument" )]
		public List<string> Instrument { get; set; }

		/// <summary>
		/// Object
		/// REQUIRED
		/// Object upon which the action is carried out, whose state is kept intact or changed.
		/// An EntityReference for Credentials, AssessmentProfile, or LearningOpportunity Profile
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:object" )]
		public List<string> Object { get; set; }

		/// <summary>
		/// Participant
		/// Co-agents that participated in the action indirectly.
		/// Provide the CTID for a participant in the Credential Registry or provide minimum data where not in the registry.
		/// LIST????
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:participant" )]
		public List<string> Participant { get; set; }

		/// <summary>
		/// Date this action starts.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:startDate" )]
		public string StartDate { get; set; }

		/// <summary>
		/// Approximate date the validity or usefulness of the information in this resource begins.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approximateStartDate" )]
		public string ApproximateStartDate { get; set; }

		/// <summary>
		/// Date this action ends.
		/// </summary>
		[JsonProperty( "ceterms:endDate" )]
		public string EndDate { get; set; }

		/// <summary>
		/// Approximate date some event or activity ends.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approximateEndDate" )]
		public string ApproximateEndDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// Outcome produced in the action.
		/// Domain: only ceterms:WorkforceDemandAction
		/// Range: ceterms:CredentialAlignmentObject
		/// NOT CURRENTLY USED
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:result" )]
		public List<CredentialAlignmentObject> Result { get; set; }

		/// <summary>
		/// Resulting Award
		/// Awarded credential resulting from an action.
		/// Domain: RenewAction, AccreditAction
		/// Range: ceterms:CredentialAssertion - Representation of a credential awarded to a person.
		/// ???		https://www.imsglobal.org/sites/default/files/Badges/OBv2p0Final/index.html#Assertion
		/// NOT CURRENTLY USED
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:resultingAward" )]
		public string ResultingAward { get; set; }

		/// <summary>
		/// Image URL
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:image" )]
		public string Image { get; set; }
	}
}
