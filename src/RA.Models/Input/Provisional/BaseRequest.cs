// <copyright file="MetricManager.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

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
        public LanguageMap NameLangMap { get; set; } = null;

        /// <summary>
        /// Organization that owns this resource
		/// NOTE: blank nodes are not allowed, so this will be one or more CTIDs
        /// OwnedBy or OfferedBy is Required
        /// </summary>
        public List<string> OwnedBy { get; set; }

        /// <summary>
        /// Organization that offers this resource
        /// NOTE: blank nodes are not allowed, so this will be one or more CTIDs
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
        public LanguageMap DescriptionLangMap { get; set; } = null;

        /// <summary>
        /// Webpage that describes this entity.
        /// OPTIONAL
        /// URL
        /// </summary>
        public string SubjectWebpage { get; set; }


    }
}
