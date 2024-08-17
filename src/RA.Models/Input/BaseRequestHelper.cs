using Newtonsoft.Json;

namespace RA.Models.Input
{
    /// <summary>
    /// Helper class for publishers that are not using upper case CTID property
    /// </summary>
    public class BaseRequestHelper
	{
		/// <summary>
		/// An identifier for use with blank nodes. 
		/// It will be ignored if included with a primary resource
		/// </summary>
		[JsonProperty( "@id" )]
		public string BlankNodeId { get; set; }



	}
}
