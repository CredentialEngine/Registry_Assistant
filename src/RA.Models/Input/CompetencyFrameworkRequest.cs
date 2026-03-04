// <copyright file="CompetencyFrameworkRequest.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	public class CompetencyFrameworkRequest : BaseRequest
	{
		public CompetencyFrameworkRequest()
		{
		}

		public CompetencyFramework CompetencyFramework
		{
			get; set;
		}

		public List<Competency> Competencies
		{
			get; set;
		}
	}

	public class CompetencyFrameworkGraphRequest : BaseRequest
	{
		public CompetencyFrameworkGraphRequest()
		{
			CompetencyFrameworkGraph = new CompetencyFrameworkGraph();
		}

		// Added when publishing from CaSS for reference
		public string CTID { get; set; }

		public CompetencyFrameworkGraph CompetencyFrameworkGraph { get; set; }
	}

	/// <summary>
	/// Request to publish a list of standalone competencies.
	/// Required:
	/// - CTID
	/// - CompetencyText
	/// - Creator
	/// 
	/// Not allowed:
	/// - HasChild
	/// - IsChildOf
	/// - IsPartOf
	/// - IsMemberOf
	/// - IsTopChildOf
	/// </summary>
	public class CompetencyListRequest : BaseRequest
	{
		public CompetencyListRequest()
		{
		}

		public List<Competency> Competencies
		{
			get; set;
		}
	}

	/// <summary>
	/// Request for standalone competency. 
	/// Required:
	/// - CTID
	/// - CompetencyText
	/// - Creator
	/// 
	/// Not allowed:
	/// - HasChild
	/// - IsChildOf
	/// - IsPartOf
	/// - IsMemberOf
	/// - IsTopChildOf
	/// </summary>
	public class CompetencyRequest : BaseRequest
	{
		public CompetencyRequest()
		{
			Competency = new Competency();
		}

		public Competency Competency { get; set; }
	}

	/// <summary>
	/// Competency Framework
	/// Required:
	/// - CTID, Name, Description, InLanguage, Publisher
	/// </summary>
	public class CompetencyFramework
	{
		public CompetencyFramework()
		{
		}

		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "CompetencyFramework";

		/// <summary>
		/// A unique URI for this framework.
		/// If not entered (RECOMMENDED), a credential registry URI will be generated using the CTID.
		/// </summary>
		public string CtdlId { get; set; } = string.Empty;

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID) by which the creator, owner or provider of a credential, learning opportunity competency, or assessment recognizes the entity in transactions with the external environment (e.g., in verifiable claims involving a credential).
		/// required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// A competency framework or competency from which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// List of URIs to frameworks
		/// </summary>
		public List<string> AlignFrom { get; set; } = new List<string>();

		/// <summary>
		/// A competency framework or competency to which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// List of URIs to frameworks
		/// </summary>
		public List<string> AlignTo { get; set; } = new List<string>();

		/// <summary>
		/// A person or organization chiefly responsible for the intellectual or artistic content of this competency framework or competency.
		/// List of Names
		/// </summary>
		public List<string> Author { get; set; } = new List<string>();

		public string CodedNotation { get; set; }

		/// <summary>
		/// A word or phrase used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// The conceptKeyword property is used in ASN-conforming data solely to denote the significant topicality of the competency using free-text keywords and phrases derived and assigned by the indexer, e.g., "George Washington", "Ayers Rock", etc.
		/// </summary>
		public List<string> ConceptKeyword { get; set; }

		public LanguageMapList ConceptKeywordLangMap { get; set; }

		/// <summary>
		/// Concept Term
		/// A term drawn from a controlled vocabulary used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// List of URIs to concepts
		/// </summary>
		public List<string> ConceptTerm { get; set; } = new List<string>();

		/// <summary>
		/// An entity primarily responsible for making this competency framework or competency.
		/// The creator property is used with non-canonical statements created by a third party.
		/// List of URIs to the creator
		/// </summary>
		public List<string> Creator { get; set; } = new List<string>();

		/// <summary>
		/// Date Copyrighted
		/// Date of a statement of copyright for this competency framework, such as ©2017.
		/// </summary>
		public string DateCopyrighted { get; set; }

		/// <summary>
		/// Date of creation of this competency framework or competency.
		/// </summary>
		public string DateCreated { get; set; }

		/// <summary>
		/// The date on which this framework or competency was most recently modified in some way.
		/// </summary>
		public string DateModified { get; set; }

		/// <summary>
		/// Beginning date of validity of this competency framework.
		/// Where the competency document is valid for a given period of time, use both the dateValidFrom and dateValidUntil properties.
		/// </summary>
		public string DateValidFrom { get; set; }

		/// <summary>
		/// End date of validity of this competency framework.
		/// Where the standard document is valid for a given period of time, use both the dateValidFrom and dateValidUntil properties.
		/// </summary>
		public string DateValidUntil { get; set; }

		/// <summary>
		/// Derived From
		/// A third party version of the entity being reference that has been modified in meaning through editing, extension or refinement.
		/// List of URIs to frameworks
		/// 2023-03-22 The datatype was changed to a list. The API will still handle a single string.
		/// </summary>
		public object DerivedFrom { get; set; }

		/// <summary>
		/// A short description of this competency framework.
		/// Required
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = null;

		/// <summary>
		/// Education Level Type
		/// Definition:	en-US:A general statement describing the education or training context. Alternatively, a more specific statement of the location of the audience in terms of its progression through an education or training context.
		/// Best practice is to use terms from the http://purl.org/ctdl/terms/AudienceLevel concept scheme.
		/// List of URIs to concepts
		/// </summary>
		public List<string> EducationLevelType { get; set; } = new List<string>();

		/// <summary>
		/// Terms and definitions applicable to the resource.
		/// URI to the concept scheme
		/// </summary>
		public List<string> HasGlossary { get; set; }

		/// <summary>
		/// List of the top competencies, those that are directly connected to the framework. Provide either a CTID for a competency that is include in the Competencies property, or the full URI formatted like the following:
		/// "https://credentialengineregistry.org/resources/ce-b1e0eca2-7a19-49e9-8841-fa16ddf8396d"
		/// List of URIs to competencies.
		/// NOTE: Just provide the CTIDs, and the system will format the proper URI for the current environment.
		/// </summary>
		public List<string> HasTopChild { get; set; } = new List<string>();

		/// <summary>
		/// Identifier
		/// An alternative URI by which this competency framework or competency is identified.
		/// List of URIs
		/// </summary>
		public List<string> Identifier { get; set; } = new List<string>();

		/// <summary>
		/// In Language
		/// The primary language used in or by this competency framework or competency.The primary language used in or by this competency framework or competency.
		/// This is the language the text is primarily written in, even if it makes use of other languages. For example, a competency for teaching spanish to english-speaking students would primarily be in english, because that is the language used to convey the material.
		/// </summary>
		public List<string> InLanguage { get; set; } = new List<string>();

		/// <summary>
		/// A legal document giving official permission to do something with this competency framework.
		/// Value must be the URI to a license document (e.g., Creative Commons license or bespoke license).
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// The text string denoting the subject of the competency framework or competency as designated by the promulgating agency.
		/// The localSubject property and subject property may or may not point to the same subject. If so, enter the text string for the subject as the value of this property and the URI for that subject in the subject property.
		/// </summary>
		public List<string> LocalSubject { get; set; } = new List<string>();

		[JsonProperty( "ceasn:localSubject" )]
		public LanguageMapList LocalSubjectLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// The name or title of this competency framework.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///  LanguageMap for Name
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; } = null;

		/// <summary>
		/// The publication status of the of this competency framework.
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// An agent responsible for making this entity available.
		/// Also referred to as the promulgating agency of the entity.
		/// List of URIs, for example to a ceterms:CredentialOrganization
		/// Or provide a list of CTIDs and the Assistant API will format the proper URL for the environment.
		/// Required
		/// </summary>
		public List<string> Publisher { get; set; } = new List<string>();

		/// <summary>
		/// Name of an agent responsible for making this entity available.
		/// </summary>
		public List<string> PublisherName { get; set; } = new List<string>();

		/// <summary>
		/// Language map for publisher name
		/// </summary>
		public LanguageMapList PublisherNameLangMap { get; set; }

		/// <summary>
		/// The date this competency framework was added to the repository.
		/// </summary>
		public string RepositoryDate { get; set; }

		/// <summary>
		/// Formerly a URI,
		/// 19-01-18 Now a language string!!
		/// </summary>
		public string Rights { get; set; }

		[JsonProperty( "ceasn:rights" )]
		public LanguageMap RightsLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// An agent owning or managing rights over this competency framework.
		/// List of URLs
		/// 2023-03-22 The datatype was changed to a list. The API will still handle a single string.
		/// </summary>
		public List<string> RightsHolder { get; set; }

		/// <summary>
		///  The original competency framework which this competency framework is based on or derived from.
		///  URI
		/// </summary>
		public List<string> Source { get; set; } = new List<string>();

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// List of URIs
		/// </summary>
		public List<string> SourceDocumentation { get; set; } = new List<string>();

		/// <summary>
		/// A list of sub-units of this competency framework.
		/// The table of contents is a "manifest", or a hierarchic, ordered, syntactic representation of the competencies that are part of this competency framework.
		/// </summary>
		public string TableOfContents { get; set; }

		public LanguageMap TableOfContentsLangMap { get; set; }

		#region Occupations, Industries, and instructional programs
		// =====================================================================
		// List of occupations from a published framework, that is with a web URL

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
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// The API will validate and format the ONet codes as Occupations
		/// </summary>
		public List<string> ONET_Codes { get; set; } = new List<string>();

		// =============================================================================

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
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> NaicsList { get; set; } = new List<string>();
		// =============================================================================

		/// <summary>
		/// InstructionalProgramType
		/// Type of instructional program; select from an existing enumeration of such types.
		/// </summary>
		public List<FrameworkItem> InstructionalProgramType { get; set; }

		/// <summary>
		/// AlternativeInstructionalProgramType
		/// Programs that are not found in a formal framework can be still added using AlternativeInstructionalProgramType.
		/// Any programs added using this property will be added to or appended to the InstructionalProgramType output.
		/// </summary>
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();

		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion

		/// <summary>
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		public string VersionCode { get; set; }

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner.
		/// The resource version captured here is any local identifier used by the resource owner to identify the version of the resource in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; }

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

		#region -- Process Profiles --

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
