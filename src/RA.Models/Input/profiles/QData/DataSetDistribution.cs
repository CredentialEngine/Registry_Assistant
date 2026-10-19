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
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
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
		/// Approximate date after which this resource is available or applicable.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// One of ApproximateDateEffective, or DateEffective is required
		/// </summary>
		public string ApproximateDateEffective { get; set; }

		/// <summary>
		/// Approximate date after which this resource is no longer offered or available.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		public string ApproximateExpirationDate { get; set; }

		/// <summary>
		/// Date after which this resource is available or applicable.
		/// One of ApproximateDateEffective, or DateEffective is required
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// </summary>
		public string ExpirationDate { get; set; }

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
		public LanguageMap RightsLangMap { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// xsd:anyURI
		/// </summary>
		public string Source { get; set; }
	}
}
