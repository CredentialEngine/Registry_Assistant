using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Content for a Place or Address)
    /// NOTE: Use either the string or LanguageMap equivalent property, but not both
    /// </summary>
	public class Place
	{
		public Place()
		{
		}

		public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();

        public string Description { get; set; }
        
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

        public string Address1 { get; set; }
		public string Address2 { get; set; }

		public string PostOfficeBoxNumber { get; set; }

		public string City { get; set; }
        public LanguageMap City_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// AddressRegion - State, Province, etc
		/// </summary>
        public string AddressRegion { get; set; }
        public LanguageMap AddressRegion_Map { get; set; } = new LanguageMap();
        public string PostalCode { get; set; }

		public string Country { get; set; }
        public LanguageMap Country_Map { get; set; } = new LanguageMap();

        public double Latitude { get; set; }

		public double Longitude { get; set; }

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

		public List<string> PhoneNumbers { get; set; }
		/// <summary>
		/// List of email addresses
		/// </summary>
		public List<string> Emails { get; set; }
		/// <summary>
		/// List of URIs to social media pages
		/// </summary>
		public List<string> SocialMediaPages { get; set; }

	}
}
