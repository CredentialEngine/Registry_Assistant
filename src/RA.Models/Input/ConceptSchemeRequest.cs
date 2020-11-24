using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{


	public class ConceptSchemeRequest : BaseRequest
	{
		public ConceptSchemeRequest()
		{
			ConceptScheme = new ConceptScheme();
		}

		/// <summary>
		/// Generate HasTopChild
		/// if true, the HasTopChild property is not included in the input document. The HasTopChild property in the JSON document will be generated from the Concept list.
		/// Should only be used where the structure is flat. That is there are no concepts have child concepts. SO that is: all concepts are top childs.
		/// Note: in some cases IsTopChild was provided and not HasTopChild. In this case: GenerateHasTopChild=true, and GenerateIsTopChild=false
		/// </summary>
		public bool GenerateHasTopChild { get; set; } = false;
		/// <summary>
		/// Generate HasTopChild where the concept has property of: TopConceptOf
		/// </summary>
		public bool GenerateHasTopChildFromIsTopChild { get; set; } = false;
		/// <summary>
		/// Generate IsTopChild
		/// if true, the IsTopChild property must not be included in the input document and the IsTopChild property in the JSON document will be generated for each concept in the list.
		/// Must only be used where the structure is flat. That is there are no concepts have child concepts.
		/// </summary>
		public bool GenerateIsTopChild { get; set; } = false;

		public ConceptScheme ConceptScheme { get; set; } = new ConceptScheme();

		public List<Concept> Concepts { get; set; } = new List<Concept>();
	}

	public class ConceptSchemeGraphRequest : GraphContentRequest
	{
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
		//public GraphInput GraphInput { get; set; }

	}
	/// <summary>
	/// Concept Scheme
	/// A controlled vocabulary.
	/// <seealso cref="https://credreg.net/ctdlasn/terms/ConceptScheme"/>
	/// </summary>
	public class ConceptScheme 
	{
		/// <summary>
		/// CTID - identifier for Concept Scheme. 
		/// REQUIRED
		/// </summary>
		public string Ctid { get; set; }

		public List<string> AltIdentifier { get; set; } = new List<string>();

		//20-08-05 no longer on credReg.net
		//public List<string> ChangeNote { get; set; } = new List<string>();
		//public LanguageMapList ChangeNote_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Concept Keyword
		/// A word or phrase used by the promulgating agency to refine and differentiate individual resources contextually.
		/// </summary>
		public List<string> ConceptKeyword { get; set; } = new List<string>();
		public LanguageMapList ConceptKeyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Concept Term
		/// A term drawn from a controlled vocabulary used by the promulgating agency to refine and differentiate individual resources contextually.
		/// </summary>
		public List<string> ConceptTerm { get; set; } = new List<string>();

		/// <summary>
		/// Creator
		/// An entity primarily responsible for making this resource.
		/// </summary>
		public OrganizationReference Creator { get; set; } = new OrganizationReference();

		/// <summary>
		/// Date Copyrighted
		/// Date of a statement of copyright for this resource - the year only.
		/// </summary>
		public string DateCopyrighted { get; set; }

		/// <summary>
		/// Date Created
		/// Date of creation of this resource.
		/// </summary>
		public string DateCreated { get; set; }

		public string DateModified { get; set; }

		/// <summary>
		/// Concept Scheme description
		/// REQUIRED
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();


		/// <summary>
		/// Top Concepts
		/// Concept of the scheme at the top of a hierarchy of narrower concepts.
		/// List of CTIDs (recommended) or actual registry URIs
		/// </summary>
		public List<string> HasTopConcept { get; set; } = new List<string>();

		//public List<string> HistoryNote { get; set; } = new List<string>();
		/// <summary>
		/// Concept Scheme history? not included anymore? 
		/// </summary>
		//public string HistoryNote { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		//public LanguageMap HistoryNote_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Language. Required unless defaultLanguage is provided
		/// </summary>
		public List<string> InLanguage { get; set; } = new List<string>();

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// Name of the Concept Scheme - required
		/// REQUIRED
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// The publication status of the of this resource.
		/// <seealso cref="https://credreg.net/ctdlasn/terms/PublicationStatus"/>
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Name of an agent responsible for making this resource available.
		/// </summary>
		public string PublisherName { get; set; }
		public LanguageMap PublisherName_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// An agent responsible for making this resource available.
		/// </summary>
		//public string Publisher { get; set; }
		public OrganizationReference Publisher { get; set; } = new OrganizationReference();

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		public string Rights { get; set; }
		public LanguageMap Rights_Map { get; set; } = new LanguageMap();

		/// <summary>
		///  An agent owning or managing rights over this resource.
		///  Use OrganizationReference for flexibility
		/// </summary>
		public OrganizationReference RightsHolder { get; set; } = new OrganizationReference();


		/// <summary>
		/// Original Source of concept scheme
		/// REQUIRED
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		///  Indicates the entity that supersedes this entity.
		/// </summary>
		public string SupersededBy { get; set; }
	}

	public class Concept  
	{
		[JsonProperty( "@context" )]
		public string Context { get; set; }


		//[JsonProperty( "@id" )]
		//public string Id { get; set; }

		/// <summary>
		/// Ctid - identifier for concept. 
		/// Format: ce-UUID (lowercase)
		/// example: ce-a044dbd5-12ec-4747-97bd-a8311eb0a042
		/// </summary>
		public string Ctid { get; set; }

		public List<string> AltLabel { get; set; } = new List<string>();

		public LanguageMapList AltLabel_Map { get; set; } = new LanguageMapList();
		public List<string> ChangeNote { get; set; } = new List<string>();
		public LanguageMapList ChangeNote_Map { get; set; } = new LanguageMapList();
		public string DateCreated { get; set; }

		/// <summary>
		///Last modified date for concept
		/// </summary>
		public string DateModified { get; set; }


		/// <summary>
		/// Concetpt description 
		/// </summary>
		public string Definition { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Definition_Map { get; set; } = new LanguageMap();

		public List<string> HiddenLabel { get; set; } = new List<string>();
		public LanguageMapList HiddenLabel_Map { get; set; } = new LanguageMapList();
		public string InScheme { get; set; }

		public string Notation { get; set; }

		public List<string> Note { get; set; } = new List<string>();
		public LanguageMapList Note_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// Concept 
		/// Required
		/// </summary>
		public string PrefLabel { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap PrefLabel_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Indicates the entity that supersedes this entity.
		/// </summary>
		public string SupersededBy { get; set; }

		/// <summary>
		/// URI to the concept scheme
		/// </summary>
		public string TopConceptOf { get; set; }

		public string Language { get; set; }

		public string Broader { get; set; }
		public List<string> BroadMatch { get; set; } = new List<string>();
		public List<string> CloseMatch { get; set; } = new List<string>();

		public List<string> ExactMatch { get; set; } = new List<string>();

		public List<string> Narrower { get; set; } = new List<string>();

		public List<string> NarrowMatch { get; set; } = new List<string>();
		public List<string> Related{ get; set; } = new List<string>();
		//public List<string> RelatedMatch { get; set; } = new List<string>();

		public List<string> Comment { get; set; } = new List<string>();
		public LanguageMapList Comment_Map { get; set; } = new LanguageMapList();

		public string CodeNEC { get; set; }
		public string LegacyCodeNEC { get; set; }
		public List<string> SourceCareerFieldCode { get; set; } = new List<string>();
	}

	//public class MilitaryConcept : Concept
	//{
	//	public string CodeNEC { get; set; }
	//	public string LegacyCodeNEC { get; set; }
	//	public string SourceCareerFieldCode { get; set; }
	//}
}
