using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class Place
	{
		public Place()
		{
			Type = "ceterms:Place";
			ContactPoint = new List<ContactPoint>();
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// An identifier for use with blank nodes. 
		/// It will be ignored if included with a primary resource
		/// </summary>
		[JsonProperty( "@id" )]
		public string Id { get; set; } = null;

		/// <summary>
		/// Entity that describes the longitude, latitude and other location details of a place.
		/// </summary>
		[JsonProperty( "ceterms:geoURI" )]
		public string GeoURI { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		// 2020-12-15 - adding back use of description
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( "ceterms:streetAddress" )]
		public LanguageMap StreetAddress { get; set; }

		[JsonProperty( "ceterms:postOfficeBoxNumber" )]
		public string PostOfficeBoxNumber { get; set; }

		[JsonProperty( "ceterms:addressLocality" )]
		public LanguageMap City { get; set; }

		[JsonProperty( "ceterms:addressRegion" )]
		public LanguageMap AddressRegion { get; set; }

        /// <summary>
        /// An administrative subdivision within a country that is intermediate between a region and a locality.
        /// </summary>
        [JsonProperty( "ceterms:subRegion" )]
        public LanguageMap SubRegion { get; set; }

        [JsonProperty( "ceterms:postalCode" )]
		public string PostalCode { get; set; }

		[JsonProperty( "ceterms:addressCountry" )]
		public LanguageMap Country { get; set; }

		/// <summary>
		/// Identifier
		/// Definition:	Alphanumeric Identifier value.
		/// List of URIs 
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		[JsonProperty( PropertyName = "ceterms:latitude" )]
		public double? Latitude { get; set; } = null;

		[JsonProperty( PropertyName = "ceterms:longitude" )]
		public double? Longitude { get; set; } = null;

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		/// <summary>
		/// Subject Webpage
		/// URL
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		[JsonProperty( "ceterms:targetContactPoint" )]
		public List<ContactPoint> ContactPoint { get; set; }
	}
}
