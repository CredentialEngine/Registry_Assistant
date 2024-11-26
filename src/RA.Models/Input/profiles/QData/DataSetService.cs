using Newtonsoft.Json;
using System.Collections.Generic;

namespace RA.Models.Input.profiles.QData
{
    /// <summary>
    /// A service that provides access to one or more datasets or data processing functions. 
    /// Required: ceterms:description
    /// </summary>
    public class DataSetService
    {

        /// <summary>
		/// Name of this Resource
		/// Recommended
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:name" )]
        public LanguageMap Name_Map { get; set; }

        /// <summary>
        /// Resource Description 
        /// REQUIRED and must be a minimum of 15 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:description" )]
        public LanguageMap Description_Map { get; set; }

        /// <summary>
        /// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
        /// </summary>
        public List<string> AlternateName { get; set; } = new List<string>();
        /// <summary>
        /// LanguageMap for AlternateName
        /// </summary>
        public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

        /// <summary>
        /// A legal document giving official permission to do something with this resource.
        /// xsd:anyURI
        /// </summary>
        public List<string> License { get; set; }

        /// <summary>
        /// Information about rights held in and over this resource.
        /// </summary>
        public string Rights { get; set; }

        [JsonProperty( PropertyName = "ceterms:rights" )]
        public LanguageMap Rights_Map { get; set; }

        /// <summary>
        /// Webpage that describes this entity.
        /// URL
        /// </summary>
        public string SubjectWebpage { get; set; }
    }
}
