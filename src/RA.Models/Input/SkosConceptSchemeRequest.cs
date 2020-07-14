using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	public class SkosGraphConceptSchemeRequest : BaseRequest
	{
		public SkosGraphConceptSchemeRequest()
		{
			ConceptSchemeGraph = new SkosConceptSchemeGraph();
		}
		public string CTID { get; set; }

		public SkosConceptSchemeGraph ConceptSchemeGraph { get; set; }

	}
	public class SkosConceptSchemeGraph
	{
		/// <summary>
		/// Main graph object
		/// </summary>
		[JsonProperty( "@graph" )]
		public object Graph { get; set; }

		[JsonProperty( "@context" )]
		public string Context { get; set; }
	}

	public class SkosConceptSchemeRequest : BaseRequest
	{
		public SkosConceptSchemeRequest()
		{
			ConceptScheme = new SkosConceptScheme();
		}
		public string Ctid { get; set; }

		//separate ConceptScheme and concepts
		public SkosConceptScheme ConceptScheme { get; set; } = new SkosConceptScheme();

		public List<SkosConcept> Concepts { get; set; } = new List<SkosConcept>();
	}


	public class SkosConceptScheme
	{
		[JsonProperty( "@context" )]
		public string Context { get; set; }

		//actually not needed, as must be "@type": "ConceptScheme"
		//[JsonProperty( "@type" )]
		//public string Type { get; set; }

		/// <summary>
		/// URI of concept scheme
		/// OR, more likely be the namespace:Name
		/// </summary>
		[JsonProperty( "@id" )]
		public string Id { get; set; }


		/// <summary>
		/// CTID - identifier for Concept Scheme. 
		/// This will have to be extracted from the id
		/// </summary>
		public string Ctid { get; set; }

		//public List<LanguageItem> ChangeNote { get; set; } = new List<LanguageItem>();
		public object ChangeNote { get; set; } 

		[JsonProperty( "ceasn:conceptKeyword" )]
		public List<LanguageItem> ConceptKeyword { get; set; } = new List<LanguageItem>();

		[JsonProperty( "ceasn:creator" )]
		public string Creator { get; set; }

		[JsonProperty( "dcterms:dateCopyrighted" )]
		public string DateCopyrighted { get; set; }

		[JsonProperty( "schema:dateCreated" )]
		public string DateCreated { get; set; }

		[JsonProperty( "schema:dateModified" )]
		public string DateModified { get; set; }

		/// <summary>
		/// Concept Scheme description 
		/// </summary>
		[JsonProperty( "dct:description" )]
		public LanguageMap Description { get; set; } = new LanguageMap();
		//public List<LanguageItem> Description { get; set; } = new List<LanguageItem>();

		[JsonProperty( "skos:historyNote" )]
		public List<LanguageItem> HistoryNote { get; set; } = new List<LanguageItem>();

		[JsonProperty( "dcterms:language" )]
		public string Language { get; set; }

		[JsonProperty( "dcterms:license" )]
		public string License { get; set; }

		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		[JsonProperty( "ceasn:publisherName" )]
		public string PublisherName { get; set; }

		[JsonProperty( "dcterms:publisher" )]
		public string Publisher { get; set; }

		[JsonProperty( "dcterms:rights" )]
		public List<LanguageItem> Rights { get; set; } = new List<LanguageItem>();

		[JsonProperty( "dcterms:rightsHolder" )]
		public string RightsHolder { get; set; }

		[JsonProperty( "rdfs:label" )]
		public LanguageMap Label { get; set; } = new LanguageMap();

		[JsonProperty( "dcterms:source" )]
		public string Source { get; set; }

		/// <summary>
		/// Name of the Concept Scheme
		/// </summary>
		[JsonProperty( "dcterms:title" )]
		public List<LanguageItem> Title { get; set; } = new List<LanguageItem>();

		[JsonProperty( "skos:hasTopConcept" )]
		public List<string> HasTopConcept { get; set; } = new List<string>();

		//in export, not sure where in interface
		//public List<string> Relation { get; set; } = new List<string>();
	}

	public class SkosConcept
	{
		//[JsonProperty( "@type" )]
		//public string Type { get; set; }

		[JsonProperty( "@context" )]
		public string Context { get; set; }


		//possibly will be the namespace:Property - could use for altLabel
		[JsonProperty( "@id" )]
		public string Id { get; set; }

		/// <summary>
		/// CTID - identifier for concept. 
		/// Format: ce-UUID (lowercase)
		/// example: ce-a044dbd5-12ec-4747-97bd-a8311eb0a042
		/// </summary>
		public string CTID { get; set; }

		[JsonProperty( "skos:altLabel" )]
		public List<LanguageItem> AltLabel { get; set; } = new List<LanguageItem>();


		[JsonProperty( "skos:changeNote" )]
		public List<LanguageItem> ChangeNote { get; set; } = new List<LanguageItem>();

		[JsonProperty( "dcterms:dateCreated" )]
		public string DateCreated { get; set; }

		/// <summary>
		///Last modified date for concept
		/// </summary>
		[JsonProperty( "schema:dateModified" )]
		public string DateModified { get; set; }

		/// <summary>
		/// Concept description 
		/// </summary>
		[JsonProperty( "skos:definition" )]
		public LanguageMap Definition { get; set; } = new LanguageMap();
		//public List<LanguageItem> Definition { get; set; } = new List<LanguageItem>();

		[JsonProperty( "skos:hiddenLabel" )]
		public List<LanguageItem> HiddenLabel { get; set; } = new List<LanguageItem>();

		[JsonProperty( "skos:inScheme" )]
		public string InScheme { get; set; }

		[JsonProperty( "skos:note" )]
		public List<LanguageItem> Note { get; set; } = new List<LanguageItem>();

		/// <summary>
		/// Concept name - required
		/// </summary>
		[JsonProperty( "skos:prefLabel" )]
		public LanguageMap PrefLabel { get; set; } = new LanguageMap();
		//public List<LanguageItem> PrefLabel { get; set; } = new List<LanguageItem>();

		/// <summary>
		/// URI to the concept scheme
		/// </summary>
		[JsonProperty( "skos:topConceptOf" )]
		public string TopConceptOf { get; set; }

		[JsonProperty( PropertyName = "skos:inLanguage" )]
		public List<string> InLanguage { get; set; }

		/*missing:
		 * all connections
		 * 
		 * 
		 * 
		 * 
		 */
		[JsonProperty( PropertyName = "skos:broadMatch" )]
		public List<string> BroadMatch { get; set; }
		[JsonProperty( PropertyName = "skos:closeMatch" )]
		public List<string> CloseMatch { get; set; }

		[JsonProperty( PropertyName = "skos:exactMatch" )]
		public List<string> ExactMatch { get; set; }

		[JsonProperty( PropertyName = "skos:narrower" )]
		public List<string> Narrower { get; set; }

		[JsonProperty( PropertyName = "skos:NarrowMatch" )]
		public List<string> NarrowMatch { get; set; }
		[JsonProperty( PropertyName = "skos:relatedMatch" )]
		public List<string> RelatedMatch { get; set; }

	}
}
