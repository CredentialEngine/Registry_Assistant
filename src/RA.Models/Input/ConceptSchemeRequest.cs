﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{


	public class ConceptSchemeRequest : BaseRequest
	{
		public ConceptSchemeRequest()
		{
			ConceptScheme = new ConceptScheme();
		}
		//public string CTID { get; set; }

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

	

	public class ConceptScheme 
	{

		/// <summary>
		/// CTID - identifier for Concept Scheme. 
		/// This will have to be extracted from the id
		/// </summary>
		public string Ctid { get; set; }

		public List<string> altIdentifier { get; set; } = new List<string>();

		public List<string> ChangeNote { get; set; } = new List<string>();
		public LanguageMapList ChangeNote_Map { get; set; } = new LanguageMapList();
		public List<string> ConceptKeyword { get; set; } = new List<string>();
		public LanguageMapList ConceptKeyword_Map { get; set; } = new LanguageMapList();

		public List<string> ConceptTerm { get; set; } = new List<string>();
		public string Creator { get; set; }

		public string DateCopyrighted { get; set; }

		public string DateCreated { get; set; }

		public string DateModified { get; set; }

		/// <summary>
		/// Concept Scheme description - required
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		//public List<string> HistoryNote { get; set; } = new List<string>();
		/// <summary>
		/// Concept Scheme description 
		/// </summary>
		public string HistoryNote { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap HistoryNote_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Language. Required unless defaultLanguage is provided
		/// </summary>
		public List<string> InLanguage { get; set; } = new List<string>();

		public string License { get; set; }

		public string PublicationStatusType { get; set; }

		public string PublisherName { get; set; }
		public LanguageMap PublisherName_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// URI
		/// </summary>
		public string Publisher { get; set; }

		public string Rights { get; set; }
		public LanguageMap Rights_Map { get; set; } = new LanguageMap();

		public string RightsHolder { get; set; }

		/// <summary>
		/// Name of the Concept Scheme - required
		/// Required
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Source of concept scheme - required
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Top Concepts - list of CTIDs or actual registry URIs
		/// </summary>
		public List<string> HasTopConcept { get; set; } = new List<string>();
	}

	public class Concept  
	{
		[JsonProperty( "@context" )]
		public string Context { get; set; }


		//[JsonProperty( "@id" )]
		//public string Id { get; set; }

		/// <summary>
		/// CTID - identifier for concept. 
		/// Format: ce-UUID (lowercase)
		/// example: ce-a044dbd5-12ec-4747-97bd-a8311eb0a042
		/// </summary>
		public string CTID { get; set; }

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