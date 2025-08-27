// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	public class SupportServiceRequest : BaseRequest
	{
		public SupportServiceRequest()
		{
			SupportService = new SupportService();
		}

		public SupportService SupportService { get; set; }
	}

	/// <summary>
	/// Allow publishing a list of Suport services.
	/// Initially there will not be a limit on the number of services that can be provided.
	/// Only used by the endpoint: bulkPublish
	/// </summary>
	public class BulkSupportServiceRequest : BaseRequest
	{
		public BulkSupportServiceRequest()
		{
		}

		/// <summary>
		/// List of support services to publish
		/// </summary>
		public List<SupportService> SupportServices { get; set; }
	}

	/// <summary>
	/// Resources and assistance that help people overcome barriers to succeed in their education and career goals.
	/// Support services can be provided at any stage of an individual's education or career, and may be targeted towards people with or without direct affiliation with an organization. The goal of support services is to provide people with the assistance they need to achieve their full potential. Examples of Support Services include career advice, job placement, childcare, transportation, tools, mentorship, counseling, and other forms of aid.
	/// </summary>
	public class SupportService : BasePrimaryResource
	{

		#region *** Required Properties ***

		/// <summary>
		/// Credential Identifier
		/// format:
		/// ce-UUID (guid)
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
		/// Statement, characterization or account of the entity.
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		/// <summary>
		/// Organization(s) that offer this resource
		/// Must have at least one of OfferedBy or OwnedBy
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; } = new List<Input.OrganizationReference>();

		/// <summary>
		/// Organization(s) that owns this resource
		/// Must have at least one of OfferedBy or OwnedBy
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<Input.OrganizationReference>();

		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages; e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// List of language codes. ex: en-US, es
		/// Required
		/// </summary>
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Required
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; } = "lifeCycle:Active";

		#endregion

		#region  RECOMMENDED

		/// <summary>
		/// Type of means by which a learning opportunity or assessment is delivered to credential seekers and by which they interact; select from an existing enumeration of such types.
		/// deliveryType:BlendedDelivery deliveryType:InPerson deliveryType:OnlineOnly
		/// ConceptScheme: <see href="https://credreg.net/ctdl/terms/Delivery"></see>
		/// </summary>
		public List<string> DeliveryType { get; set; }

		/// <summary>
		/// Estimated cost of a credential, learning opportunity or assessment.
		/// </summary>
		public List<CostProfile> EstimatedCost { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }
		#endregion

		// =========== optional ================================

		/// <summary>
		/// Type of modification to facilitate equal access for people to a physical location, resource, or service.
		/// Accommodation?
		/// <see href="https://credreg.net/ctdl/terms/AccommodationType"></see>
		/// ConceptScheme: ceterms:Accommodation
		/// <see href="https://credreg.net/ctdl/terms/Accommodation"></see>
		/// </summary>
		public List<string> AccommodationType { get; set; }

		/// <summary>
		/// List of Alternate Names for this learning opportunity
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateNameLangMap { get; set; } = null;

		/// <summary>
		/// Online location where the credential, assessment, or learning opportunity can be pursued.
		/// URL
		/// </summary>
		public List<string> AvailableOnlineAt { get; set; }

		/// <summary>
		/// Listing of online and/or physical locations where a credential can be pursued.
		/// URL
		/// </summary>
		public List<string> AvailabilityListing { get; set; }

		/// <summary>
		/// Physical location where the credential, assessment, or learning opportunity can be pursued.
		/// Place
		/// </summary>
		public List<Place> AvailableAt { get; set; }

		/// <summary>
		/// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
		/// Set constraints, prerequisites, entry conditions, or requirements that are shared across an organization, organizational subdivision, set of credentials, or category of entities and activities.
		/// </summary>
		public List<string> CommonConditions { get; set; }

		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization.
		/// Set of costs maintained at an organizational or sub-organizational level, which apply to this learning opportunity.
		/// </summary>
		public List<string> CommonCosts { get; set; }

		/// <summary>
		/// Start Date of this resource
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// End date of the learning opportunity if applicable
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see href="https://purl.org/ctdl/terms/identifier"></see>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// Reference to a relevant support service related to this support service.
		/// List of CTIDs for published resources
		/// </summary>
		public List<string> HasSupportService { get; set; } = new List<string>();

        /// Reference to a relevant support service related to this support service.
        /// List of CTIDs for published resources
        /// </summary>
        public List<string> HasSpecificService { get; set; } = new List<string>();

        /// <summary>
        /// Reference to support services that this support service is part of or related to.
        /// List of CTIDs for published resources
        /// </summary>
        public List<string> IsSpecificServiceOf { get; set; } = new List<string>();

        /// <summary>
        /// Keyword or key phrase describing relevant aspects of an entity.
        /// </summary>
        public List<string> Keyword { get; set; }

		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList KeywordLangMap { get; set; } = null;

		#region Occupation type

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

		#endregion

		/// <summary>
		/// List of Organizations that offer this learning opportunity in a specific Jurisdiction.
		/// </summary>
		public List<JurisdictionProfile> OfferedIn { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		/// Qualifying requirements for receiving a support service.
		/// </summary>
		public List<ConditionProfile> SupportServiceCondition { get; set; }

		/// <summary>
		/// Types of support services offered by an agent; select from an existing enumeration of such types.
		/// SupportService?
		/// <see href="https://credreg.net/ctdl/terms/SupportServiceType"></see>
		/// ConceptScheme: <see href="https://credreg.net/ctdl/terms/SupportServiceCategory"></see>
		/// </summary>
		public List<string> SupportServiceType { get; set; }

		#region -- Process Profiles --

		/// <summary>
		/// Description of a process by which a resource is administered.
		/// ceterms:administrationProcess
		/// </summary>
		public List<ProcessProfile> AdministrationProcess { get; set; }

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
