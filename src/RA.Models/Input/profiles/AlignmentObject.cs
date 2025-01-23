using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace RA.Models.Input
{
    /// <summary>
    /// Any value in inheriting from CredentialAlignmentObject?
    /// </summary>
    public class AlignmentObject
    {
        public AlignmentObject()
        {
            Type = "ceterms:AlignmentObject";
            AlignmentDate = null;

        }

        /// <summary>
        /// Need a custom mapping to @type based on input value
        /// </summary>
        [JsonProperty( "@type" )]
        public string Type { get; set; }

        /// <summary>
        /// An identifier for use with blank nodes, to minimize duplicates
        /// </summary>
        [JsonProperty( "@id" )]
        public string BNodeId { get; set; }

        /// <summary>
        /// Alignment Date
        /// The date the alignment was made.
        /// </summary>
        public string AlignmentDate { get; set; }

        /// <summary>
        /// Alignment Type
        /// A category of alignment between the learning resource and the framework node.
        /// </summary>
        public string AlignmentType { get; set; }

        /// <summary>
        /// Statement, characterization or account of the entity. 
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:description" )]
        public LanguageMap Description { get; set; }

        public List<string> IsPartOf { get; set; }

        /// <summary>
        /// Individual entry in a formally defined framework such as a competency or an industry, instructional program, or occupation code that is the source of an alignment. 
        /// xsd:anyURI
        /// </summary>
        public string SourceNode { get; set; }

        /// <summary>
        /// Textual description of an individual concept or competency in a formally defined framework  that is the source of an alignment.
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:sourceNodeDescription" )]
        public LanguageMap SourceNodeDescription { get; set; }

        /// <summary>
        /// Name of an individual concept or competency in a formally defined framework  that is the source of an alignment. 
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:sourceNodeName" )]
        public LanguageMap SourceNodeName { get; set; }

        /// <summary>
        /// Target Node
        /// The node of a framework targeted by the alignment. Must be a valid URL.
        /// </summary>
        public string TargetNode { get; set; }

        /// <summary>
        /// Target Description
        /// The description of a node in an established educational framework.
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:targetNodeDescription" )]
        public LanguageMap TargetNodeDescription { get; set; } = new LanguageMap();

        /// <summary>
        /// Target Node Name
        /// The name of a node in an established educational framework.
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:targetNodeName" )]
        public LanguageMap TargetNodeName { get; set; } = new LanguageMap();

        /// <summary>
        /// Weight
        /// An asserted measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
        /// </summary>
        public decimal? Weight { get; set; }
    }
}
