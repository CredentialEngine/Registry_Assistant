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

		//*** shouldn't use this!!!	- just for tracing/output files
		public string CTID { get; set; } = "";

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
	/// Graph Class
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

	public class GraphNode
	{

		[JsonProperty( "@id" )]
		public string Id { get; set; }

		[JsonProperty( "@type" )]
		public string Type { get; set; }


		/// <summary>
		/// CTID - present for toplevel node, but not always for child nodes
		/// </summary>
		[JsonProperty( "ceterms:ctid" )]
		public object CTID { get; set; }
	}
}
