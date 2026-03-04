// <copyright file="Competency.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{

	/// <summary>
	/// CTDLASN Competency Class
	/// Required:
	/// - CTID
	/// - CompetencyText
	/// Where publishing a standalone competency, Creator is also required.
	/// </summary>
	public class Competency
	{
		public Competency()
		{
			Type = "Competency";
			AbilityEmbodied = new List<string>();
			AlignFrom = new List<string>();
			AlignTo = new List<string>();
			AltCodedNotation = new List<string>();

			BroadAlignment = new List<string>();
			Comment = new List<string>();
			ComplexityLevel = new List<string>();
			ComprisedOf = new List<string>();
			ConceptKeyword = new List<string>();
			ConceptTerm = new List<string>();
			Creator = new List<string>();
			CrossSubjectReference = new List<string>();

			EducationLevelType = new List<string>();
			ExactAlignment = new List<string>();
			HasChild = new List<string>();
			Identifier = new List<string>();
			InferredCompetency = new List<string>();
			IsChildOf = new List<string>();
			IsMemberOf = new List<string>();

			Keyword = new List<string>();
			KeywordLangMap = new LanguageMapList();
			KnowledgeEmbodied = new List<string>();

			LocalSubject = new List<string>();
			LocalSubjectLangMap = new LanguageMapList();
			MajorAlignment = new List<string>();
			MinorAlignment = new List<string>();
			NarrowAlignment = new List<string>();
			PrerequisiteAlignment = new List<string>();

			SkillEmbodied = new List<string>();
			TaskEmbodied = new List<string>();
			HasTask = new List<string>();
			EnvironmentalHazardType = new List<string>();
			InLanguage = new List<string>();
			PerformanceLevelType = new List<string>();

			PhysicalCapabilityType = new List<string>();
			ProvidesTransferValueFor = new List<string>();
			ReceivesTransferValueFrom = new List<string>();
			SensoryCapabilityType = new List<string>();
			SourceDocumentation = new List<string>();

			SubstantiatingCompetencyFramework = new List<string>();
			SubstantiatingCredential = new List<string>();
			SubstantiatingJob = new List<string>();
			SubstantiatingOccupation = new List<string>();
			SubstantiatingOrganization = new List<string>();
			SubstantiatingResource = new List<string>();

			SubstantiatingTask = new List<string>();
			SubstantiatingWorkrole = new List<string>();
			HasWorkforceDemand = new List<string>();
			VersionIdentifier = new List<IdentifierValue>();

		}

		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID) by which the creator, owner or provider of a credential, learning opportunity competency, or assessment recognizes the entity in transactions with the external environment (e.g., in verifiable claims involving a credential).
		/// REQUIRED
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// The text of the competency.
		/// This property should be used to provide the actual text of the competency statement. To provide information about the competency other than the text itself, use the comment property.
		/// REQUIRED
		/// </summary>
		public string CompetencyText { get; set; }

		/// <summary>
		/// Language map for competencyText
		/// </summary>
		[JsonProperty( "ceasn:competencyText" )]
		public LanguageMap CompetencyTextLangMap { get; set; }

		/// <summary>
		/// Enduring attributes of the individual that influence performance are embodied either directly or indirectly in this resource.
		/// The abilityEmbodied property may referenced a defined ability in an ontology such as O*NET or an existing competency defined in a competency framework.
		/// List of URIs (CTIDs recommended) for a competency
		/// ceasn:abilityEmbodied
		/// </summary>
		public List<string> AbilityEmbodied { get; set; }

		/// <summary>
		/// A competency framework or competency from which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// List of CTIDs for a competency framework or competency
		/// </summary>
		public List<string> AlignFrom { get; set; }

		/// <summary>
		/// A competency framework or competency to which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// List of URIs (CTIDs recommended) for a competency framework or competency
		/// </summary>
		public List<string> AlignTo { get; set; }

		/// <summary>
		/// Alternative Coded Notation
		/// An alphanumeric notation or ID code identifying this competency in common use among end-users.
		/// ceasn:altCodedNotation
		/// </summary>
		public List<string> AltCodedNotation { get; set; }

		/// <summary>
		/// A person or organization chiefly responsible for the intellectual or artistic content of this competency.
		/// Single string
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Broad Alignment
		/// The referenced competency covers all of the relevant concepts in this competency as well as relevant concepts not found in this competency.
		/// List of URIs (CTIDs recommended) for a competency framework or competency
		/// </summary>
		public List<string> BroadAlignment { get; set; }

		/// <summary>
		/// Coded Notation
		/// An alphanumeric notation or ID code as defined by the promulgating body to identify this competency.
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Comment
		/// Supplemental text provided by the promulgating body that clarifies the nature, scope or use of this competency.
		/// </summary>
		public List<string> Comment { get; set; }

		/// <summary>
		/// LanguageMap for Comment
		/// </summary>
		[JsonProperty( PropertyName = "ceasn:comment" )]
		public LanguageMapList CommentLangMap { get; set; }

		/// <summary>
		/// Competency Category
		/// The textual label identifying the category of the competency as designated by the promulgating body.
		/// </summary>
		public string CompetencyCategory { get; set; }

		/// <summary>
		/// Language map for competencyCategory
		/// </summary>
		[JsonProperty( "ceasn:competencyCategory" )]
		public LanguageMap CompetencyCategoryLangMap { get; set; }

		/// <summary>
		/// Short identifying phrase or name applied to a competency by the creator of the competency framework.
		/// </summary>
		public string CompetencyLabel { get; set; }

		/// <summary>
		/// Language map for competencyLabel
		/// </summary>
		[JsonProperty( "ceasn:competencyLabel" )]
		public LanguageMap CompetencyLabelLangMap { get; set; }

		/// <summary>
		/// Complexity Level
		/// The expected performance level of a learner or professional as defined by a competency.
		/// ceasn:ProficiencyScale NOT IMPLEMENTED Expecting Concept
		/// </summary>
		public List<string> ComplexityLevel { get; set; }

		/// <summary>
		/// Comprised Of
		/// This competency includes, comprehends or encompasses, in whole or in part, the meaning, nature or importance of the referenced competency.
		/// List of URIs (CTIDs recommended) for a competency framework or competency
		/// </summary>
		public List<string> ComprisedOf { get; set; }

		/// <summary>
		/// Concept Keyword
		/// A word or phrase used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// </summary>
		public List<string> ConceptKeyword { get; set; }

		/// <summary>
		/// Language map list for ConceptKeyword
		/// </summary>
		[JsonProperty( "ceasn:conceptKeyword" )]
		public LanguageMapList ConceptKeywordLangMap { get; set; }

		/// <summary>
		/// Concept Term
		/// A term drawn from a controlled vocabulary used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// List of URIs (CTIDs recommended) to a concept
		/// </summary>
		public List<string> ConceptTerm { get; set; }

		/// <summary>
		/// An entity primarily responsible for making this competency framework or competency.
		/// The creator property is used with non-canonical statements created by a third party.
		/// REQUIRED for stand alone published completencies.
		/// List of URIs (CTIDs recommended) to the creator
		/// ceasn:creator
		/// </summary>
		public List<string> Creator { get; set; }

		/// <summary>
		/// Cross-Subject Reference
		/// A relationship between this competency and a competency in a separate competency framework.
		/// List of URIs (CTIDs recommended) to competencies
		/// </summary>
		public List<string> CrossSubjectReference { get; set; }

		/// <summary>
		/// Date of creation of this competency framework or competency.
		/// xsd:date
		/// </summary>
		public string DateCreated { get; set; }

		/// <summary>
		/// The date on which this framework or competency was most recently modified in some way.
		/// xsd:dateTime
		/// </summary>
		public string DateModified { get; set; }

		/// <summary>
		/// Derived From
		/// A version of the entity being referenced that has been modified in meaning through editing, extension or refinement.
		/// List of URIs (CTIDs recommended) to a competency
		/// 2023-03-22 The datatype was changed to a list. The API will still handle a single string.
		/// </summary>
		public List<string> DerivedFrom { get; set; }

		/// <summary>
		/// Education Level Type
		/// A general statement describing the education or training context. Alternatively, a more specific statement of the location of the audience in terms of its progression through an education or training context.
		/// Best practice is to use terms from the http://purl.org/ctdl/terms/AudienceLevel concept scheme.
		/// List of URIs (CTIDs recommended) to concepts
		/// </summary>
		public List<string> EducationLevelType { get; set; }

		/// <summary>
		/// Exact Alignment
		/// The relevant concepts in this competency and the referenced competency are coextensive.
		/// List of URIs (CTIDs recommended) for a competency
		/// </summary>
		public List<string> ExactAlignment { get; set; }

		/// <summary>
		/// Has Child
		/// The referenced competency is lower in some arbitrary hierarchy than this competency.List of URIs for child competencies under this competency
		/// Provide either a CTID for a competency that is include in the Competencies property, or the full URI formatted like the following:
		/// "https://credentialengineregistry.org/resources/ce-b1e0eca2-7a19-49e9-8841-fa16ddf8396d"
		/// List of URIs (CTIDs recommended) for a competency
		/// NOTE: or just provide the CTIDs, and the system will format the proper URI for the current environment.
		/// NOTE: Not allowed when publishing standalone competencies
		/// </summary>
		public List<string> HasChild { get; set; }

		/// <summary>
		/// Concept in a ProgressionModel concept scheme
		/// URI
		/// </summary>
		public string HasProgressionLevel { get; set; }

		/// <summary>
		/// Identifier
		/// An alternative URI by which this competency framework or competency is identified.
		/// List of URIs
		/// </summary>
		public List<string> Identifier { get; set; }

		/// <summary>
		/// Competency deduced or arrive at by reasoning on the competency being described.
		/// List of URIs (CTIDs recommended) to competencies
		/// </summary>
		public List<string> InferredCompetency { get; set; }

		/// <summary>
		/// Is Child Of
		/// The referenced competency is higher in some arbitrary hierarchy than this competency.
		/// List of URIs (CTIDs recommended) to competencies.
		/// NOTE: Not allowed when publishing standalone competencies
		/// </summary>
		public List<string> IsChildOf { get; set; }

		/// <summary>
		/// Indicates that this competency is at the top of the framework.
		/// NOTE: Not allowed when publishing standalone competencies
		/// </summary>
		public string IsTopChildOf { get; set; }

		/// <summary>
		/// Competency framework that this competency is a part of.
		/// NOTE: Not allowed when publishing standalone competencies
		/// </summary>
		public string IsPartOf { get; set; }

		/// <summary>
		/// Collection to which this resource belongs.
		/// Only used where part of a Collection
		/// </summary>
		public List<string> IsMemberOf { get; set; }

		/// <summary>
		/// A related competency of which this competency is a version, edition, or adaptation.
		/// List of URIs (CTIDs recommended) for a competency
		/// </summary>
		public string IsVersionOf { get; set; }

		public List<string> Keyword { get; set; }

		/// <summary>
		/// Language map list for keyword
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList KeywordLangMap { get; set; }

		/// <summary>
		/// Body of information embodied either directly or indirectly in this resource.
		/// List of URIs (CTIDs recommended) for a competency
		/// ceasn:knowledgeEmbodied
		/// </summary>
		public List<string> KnowledgeEmbodied { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this competency framework.
		/// Value must be the URI to a license document (e.g., Creative Commons license or bespoke license).
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// An alphanumeric string indicating the relative position of a resource in an ordered list of resources such as "A", "B", or "a", "b", or "I", "II", or "1", "2".
		/// </summary>
		public string ListID { get; set; }

		/// <summary>
		/// The text string denoting the subject of the competency as designated by the promulgating agency.
		/// </summary>
		public List<string> LocalSubject { get; set; }

		/// <summary>
		/// Language map list for local subject
		/// </summary>
		[JsonProperty( "ceasn:localSubject" )]
		public LanguageMapList LocalSubjectLangMap { get; set; }

		/// <summary>
		/// Major overlap of relevant concepts between this competency and the referenced competency.
		/// List of URIs (CTIDs recommended) for a competency
		/// </summary>
		public List<string> MajorAlignment { get; set; }

		/// <summary>
		/// Minor overlap of relevant concepts between this competency and the referenced competency.
		/// List of URIs (CTIDs recommended) for a competency
		/// </summary>
		public List<string> MinorAlignment { get; set; }

		/// <summary>
		/// This competency covers all of the relevant concepts in the referenced competency as well as relevant concepts not found in the referenced competency.
		/// List of URIs (CTIDs recommended) for a competency
		/// </summary>
		public List<string> NarrowAlignment { get; set; }

		/// <summary>
		/// The referenced resource is a prerequisite to this resource.
		/// List of URIs (CTIDs recommended) for a competency
		/// </summary>
		public List<string> PrerequisiteAlignment { get; set; }

		/// <summary>
		/// Indicates whether correlators should or should not assign the competency during correlation.
		/// </summary>
		public bool? ShouldIndex { get; set; }

		/// <summary>
		///Ability to apply knowledge and use know-how to complete tasks and solve problems including types or categories of developed proficiency or dexterity in mental operations and physical processes is embodied either directly or indirectly in this resource.
		/// List of URIs (CTIDs recommended) for a competency
		/// ceasn:skillEmbodied
		/// </summary>
		public List<string> SkillEmbodied { get; set; }

		/// <summary>
		/// Specifically defined piece of work embodied either directly or indirectly in this resource.
		/// ceasn:taskEmbodied
		/// </summary>
		public List<string> TaskEmbodied { get; set; }

		/// <summary>
		/// An asserted measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		public string Weight { get; set; }

		/// <summary>
		/// Task related to this resource.
		/// List of URIs (CTIDs recommended) for a Task
		/// </summary>
		public List<string> HasTask { get; set; }

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
		public List<string> AlternativeOccupationType { get; set; }

		/// <summary>
		/// Language map list for AlternativeOccupationType
		/// </summary>
		public LanguageMapList AlternativeOccupationTypeLangMap { get; set; }

		/// <summary>
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// The API will validate and format the ONet codes as Occupations
		/// </summary>
		public List<string> ONET_Codes { get; set; }

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
		public List<string> AlternativeIndustryType { get; set; }

		/// <summary>
		/// Language map list for AlternativeIndustryType
		/// </summary>
		public LanguageMapList AlternativeIndustryTypeLangMap { get; set; }

		/// <summary>
		/// List of valid NAICS codes. These will be mapped to industry type
		/// See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> NaicsList { get; set; }

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
		/// Language map list for AlternativeInstructionalProgramType
		/// </summary>
		public LanguageMapList AlternativeInstructionalProgramType_Map { get; set; }

		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion

		/// <summary>
		/// The publication status of the of this resource.
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Type of condition in the physical work performance environment that entails risk exposures requiring mitigating processes;
		/// List of URIs (CTIDs recommended) for Concept
		/// </summary>
		public List<string> EnvironmentalHazardType { get; set; }

		/// <summary>
		/// The primary language used in or by this resource.
		/// Collections only
		/// </summary>
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Type of required or expected performance level for a resource; select from an existing enumeration of such types.
		/// List of URIs (CTIDs recommended) for Concept
		/// </summary>
		public List<string> PerformanceLevelType { get; set; }

		/// <summary>
		/// Type of physical activity required or expected in performance; select from an existing enumeration of such types.
		/// List of URIs (CTIDs recommended) for Concept
		/// </summary>
		public List<string> PhysicalCapabilityType { get; set; }

		/// <summary>
		/// This resource provides transfer value for the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		public List<string> ProvidesTransferValueFor { get; set; }

		/// <summary>
		/// This resource receives transfer value from the referenced Transfer Value Profile.
		/// Refer to the referenced Transfer Value Profile for more information. Other resources may be included for the full value.
		/// </summary>
		public List<string> ReceivesTransferValueFrom { get; set; }

		/// <summary>
		/// Type of required or expected sensory capability; select from an existing enumeration of such types.
		/// List of URIs (CTIDs recommended) for Concept
		/// </summary>
		public List<string> SensoryCapabilityType { get; set; }

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// List of URIs
		/// </summary>
		public List<string> SourceDocumentation { get; set; }

		/// <summary>
		/// Aspects of the referenced Competency Framework provide some justification that the resource being described is useful.
		/// List of URIs (CTIDs recommended) for a competency framework
		/// </summary>
		public List<string> SubstantiatingCompetencyFramework { get; set; }

		/// <summary>
		/// Aspects of the referenced Credential provide some justification that the resource being described is useful.
		/// List of URIs (CTIDs recommended) for a Credential
		/// </summary>
		public List<string> SubstantiatingCredential { get; set; }

		/// <summary>
		/// Aspects of the referenced Job provide some justification that the resource being described is useful.
		/// </summary>
		public List<string> SubstantiatingJob { get; set; }

		/// <summary>
		/// Aspects of the referenced Occupation provide some justification that the resource being described is useful.
		/// List of URIs (CTIDs recommended) for an Occupation
		/// </summary>
		public List<string> SubstantiatingOccupation { get; set; }

		/// <summary>
		/// Aspects of the referenced Organization provide some justification that the resource being described is useful.
		/// List of URIs (CTIDs recommended) for an Organization
		/// </summary>
		public List<string> SubstantiatingOrganization { get; set; }

		/// <summary>
		/// Aspects of the referenced resource provide some justification that the resource being described is useful.
		/// List of URIs (CTIDs recommended) for a
		/// </summary>
		public List<string> SubstantiatingResource { get; set; }

		/// <summary>
		/// Referenced Task attests to some level of achievement/mastery of the competency being described.
		/// List of URIs (CTIDs recommended) for a Task
		/// </summary>
		public List<string> SubstantiatingTask { get; set; }

		/// <summary>
		/// Referenced Workrole attests to some level of achievement/mastery of the competency being described.
		/// List of URIs (CTIDs recommended) for a Work role
		/// </summary>
		public List<string> SubstantiatingWorkrole { get; set; }

		/// <summary>
		/// Level of workforce demand for the resource.
		/// List of URIs (CTIDs recommended) for a WorkforceDemandAction
		/// </summary>
		public List<string> HasWorkforceDemand { get; set; }

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
