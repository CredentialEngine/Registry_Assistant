using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{

	public class CredentialType : BaseResourceDocument
	{

		public CredentialType()
		{
			ApprovedBy = null;
			ApprovedIn = null;

			CommonConditions = new List<string>();

			InLanguage = new List<string>();
			OfferedIn = null;
			OwnedBy = null;
			RecognizedBy = null;
			RecognizedIn = null;
			RegulatedBy = null;
			RegulatedIn = null;
			SubjectWebpage = null;
		}

		#region Required
		[JsonProperty( "@type" )]
		public string Type { get; set; } = "ceterms:CredentialType";

		[JsonProperty( "@id" )]
		public string Id { get; set; }

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:credentialStatusType" )]
		public CredentialAlignmentObject CredentialStatusType { get; set; }

		[JsonProperty( PropertyName = "ceterms:inLanguage" )]
		public List<string> InLanguage { get; set; }
		/// <summary>
		/// OwnedBy
		/// Will either by an Id array, or a thin organization array
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }
		#endregion
		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		[JsonProperty( PropertyName = "ceterms:approvedBy" )]
		public List<string> ApprovedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:approvedIn" )]
		public List<JurisdictionProfile> ApprovedIn { get; set; }
		/// <summary>
		/// Indicates the stage or level of achievement in a progression of learning.
		/// range: ceterms:CredentialAlignmentObject
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:atLevel" )]
		public List<CredentialAlignmentObject> AtLevel { get; set; }

		[JsonProperty( PropertyName = "ceterms:audienceLevelType" )]
		public List<CredentialAlignmentObject> AudienceLevelType { get; set; }

		/// <summary>
		/// Item that covers all of the relevant concepts in the item being described as well as additional relevant concepts.
		/// Range: valid credential class
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:broadAlignment" )]
		public List<string> BroadAlignment { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		[JsonProperty( PropertyName = "ceterms:commonConditions" )]
		public List<string> CommonConditions { get; set; }

		[JsonProperty( PropertyName = "ceterms:coPrerequisite" )]
		public List<ConditionProfile> CoPrerequisite { get; set; }

		[JsonProperty( PropertyName = "ceterms:copyrightHolder" )]
		public List<string> CopyrightHolder { get; set; } = null;

		[JsonProperty( PropertyName = "ceterms:corequisite" )]
		public List<ConditionProfile> Corequisite { get; set; }

		[JsonProperty( PropertyName = "ceterms:credentialId" )]
		public string CredentialId { get; set; }

		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		[JsonProperty( PropertyName = "ceterms:estimatedDuration" )]
		public List<DurationProfile> EstimatedDuration { get; set; }
		/// <summary>
		/// Relevant concepts in two entities being compared are coextensive.
		/// Range: valid credential class
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:exactAlignment" )]
		public List<string> ExactAlignment { get; set; }

		[JsonProperty( PropertyName = "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:financialAssistance" )]
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; }

		[JsonProperty( PropertyName = "ceterms:hasProxy" )]
		public List<string> HasProxy { get; set; }
		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		// Image URL
		[JsonProperty( PropertyName = "ceterms:image" )]
		public string Image { get; set; }
		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:inCatalog" )]
		public string InCatalog { get; set; }

		#region Industry

		[JsonProperty( PropertyName = "ceterms:industryType" )]
		public List<CredentialAlignmentObject> IndustryType { get; set; }

		[JsonProperty( PropertyName = "ceterms:naics" )]
		public List<string> Naics { get; set; } = new List<string>();

		#endregion


		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandards" )]
		public string ProcessStandards { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription { get; set; }

		/// <summary>
		/// Major overlap of relevant concepts between the two resources being compared.
		/// Range: valid credential class type
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:majorAlignment" )]
		public List<string> MajorAlignment { get; set; }

		/// <summary>
		/// Item that covers all of the relevant concepts in the item being described as well as additional relevant concepts.
		/// Range: valid credential class
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:minorAlignment" )]
		public List<string> MinorAlignment { get; set; }
		/// <summary>
		/// Credential covers all of the relevant concepts in another credential as well as relevant concepts not found in the other credential.
		/// Range: valid credential class
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:narrowAlignment" )]
		public List<string> NarrowAlignment { get; set; }

		[JsonProperty( PropertyName = "ceterms:offeredIn" )]
		public List<JurisdictionProfile> OfferedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:recognizedBy" )]
		public List<string> RecognizedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:recognizedIn" )]
		public List<JurisdictionProfile> RecognizedIn { get; set; }

		/// <summary>
		/// Organizations that have permission to offer credentials of a
		/// specific CredentialType.
		/// The value of this property should be a ceterms:Collection of ceterms:CredentialOrganizations (or similar).
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:recognizedOfferers" )]
		public List<string> RecognizedOfferers { get; set; }

		[JsonProperty( PropertyName = "ceterms:regulatedBy" )]
		public List<string> RegulatedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:regulatedIn" )]
		public List<JurisdictionProfile> RegulatedIn { get; set; }

		/// <summary>
		/// Action related to the credential
		/// This may end up being a list of URIs?
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:relatedAction" )]
		public List<string> RelatedAction { get; set; }
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
		/// <summary>
		///  Indicates a broader type or class than the one being described.
		///  Used in CTDL to indicate the Credential Class (i.e. subclass of ceterms:Credential) that encompasses this type of credential.
		///  Range: Any Credential subclass
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subclassOf" )]
		public string SubclassOf { get; set; }

		/// <summary>
		/// Uses Verification Service
		/// Reference to a service that is used to verify this Credential.
		/// Range: ceterms:VerificationServiceProfile
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:usesVerificationService" )]
		public List<string> UsesVerificationService { get; set; }

		#region Condition Profiles

		[JsonProperty( PropertyName = "ceterms:isPreparationFor" )]
		public List<ConditionProfile> IsPreparationFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:isRequiredFor" )]
		public List<ConditionProfile> IsRequiredFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:requires" )]
		public List<ConditionProfile> Requires { get; set; }

		#endregion

		#region process profiles
		[JsonProperty( PropertyName = "ceterms:administrationProcess" )]
		public List<ProcessProfile> AdministrationProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion

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

	}

}
