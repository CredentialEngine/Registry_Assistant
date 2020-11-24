using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Graph Request- the input data is formatted as a list of objects under a graph property.
	/// Caller must indicate HasLanguageMaps. If false, text data will have a type of string. If true, text data/lists will be has types of LanguageMap/LanguageMapList
	/// </summary>
	public class GraphContentRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public GraphContentRequest()
		{
			GraphInput = new GraphInput();
			HasLanguageMaps = true;
		}

		//*** shouldn't use this!!!	
		public string CTID { get; set; } = "";

		/// <summary>
		/// Generate HasTopChild
		/// if true, the HasTopChild property is not included in the input document. The HasTopChild property in the JSON document will be generated from the Concept list.
		/// Should only be used where the structure is flat. That is there are no concepts have child concepts. SO that is: all concepts are top childs.
		/// Note: in some cases IsTopChild was provided and not HasTopChild. In this case: GenerateHasTopChild=true, and GenerateIsTopChild=false
		/// </summary>
		//public bool GenerateHasTopChild { get; set; } = false;
		/// <summary>
		/// Generate HasTopChild where the child has property of: Top'Child'Of
		/// </summary>
		//public bool GenerateHasTopChildFromIsTopChild { get; set; } = false;
		/// <summary>
		/// Generate IsTopChild
		/// if true, the IsTopChild property must not be included in the input document and the IsTopChild property in the JSON document will be generated for each concept in the list.
		/// Must only be used where the structure is flat. That is there are no concepts have child concepts.
		/// </summary>
		//public bool GenerateIsTopChild { get; set; } = false;
		/// <summary>
		/// GenerateInScheme
		/// If true, generate the inScheme property where not provided for concepts
		/// </summary>
		//public bool GenerateInScheme{ get; set; } = false;

		/// <summary>
		/// HasLanguageMaps
		/// If false, will format input using the Plain classes
		/// </summary>
		public bool HasLanguageMaps { get; set; }

		/// <summary>
		/// Main input data as a graph
		/// </summary>
		public GraphInput GraphInput { get; set; }

	}
	/// <summary>
	/// The plain graph doesn't include language maps
	/// </summary>
	public class GraphInput
	{

		[JsonProperty( "@context" )]
		public string Context { get; set; }

		[JsonProperty( "@id" )]
		public string Id { get; set; }

		/// <summary>
		/// Main graph object
		/// </summary>
		[JsonProperty( "@graph" )]
		public object Graph { get; set; }
	}
}
