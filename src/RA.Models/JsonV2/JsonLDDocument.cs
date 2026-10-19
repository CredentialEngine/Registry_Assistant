using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Base resource document
	/// </summary>
	public class BaseResourceDocument
	{
		/// <summary>
		/// constructor
		/// </summary>
		public BaseResourceDocument()
		{
			Context = "https://credreg.net/ctdl/schema/context/json";
		}

		[JsonIgnore]
		[JsonProperty( "@context" )]
		public string Context { get; set; }

		[JsonProperty( "schema:datePublished" )]
		public string DatePublished { get; set; } = null;

		/// <summary>
		/// Another functionally similar entity.
		/// The property is generally applicable in describing the similarity between any two entities.
		/// ceterms:isSimilarTo
		/// </summary>
		[JsonProperty( "ceterms:isSimilarTo" )]
		public List<string> IsSimilarTo { get; set; } = new List<string>();
	}
}
