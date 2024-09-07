using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles.QData
{
    /// <summary>
    /// Aspect or characteristic along which observations in a dataset can be organized and identified.
    /// </summary>
    public class Dimension
    {
        /// <summary>
        /// Name of this Resource
        /// Required
        /// ceterms:name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Name_Map { get; set; }

        /// <summary>
        /// Resource Description 
        /// REQUIRED and must be a minimum of 15 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; }

        /// <summary>
        /// Type of resource that may be a point in the dimension.
        /// Items in a dimension will typically be instances of CTDL classes or concepts drawn from a CTDL concept scheme.
        /// Concept in ConceptScheme: qdata:DemographicCategory 
        /// </summary>
        public string DimensionType { get; set; }

        /// <summary>
        /// Identifies a specific entity in a  Dimension. 
        /// The value of this property will be a resource with any type, e.g. skos:Concept, ceterms:Credential (or any subclass thereof), ceterms:Industry, ceterms:Place, etc.
        /// rdfs:Resource - a URI or CTID?
        /// </summary>
        public string HasPoint { get; set; }
    }
}
