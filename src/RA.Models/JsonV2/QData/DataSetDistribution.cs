using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2.QData
{
	/// <summary>
	/// A specific representation of a dataset.
	/// Required: ceterms:description
	/// </summary>
	public class DataSetDistribution
	{

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
		/// An established standard to which the described resource conforms.
		/// qdata:conformsTo
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( "qdata:conformsTo" )]
		public List<string> ConformsTo { get; set; }

		/// <summary>
		/// Distribution File
		/// Downloadable form of this dataset, at a specific location, in a specific format.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "qdata:distributionFile" )]
		public string DistributionFile { get; set; }

		/// <summary>
		/// Effective date of this resource's content.
		/// </summary>
		[JsonProperty( "ceterms: dateEffective" )]
		public string DateEffective { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( "ceasn:license" )]
		public List<string> License { get; set; }

		/// <summary>
		/// A file format or physical medium.
		/// </summary>
		[JsonProperty( "qdata:mediaType" )]
		public string MediaType { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		[JsonProperty( "ceasn:rights" )]
		public LanguageMap Rights { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( "ceasn:source" )]
		public List<string> Source { get; set; }
	}
}
