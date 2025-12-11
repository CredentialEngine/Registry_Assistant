// <copyright file="SupportService.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class SupportService : BaseResourceDocument
	{
		public SupportService()
		{
			Type = "ceterms:SupportService";
		}

		/// <summary>
		/// Need a custom mapping to @type based on input value
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// Resource Locator
		/// </summary>
		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		/// <summary>
		/// Name or title of the resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		/// <summary>
		/// Statement, characterization or account of the entity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Agent that offers the resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:offeredBy" )]
		public List<string> OfferedBy { get; set; }

		/// <summary>
		/// Agent that owns the resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages; e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// List of language codes. ex: en, es
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:inLanguage" )]
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Type of modification to facilitate equal access for people to a physical location, resource, or service.
		/// Accommodation?
		/// <see href="https://credreg.net/ctdl/terms/Accommodation"></see>
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:accommodationType" )]
		public List<CredentialAlignmentObject> AccommodationType { get; set; }

		/// <summary>
		/// The status type of this LearningOpportunityProfile.
		/// The default is Active.
		/// ConceptScheme: ceterms:LifeCycleStatus
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:lifeCycleStatusType" )]
		public CredentialAlignmentObject LifeCycleStatusType { get; set; }

		/// <summary>
		/// Start Date of this resource
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		/// <summary>
		/// End date of the learning opportunity if applicable
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		/// <summary>
		/// List of Alternate Names for this learning opportunity
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		/// <summary>
		/// Physical location where the credential, assessment, or learning opportunity can be pursued.
		/// Place
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:availableAt" )]
		public List<Place> AvailableAt { get; set; }

		/// <summary>
		/// Listing of online and/or physical locations where a credential can be pursued.
		/// URL
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:availabilityListing" )]
		public List<string> AvailabilityListing { get; set; }

		/// <summary>
		/// Online location where the credential, assessment, or learning opportunity can be pursued.
		/// URL
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:availableOnlineAt" )] // URL
		public List<string> AvailableOnlineAt { get; set; }

		/// <summary>
		/// Type of means by which a learning opportunity or assessment is delivered to credential seekers and by which they interact; select from an existing enumeration of such types.
		/// deliveryType:BlendedDelivery deliveryType:InPerson deliveryType:OnlineOnly
		/// <see href="https://credreg.net/ctdl/terms/Delivery"></see>
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:deliveryType" )]
		public List<CredentialAlignmentObject> DeliveryType { get; set; }

		/// <summary>
		/// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
		/// Set constraints, prerequisites, entry conditions, or requirements that are shared across an organization, organizational subdivision, set of credentials, or category of entities and activities.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:commonConditions" )]
		public List<string> CommonConditions { get; set; }

		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization.
		/// Set of costs maintained at an organizational or sub-organizational level, which apply to this learning opportunity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:commonCosts" )]
		public List<string> CommonCosts { get; set; }

		/// <summary>
		/// Estimated cost of a credential, learning opportunity or assessment.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:estimatedCost" )]
		public List<CostProfile> EstimatedCost { get; set; }

		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:financialAssistance" )]
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; }

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see href="https://purl.org/ctdl/terms/identifier"></see>
		/// ceterms:identifier
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		/// <summary>
		/// Support service that is part of this support service.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasSpecificService" )]
		public List<string> HasSpecificService { get; set; }

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net.
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:occupationType" )]
		public List<CredentialAlignmentObject> OccupationType { get; set; }

		/// <summary>
		/// List of Organizations that offer this learning opportunity in a specific Jurisdiction.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:offeredIn" )]
		public List<JurisdictionProfile> OfferedIn { get; set; }

		/// <summary>
		/// Qualifying requirements for receiving a support service.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:supportServiceCondition" )]
		public List<ConditionProfile> SupportServiceCondition { get; set; }

		/// <summary>
		/// Types of support services offered by an agent; select from an existing enumeration of such types.
		/// ConceptScheme:SupportServiceCategory
		/// <see href="https://credreg.net/ctdl/terms/SupportServiceCategory"></see>
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:supportServiceType" )]
		public List<CredentialAlignmentObject> SupportServiceType { get; set; }

		#region process profiles
		[JsonProperty( PropertyName = "ceterms:administrationProcess" )]
		public List<ProcessProfile> AdministrationProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:complaintProcess" )]
		public List<ProcessProfile> ComplaintProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }
		#endregion
	}
}
