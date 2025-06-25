using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	public class AlignmentMap
	{

		[JsonProperty( "@type" )]
		public string Type { get; set; } = "ceterms:AlignmentMap";

		#region Required Properties
		public string CTID { get; set; }

		/// <summary>
		/// Name of this resource
		/// REQUIRED
		/// </summary>
		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name_LangMap { get; set; }

		/// <summary>
		/// Statement, characterization or account of the entity. 
		/// REQUIRED
		/// </summary>
		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description_LangMap { get; set; }

		/// <summary>
		/// Indicates a separately identifiable and independently useful component of the entity. 
		/// A this time, the only valid CTDL type is AlignmentObject, so this will be a list of bnode identifiers
		///     (provided in request.ReferenceObjects)
		/// REQUIRED
		/// </summary>
		public List<AlignmentObject> HasPart { get; set; }

		/// <summary>
		/// The primary language or languages of the entity, even if it makes use of other languages; 
		/// e.g., a course offered in English to teach Spanish would have an inLanguage of English, while a credential in Quebec could have an inLanguage of both French and English.
		/// REQUIRED
		/// </summary>
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Type of official status of the Assessment; select from an enumeration of such types.
		/// URI to a concept
		/// REQUIRED
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// Owning organization
		/// REQUIRED
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		/// <summary>
		/// The publication status of the resource.
		/// REQUIRED
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// An agent responsible for making this resource available.
		/// List of CTIDs, or bnode identifiers
		/// REQUIRED
		/// </summary>
		public List<OrganizationReference> Publisher { get; set; }

		/// <summary>
		/// Or allow from a bnode list
		/// </summary>
		public List<string> PublisherList { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// REQUIRED
		/// </summary>
		public string SubjectWebpage { get; set; }
		#endregion

		/// <summary>
		/// Subject matter of the resource.
		/// Only a QualificationsFramework may be referenced
		/// URI
		/// </summary>
		public List<string> About { get; set; }

		/// <summary>
		/// A person or organization chiefly responsible for the intellectual or artistic content of this competency framework or competency.
		/// </summary>
		public List<string> Author { get; set; }

		/// <summary>
		/// An entity primarily responsible for making this resource.
		/// </summary>
		public List<OrganizationReference> Creator { get; set; }

		/// <summary>
		/// Date of a statement of copyright for this resource, such as ©2017.
		/// </summary>
		public string DateCopyrighted { get; set; }

		/// <summary>
		/// Date of creation of this resource.
		/// </summary>
		public string DateCreated { get; set; }

		/// <summary>
		/// The date on which this resource was most recently modified in some way.
		/// </summary>
		public string DateModified { get; set; }

		/// <summary>
		/// Beginning date of validity of this resource.
		/// </summary>
		public string DateValidFrom { get; set; }

		/// <summary>
		/// End date of validity of this resource.
		/// </summary>
		public string DateValidUntil { get; set; }

		/// <summary>
		/// The entity being described has been modified, extended or refined from the referenced resource.
		/// </summary>
		public List<string> DerivedFrom { get; set; }

		/// <summary>
		/// Alignment assertion belonging to the alignment map.
		/// URLs
		/// </summary>
		public List<string> HasStatement { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// Name of an agent responsible for making this resource available.
		/// </summary>
		public List<string> PublisherName { get; set; }

		[JsonProperty( "ceasn:publisherName" )]
		public LanguageMapList PublisherName_LangMap { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		public string Rights { get; set; }

		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights_LangMap { get; set; }

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// URI
		/// </summary>
		public List<string> SourceDocumentation { get; set; }

		#region Process profiles

		/// <summary>
		///  Description of a process by which a resource was created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }

		/// <summary>
		/// Entity describing the process by which the resource is revoked.
		/// </summary>
		public List<ProcessProfile> RevocationProcess { get; set; }

		#endregion

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		public List<string> AlternateName { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName_LangMap { get; set; }

		/// <summary>
		/// Organization that pronounces favorable judgment for this credential, assessment, learning opportunity, or organization.
		/// </summary>
		public List<OrganizationReference> ApprovedBy { get; set; }

		/// <summary>
		/// Region or political jurisdiction such as a state, province or locale in which an organization pronounces favorable judgment for this credential, assessment, learning opportunity, or organization.
		/// </summary>
		public List<JurisdictionProfile> ApprovedIn { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// skos:Concept
		/// </summary>
		public List<string> Classification { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Framework that contains terms that are compared in this AlignmentMap.
		/// Frameworks may be concept schemes, progression models and other educational frameworks.
		/// skos:ConceptScheme
		/// asn:ProgressionModel
		/// ceasn:CompetencyFramework
		/// </summary>
		public List<string> Compares { get; set; } = null;

		/// <summary>
		/// Person or organization holding the rights in copyright to entities such as credentials, learning opportunities, assessments, competencies or concept schemes.
		/// </summary>
		public List<OrganizationReference> CopyrightHolder { get; set; } = null;

		/// <summary>
		/// Effective date of this resource's content.
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Identifier
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// List of IdentifierValue 
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; }

		/// <summary>
		/// Image, icon or logo that represents the entity including registered trade or service marks.
		/// xsd:anyURI
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// Geographic or political region in which the credential is formally applicable or an organization has authority to act.
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity. 
		/// </summary>
		public List<string> Keyword { get; set; }

		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList Keyword_LangMap { get; set; }

		/// <summary>
		/// ObjectOfAction
		/// Action carried out upon this resource.
		/// Refer to the referenced Action for more information. Other resources may be included for the full value.
		/// This typically an inverse property. It is used on a CredentialingAction resource, not this (the resource itself)
		/// </summary>

		/// <summary>
		/// Textual description of the criteria, standards, and/or requirements used with a process.
		/// </summary>
		public string ProcessStandardsDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription_LangMap { get; set; }

		/// <summary>
		/// Webpage or online document that describes the criteria, standards, and/or requirements used with a process.
		/// xsd:anyURI
		/// </summary>
		public string ProcessStandards { get; set; }

		/// <summary>
		/// Agent that acknowledges the validity of the credential, learning opportunity of assessment.
		/// </summary>
		public List<OrganizationReference> RecognizedBy { get; set; }

		/// <summary>
		/// Region or political jurisdiction such as a state, province or locale in which the credential, learning resource, or assessment has been publicly recommended, acknowledged or endorsed.
		/// </summary>
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
		public List<string> RelatedAction { get; set; }

		/// <summary>
		/// Another source of information about the entity being described.
		/// List of URIs
		/// ceterms:sameAs
		/// </summary>
		public List<string> SameAs { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// URL
		/// </summary>
		public List<string> Source { get; set; }

		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// </summary>
		public List<string> Subject { get; set; } = new List<string>();

		/// <summary>
		/// Language map list for Subject
		/// </summary>
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		public string VersionCode { get; set; }

		/// <summary>
		/// Alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; }

		#region Versions

		/// <summary>
		/// Latest version of the resource.
		/// xsd:anyURI
		/// </summary>
		public string LatestVersion { get; set; }

		/// <summary>
		/// Version of the resource that immediately precedes this version.
		/// xsd:anyURI
		/// </summary>
		public string PreviousVersion { get; set; }

		/// <summary>
		/// Version of the resource that immediately follows this version.
		/// xsd:anyURI
		/// </summary>
		public string NextVersion { get; set; }

		/// <summary>
		/// Resource that replaces this resource.
		/// </summary>
		public string SupersededBy { get; set; }

		/// <summary>
		/// Resource that this resource replaces.
		/// </summary>
		public string Supersedes { get; set; }

		#endregion
	}
}
