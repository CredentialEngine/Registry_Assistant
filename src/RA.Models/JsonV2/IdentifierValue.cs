
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class IdentifierValue
	{
		public IdentifierValue()
		{
			Type = "ceterms:IdentifierValue";
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:identifierTypeName" )]
		public LanguageMap IdentifierTypeName { get; set; }

		//[JsonProperty( "ceterms:description" )]
		//public LanguageMap Description { get; set; }

		[JsonProperty( "ceterms:identifierType" )]
		public string IdentifierType { get; set; }

		[JsonProperty( "ceterms:identifierValueCode" )]
		public string IdentifierValueCode { get; set; }
	}
}
