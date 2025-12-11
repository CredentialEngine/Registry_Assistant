// <copyright file="PriorLearningPolicy.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Prior Learning Policy
	/// Authoritative statement of rules, principles, or procedures adopted by an organization to govern decisions and actions within the scope of prior learning.
	/// 
	/// Inverse properities, not published in this resource:
	/// - ceterms:objectOfAction
	/// - ceterms:isMemberOf
	/// </summary>
	public class PriorLearningPolicy : BaseResourceDocument
	{
		[JsonIgnore]
		public static string classType = "ceterms:PriorLearningPolicy";

		public PriorLearningPolicy()
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

		[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( "ceterms:inLanguage" )]
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Organization or person with an enforceable claim or legal title to the resource. 
		/// </summary>
		[JsonProperty( "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }

		/// <summary>
		/// The publication status of the resource
		/// </summary>
		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		[JsonProperty( "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		#endregion

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		[JsonProperty( "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		/// <summary>
		/// The type of audience for which the resource is applicable, intended, or useful; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( "ceterms:audienceType" )]
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
		[JsonProperty( "ceterms:codedNotation" )]
		public string CodedNotation { get; set; }

		/// <summary>
		/// Person or organization holding the rights in copyright to this resource. 
		/// </summary>
		[JsonProperty( "ceterms:copyrightHolder" )]
		public List<string> CopyrightHolder { get; set; } = null;

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
		/// Date after which this resource is available or applicable.
		/// </summary>
		[JsonProperty( "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

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

		[JsonProperty( "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Terms and definitions applicable to the resource.
		/// URI to the concept scheme
		/// </summary>
		[JsonProperty( "ceterms:hasGlossary" )]
		public List<string> HasGlossary { get; set; }

		[JsonProperty( "ceterms:hasPart" )]
		public List<string> HasPart { get; set; }

		/// <summary>
		/// Indicates contextualized data that reproduces or links to text such as part of a document or information about some aspect of a resource.
		/// Do not use this property if a simple text property such as ceterms:description is adequate
		/// Range: ceterms:StructuredStatement
		/// </summary>
		[JsonProperty( "ceterms:hasStatement" )]
		public List<string> HasStatement { get; set; }

		/// <summary>
		/// Reference to a relevant support service available for this resource.
		/// </summary>
		[JsonProperty( "ceterms:hasSupportService" )]
		public List<string> HasSupportService { get; set; }

		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue
		/// </summary>
		[JsonProperty( "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		// Image URL
		[JsonProperty( "ceterms:image" )]
		public string Image { get; set; }

		[JsonProperty( "ceterms:isPartOf" )]
		public List<string> IsPartOf { get; set; }

		[JsonProperty( "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		/// <summary>
		/// Type of prior learning evidence accepted for evaluation under the policy; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( "ceterms:learningRecognitionEvidenceType" )]
		public List<CredentialAlignmentObject> LearningRecognitionEvidenceType { get; set; }

		/// <summary>
		/// Type of method used to evaluate and recognize prior learning; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( "ceterms:learningRecognitionMethodType" )]
		public List<CredentialAlignmentObject> LearningRecognitionMethodType { get; set; }

		/// <summary>
		/// Type of context from which prior learning may be recognized; select from an existing enumeration of such types
		/// </summary>
		[JsonProperty( "ceterms:learningRecognitionSourceType" )]
		public List<CredentialAlignmentObject> LearningRecognitionSourceType { get; set; }

		/// <summary>
		/// Type of outcome governed or permitted by the policy; select from an existing enumeration of such types.
		/// </summary>
		[JsonProperty( "ceterms:learningRecognitionOutcomeType" )]
		public List<CredentialAlignmentObject> LearningRecognitionOutcomeType { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// </summary>
		[JsonProperty( "ceasn:license" )]
		public string License { get; set; }

		[JsonProperty( "ceterms:processStandards" )]
		public string ProcessStandards { get; set; }

		[JsonProperty( "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription { get; set; }

		/// <summary>
		/// Action related to the credential
		/// This may end up being a list of URIs?
		/// </summary>
		[JsonProperty( "ceterms:relatedAction" )]
		public List<string> RelatedAction { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; }

		/// <summary>
		/// An agent owning or managing rights over this resource.
		/// xsd:uri
		/// </summary>
		[JsonProperty( "ceasn:rightsHolder" )]
		public List<string> RightsHolder { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// List of URIs
		/// ceterms:sameAs
		/// </summary>
		[JsonProperty( "ceterms:sameAs" )]
		public List<string> SameAs { get; set; }

		/// <summary>
		/// A list of sub-units of this resource. 
		/// </summary>
		[JsonProperty( "ceasn:tableOfContents" )]
		public LanguageMap TableOfContents { get; set; }


		#region Versions
		[JsonProperty( "ceterms:latestVersion" )]
		public string LatestVersion { get; set; }

		[JsonProperty( "ceterms:previousVersion" )]
		public string PreviousVersion { get; set; }

		[JsonProperty( "ceterms:nextVersion" )]
		public string NextVersion { get; set; }

		[JsonProperty( "ceterms:supersededBy" )]
		public string SupersededBy { get; set; }

		[JsonProperty( "ceterms:supersedes" )]
		public string Supersedes { get; set; }

		/// <summary>
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		[JsonProperty( "ceterms:versionCode" )]
		public string VersionCode { get; set; }

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		[JsonProperty( "ceterms:versionIdentifier" )]
		public List<IdentifierValue> VersionIdentifier { get; set; }
		#endregion


		#region process profiles


		[JsonProperty( "ceterms:complaintProcess" )]
		public List<ProcessProfile> ComplaintProcess { get; set; }

		[JsonProperty( "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion


		#region BY/Ins


		[JsonProperty( "ceterms:recognizedBy" )]
		public List<string> RecognizedBy { get; set; }

		[JsonProperty( "ceterms:recognizedIn" )]
		public List<JurisdictionProfile> RecognizedIn { get; set; }
		#endregion



	}
}
