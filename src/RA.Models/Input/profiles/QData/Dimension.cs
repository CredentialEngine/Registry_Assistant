using Newtonsoft.Json;
using System.Collections.Generic;

namespace RA.Models.Input.profiles.QData
{
    /// <summary>
    /// Aspect or characteristic along which observations in a dataset can be organized and identified.
    /// Require One Of: ceterms:name, ceterms:description
    /// </summary>
    public class Dimension
    {
        public Dimension()
        {
            Type = "qdata:Dimension";
        }
        [JsonProperty( "@type" )]
        public string Type { get; set; }

        /// <summary>
        /// An identifier for use with blank nodes. 
        /// It will be ignored if included with a primary resource
        /// </summary>
        [JsonProperty( "@id" )]
        public string BlankNodeId { get; set; }

        /// <summary>
        /// Name of this Resource
        /// Name or Description is Required
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
        /// Name or Description is Required
        /// Must be a minimum of 15 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:description" )]
        public LanguageMap Description_Map { get; set; }

        /// <summary>
        /// Type of resource that may be a point in the dimension.
        /// Items in a dimension will typically be instances of CTDL classes or concepts drawn from a CTDL concept scheme.
        /// While this is not limited to the use of a CTDL ConceptScheme, qdata:DemographicCategory is available
        /// </summary>
        public string DimensionType { get; set; }

        /// <summary>
        /// Identifies a specific entity in a  Dimension. 
        /// The value of this property will be a resource with any type, e.g. skos:Concept, ceterms:Credential (or any subclass thereof), ceterms:Industry, ceterms:Place, etc.
        /// rdfs:Resource - a URI or CTID?
        /// </summary>
        public List<string> HasPoint { get; set; }
    }
}
