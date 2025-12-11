using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class CompetencyFrameworksGraph
	{

		public CompetencyFrameworksGraph()
		{
		}

		[JsonProperty( "@context" )]
		public string Context { get; set; } = "https://credreg.net/ctdlasn/schema/context/json";

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		/// <summary>
		/// Main graph object
		/// </summary>
		[JsonProperty( "@graph" )]
		public object Graph { get; set; }
	}

	public class CompetencyFramework // : JsonLDDocument
	{
		public CompetencyFramework()
		{
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; } = "ceasn:CompetencyFramework";

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( "ceasn:alignFrom" )]
		public List<string> AlignFrom { get; set; }

		[JsonProperty( "ceasn:alignTo" )]
		public List<string> AlignTo { get; set; }

		[JsonProperty( "ceasn:author" )]
		public List<string> Author { get; set; }

		[JsonProperty( "ceasn:codedNotation" )]
		public string CodedNotation { get; set; }

		[JsonProperty( "ceasn:conceptKeyword" )]
		public LanguageMapList ConceptKeyword { get; set; }

		[JsonProperty( "ceasn:conceptTerm" )]
		public List<string> ConceptTerm { get; set; }

		[JsonProperty( "ceasn:creator" )]
		public List<string> Creator { get; set; }

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
		/// xsd:dateTime
		/// </summary>
		[JsonProperty( "ceasn:dateValidFrom" )]
		public string DateValidFrom { get; set; }

		/// <summary>
		/// xsd:dateTime
		/// </summary>
		[JsonProperty( "ceasn:dateValidUntil" )]
		public string DateValidUntil { get; set; }

		/// <summary>
		/// single per https://github.com/CredentialEngine/CompetencyFrameworks/issues/66
		/// 23-03-22 back to a list
		/// but store as object due to old resources as string
		/// </summary>
		[JsonProperty( "ceasn:derivedFrom" )]
		public object DerivedFrom { get; set; }

		[JsonProperty( "ceasn:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Education Level Type
		/// Best practice is to use terms from the http://purl.org/ctdl/terms/AudienceLevel concept scheme.
		/// Range: skos:Concept
		/// </summary>
		[JsonProperty( "ceasn:educationLevelType" )]
		public List<string> EducationLevelType { get; set; }

		/// <summary>
		/// Terms and definitions applicable to the resource.
		/// URI to the concept scheme
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasGlossary" )]
		public List<string> HasGlossary { get; set; }

		/// <summary>
		/// Top-level child competency of a competency framework.
		/// </summary>
		[JsonProperty( "ceasn:hasTopChild" )]
		public List<string> HasTopChild { get; set; }

		[JsonProperty( "ceasn:identifier" )]
		public List<string> Identifier { get; set; }

		[JsonProperty( "ceasn:inLanguage" )]
		public List<string> InLanguage { get; set; }

		[JsonProperty( "ceasn:license" )]
		public string License { get; set; }

		[JsonProperty( "ceasn:localSubject" )]
		public LanguageMapList LocalSubject { get; set; }

		[JsonProperty( "ceasn:name" )]
		public LanguageMap Name { get; set; } = new LanguageMap();

		/// <summary>
		/// The publication status of the resource.
		/// Range: skos:Concept
		/// </summary>
		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		[JsonProperty( "ceasn:publisher" )]
		public List<string> Publisher { get; set; }

		[JsonProperty( "ceasn:publisherName" )]
		public LanguageMapList PublisherName { get; set; }

		[JsonProperty( "ceasn:repositoryDate" )]
		public string RepositoryDate { get; set; }

		/// <summary>
		/// 19-01-18 Changed to a language string
		/// </summary>
		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; }

		[JsonProperty( "ceasn:rightsHolder" )]
		public object RightsHolder { get; set; }

		[JsonProperty( "ceasn:source" )]
		public List<string> Source { get; set; }

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// URI
		/// </summary>
		[JsonProperty( "ceasn:sourceDocumentation" )]
		public List<string> SourceDocumentation { get; set; }

		[JsonProperty( "ceasn:tableOfContents" )]
		public LanguageMap TableOfContents { get; set; }

		[JsonProperty( "ceterms:occupationType" )]
		public List<CredentialAlignmentObject> OccupationType { get; set; }

		[JsonProperty( "ceterms:industryType" )]
		public List<CredentialAlignmentObject> IndustryType { get; set; }

		[JsonProperty( PropertyName = "ceterms:instructionalProgramType" )]
		public List<CredentialAlignmentObject> InstructionalProgramType { get; set; }

		#region Helper properties where publishing input is a graph. These will not be published

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

		/// <summary>
		/// NAICS List is a helper when publishing from a graph. It will not be published
		/// </summary>
		[JsonProperty( "naicsList" )]
		public List<string> NaicsList { get; set; } = null;
		#endregion

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

		/// <summary>
		/// temporary helpers pending a CaSS update
		/// </summary>
		[JsonProperty( PropertyName = "ceasn:versionIdentifier" )]
		public List<string> VersionIdentifier1 { get; set; } = null;

		[JsonProperty( PropertyName = "ceasn:versionIdentifier_1" )]
		public List<string> VersionIdentifier2 { get; set; } = null;

		[JsonProperty( PropertyName = "ceasn:versionIdentifier_2" )]
		public List<string> VersionIdentifier3 { get; set; } = null;

		[JsonProperty( PropertyName = "ceasn:versionIdentifier_3" )]
		public List<string> VersionIdentifier4 { get; set; } = null;

		[JsonProperty( PropertyName = "ceasn:versionIdentifier_4" )]
		public List<string> VersionIdentifier5 { get; set; } = null;

		[JsonProperty( PropertyName = "ceasn:versionIdentifier_5" )]
		public List<string> VersionIdentifier6 { get; set; } = null;

		/// <summary>
		/// Latest version of the credential.
		/// full URL OR CTID (recommended)
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:latestVersion" )]
		public string LatestVersion { get; set; }

		/// <summary>
		/// Version of the resource that immediately precedes this version.
		/// full URL OR CTID (recommended)
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:previousVersion" )]
		public string PreviousVersion { get; set; }

		/// <summary>
		/// Version of the resource that immediately follows this version.
		/// full URL OR CTID (recommended)
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:nextVersion" )]
		public string NextVersion { get; set; }

		#region process profiles

		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion
	}

	public class CompetencyFrameworkPlain
	{
		public CompetencyFrameworkPlain()
		{
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; } = "ceasn:CompetencyFramework";

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( "ceasn:alignFrom" )]
		public List<string> AlignFrom { get; set; } = new List<string>();

		[JsonProperty( "ceasn:alignTo" )]
		public List<string> AlignTo { get; set; } = new List<string>();

		[JsonProperty( "ceasn:altIdentifier" )]
		public List<string> AltIdentifier { get; set; }

		[JsonProperty( "ceasn:author" )]
		public List<string> Author { get; set; }

		[JsonProperty( "ceasn:conceptKeyword" )]
		public List<string> ConceptKeyword { get; set; } = new List<string>();

		[JsonProperty( "ceasn:conceptTerm" )]
		public List<string> ConceptTerm { get; set; } = new List<string>();

		[JsonProperty( "ceasn:creator" )]
		public List<string> Creator { get; set; } = new List<string>();

		[JsonProperty( "ceasn:dateCopyrighted" )]
		public string DateCopyrighted { get; set; }

		[JsonProperty( "ceasn:dateCreated" )]
		public string DateCreated { get; set; }

		[JsonProperty( "ceasn:dateModified" )]
		public string DateModified { get; set; }

		[JsonProperty( "ceasn:dateValidFrom" )]
		public string DateValidFrom { get; set; }

		[JsonProperty( "ceasn:dateValidUntil" )]
		public string DateValidUntil { get; set; }

		/// <summary>
		/// single per https://github.com/CredentialEngine/CompetencyFrameworks/issues/66
		/// 23-03-22 back to a list
		/// </summary>
		[JsonProperty( "ceasn:derivedFrom" )]
		public List<string> DerivedFrom { get; set; }

		[JsonProperty( "ceasn:description" )]
		public string Description { get; set; }

		[JsonProperty( "ceasn:educationLevelType" )]
		public List<string> EducationLevelType { get; set; } = new List<string>();

		/// <summary>
		/// Top-level child competency of a competency framework.
		/// </summary>
		[JsonProperty( "ceasn:hasTopChild" )]
		public List<string> HasTopChild { get; set; } = new List<string>();

		[JsonProperty( "ceasn:identifier" )]
		public List<string> Identifier { get; set; } = new List<string>();

		[JsonProperty( "ceasn:inLanguage" )]
		public List<string> InLanguage { get; set; } = new List<string>();

		[JsonProperty( "ceasn:license" )]
		public string License { get; set; }

		[JsonProperty( "ceasn:localSubject" )]
		public List<string> LocalSubject { get; set; } = new List<string>();

		[JsonProperty( "ceasn:name" )]
		public string Name { get; set; }

		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		[JsonProperty( "ceasn:publisher" )]
		public List<string> Publisher { get; set; } = new List<string>();

		[JsonProperty( "ceasn:publisherName" )]
		public List<string> PublisherName { get; set; } = new List<string>();

		[JsonProperty( "ceasn:repositoryDate" )]
		public string RepositoryDate { get; set; }

		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; } = new LanguageMap();

		[JsonProperty( "ceasn:rightsHolder" )]
		public List<string> RightsHolder { get; set; }

		[JsonProperty( "ceasn:source" )]
		public List<string> Source { get; set; } = new List<string>();

		[JsonProperty( "ceasn:tableOfContents" )]
		public string TableOfContents { get; set; }

		[JsonProperty( "ceterms:occupationType" )]
		public List<CredentialAlignmentObject> OccupationType { get; set; } = new List<CredentialAlignmentObject>();

		[JsonProperty( "ceterms:industryType" )]
		public List<CredentialAlignmentObject> IndustryType { get; set; } = new List<CredentialAlignmentObject>();
	}
}
