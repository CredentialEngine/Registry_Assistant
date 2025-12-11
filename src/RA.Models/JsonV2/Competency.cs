using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Competency class
	/// </summary>
	public class Competency // : JsonLDDocument
	{
		public Competency()
		{
			Type = "ceasn:Competency";
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( "ceasn:competencyText" )]
		public LanguageMap CompetencyText { get; set; }

		[JsonProperty( "ceasn:creator" )]
		public List<string> Creator { get; set; }

		[JsonProperty( "ceasn:competencyCategory" )]
		public LanguageMap CompetencyCategory { get; set; }

		[JsonProperty( "ceasn:competencyLabel" )]
		public LanguageMap CompetencyLabel { get; set; }

		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Enduring attributes of the individual that influence performance are embodied either directly or indirectly in this resource.
		/// The abilityEmbodied property may referenced a defined ability in an ontology such as O*NET or an existing competency defined in a competency framework.
		/// Competency
		/// </summary>
		[JsonProperty( "ceasn:abilityEmbodied" )]
		public List<string> AbilityEmbodied { get; set; }

		/// <summary>
		/// Competency uris
		/// </summary>
		[JsonProperty( "ceasn:alignFrom" )]
		public List<string> AlignFrom { get; set; }

		/// <summary>
		/// Competency uris
		/// </summary>
		[JsonProperty( "ceasn:alignTo" )]
		public List<string> AlignTo { get; set; }

		[JsonProperty( "ceasn:altCodedNotation" )]
		public List<string> AltCodedNotation { get; set; }

		[JsonProperty( "ceasn:author" )]
		public string Author { get; set; }

		[JsonProperty( "ceasn:codedNotation" )]
		public string CodedNotation { get; set; }

		[JsonProperty( "ceasn:comment" )]
		public LanguageMapList Comment { get; set; }

		[JsonProperty( "ceasn:complexityLevel" )]
		public List<string> ComplexityLevel { get; set; }

		[JsonProperty( "ceasn:comprisedOf" )]
		public List<string> ComprisedOf { get; set; }

		[JsonProperty( "ceasn:conceptKeyword" )]
		public LanguageMapList ConceptKeyword { get; set; }

		[JsonProperty( "ceasn:conceptTerm" )]
		public List<string> ConceptTerm { get; set; }

		[JsonProperty( "ceasn:dateCreated" )]
		public string DateCreated { get; set; }

		[JsonProperty( "ceasn:dateModified" )]
		public string DateModified { get; set; }

		/// <summary>
		/// A version of the entity being referenced that has been modified in meaning through editing, extension or refinement.
		/// 23-03-22 - change to a list, sigh
		/// </summary>
		[JsonProperty( "ceasn:derivedFrom" )]
		public List<string> DerivedFrom { get; set; }

		/// <summary>
		/// Education Level Type
		/// Concept URI
		/// </summary>
		[JsonProperty( "ceasn:educationLevelType" )]
		public List<string> EducationLevelType { get; set; }

		[JsonProperty( "ceasn:hasChild" )]
		public List<string> HasChild { get; set; }

		[JsonProperty( "ceasn:identifier" )]
		public List<string> Identifier { get; set; }

		/// <summary>
		/// Competency deduced or arrive at by reasoning on the competency being described.
		/// List of URIs (CTIDs recommended) to competencies
		/// </summary>
		[JsonProperty( "ceasn:inferredCompetency" )]
		public List<string> InferredCompetency { get; set; }

		/// <summary>
		/// Is Child Of
		/// The referenced competency is higher in some arbitrary hierarchy than this competency.
		/// List of URIs (CTIDs recommended) to competenciesenvironment.
		/// </summary>
		[JsonProperty( "ceasn:isChildOf" )]
		public List<string> IsChildOf { get; set; }

		/// <summary>
		/// URI to the framework that this competency is part of.
		/// Will not be present for a member of a collection.
		/// </summary>
		[JsonProperty( "ceasn:isPartOf" )]
		public string IsPartOf { get; set; }

		[JsonProperty( "ceasn:isTopChildOf" )]
		public string IsTopChildOf { get; set; }

		/// <summary>
		/// A related competency of which this competency is a version, edition, or adaptation.
		/// </summary>
		[JsonProperty( "ceasn:isVersionOf" )]
		public string IsVersionOf { get; set; }

		/// <summary>
		/// Concept in a ProgressionModel concept scheme
		/// 24-09-05 mp - temporarily defining as an object due to a CaSS bug that exports the data as a list.
		/// </summary>
		[JsonProperty( PropertyName = "asn:hasProgressionLevel" )]
		public object HasProgressionLevel { get; set; }

		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		[JsonProperty( "ceasn:listID" )]
		public string ListID { get; set; }

		[JsonProperty( "ceasn:localSubject" )]
		// public LanguageMapList LocalSubject { get; set; }
		public object LocalSubject { get; set; }

		#region alignments

		/// <summary>
		/// Competency uris
		/// </summary>
		[JsonProperty( "ceasn:broadAlignment" )]
		public List<string> BroadAlignment { get; set; }

		/// <summary>
		/// A relationship between this competency and a competency in a separate competency framework.
		/// Range Includes: ceasn:Competency
		/// </summary>
		[JsonProperty( "ceasn:crossSubjectReference" )]
		public List<string> CrossSubjectReference { get; set; }

		/// <summary>
		/// Competency uris
		/// This should be a list of URIs. The data type is object to handle receiving a string, which will be converted to a list of strings
		/// </summary>
		[JsonProperty( "ceasn:exactAlignment" )]
		// public List<string> exactAlignment { get; set; }
		public object ExactAlignment { get; set; }

		/// <summary>
		/// Competency uris
		/// </summary>
		[JsonProperty( "ceasn:majorAlignment" )]
		public List<string> MajorAlignment { get; set; }

		/// <summary>
		/// Competency uris
		/// </summary>
		[JsonProperty( "ceasn:minorAlignment" )]
		public List<string> MinorAlignment { get; set; }

		/// <summary>
		/// Competency uris
		/// </summary>
		[JsonProperty( "ceasn:narrowAlignment" )]
		public List<string> NarrowAlignment { get; set; }

		/// <summary>
		/// This competency is a prerequisite to the referenced competency.
		/// Uri to a competency
		/// </summary>
		[JsonProperty( "ceasn:prerequisiteAlignment" )]
		public List<string> PrerequisiteAlignment { get; set; }
		#endregion

		/// <summary>
		/// Body of information embodied either directly or indirectly in this competency.
		/// Competency
		/// </summary>
		[JsonProperty( "ceasn:knowledgeEmbodied" )]
		public List<string> KnowledgeEmbodied { get; set; }

		/// <summary>
		/// Specifically defined piece of work embodied either directly or indirectly in this competency.
		/// Task, Competency
		/// </summary>
		[JsonProperty( "ceasn:taskEmbodied" )]
		public List<string> TaskEmbodied { get; set; }

		[JsonProperty( "ceterms:occupationType" )]
		public List<CredentialAlignmentObject> OccupationType { get; set; }

		[JsonProperty( "ceterms:industryType" )]
		public List<CredentialAlignmentObject> IndustryType { get; set; }

		[JsonProperty( PropertyName = "ceterms:instructionalProgramType" )]
		public List<CredentialAlignmentObject> InstructionalProgramType { get; set; } = new List<CredentialAlignmentObject>();

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

		[JsonProperty( "ceterms:hasWorkforceDemand" )]
		public List<string> HasWorkforceDemand { get; set; }

		/// <summary>
		/// An asserted measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// Float
		/// </summary>
		[JsonProperty( "ceasn:weight" )]
		public string Weight { get; set; }

		/// <summary>
		/// Task related to this resource.
		/// </summary>
		[JsonProperty( "ceterms:hasTask" )]
		public List<string> HasTask { get; set; }

		/// <summary>
		/// Type of condition in the physical work performance environment that entails risk exposures requiring mitigating processes; select from an existing enumeration of such types.
		/// Collection only
		/// </summary>
		[JsonProperty( "ceasn:environmentalHazardType" )]
		public List<string> EnvironmentalHazardType { get; set; }

		[JsonProperty( "ceasn:inLanguage" )]
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// Collections only
		/// </summary>
		[JsonProperty( "ceasn:license" )]
		public string License { get; set; }

		/// <summary>
		/// Type of required or expected performance level for a resource;
		/// There is no concept scheme for this. Must allow any URI.
		/// </summary>
		[JsonProperty( "ceasn:performanceLevelType" )]
		public List<string> PerformanceLevelType { get; set; }

		/// <summary>
		/// Type of physical activity required or expected in performance;
		/// There is no concept scheme for this. Must allow any URI.
		/// </summary>
		[JsonProperty( "ceasn:physicalCapabilityType" )]
		public List<string> PhysicalCapabilityType { get; set; }

		/// <summary>
		/// Type of required or expected sensory capability;
		/// There is no concept scheme for this. Must allow any URI.
		/// </summary>
		[JsonProperty( "ceasn:sensoryCapabilityType" )]
		public List<string> SensoryCapabilityType { get; set; }

		/// <summary>
		/// Indicates whether correlators should or should not assign the competency during correlation.
		/// </summary>
		[JsonProperty( "ceasn:shouldIndex" )]
		public bool? ShouldIndex { get; set; }

		/// <summary>
		/// Cognitive, affective, and psychomotor skills directly or indirectly embodied in this competency.
		/// Competency
		/// </summary>
		[JsonProperty( "ceasn:skillEmbodied" )]
		public List<string> SkillEmbodied { get; set; }

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// URI
		/// </summary>
		[JsonProperty( "ceasn:sourceDocumentation" )]
		public List<string> SourceDocumentation { get; set; }

		/// <summary>
		/// Aspects of the referenced Competency Framework provide some justification that the resource being described is useful.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingCompetencyFramework" )]
		public List<string> SubstantiatingCompetencyFramework { get; set; }

		/// <summary>
		/// Aspects of the referenced Credential provide some justification that the resource being described is useful.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingCredential" )]
		public List<string> SubstantiatingCredential { get; set; }

		/// <summary>
		/// Aspects of the referenced Job provide some justification that the resource being described is useful.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingJob" )]
		public List<string> SubstantiatingJob { get; set; }

		/// <summary>
		/// Aspects of the referenced Occupation provide some justification that the resource being described is useful.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingOccupation" )]
		public List<string> SubstantiatingOccupation { get; set; }

		/// <summary>
		/// Aspects of the referenced Organization provide some justification that the resource being described is useful.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingOrganization" )]
		public List<string> SubstantiatingOrganization { get; set; }

		/// <summary>
		/// Aspects of the referenced resource provide some justification that the resource being described is useful.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingResource" )]
		public List<string> SubstantiatingResource { get; set; }

		/// <summary>
		/// Referenced Task attests to some level of achievement/mastery of the competency being described.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingTask" )]
		public List<string> SubstantiatingTask { get; set; }

		/// <summary>
		/// Referenced Workrole attests to some level of achievement/mastery of the competency being described.
		/// </summary>
		[JsonProperty( "ceasn:substantiatingWorkrole" )]
		public List<string> SubstantiatingWorkrole { get; set; }

		//--------------- helpers ---------------------------------------

		/// <summary>
		/// CIP List is a helper when publishing from a graph. It will not be published
		/// </summary>
		[JsonProperty( "cipList" )]
		public List<string> CIPList { get; set; }

		/// <summary>
		/// SOC List is a helper when publishing from a graph. It will not be published
		/// </summary>
		[JsonProperty( "socList" )]
		public List<string> SOCList { get; set; }

		/// NAICS List is a helper when publishing from a graph. It will not be published
		[JsonProperty( "naicsList" )]
		public List<string> NaicsList { get; set; }
		// temp??

		/// <summary>
		/// Only used where part of a Collection
		/// </summary>
		[JsonProperty( "ceterms:isMemberOf" )]
		public List<string> IsMemberOf { get; set; }

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
		public string LatestVersion { get; set; } // URL

		/// <summary>
		/// Version of the resource that immediately precedes this version.
		/// full URL OR CTID (recommended)
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:previousVersion" )]
		public string PreviousVersion { get; set; } // URL

		/// <summary>
		/// Version of the resource that immediately follows this version.
		/// full URL OR CTID (recommended)
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:nextVersion" )]
		public string NextVersion { get; set; } // URL

		#region process profiles

		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion
	}

	public class CompetencyPlain : Competency
	{

		public CompetencyPlain()
		{
		}

		[JsonProperty( "ceasn:comment" )]
		public new List<string> Comment { get; set; } = new List<string>();

		[JsonProperty( "ceasn:competencyCategory" )]
		public new string CompetencyCategory { get; set; }

		[JsonProperty( "ceasn:competencyText" )]
		public new string CompetencyText { get; set; }

		[JsonProperty( "ceasn:competencyLabel" )]
		public new string CompetencyLabel { get; set; }

		[JsonProperty( "ceasn:conceptKeyword" )]
		public new List<string> ConceptKeyword { get; set; } = new List<string>();

		[JsonProperty( "ceasn:localSubject" )]
		public new List<string> LocalSubject { get; set; } = new List<string>();
	}

}
