using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Input
{

	public class GraphRequest : BaseRequest
	{
		public GraphRequest()
		{
			GraphInput = new GraphInput();
			HasLanguageMaps = true;
		}
		public string CTID { get; set; }
		public bool GenerateHasTopIsTopChild { get; set; } = false;
		public bool HasLanguageMaps { get; set; }

		public GraphInput GraphInput { get; set; }

	}
	/// <summary>
	/// The plain graph doesn't include language maps
	/// </summary>
	public class GraphInput
	{
		/// <summary>
		/// Main graph object
		/// </summary>
		[JsonProperty( "@graph" )]
		public object Graph { get; set; }

		[JsonProperty( "@context" )]
		public string Context { get; set; }
	}
}
