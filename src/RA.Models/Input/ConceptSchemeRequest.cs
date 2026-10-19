using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{

	/// <summary>
	/// Concept Scheme Publishing Request
	/// </summary>
	public class ConceptSchemeRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public ConceptSchemeRequest()
		{
			ConceptScheme = new ConceptScheme();
		}

		/// <summary>
		/// Generate HasTopConcept
		/// if true, the HasTopConcept property is not included in the input document. The HasTopConcept property in the JSON document will be generated from the Concept list.
		/// Should only be used where the structure is flat. That is there are no concepts that have child concepts. SO that is: all concepts are top childs.
		/// Note: in some cases TopConceptOf was provided and not HasTopConcept. In this case:
		/// - GenerateHasTopConcept=true, and
		/// - GenerateTopConceptOf=false
		/// </summary>
		public bool GenerateHasTopConcept { get; set; } = false;

		/// <summary>
		/// Generate HasTopConcept where the concept has property of: TopConceptOf
		/// </summary>
		public bool GenerateHasTopConceptFromTopConceptOf { get; set; } = false;

		/// <summary>
		/// Generate TopConceptOf
		/// if true, the TopConceptOf property must not be included in the input document and the TopConceptOf property in the JSON document will be generated for each concept in the list.
		/// Must only be used where the structure is flat. That is there are no concepts have child concepts.
		/// </summary>
		public bool GenerateTopConceptOf { get; set; } = false;

		/// <summary>
		/// ConceptScheme
		/// Required
		/// </summary>
		public ConceptScheme ConceptScheme { get; set; }

		/// <summary>
		/// Concepts for ConceptScheme
		/// Required
		/// </summary>
		public List<Concept> Concepts { get; set; } = new List<Concept>();
	}

	/// <summary>
	/// Concept Scheme Publishing Request from a graph
	/// </summary>
	public class ConceptSchemeGraphRequest : GraphContentRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConceptSchemeGraphRequest()
		{
			GraphInput = new GraphInput();
		}

		/// <summary>
		/// Generate HasTopChild
		/// if true, the HasTopChild property is not included in the input document. The HasTopChild property in the JSON document will be generated from the Concept list.
		/// Should only be used where the structure is flat. That is there are no concepts have child concepts. SO that is: all concepts are top childs.
		/// Note: in some cases IsTopChild was provided and not HasTopChild. In this case: GenerateHasTopChild=true, and GenerateIsTopChild=false
		/// </summary>
		public bool GenerateHasTopChild { get; set; } = false;

		/// <summary>
		/// Generate HasTopChild where the child has property of: Top'Child'Of
		/// </summary>
		public bool GenerateHasTopChildFromIsTopChild { get; set; } = false;

		/// <summary>
		/// Generate IsTopChild
		/// if true, the IsTopChild property must not be included in the input document and the IsTopChild property in the JSON document will be generated for each concept in the list.
		/// Must only be used where the structure is flat. That is there are no concepts have child concepts.
		/// </summary>
		public bool GenerateIsTopChild { get; set; } = false;

		/// <summary>
		/// GenerateInScheme
		/// If true, generate the inScheme property where not provided for concepts
		/// </summary>
		public bool GenerateInScheme { get; set; } = false;
		// public GraphInput GraphInput { get; set; }
	}

	/// <summary>
	/// Concept Scheme
	/// A controlled vocabulary.
	/// <seealso cref="https://credreg.net/ctdlasn/terms/ConceptScheme"/>
	/// </summary>
	public class ConceptScheme : BaseRequestHelper
	{
		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "skos:ConceptScheme";

		#region Required properties
		/// <summary>
		/// CTID - identifier for Concept Scheme.
		/// REQUIRED
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name of the Concept Scheme - required
		/// REQUIRED
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceasn:name" )]
		public LanguageMap NameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Concept Scheme description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceasn:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// An agent responsible for making this resource available.
		/// required
		/// </summary>
		public List<OrganizationReference> Publisher { get; set; } = new List<OrganizationReference>();


		/// <summary>
		/// Language. Required unless defaultLanguage is provided
		/// </summary>
		public List<string> InLanguage { get; set; } = new List<string>();

		#endregion

		/// <summary>
		/// Approximate date of creation of this resource.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		public string ApproximateDateCreated { get; set; }

		/// <summary>
		/// Approximate date on which this resource was most recently modified in some way.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		public string ApproximateDateModified { get; set; }

		/// <summary>
		/// Creator
		/// An entity primarily responsible for making this resource.
		/// OPTIONAL
		/// </summary>
		public List<OrganizationReference> Creator { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Concept Keyword
		/// A word or phrase used by the promulgating agency to refine and differentiate individual resources contextually.
		/// </summary>
		public List<string> ConceptKeyword { get; set; } = new List<string>();

		/// <summary>
		/// Concept Keywords via LanguageMapList
		/// </summary>
		[JsonProperty( PropertyName = "ceasn:conceptKeyword" )]
		public LanguageMapList ConceptKeywordLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// Concept Term
		/// A term drawn from a controlled vocabulary used by the promulgating agency to refine and differentiate individual resources contextually.
		/// skos:Concept
		/// </summary>
		public List<string> ConceptTerm { get; set; } = new List<string>();

		/// <summary>
		/// Date Copyrighted
		/// Date of a statement of copyright for this resource. Typically the year only, but a full date is allowed.
		/// xsd:string
		/// </summary>
		public string DateCopyrighted { get; set; }

		/// <summary>
		/// Date Created
		/// Date of creation of this resource.
		/// xsd:date
		/// </summary>
		public string DateCreated { get; set; }

		/// <summary>
		/// The date on which this resource was most recently modified in some way.
		/// xsd:dateTime
		/// </summary>
		public string DateModified { get; set; }

		/// <summary>
		/// Top Concepts
		/// Concept of the scheme at the top of a hierarchy of narrower concepts.
		/// List of CTIDs (recommended) or actual registry URIs
		/// </summary>
		public List<string> HasTopConcept { get; set; } = new List<string>();

		[JsonProperty( PropertyName = "skos:hasTopConcept" )]
		public List<string> SkosHasTopConcept { get; set; } = new List<string>();

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// xsd:anyURI
		/// </summary>
		public string License { get; set; }


		/// <summary>
		/// The publication status of the of this resource.
		/// <seealso cref="https://credreg.net/ctdlasn/terms/PublicationStatus"/>
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Name of an agent responsible for making this resource available.
		/// </summary>
		public List<string> PublisherName { get; set; } = new List<string>();

		[JsonProperty( PropertyName = "ceasn:publisherName" )]
		public LanguageMapList PublisherNameLangMap { get; set; } = new LanguageMapList();


		/// <summary>
		/// Information about rights held in and over this resource.
		/// rdf:langString
		/// </summary>
		public string Rights { get; set; }

		[JsonProperty( PropertyName = "ceasn:rights" )]
		public LanguageMap RightsLangMap { get; set; } = new LanguageMap();

		/// <summary>
		///  An agent owning or managing rights over this resource.
		///  Use OrganizationReference for flexibility
		/// </summary>
		public List<OrganizationReference> RightsHolder { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Original Source of concept scheme
		/// xsd:anyURI
		/// </summary>
		public List<string> Source { get; set; }

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// URI
		/// </summary>
		public List<string> SourceDocumentation { get; set; }

		/// <summary>
		///  Indicates the entity that supersedes this entity.
		///  Must exist.
		///  xsd:anyURI
		/// </summary>
		public List<string> SupersededBy { get; set; }
	}

	/// <summary>
	/// Concept class
	/// </summary>
	public class Concept : BasePrimaryResource
	{
		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; } = "skos:Concept";

		/// <summary>
		/// CTID - identifier for concept.
		/// Format: ce-UUID (lowercase)
		/// Required
		/// example: ce-a044dbd5-12ec-4747-97bd-a8311eb0a042
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Concept
		/// Required
		/// </summary>
		public string PrefLabel { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "skos:prefLabel" )]
		public LanguageMap PrefLabelMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Alternative Label
		/// Non-preferred label for the concept used to relate a concept synonym to the preferred label.
		/// </summary>
		public List<string> AltLabel { get; set; } = new List<string>();

		/// <summary>
		///  Alternative Label language map
		/// </summary>
		[JsonProperty( PropertyName = "skos:altLabel" )]
		public LanguageMapList AltLabel_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Concept that is broader in some way than this concept.
		/// Concept URL (CTID)
		/// </summary>
		public string Broader { get; set; }

		/// <summary>
		/// Assertion indicates that the referenced concept is broader in some way than this concept.
		/// List of Concept URLs(CTIDs)
		/// </summary>
		public List<string> BroadMatch { get; set; } = new List<string>();

		/// <summary>
		/// Text describing a significant change to the concept.
		/// </summary>
		public List<string> ChangeNote { get; set; } = new List<string>();

		[JsonProperty( PropertyName = "skos:changeNote" )]
		public LanguageMapList ChangeNote_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Assertion indicates that two concepts are sufficiently similar that they can be used interchangeably.
		/// List of Concept URLs(CTIDs)
		/// </summary>
		public List<string> CloseMatch { get; set; } = new List<string>();

		/// <summary>
		/// Supplies a complete explanation of the intended meaning of a concept.
		/// </summary>
		public string Definition { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "skos:definition" )]
		public LanguageMap Definition_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Indicates semantic similarity denoting an even higher degree of closeness than skos:closeMatch.
		/// List of Concept URLs(CTIDs)
		/// </summary>
		public List<string> ExactMatch { get; set; } = new List<string>();

		/// <summary>
		///Label not intended for public presentation but to assist applications in disambiguating searcher intent - e.g., hidden labels can be used for common misspelling or a colloquial expression.
		/// </summary>
		public List<string> HiddenLabel { get; set; } = new List<string>();

		public LanguageMapList HiddenLabelLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		///	Concept scheme to which this concept belongs.
		/// </summary>
		public string InScheme { get; set; }

		/// <summary>
		/// Concept that is narrower in some way than this concept.
		/// List of Concept URLs(CTIDs)
		/// </summary>
		public List<string> Narrower { get; set; } = new List<string>();

		/// <summary>
		/// Assertion indicates that the referenced concept is narrower in some way than this concept.
		/// List of Concept URLs(CTIDs)
		/// </summary>
		public List<string> NarrowMatch { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric notation or ID code as defined by the promulgating body to identify this resource.
		/// </summary>
		public string Notation { get; set; }

		/// <summary>
		/// Annotations to the concept for purposes of general documentation.
		/// </summary>
		public List<string> Note { get; set; } = new List<string>();

		/// <summary>
		/// Annotations to the concept for purposes of general documentation - language map list.
		/// </summary>
		public LanguageMapList Note_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Resource that logically comes after this resource.
		/// This property indicates a simple or suggested ordering of resources; if a required ordering is intended, use ceterms:prerequisite instead.
		/// Provide the CTID or the full URI for the target environment.
		/// ceterms:ComponentCondition
		/// </summary>
		public List<string> Precedes { get; set; } = new List<string>();

		/// <summary>
		/// Component is preceded by the referenced components
		/// </summary>
		public List<string> PrecededBy { get; set; }

		/// <summary>
		/// Assertion indicating an associative, non-hierarchical relationship between the two concepts where neither is broader nor narrower than the other.
		/// List of Concept URLs(CTIDs)
		/// </summary>
		public List<string> Related { get; set; } = new List<string>();

		/// <summary>
		/// Human-readable information resource other than a competency framework from which this competency was generated or derived by humans or machines.
		/// URI
		/// </summary>
		public List<string> SourceDocumentation { get; set; }

		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Indicates the entity that supersedes this entity.
		/// URL
		/// </summary>
		public List<string> SupersededBy { get; set; }

		/// <summary>
		/// URI to the concept scheme
		/// </summary>
		public string TopConceptOf { get; set; }
	}
}
