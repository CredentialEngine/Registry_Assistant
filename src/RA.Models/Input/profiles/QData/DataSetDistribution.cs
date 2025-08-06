using Newtonsoft.Json;
using System.Collections.Generic;

namespace RA.Models.Input.profiles.QData
{
	/// <summary>
	/// A specific representation of a dataset.
	/// Required: ceterms:description
	/// </summary>
	public class DataSetDistribution
	{
		/// <summary>
		/// Name of this Resource
		/// Optional
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; }

		/// <summary>
		/// Resource Description 
		/// Description is Required
		/// Must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateNameLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// An established standard to which the described resource conforms.
		/// qdata:conformsTo
		/// xsd:anyURI
		/// </summary>
		public List<string> ConformsTo { get; set; } = new List<string>();

		/// <summary>
		/// Distribution File
		/// Downloadable form of this dataset, at a specific location, in a specific format.
		/// qdata:distributionFile
		/// xsd:anyURI
		/// </summary>
		public string DistributionFile { get; set; }

		/// <summary>
		/// Effective date of this resource's content.
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// xsd:anyURI
		/// </summary>
		public List<string> License { get; set; }

		/// <summary>
		/// A file format or physical medium.
		/// </summary>
		public string MediaType { get; set; }

		/// <summary>
		/// Information about rights held in and over this resource.
		/// </summary>
		public string Rights { get; set; }

		[JsonProperty( PropertyName = "ceterms:rights" )]
		public LanguageMap Rights_Map { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// xsd:anyURI
		/// </summary>
		public List<string> Source { get; set; }
	}
}
