// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{

	/// <summary>
	/// Single TransferValueProfileRequest requests
	/// </summary>
	public class TransferValueProfileRequest : BaseRequest
	{
		public TransferValueProfileRequest()
		{
		}

		/// <summary>
		/// List of TransferValueProfiles to publish
		/// </summary>
		public TransferValueProfile TransferValueProfile { get; set; } = new TransferValueProfile();
	}

	/// <summary>
	/// List of TransferValueProfileRequest requests
	/// </summary>
	public class TransferValueProfileBulkRequest : BaseRequest
	{
		public TransferValueProfileBulkRequest()
		{
		}

		/// <summary>
		/// List of TransferValueProfiles to publish
		/// </summary>
		public List<TransferValueProfile> TransferValueProfiles { get; set; } = new List<TransferValueProfile>();
	}

	/// <summary>
	/// Description of transfer value for a resource.
	/// History
	/// 21-01-13 Added DevelopementProcess
	/// </summary>
	public class TransferValueProfile
	{

		#region Required

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name or title of the resource.
		/// Required
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///  LanguageMap for Name
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; } = null;

		/// <summary>
		/// Transfer Value Profile Description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		/// <summary>
		/// A third party version of the entity being referenced that has been modified in meaning through editing, extension or refinement.
		/// List of CTIDs - must exist in the registry
		/// ceasn:derivedFrom
		/// </summary>
		public List<string> DerivedFrom { get; set; } = new List<string>();

		/// <summary>
		/// Organization(s) that owns this resource
		/// Required
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Webpage that describes this entity.
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; } // URL

		/// <summary>
		/// A suggested or articulated credit- or point-related transfer value.
		/// Required
		/// </summary>
		public List<ValueProfile> TransferValue { get; set; } = new List<ValueProfile>();
		#endregion

		/// <summary>
		/// Identifier
		/// Definition:	en-US: Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		public string InCatalog { get; set; }

		/// <summary>
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Recommended
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// Date the validity or usefulness of the information in this resource begins.
		/// </summary>
		public string StartDate { get; set; }

		/// <summary>
		/// Date this assertion ends.
		/// Required
		/// </summary>
		public string EndDate { get; set; }

		/// <summary>
		///  Resource that provides the transfer value described by this resource, according to the entity providing this resource.
		///  2020-12-07 The data type was changed to object. The handled CTDL classes are:
		///  - AssessmentProfile
		///  - LearningOpportunityProfile
		///  - All credential classes
		///  - Competency - TBD
		///  - Job  - TBD
		///  - Occupation  - TBD
		///  2021-05-10 An additional type will be a EntityReference. Actually any of the latter with a type and CTID
		///  23-08-23 mparsons - this will be much easier to start using just URIs, and ReferenceObjects for blank nodes
		/// </summary>
		public List<object> TransferValueFrom { get; set; } = new List<object>();

		/// <summary>
		///  Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
		///  2020-12-07 The data type was changed to object. The handled CTDL classes are:
		///  - AssessmentProfile
		///  - LearningOpportunityProfile
		///  - All credential classes
		///  - Competency - TBD
		///  - Job  - TBD
		///  - Occupation  - TBD
		///  2021-05-10 An additional type will be a EntityReference. Actually any of the latter with a type and CTID
		/// </summary>
		public List<object> TransferValueFor { get; set; } = new List<object>();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateNameLangMap { get; set; } = null;
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
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		public string VersionCode { get; set; }

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner.
		/// The resource version captured here is any local identifier used by the resource owner to identify the version of the resource in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();

		#endregion

		#region -- Process Profiles --

		/// <summary>
		/// Description of a process by which a resource is administered.
		/// ceterms:administrationProcess
		/// </summary>
		public List<ProcessProfile> AdministrationProcess { get; set; }

		/// <summary>
		/// Description of a formal process for objecting to decisions of an organization.
		/// </summary>
		public List<ProcessProfile> AppealProcess { get; set; }

		/// <summary>
		/// Description of a process for handling complaints about a resource or related resources.
		/// </summary>
		public List<ProcessProfile> ComplaintProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource was created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		///  Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion

	}
}
