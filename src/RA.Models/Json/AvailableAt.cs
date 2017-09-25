using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.Json
{
	public class AvailableAt
	{
		public AvailableAt()
		{
			Type = "ceterms:GeoCoordinates";
			Address = new List<Json.Address>();
		}
		[ JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:name" )]
		public string Name { get; set; }


		[JsonProperty( "ceterms:address" )]
		public List<Address> Address { get; set; }
	}
}
