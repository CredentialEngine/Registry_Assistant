using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2.QData
{
	/// <summary>
	/// A service that provides access to one or more datasets or data processing functions. 
	/// </summary>
	public class DataSetService
	{
		public DataSetService()
		{
			Name = null;
			SubjectWebpage = null;
		}

		/// <summary>
		/// Name of this Resource
		/// ceterms:name
		/// </summary>
		[JsonProperty( "ceterms:name" )]
		public LanguageMap Name { get; set; }

		/// <summary>
		/// Resource Description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		[JsonProperty( "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( "ceasn:license" )]
		public List<string> License { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// URL
		/// </summary>
		[JsonProperty( "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }
	}
}
