using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Json
{
	public class JurisdictionProfile
	{
        public JurisdictionProfile()
        {
            Type = "ceterms:JurisdictionProfile";
			//MainJurisdiction = null;
			MainJurisdiction = new List<GeoCoordinates>();
			JurisdictionException = new List<GeoCoordinates>();
			//AssertedBy = new List<OrganizationBase>();
			AssertedBy = null;
		}

        [JsonProperty( "@type" )]
        public string Type { get; set; }

		[JsonProperty( PropertyName = "ceterms:globalJurisdiction" , DefaultValueHandling = DefaultValueHandling.Include)]        
		public bool? GlobalJurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public string Description { get; set; }

		/// <summary>
		/// The main jurisdiction, commonly a country name.
		/// The schema is defined as an array. However, the RA, and editor only allow a single MainJurisdiction
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:mainJurisdiction" )]
		public List<GeoCoordinates> MainJurisdiction { get; set; }

		//[JsonProperty( PropertyName = "ceterms:jurisdictionException" )]
		//public List<GeoCoordinates> JurisdictionException { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdictionException" )]
		public List<GeoCoordinates> JurisdictionException { get; set; }

		/// <summary>
		/// Asserted by is typically only explicitly entered for jurisdiction assertions the INs
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:assertedBy" )]
		public object AssertedBy { get; set; }
		//public OrganizationBase AssertedBy { get; set; }
		//public List<OrganizationBase> AssertedBy { get; set; }
	}
	public class Place
	{
		public Place()
		{
			Type = "ceterms:Place";
		}
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:geoURI" )]
		public string GeoURI { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public string Description { get; set; }

		[JsonProperty( "ceterms:streetAddress" )]
		public string StreetAddress { get; set; }

		[JsonProperty( "ceterms:postOfficeBoxNumber" )]
		public string PostOfficeBoxNumber { get; set; }

		[JsonProperty( "ceterms:addressLocality" )]
		public string City { get; set; }

		[JsonProperty( "ceterms:addressRegion" )]
		public string AddressRegion { get; set; }
		[JsonProperty( "ceterms:postalCode" )]
		public string PostalCode { get; set; }

		[JsonProperty( "ceterms:addressCountry" )]
		public string Country { get; set; }

		[JsonProperty( PropertyName = "ceterms:latitude" )]
		public string Latitude { get; set; }

		[JsonProperty( PropertyName = "ceterms:longitude" )]
		public string Longitude { get; set; }


		[JsonProperty( "ceterms:targetContactPoint" )]
		public List<ContactPoint> ContactPoint { get; set; }
	}
	/// <summary>
	/// If a url exists, then only output name plus the @id, otherwise, use the other properties 
	/// </summary>
	public class GeoCoordinates 
	{
		public GeoCoordinates()
		{
			Type = "ceterms:GeoCoordinates";
			Address = new List<Json.Address>();
		}
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:geoURI" )]
		public string GeoURI { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public string Name { get; set; }

		//[JsonProperty( PropertyName = "ceterms:country" )]
		//public string Country { get; set; }

		//[JsonProperty( PropertyName = "ceterms:region" )]
		//public string Region { get; set; }

		[JsonProperty( PropertyName = "ceterms:latitude" )]
		public string Latitude { get; set; }

		[JsonProperty( PropertyName = "ceterms:longitude" )]
		public string Longitude { get; set; }

		[JsonProperty( PropertyName = "ceterms:address" )]
		public List<Address> Address { get; set; }

	}
}
