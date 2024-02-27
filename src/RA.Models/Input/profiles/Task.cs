using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Specific activity, typically related to performing a function or achieving a goal.
	/// NOTES:
	/// IsMemberOf - this is an inverse property. It would not be published with a Task.
	/// </summary>
	public class Task : BaseEmploymentToWorkObject
	{
		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "Task";

		#region Required
		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID) by which the creator, owner or provider of a resource recognizes it in transactions with the external environment (e.g., in verifiable claims involving the resource).
		/// Required
		/// - CTID
		/// - NAME
		/// <see cref="https://credreg.net/ctdl/terms/ctid"/>
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Profile Description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Agent making a statement based on fact or belief.
		/// Required
		/// Single is more likely
		/// </summary>
		public List<OrganizationReference> AssertedBy { get; set; } = new List<OrganizationReference>();
		#endregion


		/// <summary>
		/// Name of this Task
		/// NOT Required
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();


		/// <summary>
		/// AbilityEmbodied
		/// Enduring attributes of the individual that influence performance are embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:abilityEmbodied
		/// </summary>
		public List<string> AbilityEmbodied { get; set; } = new List<string>();

		/// <summary>
		/// Category or classification of this resource.
		/// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
		/// URI to a concept(based on the ONet work activities example)
		/// ceterms:classification
		/// </summary>
		public List<string> Classification { get; set; } = new List<string>();

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies an item and supports its discovery and use.
		/// ceterms:codedNotation
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Comment
		/// Definition:	en-US: Supplemental text provided by the promulgating body that clarifies the nature, scope or use of this competency.
		/// ceasn:comment
		/// </summary>
		public List<string> Comment { get; set; } = new List<string>();
		public LanguageMapList Comment_map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Environmental Hazard Type
		/// Type of condition in the physical work performance environment that entails risk exposures requiring mitigating processes; 
		/// select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> EnvironmentalHazardType { get; set; } = new List<string>();

		/// <summary>
		/// Type of required or expected human performance level; select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> PerformanceLevelType { get; set; } = new List<string>();

		/// <summary>
		/// Type of physical activity required or expected in performance; select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> PhysicalCapabilityType { get; set; } = new List<string>();

		/// <summary>
		/// Type of required or expected sensory capability; select from an existing enumeration of such types.
		/// skos:Concept
		/// Blank nodes!
		/// </summary>
		public List<string> SensoryCapabilityType { get; set; } = new List<string>();

		/// <summary>
		/// The referenced resource is lower in some arbitrary hierarchy than this resource.
		/// CTID for an existing Task
		/// ceasn:hasChild
		/// </summary>
		public List<string> HasChild { get; set; } = new List<string>();

		/// <summary>
		/// Rubric related to this resource.
		/// <see cref="https://credreg.net/ctdl/terms/hasRubric"/>
		/// ceterms:hasRubric
		/// </summary>
		public List<string> HasRubric { get; set; } = new List<string>();

		/// <summary>
		/// The referenced resource is higher in some arbitrary hierarchy than this resource
		/// CTID for an existing Task
		/// <see cref="https://credreg.net/ctdl/terms/hasTask"/>
		/// ceasn:isChildOf
		/// </summary>
		public List<string> IsChildOf { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		///// <summary>
		///// Is Member Of
		///// Collection to which this resource belongs.
		///// Inverse property that cannot be used here. 
		///// </summary>
		//public List<string> IsMemberOf { get; set; } = new List<string>();

		/// <summary>
		/// Body of information embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:knowledgeEmbodied
		/// </summary>
		public List<string> KnowledgeEmbodied { get; set; } = new List<string>();

		/// <summary>
		/// An alphanumeric string found in the source framework indicating the relative position of a competency in an ordered list of competencies such as "A", "B", or "a", "b", or "I", "II", or "1", "2".
		/// </summary>
		public string ListID { get; set; }

		/// <summary>
		/// Organization(s) that offer this resource
		/// TBD
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }

		/// <summary>
		///Ability to apply knowledge and use know-how to complete tasks and solve problems including types or categories of developed proficiency or dexterity in mental operations and physical processes is embodied either directly or indirectly in this resource.
		/// CTID/URI to any of:
		/// ceasn:Competency ceterms:Job ceterms:Occupation ceterms:Task ceterms:WorkRole
		/// ceasn:skillEmbodied
		/// </summary>
		public List<string> SkillEmbodied { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// ceterms:versionIdentifier
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();


		/// <summary>
		/// Job related to this resource.
		/// CTID for an existing Job
		/// <see cref="https://credreg.net/ctdl/terms/hasJob"/>
		/// ceterms:hasJob
		/// </summary>
		public List<string> HasJob { get; set; } = new List<string>();

		/// <summary>
		/// Work Role related to this resource.
		/// CTID for an existing WorkRole
		/// ceterms:hasWorkRole
		/// </summary>
		public List<string> HasWorkRole { get; set; } = new List<string>();
		/// <summary>
		/// Occupation related to this resource.
		/// CTID for an existing Ocuupation
		/// ceterms:hasOccupation
		/// </summary>
		public List<string> HasOccupation { get; set; } = new List<string>();

	}
}
