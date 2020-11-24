using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Specific activity, typically related to performing a function or achieving a goal.
	/// </summary>
	public class Task
	{

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID) by which the creator, owner or provider of a resource recognizes it in transactions with the external environment (e.g., in verifiable claims involving the resource).
		/// requires
		/// - CTID
		/// - NAME
		/// <see cref="https://credreg.net/ctdl/terms/ctid"/>
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name of this Task
		/// Required
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Task description 
		/// Required
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();


		/// <summary>
		/// AbilityEmbodied
		/// Enduring attributes of the individual that influence performance are embodied either directly or indirectly in this resource.
		/// URI to a competency
		/// ceasn:abilityEmbodied
		/// </summary>
		public List<string> AbilityEmbodied { get; set; } = new List<string>();

		/// <summary>
		/// Category or classification of this resource.
		/// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
		/// URI to a competency
		/// ceterms:classification
		/// </summary>
		public List<string> Classification { get; set; } = new List<string>();
		//public List<CredentialAlignmentObject> Classification { get; set; } = new List<CredentialAlignmentObject>();
		/// <summary>
		/// Comment
		/// Definition:	en-US: Supplemental text provided by the promulgating body that clarifies the nature, scope or use of this competency.
		/// ceasn:comment
		/// </summary>
		public List<string> Comment { get; set; } = new List<string>();
		public LanguageMapList Comment_map { get; set; } = new LanguageMapList();

		/// <summary>
		/// The referenced resource is lower in some arbitrary hierarchy than this resource.
		/// CTID for an existing Task
		/// ceasn:hasChild
		/// </summary>
		public List<string> HasChild { get; set; } = new List<string>();

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

		/// <summary>
		/// Body of information embodied either directly or indirectly in this resource.
		/// List of CTIDs for a competency
		/// ceasn:knowledgeEmbodied
		/// </summary>
		public List<string> KnowledgeEmbodied { get; set; } = new List<string>();

		/// <summary>
		/// An alphanumeric string found in the source framework indicating the relative position of a competency in an ordered list of competencies such as "A", "B", or "a", "b", or "I", "II", or "1", "2".
		/// </summary>
		public string ListID { get; set; }

		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }

		/// <summary>
		///Ability to apply knowledge and use know-how to complete tasks and solve problems including types or categories of developed proficiency or dexterity in mental operations and physical processes is embodied either directly or indirectly in this resource.
		/// List of CTIDs for a competency
		/// ceasn:skillEmbodied
		/// </summary>
		public List<string> SkillEmbodied { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// ceterms:versionIdentifier
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();

	}
}
