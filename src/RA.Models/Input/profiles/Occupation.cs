using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles
{
	public class Occupation
	{
		public string CTID { get; set; }

		/// <summary>
		/// Name of this Occupation
		/// Required
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies the credential and supports its discovery and use.
		/// </summary>
		public string CodedNotation { get; set; }


		/// <summary>
		/// Supplemental text to support the description
		/// </summary>
		public List<string> Comment { get; set; } = new List<string>();
		public LanguageMapList Comment_Map { get; set; } = new LanguageMapList();


		/// <summary>
		/// Occupation description 
		/// Required
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// More specialized profession, trade, or career field that is encompassed by the one being described.
		/// <see cref="https://credreg.net/ctdl/terms/hasSpecialization"/>
		/// </summary>
		public List<string> HasSpecialization { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		public List<string> IspecializationOf { get; set; } = new List<string>();

		public List<string> Keyword { get; set; } = new List<string>();
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		public List<string> SameAs { get; set; } = new List<string>();

		/// <summary>
		/// Subject Webpage
		/// URL
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// Version
		/// Is this a URL, or should it be an identifierValue?
		/// </summary>
		public string Version { get; set; } //URL

		//=============================================================================
		//List of occupations from a published framework, that is with a web URL
		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net. 
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		/// </summary>
		public List<FrameworkItem> OccupationType { get; set; } = new List<FrameworkItem>();
		/// <summary>


		//=============================================================================
		/// <summary>
		/// IndustryType
		/// Type of industry; select from an existing enumeration of such types such as the SIC, NAICS, and ISIC classifications.
		/// Best practice in identifying industries for U.S. credentials is to provide the NAICS code using the ceterms:naics property. 
		/// Other credentials may use the ceterms:industrytype property and any framework of the class ceterms:IndustryClassification.
		/// </summary>
		public List<FrameworkItem> IndustryType { get; set; } = new List<FrameworkItem>();


	}
}
