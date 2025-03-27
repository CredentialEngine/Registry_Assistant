using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace RA.Models.Input
{
    /// <summary>
    /// Entity describing the relationship between two nodes in different frameworks.
    /// Requirements
    /// AlignmentType
    /// - Must have a Target Node or a Target Node Name.
    /// - Must have a Source Node or a Source Node Name. 
    /// </summary>
    public class AlignmentObject
    {
        public AlignmentObject()
        {
            Type = "ceterms:AlignmentObject";
            AlignmentDate = null;
        }

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
        /// REQUIRED
        /// From conceptScheme: ceterms:Alignment
        /// </summary>
        public string AlignmentType { get; set; }

        /// <summary>
        /// Statement, characterization or account of the entity. 
        /// </summary>
        public string Description { get; set; }

        [JsonProperty( PropertyName = "ceterms:description" )]
        public LanguageMap Description_LangMap { get; set; }

        /// <summary>
        /// Individual entry in a formally defined framework such as a competency or an industry, instructional program, or occupation code that is the source of an alignment. 
        /// - Must have a Source Node or a Source Node Name. 
        /// xsd:anyURI
        /// </summary>
        public string SourceNode { get; set; }
        /// <summary>
        /// Name of an individual concept or competency in a formally defined framework  that is the source of an alignment. 
        /// - Must have a Source Node or a Source Node Name. 
        /// </summary>
        public string SourceNodeName { get; set; }

        [JsonProperty( PropertyName = "ceterms:sourceNodeName" )]
        public LanguageMap SourceNodeName_LangMap { get; set; }

        /// <summary>
        /// Textual description of an individual concept or competency in a formally defined framework  that is the source of an alignment.
        /// </summary>
        public string SourceNodeDescription { get; set; }

        [JsonProperty( PropertyName = "ceterms:sourceNodeDescription" )]
        public LanguageMap SourceNodeDescription_LangMap { get; set; }

        /// <summary>
        /// Target Node
        /// The node of a framework targeted by the alignment. Must be a valid URL.
        /// - Must have a Target Node or a Target Node Name.
        /// </summary>
        public string TargetNode { get; set; }

        /// <summary>
        /// Target Node Name
        /// The name of a node in an established educational framework.
        /// - Must have a Target Node or a Target Node Name.
        /// </summary>
        public string TargetNodeName { get; set; }

        [JsonProperty( PropertyName = "ceterms:targetNodeName" )]
        public LanguageMap TargetNodeName_LangMap { get; set; }

        /// <summary>
        /// Target Description
        /// The description of a node in an established educational framework.
        /// </summary>
        public string TargetNodeDescription { get; set; }

        [JsonProperty( PropertyName = "ceterms:targetNodeDescription" )]
        public LanguageMap TargetNodeDescription_LangMap { get; set; }

        /// <summary>
        /// Weight
        /// An asserted measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
        /// </summary>
        public decimal? Weight { get; set; }
    }
}
