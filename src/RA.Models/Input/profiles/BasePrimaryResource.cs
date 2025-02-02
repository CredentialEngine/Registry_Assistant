using Newtonsoft.Json;

namespace RA.Models.Input
{
    public class BasePrimaryResource
	{

		/// <summary>
		/// An identifier for use with blank nodes. 
		/// It will be ignored if included with a primary resource
		/// </summary>
		[JsonProperty( "@id" )]
		public string Id { get; set; }

	}
}
