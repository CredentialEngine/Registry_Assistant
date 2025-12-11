using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class QualificationsFramework
	{

		[JsonProperty( "@type" )]
		public string Type { get; set; } = "ceterms:QualificationsFramework";

		[JsonProperty( "@id" )]
		public string Id { get; set; }

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; } = new LanguageMap();

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; } = new LanguageMap();

		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }

		/// <summary>
		/// A person or organization chiefly responsible for the intellectual or artistic content of this resource.
		/// List of names (not URIs)
		/// </summary>
		[JsonProperty( "ceasn:author" )]
		public List<string> Author { get; set; }

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
		/// Date of creation of this resource.
		/// </summary>
		[JsonProperty( "ceasn:dateCreated" )]
		public string DateCreated { get; set; }

		/// <summary>
		/// The date on which this resource was most recently modified in some way.
		/// </summary>
		[JsonProperty( "ceasn:dateModified" )]
		public string DateModified { get; set; }

		/// <summary>
		/// Beginning date of validity of this resource.
		/// </summary>
		[JsonProperty( "ceasn:dateValidFrom" )]
		public string DateValidFrom { get; set; }

		/// <summary>
		/// End date of validity of this resource.
		/// </summary>
		[JsonProperty( "ceasn:dateValidUntil" )]
		public string DateValidUntil { get; set; }

		/// <summary>
		/// The entity being described has been modified, extended or refined from the referenced resource.
		/// </summary>
		[JsonProperty( "ceasn:derivedFrom" )]
		public List<string> DerivedFrom { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// </summary>
		[JsonProperty( "ceasn:license" )]
		public string License { get; set; }

		/// <summary>
		/// The publication status of the resource.
		/// </summary>
		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// An agent responsible for making this resource available.
		/// </summary>
		[JsonProperty( "ceasn:publisher" )]
		public List<string> Publisher { get; set; }

		/// <summary>
		/// Name of an agent responsible for making this resource available.
		/// </summary>
		[JsonProperty( "ceasn:publisherName" )]
		public LanguageMapList PublisherName { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; }

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// URI
		/// </summary>
		[JsonProperty( "ceasn:sourceDocumentation" )]
		public List<string> SourceDocumentation { get; set; }

		#region Process profiles

		/// <summary>
		/// Description of a process by which a resource is administered.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:administrationProcess" )]
		public List<ProcessProfile> AdministrationProcess { get; set; }

		/// <summary>
		///  Description of a process by which a resource was created.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		/// <summary>
		/// Entity describing the process by which the resource is revoked.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:revocationProcess" )]
		public List<ProcessProfile> RevocationProcess { get; set; }

		#endregion

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		/// <summary>
		/// Organization that pronounces favorable judgment for this credential, assessment, learning opportunity, or organization.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approvedBy" )]
		public List<string> ApprovedBy { get; set; }

		/// <summary>
		/// Region or political jurisdiction such as a state, province or locale in which an organization pronounces favorable judgment for this credential, assessment, learning opportunity, or organization.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approvedIn" )]
		public List<JurisdictionProfile> ApprovedIn { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// skos:Concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:codedNotation" )]
		public string CodedNotation { get; set; }

		/// <summary>
		/// Person or organization holding the rights in copyright to entities such as credentials, learning opportunities, assessments, competencies or concept schemes.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:copyrightHolder" )]
		public List<string> CopyrightHolder { get; set; } = null;

		/// <summary>
		/// Effective date of this resource's content.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Concept scheme used characterize different aspects (facets) of descriptors of nodes in a framework or similar resource.
		/// URI to the concept
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasFacetScheme" )]
		public List<string> HasFacetScheme { get; set; }

		/// <summary>
		/// Terms and definitions applicable to the resource.
		/// URI to the concept scheme
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasGlossary" )]
		public List<string> HasGlossary { get; set; }

		/// <summary>
		/// Reference to a progression model used.
		/// </summary>
		[JsonProperty( PropertyName = "asn:hasProgressionModel" )]
		public string HasProgressionModel { get; set; }

		/// <summary>
		/// Has Sub-framework scheme
		/// Indicates a Concept Scheme used to differentiate parts of a framework or related resources that are considered to be somewhat independent.
		/// Range: ConceptScheme
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasSubFrameworkScheme" )]
		public string HasSubFrameworkScheme { get; set; }

		/// <summary>
		/// Identifier
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// List of IdentifierValue
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		/// <summary>
		/// Image, icon or logo that represents the entity including registered trade or service marks.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:image" )]
		public string Image { get; set; }

		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages;
		/// e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:inLanguage" )]
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Geographic or political region in which the credential is formally applicable or an organization has authority to act.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		/// <summary>
		/// Type of official status of the Assessment; select from an enumeration of such types.
		/// URI to a concept
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:lifeCycleStatusType" )]
		public CredentialAlignmentObject LifeCycleStatusType { get; set; }

		/// <summary>
		/// Textual description of the criteria, standards, and/or requirements used with a process.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription { get; set; }

		/// <summary>
		/// Webpage or online document that describes the criteria, standards, and/or requirements used with a process.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:processStandards" )]
		public string ProcessStandards { get; set; }

		/// <summary>
		/// Agent that acknowledges the validity of the credential, learning opportunity of assessment.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:recognizedBy" )]
		public List<string> RecognizedBy { get; set; }

		/// <summary>
		/// Region or political jurisdiction such as a state, province or locale in which the credential, learning resource, or assessment has been publicly recommended, acknowledged or endorsed.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:recognizedIn" )]
		public List<JurisdictionProfile> RecognizedIn { get; set; }

		/// <summary>
		/// Action related to the resource
		/// URI to a credentialing action, including:
		/// ceterms:ApproveAction
		/// ceterms:RecognizeAction
		/// ceterms:RegistrationAction
		/// ceterms:RenewAction
		/// ceterms:RevokeAction
		/// ceterms:RightsAction
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
		/// Authoritative source of an entity's information.
		/// URL
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:source" )]
		public List<string> Source { get; set; }

		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subject" )]
		public List<CredentialAlignmentObject> Subject { get; set; }

		/// <summary>
		/// A CreativeWork or Event about this Thing.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "schema:subjectOf" )]
		public List<string> SubjectOf { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:versionCode" )]
		public string VersionCode { get; set; }

		/// <summary>
		/// Alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:versionIdentifier" )]
		public List<IdentifierValue> VersionIdentifier { get; set; }

		#region Versions

		/// <summary>
		/// Latest version of the resource.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:latestVersion" )]
		public string LatestVersion { get; set; }

		/// <summary>
		/// Version of the resource that immediately precedes this version.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:previousVersion" )]
		public string PreviousVersion { get; set; }

		/// <summary>
		/// Version of the resource that immediately follows this version.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:nextVersion" )]
		public string NextVersion { get; set; }

		/// <summary>
		/// Resource that replaces this resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:supersededBy" )]
		public string SupersededBy { get; set; }

		/// <summary>
		/// Resource that this resource replaces.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:supersedes" )]
		public string Supersedes { get; set; }

		#endregion
	}
}
