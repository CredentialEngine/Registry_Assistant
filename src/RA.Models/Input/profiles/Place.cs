using System;
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
		public Place()
		{
		}
		/// <summary>
		/// Optional Name for the address
		/// </summary>
		public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Street Address1
		/// </summary>
		public string Address1 { get; set; }
		/// <summary>
		/// Street Address2 - this will be appended to Address1 on publish to the registry
		/// </summary>
		public string Address2 { get; set; }

		/// <summary>
		/// Post office box number for post office addresses.
		/// </summary>
		public string PostOfficeBoxNumber { get; set; }

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
		/// <summary>
		/// Named area or division within a region, such as a county in the U.S. or Canada.
		/// 20-11-06 PROPOSED NOT OFFICIAL AT THIS TIME
		/// </summary>
		public string SubRegion { get; set; }
		public LanguageMap SubRegion_Map { get; set; } = new LanguageMap();

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
			SocialMediaPages = new List<string>();
		}
		/// <summary>
		/// Name of the Contact Point 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Specification of the type of contact
		/// Example: Registration
		/// </summary>
		public string ContactType { get; set; }

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
