// <copyright file="PriorLearningPolicy.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Prior Learning Policy
	/// Authoritative statement of rules, principles, or procedures adopted by an organization to govern decisions and actions within the scope of prior learning.
	/// Required properties:
	/// - CTID
	/// - Name
	/// - Description
	/// - InLanguage
	/// - OwnedBy
	/// - PublicationStatusType
	/// - SubjectWebpage
	/// 
	/// Inverse properities, not published in this resource:
	/// - ceterms:objectOfAction
	/// - ceterms:isMemberOf
	/// - ceterms:IsPartOf
	/// </summary>
	public class PriorLearningPolicy
	{

		public PriorLearningPolicy()
		{
			Type = "ceterms:PriorLearningPolicy";
			HasPart = null;
			InLanguage = new List<string>();
			Jurisdiction = null;
			OwnedBy = new List<OrganizationReference>();

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

		public string CTID { get; set; }

		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; }

		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Organization or person with an enforceable claim or legal title to the resource. 
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		/// <summary>
		/// The publication status of the resource
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		public string SubjectWebpage { get; set; }

		#endregion

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		public List<string> AlternateName { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateNameLangMap { get; set; }


		/// <summary>
		/// The type of audience for which the resource is applicable, intended, or useful; select from an existing enumeration of such types.
		/// </summary>
		public List<string> AudienceType { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		public string CodedNotation { get; set; }

        /// <summary>
        /// Person or organization holding the rights in copyright to this resource. 
        /// Range: Organization
        /// </summary>
        public OrganizationReference CopyrightHolder { get; set; }

        /// <summary>
        /// Date of a statement of copyright for this resource, such as ©2017.
        /// </summary>
        public string DateCopyrighted { get; set; }

		/// <summary>
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		public string DateCreated { get; set; }

		/// <summary>
		/// Date after which this resource is available or applicable.
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Originally only allowing date (yyyy-mm-dd), no time.
		/// However, this is defined as: xsd:dateTime. So consumers like the credential registry search, expect a datetime format.
		/// </summary>
		public string DateModified { get; set; }

		/// <summary>
		/// Date on which the document being described was signed.
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		public string DateSigned { get; set; }

        /// <summary>
        /// The entity being described has been modified, extended or refined from the referenced resource.
        /// Range: PriorLearningPolicy
        /// </summary>
        public List<string> DerivedFrom { get; set; }

		public string ExpirationDate { get; set; }

		/// <summary>
		/// Terms and definitions applicable to the resource.
		/// URI to the concept scheme
		/// </summary>
		public List<string> HasGlossary { get; set; }

        /// <summary>
        /// Range: PriorLearningPolicy
        /// </summary>
        public List<string> HasPart { get; set; }

		/// <summary>
		/// Indicates contextualized data that reproduces or links to text such as part of a document or information about some aspect of a resource.
		/// Do not use this property if a simple text property such as ceterms:description is adequate
		/// Range: ceterms:StructuredStatement
		/// </summary>
		public List<StructuredStatement> HasStructuredStatement { get; set; }

		/// <summary>
		/// Reference to a relevant support service available for this resource.
		/// </summary>
		public List<string> HasSupportService { get; set; }

		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; }

		// Image URL
		public string Image { get; set; }


		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; }

		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList KeywordLangMap { get; set; } = null;

		/// <summary>
		/// Type of prior learning evidence accepted for evaluation under the policy; select from an existing enumeration of such types.
		/// ConceptScheme: ceterms:LearningRecognitionEvidenceCategory
		/// </summary>
		public List<string> LearningRecognitionEvidenceType { get; set; }

		/// <summary>
		/// Type of method used to evaluate and recognize prior learning; select from an existing enumeration of such types.
		/// ConceptScheme: ceterms:LearningRecognitionMethodCategory 
		/// </summary>
		public List<string> LearningRecognitionMethodType { get; set; }

		/// <summary>
		/// Type of context from which prior learning may be recognized; select from an existing enumeration of such types
		/// ConceptScheme: ceterms:PriorLearningRecognitionSourceCategory
		/// </summary>
		public List<string> LearningRecognitionSourceType { get; set; }

		/// <summary>
		/// Type of outcome governed or permitted by the policy; select from an existing enumeration of such types.
		/// ConceptScheme: ceterms:LearningRecognitionOutcomeCategory
		/// </summary>
		public List<string> LearningRecognitionOutcomeType { get; set; }

        /// <summary>
        /// A legal document giving official permission to do something with this resource.
        /// ceasn:license
        /// Range Includes:	xsd:anyURI
        /// </summary>
		public string License { get; set; }

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
		/// Action related to the resource
		/// Range: A valid CredentialingAction resource
		/// </summary>
		public List<string> RelatedAction { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		public string Rights { get; set; }

		[JsonProperty( "ceasn:rights" )]
		public LanguageMap RightsLangMap { get; set; }

		/// <summary>
		/// An agent owning or managing rights over this resource.
		/// </summary>
		public List<OrganizationReference> RightsHolder { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// List of URIs
		/// ceterms:sameAs
		/// </summary>
		public List<string> SameAs { get; set; }

		/// <summary>
		/// A list of sub-units of this resource. 
		/// </summary>
		public string TableOfContents { get; set; }

		[JsonProperty( "ceasn:tableOfContents" )]
		public LanguageMap TableOfContentsLangMap { get; set; }

		#region Versions
		public string LatestVersion { get; set; }

		public string PreviousVersion { get; set; }

		public string NextVersion { get; set; }

		public string SupersededBy { get; set; }

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
		public List<IdentifierValue> VersionIdentifier { get; set; }
		#endregion

		#region process profiles

		public List<ProcessProfile> ComplaintProcess { get; set; }

		public List<ProcessProfile> DevelopmentProcess { get; set; }

		public List<ProcessProfile> MaintenanceProcess { get; set; }

		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion

		#region BY/Ins

		/// <summary>
		/// Agent(s) that acknowledges the validity of the resource.
		/// </summary>
		public List<OrganizationReference> RecognizedBy { get; set; }

		/// <summary>
		/// Region or political jurisdiction such as a state, province or locale in which the resource has been publicly recommended, acknowledged or endorsed.
		/// </summary>
		public List<JurisdictionProfile> RecognizedIn { get; set; }
		#endregion

	}
}
