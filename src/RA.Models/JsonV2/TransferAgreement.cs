// <copyright file="TransferAgreement.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{

	/// <summary>
	/// Transfer Agreement
	/// Formal agreement between two or more parties that specifies how prior learning is recognized, including its transfer and applicability for credit.
	/// 
	/// Inverse properities, not published in this resource:
	/// - ceterms:objectOfAction
	/// - ceterms:isMemberOf
	/// </summary>
	public class TransferAgreement : BaseResourceDocument
	{
		[JsonIgnore]
		public static string classType = "ceterms:TransferAgreement";

		public TransferAgreement()
		{

			HasPart = null;
			InLanguage = new List<string>();
			IsPartOf = null;
			Jurisdiction = null;
			OwnedBy = null;

			RecognizedBy = null;
			RecognizedIn = null;
			SubjectWebpage = null;
		}

		#region Required properties
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

		/// <summary>
		/// Date after which this resource is available or applicable.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		[JsonProperty( PropertyName = "ceterms:inLanguage" )]
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// OwnedBy
		/// Organization or person with an enforceable claim or legal title to the resource. 
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }

		/// <summary>
		/// The publication status of the resource
		/// </summary>
		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Organization to which students transfer under the provisions of an agreement.
		/// Valid types: ceterms:CredentialOrganization, ceterms:Organization
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:receivingOrganization" )]
		public List<string> ReceivingOrganization { get; set; }

		/// <summary>
		/// Organization from which students transfer under the provisions of an agreement.
		/// Valid types: ceterms:CredentialOrganization, ceterms:Organization
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sendingOrganization" )]
		public List<string> SendingOrganization { get; set; }

		#endregion

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }


		/// <summary>
		/// Type or nature of the document that is being described.
		/// Best practice is to draw from an existing concept scheme such as ceterms:DocumentCategory, 
		/// which may be supplemented by more precise terms from a locally defined concept scheme if required.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:agreementType" )]
		public List<CredentialAlignmentObject> AgreementType { get; set; }


		[JsonProperty( PropertyName = "ceterms:approvedBy" )]
		public List<string> ApprovedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:approvedIn" )]
		public List<JurisdictionProfile> ApprovedIn { get; set; }

		/// <summary>
		/// The type of audience for which the resource is applicable, intended, or useful; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:audienceType" )]
		public List<CredentialAlignmentObject> AudienceType { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:codedNotation" )]
		public string CodedNotation { get; set; }

		/// <summary>
		/// Person or organization holding the rights in copyright to this resource. 
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:copyrightHolder" )]
		public List<string> CopyrightHolder { get; set; } = null;

		/// <summary>
		/// An entity primarily responsible for making this resource.
		/// </summary>
		[JsonProperty( "ceasn:creator" )]
		public List<string> Creator { get; set; }

		/// <summary>
		/// Date of a statement of copyright for this resource, such as ©2017.
		/// </summary>
		[JsonProperty( "ceasn:dateCopyrighted" )]
		public string DateCopyrighted { get; set; }

		/// <summary>
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		[JsonProperty( "ceasn:dateCreated" )]
		public string DateCreated { get; set; }

		/// <summary>
		/// Originally only allowing date (yyyy-mm-dd), no time.
		/// However, this is defined as: xsd:dateTime. So consumers like the credential registry search, expect a datetime format.
		/// </summary>
		[JsonProperty( "ceasn:dateModified" )]
		public string DateModified { get; set; }

		/// <summary>
		/// Date on which the document being described was signed.
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		[JsonProperty( "ceterms:dateSigned" )]
		public string DateSigned { get; set; }

		/// <summary>
		/// The entity being described has been modified, extended or refined from the referenced resource.
		/// </summary>
		[JsonProperty( "ceasn:derivedFrom" )]
		public List<string> DerivedFrom { get; set; }

		[JsonProperty( PropertyName = "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Terms and definitions applicable to the resource.
		/// URI to the concept scheme
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasGlossary" )]
		public List<string> HasGlossary { get; set; }

		[JsonProperty( PropertyName = "ceterms:hasPart" )]
		public List<string> HasPart { get; set; }

		/// <summary>
		/// Indicates contextualized data that reproduces or links to text such as part of a document or information about some aspect of a resource.
		/// Do not use this property if a simple text property such as ceterms:description is adequate
		/// Range: ceterms:StructuredStatement
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasStatement" )]
		public List<string> HasStatement { get; set; }

		/// <summary>
		/// Reference to a relevant support service available for this resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasSupportService" )]
		public List<string> HasSupportService { get; set; }

		/// <summary>
		/// Description of transfer value that is part of this resource.
		/// Range: ceterms:TransferValueProfile
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasTransferValueProfile" )]
		public List<string> HasTransferValueProfile { get; set; }

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

		[JsonProperty( PropertyName = "ceterms:isPartOf" )]
		public List<string> IsPartOf { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		/// <summary>
		/// Type of prior learning evidence accepted for evaluation under the policy; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:learningRecognitionEvidenceType" )]
		public List<CredentialAlignmentObject> LearningRecognitionEvidenceType { get; set; }

		/// <summary>
		/// Type of method used to evaluate and recognize prior learning; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:learningRecognitionMethodType" )]
		public List<CredentialAlignmentObject> LearningRecognitionMethodType { get; set; }

		/// <summary>
		/// Type of context from which prior learning may be recognized; select from an existing enumeration of such types
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:learningRecognitionSourceType" )]
		public List<CredentialAlignmentObject> LearningRecognitionSourceType { get; set; }

		/// <summary>
		/// Type of outcome governed or permitted by the policy; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:learningRecognitionOutcomeType" )]
		public List<CredentialAlignmentObject> LearningRecognitionOutcomeType { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// </summary>
		[JsonProperty( "ceasn:license" )]
		public string License { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandards" )]
		public string ProcessStandards { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription { get; set; }

		/// <summary>
		/// ReceivingDepartment
		/// Department (or other subdivision of an organization) to which students transfer under the provisions of an agreement.
		/// Valid types: ceterms:CredentialOrganization, ceterms:Organization
		/// </summary>
		[ JsonProperty( PropertyName = "ceterms:receivingDepartment" )]
		public List<string> ReceivingDepartment { get; set; }

		/// <summary>
		/// Program to which students transfer under the provisions of an agreement.
		/// Valid types: ceterms:LearningProgram
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:receivingProgram" )]
		public List<string> ReceivingProgram { get; set; }

		/// <summary>
		/// Policy that is important in some way to this resource.
		/// Valid types: ceterms:PriorLearningPolicy
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:relevantPolicy" )]
		public List<string> relevantPolicy { get; set; }

		/// <summary>
		/// Department (or other subdivision of an organization) from which students transfer under the provisions of an agreement.
		/// Valid types: ceterms:CredentialOrganization, ceterms:Organization
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sendingDepartment" )]
		public List<string> SendingDepartment { get; set; }

		/// <summary>
		/// Program from which students transfer under the provisions of an agreement.
		/// Valid types: ceterms:LearningProgram
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sendingProgram" )]
		public List<string> SendingProgram { get; set; }


		/// <summary>
		/// Action related to the credential
		/// This may end up being a list of URIs?
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:relatedAction" )]
		public List<string> RelatedAction { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; }

		/// <summary>
		/// An agent owning or managing rights over this resource.
		/// </summary>
		[JsonProperty( "ceasn:rightsHolder" )]
		public List<string> RightsHolder { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// List of URIs
		/// ceterms:sameAs
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sameAs" )]
		public List<string> SameAs { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// A list of sub-units of this resource. 
		/// </summary>
		[JsonProperty( "ceasn:tableOfContents" )]
		public LanguageMap TableOfContents { get; set; }


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


		#region process profiles


		[JsonProperty( PropertyName = "ceterms:complaintProcess" )]
		public List<ProcessProfile> ComplaintProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion


		#region BY/Ins

		/// <summary>
		/// Agent(s) that acknowledges the validity of the resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:recognizedBy" )]
		public List<string> RecognizedBy { get; set; }

		/// <summary>
		/// Region or political jurisdiction such as a state, province or locale in which the resource has been publicly recommended, acknowledged or endorsed.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:recognizedIn" )]
		public List<JurisdictionProfile> RecognizedIn { get; set; }
		#endregion



	}

}
