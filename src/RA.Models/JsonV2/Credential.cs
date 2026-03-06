using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{

	public class Credential : BaseResourceDocument
	{
		[JsonIgnore]
		public static string classType = "ceterms:Credential";

		public Credential()
		{
			AccreditedBy = null;
			AccreditedIn = null;
			AdvancedStandingFrom = new List<ConditionProfile>();
			AppealProcess = new List<ProcessProfile>();
			ApprovedBy = null;
			ApprovedIn = null;
			AudienceLevelType = new List<CredentialAlignmentObject>();
			AudienceType = new List<CredentialAlignmentObject>();
			AvailabilityListing = new List<string>();
			AvailableAt = new List<Place>();
			AvailableOnlineAt = new List<string>();

			CommonConditions = new List<string>();
			CommonCosts = new List<string>();
			ComplaintProcess = new List<ProcessProfile>();
			DegreeConcentration = new List<CredentialAlignmentObject>();
			DegreeMajor = new List<CredentialAlignmentObject>();
			DegreeMinor = new List<CredentialAlignmentObject>();
			DevelopmentProcess = new List<ProcessProfile>();

			EstimatedCost = new List<CostProfile>();
			HasPart = null;
			InLanguage = new List<string>();
			IsAdvancedStandingFor = new List<ConditionProfile>();
			IsPartOf = null;
			IsPreparationFor = new List<ConditionProfile>();
			IsRecommendedFor = new List<ConditionProfile>();
			IsRequiredFor = new List<ConditionProfile>();
			Jurisdiction = new List<JurisdictionProfile>();
			MaintenanceProcess = new List<ProcessProfile>();
			OfferedBy = null;
			OfferedIn = null;
			OwnedBy = null;
			PreparationFrom = new List<ConditionProfile>();

			RecognizedBy = null;
			RecognizedIn = null;
			Recommends = new List<ConditionProfile>();
			RegulatedBy = null;
			RegulatedIn = null;
			Renewal = new List<ConditionProfile>();
			RenewedBy = null;
			RenewedIn = null;
			Requires = new List<ConditionProfile>();
			ReviewProcess = new List<ProcessProfile>();
			Revocation = new List<RevocationProfile>();
			RevocationProcess = new List<ProcessProfile>();
			RevokedBy = null;
			RevokedIn = null;

			Subject = new List<CredentialAlignmentObject>();
			SubjectWebpage = null;
			VersionIdentifier = new List<IdentifierValue>();
		}

		/// <summary>
		/// Need a custom mapping to @type based on input value
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "@id" )]
		public string Id { get; set; }

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:aggregateData" )]
		public List<AggregateDataProfile> AggregateData { get; set; }

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		[JsonProperty( PropertyName = "ceterms:assessmentDeliveryType" )]
		public List<CredentialAlignmentObject> AssessmentDeliveryType { get; set; }

		/// <summary>
		/// Indicates the stage or level of achievement in a progression of learning.
		/// range: ceterms:CredentialAlignmentObject
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:atLevel" )]
		public List<CredentialAlignmentObject> AtLevel { get; set; }

		[JsonProperty( PropertyName = "ceterms:audienceLevelType" )]
		public List<CredentialAlignmentObject> AudienceLevelType { get; set; }

		[JsonProperty( PropertyName = "ceterms:audienceType" )]
		public List<CredentialAlignmentObject> AudienceType { get; set; }

		[JsonProperty( PropertyName = "ceterms:availableAt" )]
		public List<Place> AvailableAt { get; set; }

		[JsonProperty( PropertyName = "ceterms:availabilityListing" )]
		public List<string> AvailabilityListing { get; set; }

		[JsonProperty( PropertyName = "ceterms:availableOnlineAt" )]
		public List<string> AvailableOnlineAt { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		[JsonProperty( PropertyName = "ceterms:copyrightHolder" )]
		public List<string> CopyrightHolder { get; set; } = null;

		[JsonProperty( PropertyName = "ceterms:credentialStatusType" )]
		public CredentialAlignmentObject CredentialStatusType { get; set; }

		[JsonProperty( PropertyName = "ceterms:credentialId" )]
		public string CredentialId { get; set; }

		/// <summary>
		/// CredentialType: Type of credential as defined by an authoritative body 
		///		for use within an officially regulated qualification system.
		/// The actual CTDL term is ceterms:credentialType, but the latter property is already in use, 
		///		so using GovernmentCredentialType as the input class
		///  <see cref="https://purl.org/ctdl/terms/credentialType"/>
		/// Range: is an object of type: CredentialType
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:credentialType" )]
		public List<string> GovernmentCredentialType { get; set; }

		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		[JsonProperty( PropertyName = "ceterms:degreeConcentration" )]
		public List<CredentialAlignmentObject> DegreeConcentration { get; set; }

		[JsonProperty( PropertyName = "ceterms:degreeMajor" )]
		public List<CredentialAlignmentObject> DegreeMajor { get; set; }

		[JsonProperty( PropertyName = "ceterms:degreeMinor" )]
		public List<CredentialAlignmentObject> DegreeMinor { get; set; }

		[JsonProperty( PropertyName = "ceterms:estimatedDuration" )]
		public List<DurationProfile> EstimatedDuration { get; set; }

		[JsonProperty( PropertyName = "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:financialAssistance" )]
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; }

		[JsonProperty( PropertyName = "ceterms:hasPart" )]
		public List<string> HasPart { get; set; }

		/// <summary>
		/// Reference to a relevant support service available for this resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasSupportService" )]
		public List<string> HasSupportService { get; set; }

		/// <summary>
		/// Rubric related to this resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasRubric" )]
		public List<string> HasRubric { get; set; }

		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:inCatalog" )]
		public string InCatalog { get; set; }

		// Image URL
		[JsonProperty( PropertyName = "ceterms:image" )]
		public string Image { get; set; }

		[JsonProperty( PropertyName = "ceterms:inLanguage" )]
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// ISIC Revision 4 Code
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:isicV4" )]
		public string ISICV4 { get; set; }

		/// <summary>
		/// Is Non-Credit
		/// Will be null unless true
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:isNonCredit" )]
		public bool? IsNonCredit { get; set; }

		[JsonProperty( PropertyName = "ceterms:isPartOf" )]
		public List<string> IsPartOf { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		[JsonProperty( PropertyName = "ceterms:learningDeliveryType" )]
		public List<CredentialAlignmentObject> LearningDeliveryType { get; set; }

		[JsonProperty( PropertyName = "ceterms:learningMethodType" )]
		public List<CredentialAlignmentObject> LearningMethodType { get; set; }

		/// <summary>
		/// Learning Method Description
		///  Description of the learning methods for a resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:learningMethodDescription" )]
		public LanguageMap LearningMethodDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandards" )]
		public string ProcessStandards { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription { get; set; }

		/// <summary>
		/// This resource provides transfer value for the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:providesTransferValueFor" )]
		public List<string> ProvidesTransferValueFor { get; set; }

		/// <summary>
		/// This resource receives transfer value from the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:receivesTransferValueFrom" )]
		public List<string> ReceivesTransferValueFrom { get; set; }

		/// <summary>
		/// Action related to the credential
		/// This may end up being a list of URIs?
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:relatedAction" )]
		public List<string> RelatedAction { get; set; }

		[JsonProperty( PropertyName = "ceterms:renewalFrequency" )]
		public string RenewalFrequency { get; set; }

		[JsonProperty( PropertyName = "ceterms:revocation" )]
		public List<RevocationProfile> Revocation { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// List of URIs
		/// ceterms:sameAs
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sameAs" )]
		public List<string> SameAs { get; set; }

		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subject" )]
		public List<CredentialAlignmentObject> Subject { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		// <summary>
		// NOTE: the TargetPathway is an inverse property for a credential. That is, it will not be published with the credential, and is instead derived via a PathwayComponent
		// </summary>

		/// <summary>
		/// Uses Verification Service
		/// Reference to a service that is used to verify this Credential.
		/// Range: ceterms:VerificationServiceProfile
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:usesVerificationService" )]
		public List<string> UsesVerificationService { get; set; }

		#region Versions
		[JsonProperty( PropertyName = "ceterms:latestVersion" )]
		public string LatestVersion { get; set; }

		[JsonProperty( PropertyName = "ceterms:previousVersion" )]
		public string PreviousVersion { get; set; }

		[JsonProperty( PropertyName = "ceterms:nextVersion" )]
		public string NextVersion { get; set; }

		[JsonProperty( PropertyName = "ceterms:supersededBy" )]
		public string SupersededBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:supersedes" )]
		public string Supersedes { get; set; }

		/// <summary>
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:versionCode" )]
		public string VersionCode { get; set; }

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:versionIdentifier" )]
		public List<IdentifierValue> VersionIdentifier { get; set; }
		#endregion

		#region Occupation, Industry, Program
		[JsonProperty( PropertyName = "ceterms:occupationType" )]
		public List<CredentialAlignmentObject> OccupationType { get; set; }

		[JsonProperty( PropertyName = "ceterms:industryType" )]
		public List<CredentialAlignmentObject> IndustryType { get; set; }

		[JsonProperty( PropertyName = "ceterms:naics" )]
		public List<string> Naics { get; set; } = new List<string>();

		[JsonProperty( PropertyName = "ceterms:instructionalProgramType" )]
		public List<CredentialAlignmentObject> InstructionalProgramType { get; set; }
		#endregion

		#region costs
		[JsonProperty( PropertyName = "ceterms:estimatedCost" )]
		public List<CostProfile> EstimatedCost { get; set; }

		[JsonProperty( PropertyName = "ceterms:commonCosts" )]
		public List<string> CommonCosts { get; set; }

		#endregion

		#region Condition Profiles
		[JsonProperty( PropertyName = "ceterms:requires" )]
		public List<ConditionProfile> Requires { get; set; }

		[JsonProperty( PropertyName = "ceterms:corequisite" )]
		public List<ConditionProfile> Corequisite { get; set; }

		[JsonProperty( PropertyName = "ceterms:coPrerequisite" )]
		public List<ConditionProfile> CoPrerequisite { get; set; }

		[JsonProperty( PropertyName = "ceterms:recommends" )]
		public List<ConditionProfile> Recommends { get; set; }

		[JsonProperty( PropertyName = "ceterms:renewal" )]
		public List<ConditionProfile> Renewal { get; set; }

		[JsonProperty( PropertyName = "ceterms:commonConditions" )]
		public List<string> CommonConditions { get; set; }

		#endregion

		#region process profiles
		[JsonProperty( PropertyName = "ceterms:administrationProcess" )]
		public List<ProcessProfile> AdministrationProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:appealProcess" )]
		public List<ProcessProfile> AppealProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:complaintProcess" )]
		public List<ProcessProfile> ComplaintProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:revocationProcess" )]
		public List<ProcessProfile> RevocationProcess { get; set; }
		#endregion

		#region Connections
		[JsonProperty( PropertyName = "ceterms:advancedStandingFrom" )]
		public List<ConditionProfile> AdvancedStandingFrom { get; set; }

		[JsonProperty( PropertyName = "ceterms:isAdvancedStandingFor" )]
		public List<ConditionProfile> IsAdvancedStandingFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:isPreparationFor" )]
		public List<ConditionProfile> IsPreparationFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:isRecommendedFor" )]
		public List<ConditionProfile> IsRecommendedFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:isRequiredFor" )]
		public List<ConditionProfile> IsRequiredFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:preparationFrom" )]
		public List<ConditionProfile> PreparationFrom { get; set; }
		#endregion

		#region BYs

		/// <summary>
		/// OwnedBy
		/// Will either by an Id array, or a thin organization array
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:accreditedBy" )]
		public List<string> AccreditedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:approvedBy" )]
		public List<string> ApprovedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:offeredBy" )]
		public List<string> OfferedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:recognizedBy" )]
		public List<string> RecognizedBy { get; set; }

		/// <summary>
		/// Agent with whom an apprenticeship is registered.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:registeredBy" )]
		public List<string> RegisteredBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:regulatedBy" )]
		public List<string> RegulatedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:revokedBy" )]
		public List<string> RevokedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:renewedBy" )]
		public List<string> RenewedBy { get; set; }
		#endregion

		#region Ins

		[JsonProperty( PropertyName = "ceterms:accreditedIn" )]
		public List<JurisdictionProfile> AccreditedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:approvedIn" )]
		public List<JurisdictionProfile> ApprovedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:offeredIn" )]
		public List<JurisdictionProfile> OfferedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:recognizedIn" )]
		public List<JurisdictionProfile> RecognizedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:regulatedIn" )]
		public List<JurisdictionProfile> RegulatedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:renewedIn" )]
		public List<JurisdictionProfile> RenewedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:revokedIn" )]
		public List<JurisdictionProfile> RevokedIn { get; set; }

		#endregion

		/// <summary>
		/// Pathway in which this resource is a potential component.
		/// This is an inverse property and would not be published with this resource
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:targetPathway" )]
		public List<string> TargetPathway { get; set; }

		// *** Helper properties where publishing input is a graph. These will not be published

		/// <summary>
		/// CIP List is a helper when publishing from a graph. It will not be published
		/// </summary>
		[JsonProperty( "cipList" )]
		public List<string> CIPList { get; set; } = null;

		/// <summary>
		/// SOC List is a helper when publishing from a graph. It will not be published
		/// </summary>
		[JsonProperty( "socList" )]
		public List<string> SOCList { get; set; } = null;

		/// NAICS List is a helper when publishing from a graph. It will not be published
		[JsonProperty( "naicsList" )]
		public List<string> NaicsList { get; set; } = null;
	}

	public class RevocationProfile
	{
		public RevocationProfile()
		{
			Type = "ceterms:RevocationProfile";
			Jurisdiction = new List<JurisdictionProfile>();
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:revocationCriteria" )]
		public string RevocationCriteria { get; set; }

		[JsonProperty( PropertyName = "ceterms:revocationCriteriaDescription" )]
		public LanguageMap RevocationCriteriaDescription { get; set; }
	}
}
