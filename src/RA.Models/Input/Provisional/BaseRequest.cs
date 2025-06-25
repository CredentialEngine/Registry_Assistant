using System.Collections.Generic;

using Newtonsoft.Json;


namespace RA.Models.Input.Provisional
{
    public class BaseProvisional
    {
        /// <summary>
        /// Helper property for use with blank nodes
        /// This is a 'broad' type. CredentialType is still required.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Credential Identifier
        /// format: 
        /// ce-UUID (guid)
        /// Required
        /// </summary>
        public string CTID { get; set; }

        /// <summary>
        /// Name or title of the resource.
        /// Required
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  LanguageMap for Name
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:name" )]
        public LanguageMap NameLangMap { get; set; } = new LanguageMap();

        /// <summary>
        /// Organization that owns this resource
        /// OwnedBy or OfferedBy is Required
        /// </summary>
        public List<string> OwnedBy { get; set; }

        /// <summary>
        /// Organization that offers this resource
        /// OwnedBy or OfferedBy is Required
        /// </summary>
        public List<string> OfferedBy { get; set; }

        /// <summary>
        /// Description 
        /// OPTIONAL
        /// When present must be a minimum of 15 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:description" )]
        public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

        /// <summary>
        /// Webpage that describes this entity.
        /// OPTIONAL
        /// URL
        /// </summary>
        public string SubjectWebpage { get; set; }


    }
}
