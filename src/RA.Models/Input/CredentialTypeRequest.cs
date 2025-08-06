// <copyright file="CredentialRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Class used with a CredentialType format or publish request
	/// Inverse properties. These properties are defined in CTDL for CredentialType class but are not published with a resource:
	/// - objectOfAction
	/// - relevantDataSet
	/// - targetPathway
	/// </summary>
	public class CredentialTypeRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public CredentialTypeRequest()
		{
			CredentialType = new CredentialType();
		}

		/// <summary>
		/// Credential Type Input Class
		/// </summary>
		public CredentialType CredentialType { get; set; }
	}

	/// <summary>
	/// Credential Type class
	/// REQUIRED
	/// - CTID
	/// - Name
	/// - Description
	/// - CredentialStatusType
	/// - OwnedBy
	/// - RegulatedBy
	/// - RegulatedIn
	/// </summary>
	public class CredentialType : BaseRequestHelper
	{
		/// <summary>
		/// constructor
		/// </summary>
		public CredentialType()
		{
			AudienceLevelType = new List<string>();
			InLanguage = new List<string>();
			Keyword = new List<string>();
			OwnedBy = new List<OrganizationReference>();
			IndustryType = new List<FrameworkItem>();
			Naics = new List<string>();
			Subject = new List<string>();

			ApprovedBy = new List<Input.OrganizationReference>();
			RecognizedBy = new List<Input.OrganizationReference>();
			RegulatedBy = new List<Input.OrganizationReference>();

			Corequisite = new List<ConditionProfile>();

			CommonConditions = new List<string>();

			HasProxy = new List<string>();

			IsPreparationFor = new List<ConditionProfile>();
			IsRequiredFor = new List<ConditionProfile>();

			AdministrationProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();
			ReviewProcess = new List<ProcessProfile>();
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; } = "ceterms:CredentialType";

		#region *** Required Properties ***

		/// <summary>
		/// Globally unique Credential Transparency Identifier
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
		public LanguageMap NameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Organization that owns this credential
		/// REQUIRED
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<string> OwnedByList { get; set; }

		/// <summary>
		/// The status type of this credential.
		/// REQUIRED
		/// The default is Active.
		/// </summary>
		public string CredentialStatusType { get; set; }

		[JsonProperty( PropertyName = "ceterms:credentialStatusType" )]
		public RA.Models.JsonV2.CredentialAlignmentObject CredentialStatusTypeCAO { get; set; }

		/// <summary>
		/// List of Organizations that regulate this resource
		/// REQUIRED
		/// </summary>
		public List<OrganizationReference> RegulatedBy { get; set; }

		/// <summary>
		/// List of Organizations that regulate this resource in a specific Jurisdiction.
		/// REQUIRED
		/// </summary>
		public List<JurisdictionProfile> RegulatedIn { get; set; } = new List<JurisdictionProfile>();

		#endregion

		/// <summary>
		/// List of Alternate Names for this credential
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateNameLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// List of Organizations that approve this resource
		/// </summary>
		public List<OrganizationReference> ApprovedBy { get; set; }

		/// <summary>
		/// List of Organizations that approve this resource in a specific Jurisdiction.
		/// </summary>
		public List<JurisdictionProfile> ApprovedIn { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		/// Indicates the stage or level of achievement in a progression of learning.
		/// The Target can be a ProgressionLevel or a Concept. Typically just a CTID needs to be provided.
		/// If the Target is not in the registry, then both the ProgressionModel (framework) and ProgressLevel (targetNode) will be required.
		/// range: ceterms:CredentialAlignmentObject
		/// </summary>
		public List<CredentialAlignmentObject> AtLevel { get; set; }

		/// <summary>
		/// Type of level indicating a point in a progression through an educational or training context, for which the credential is intended; select from an existing enumeration of such types.
		/// audLevel:AdvancedLevel audLevel:AssociatesDegreeLevel audLevel:BachelorsDegreeLevel audLevel:BeginnerLevel audLevel:DoctoralDegreeLevel audLevel:GraduateLevel audLevel:IntermediateLevel audLevel:LowerDivisionLevel audLevel:MastersDegreeLevel audLevel:PostSecondaryLevel audLevel:ProfessionalLevel audLevel:SecondaryLevel audLevel:UndergraduateLevel audLevel:UpperDivisionLevel
		/// <see href="https://credreg.net/ctdl/terms/AudienceLevel"></see>
		/// </summary>
		public List<string> AudienceLevelType { get; set; }

		/// <summary>
		/// Item that covers all of the relevant concepts in the item being described as well as additional relevant concepts.
		/// Range: valid credential class
		/// </summary>
		public List<string> BroadAlignment { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
		/// URI to a concept(based on the ONet work activities example) or to a blank node in RA.Models.Input.BaseRequest.ReferenceObjects
		/// ceterms:classification
		/// </summary>
		public List<string> Classification { get; set; } = new List<string>();

		/// <summary>
		/// List of CTIDs or full URLs for a ConditionManifest published by the owning organization
		/// Range: ConditionManifest
		/// </summary>
		public List<string> CommonConditions { get; set; }

		/// <summary>
		/// Person or organization holding the rights in copyright to entities such as credentials, learning opportunities, assessments, competencies or concept schemes.
		/// </summary>
		public OrganizationReference CopyrightHolder { get; set; }

		/// <summary>
		/// Credential Identifier
		/// Globally unique identifier by which the creator, owner or provider of a credential recognizes that credential in transactions with the external environment (e.g., in verifiable claims involving the credential).
		/// NOTE: Acronyns should not be used here. Use AlternateName for providing acronyms.
		/// </summary>
		public string CredentialId { get; set; }

		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Estimated time it will take to complete an activity, event, or resource.
		/// List of duration profiles expressing one or more duration profiles to attain this resource.
		/// </summary>
		public List<DurationProfile> EstimatedDuration { get; set; }

		/// <summary>
		/// Relevant concepts in two entities being compared are coextensive.
		/// Range: valid credential class
		/// </summary>
		public List<string> ExactAlignment { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// Previously earned, completed, or attained resources may still be valid even if they are no longer offered.
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Entity that describes financial assistance that is offered or available.
		/// </summary>
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; }

		/// <summary>
		/// Indicates a resource that acts as a stand-in for the resource.
		/// True range: CredentialType
		/// </summary>
		public List<string> HasProxy { get; set; }

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see href="https://credreg.net/ctdl/terms/identifier">Identifier</see>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// Image URL
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		public string InCatalog { get; set; }

		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages; e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// Required
		/// List of language codes. ex: en, es
		/// </summary>
		public List<string> InLanguage { get; set; }

		#region Industry type

		/// <summary>
		/// IndustryType
		/// Type of industry; select from an existing enumeration of such types such as the SIC, NAICS, and ISIC classifications.
		/// Best practice in identifying industries for U.S. credentials is to provide the NAICS code using the ceterms:naics property.
		/// Other credentials may use the ceterms:industrytype property and any framework of the class ceterms:IndustryClassification.
		/// </summary>
		public List<FrameworkItem> IndustryType { get; set; }

		/// <summary>
		/// AlternativeIndustryType
		/// Industries that are not found in a formal framework can be still added using AlternativeIndustryType.
		/// Any industries added using this property will be added to or appended to the IndustryType output.
		/// </summary>
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();

		/// <summary>
		/// Language map list for AlternativeIndustryType
		/// </summary>
		public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> Naics { get; set; }

		#endregion

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile">JurisdictionProfile</see>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; }

		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList KeywordLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// Webpage or online document that describes the criteria, standards, and/or requirements used with a process.
		/// </summary>
		public string ProcessStandards { get; set; }
		/// <summary>
		/// Textual description of the criteria, standards, and/or requirements used with a process
		/// </summary>
		public string ProcessStandardsDescription { get; set; }
		/// <summary>
		/// Language map - Textual description of the criteria, standards, and/or requirements used with a process
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescriptionLangMap { get; set; }

		/// <summary>
		/// Major overlap of relevant concepts between the two resources being compared.
		/// Range: valid credential class type
		/// </summary>
		public List<string> MajorAlignment { get; set; }

		/// <summary>
		/// Item that covers all of the relevant concepts in the item being described as well as additional relevant concepts.
		/// Range: valid credential class
		/// </summary>
		public List<string> MinorAlignment { get; set; }

		/// <summary>
		/// Credential covers all of the relevant concepts in another credential as well as relevant concepts not found in the other credential.
		/// Range: valid credential class
		/// </summary>
		public List<string> NarrowAlignment { get; set; }

		/// <summary>
		/// List of Organizations that offer this resource in a specific Jurisdiction.
		/// </summary>
		public List<JurisdictionProfile> OfferedIn { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		/// List of Organizations that recognize this resource
		/// </summary>
		public List<OrganizationReference> RecognizedBy { get; set; }

		/// <summary>
		/// Region or political jurisdiction such as a state, province or
		/// locale in which the credential, learning resource,
		/// or assessment has been publicly recommended, acknowledged or endorsed.
		/// </summary>
		public List<JurisdictionProfile> RecognizedIn { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		/// Organizations that have permission to offer credentials of a
		/// specific CredentialType.
		/// The value of this property should be a ceterms:Collection of ceterms:CredentialOrganizations (or similar).
		/// </summary>
		public List<string> RecognizedOfferers { get; set; }

		/// <summary>
		/// List of Organizations that recognize this credential in a specific Jurisdiction.
		/// </summary>
		/// <summary>
		/// Action related to the credential
		/// list of CTIDs
		/// </summary>
		public List<string> RelatedAction { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// HINT: If the SameAs target is a resource in the Credential Registry, just the CTID needs to be provided.
		/// ceterms:sameAs
		/// </summary>
		public List<string> SameAs { get; set; } = new List<string>();

		/// <summary>
		///  Indicates a broader type or class than the one being described.
		///  Used in CTDL to indicate the Credential Class (i.e. subclass of ceterms:Credential) that encompasses this type of credential.
		///  Range: Any Credential Subclass (not an object, the short class URI)
		///  example: ceterms:Certificate
		/// </summary>
		public string SubclassOf { get; set; }

		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// </summary>
		public List<string> Subject { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Language map list for Subject
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subject" )]
		public LanguageMapList SubjectLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// Uses Verification Service
		/// Reference to a service that is used to verify this resource.
		/// Range: ceterms:VerificationServiceProfile
		/// List of CTIDs that reference a VerificationServiceProfile in the registry
		/// </summary>
		public List<string> UsesVerificationService { get; set; } = new List<string>();

		#region -- Condition Profiles --

		/// <summary>
		///  Resource that must be completed prior to, or pursued at the same time as, this resource.
		/// </summary>
		public List<ConditionProfile> CoPrerequisite { get; set; } = new List<ConditionProfile>();

		/// <summary>
		///  Resources that must be pursued concurrently.
		/// </summary>
		public List<ConditionProfile> Corequisite { get; set; }

		/// <summary>
		/// this resource type preparation for the credential, assessment, or learning opportunity being referenced.
		/// ceterms:isPreparationFor
		/// <seealso href="https://credreg.net/ctdl/terms/isPreparationFor">isPreparationFor</seealso>
		/// </summary>
		public List<ConditionProfile> IsPreparationFor { get; set; }

		/// <summary>
		/// This credential type must be earned or completed prior to attempting to earn or complete the referenced credential, assessment, or learning opportunity.
		/// ceterms:isRequiredFor
		/// <seealso href="https://credreg.net/ctdl/terms/isRequiredFor"></seealso>
		/// </summary>
		public List<ConditionProfile> IsRequiredFor { get; set; }

		public List<ConditionProfile> Requires { get; set; }


		#endregion

		#region -- Process Profiles --

		/// <summary>
		/// Description of a process by which a resource is administered.
		/// ceterms:administrationProcess
		/// </summary>
		public List<ProcessProfile> AdministrationProcess { get; set; }

		/// <summary>
		///  Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion

		#region Version related properties

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
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();

		#endregion


	}
}
