﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Content for a Place (or Address)
    /// NOTE: Use either the string or LanguageMap equivalent property, but not both
    /// </summary>
	public class Place
	{
		/// <summary>
		/// constructor
		/// </summary>
		public Place()
		{
		}
		/// <summary>
		/// Optional Name for the address
		/// </summary>
		public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Optional description 
		/// 2020-12-15 - adding back use of description
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Street Address1
		/// </summary>
		public string Address1 { get; set; }
		public LanguageMap Address_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Street Address2 - this will be appended to Address1 on publish to the registry
		/// </summary>
		public string Address2 { get; set; }

		/// <summary>
		/// Post office box number for post office addresses.
		/// </summary>
		public string PostOfficeBoxNumber { get; set; }
		//public LanguageMap PostOfficeBoxNumber_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Town, city, or village in which a particular location is situtated.
		/// Published to ceterms:addressLocality
		/// </summary>
		public string City { get; set; }
        public LanguageMap City_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// AddressRegion - State, Province, etc
		/// </summary>
        public string AddressRegion { get; set; }
        public LanguageMap AddressRegion_Map { get; set; } = new LanguageMap();
		///// <summary>
		///// Named area or division within a region, such as a county in the U.S. or Canada.
		///// 20-11-06 PROPOSED NOT OFFICIAL AT THIS TIME
		///// 21-03-01 Identifier has been added, and so this may no longer be relevant?
		///// </summary>
		//public string SubRegion { get; set; }
		//public LanguageMap SubRegion_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Postal Code
		/// </summary>
		public string PostalCode { get; set; }

		/// <summary>
		/// Country
		/// </summary>
		public string Country { get; set; }
        public LanguageMap Country_Map { get; set; } = new LanguageMap();

        public double Latitude { get; set; }

		public double Longitude { get; set; }

		/// <summary>
		/// Identifier
		/// Definition:	en-US: Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// For example state FIPS codes, or LWIAs (Local workforcement investment areas)
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();


        /// <summary>
        /// Subject Webpage
        /// URL
        /// Optional
        /// URL
        /// </summary>
        public string SubjectWebpage { get; set; }

        /// <summary>
        /// URI to geonames location
        /// </summary>
        public string GeoURI { get; set; }
		public List<ContactPoint> ContactPoint { get; set; } = new List<Input.ContactPoint>();
	}

	public class ContactPoint
	{
		public ContactPoint()
		{
			PhoneNumbers = new List<string>();
			Emails = new List<string>();
			FaxNumber = new List<string>();
			SocialMediaPages = new List<string>();
		}
		/// <summary>
		/// Name of the Contact Point 
		/// </summary>
		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Specification of the type of contact
		/// Example: Registration
		/// </summary>
		public string ContactType { get; set; }
		public LanguageMap ContactType_Map { get; set; } = new LanguageMap();
		//
		public List<string> FaxNumber { get; set; }
		/// <summary>
		/// List of phone numbers for this contact point
		/// </summary>
		public List<string> PhoneNumbers { get; set; }
		/// <summary>
		/// List of email addresses for this contact point
		/// </summary>
		public List<string> Emails { get; set; }
		/// <summary>
		/// List of URIs to social media pages for this contact point
		/// </summary>
		public List<string> SocialMediaPages { get; set; }

	}
}
