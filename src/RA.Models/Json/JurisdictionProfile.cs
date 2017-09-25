﻿using Newtonsoft.Json;
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
		public OrganizationBase AssertedBy { get; set; }
		//public List<OrganizationBase> AssertedBy { get; set; }
	}

	/// <summary>
	/// If a url exists, then only output name plus the @id, otherwise, use the other properties 
	/// </summary>
	public class GeoCoordinates 
	{
		public GeoCoordinates()
		{
			Type = "ceterms:GeoCoordinates";
		}
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:geoURI" )]
		public IdProperty GeoURI { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public string Name { get; set; }

		//[JsonProperty( PropertyName = "ceterms:country" )]
		//public string Country { get; set; }

		//[JsonProperty( PropertyName = "ceterms:region" )]
		//public string Region { get; set; }

		[JsonProperty( PropertyName = "ceterms:latitude" )]
		public double Latitude { get; set; }

		[JsonProperty( PropertyName = "ceterms:longitude" )]
		public double Longitude { get; set; }

		//[JsonProperty( PropertyName = "ceterms:url" )]
		//public string Url { get; set; }

		[JsonProperty( PropertyName = "ceterms:address" )]
		public Address Address { get; set; }

	}
}
