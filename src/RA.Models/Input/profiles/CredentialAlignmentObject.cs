using Newtonsoft.Json;

namespace RA.Models.Input
{
    /// <summary>
    /// 2018-09-02 Where LanguageMap alternates are available, only enter one. The system will check the string version first. 
    /// 2025-02-10 - added AlignmentType
    /// </summary>
    public class CredentialAlignmentObject
    {
        public string Type { get; set; } = "ceterms:CredentialAlignmentObject";

        /// <summary>
        /// Alignment Date
        /// The date the alignment was made.
        /// </summary>
        public string AlignmentDate { get; set; }

        /// <summary>
        /// Alignment Type
        /// A category of alignment between the learning resource and the framework node.
        /// From conceptScheme: ceterms:Alignment
        /// </summary>
        public string AlignmentType { get; set; }

        /// <summary>
        /// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
        /// </summary>
        public string CodedNotation { get; set; }

        /// <summary>
        /// Framework URL
        /// URL or CTID
        /// </summary>
        public string Framework { get; set; }

        /// <summary>
        /// Framework Name
        /// Formal name of the framework, or progression model, etc.
        /// </summary>
        public string FrameworkName { get; set; }

        /// <summary>
        /// Name of the framework - using LanguageMap
        /// </summary>
        [JsonProperty( PropertyName = "ceterms:frameworkName" )]

        public LanguageMap FrameworkName_LangMap { get; set; } = null;

        /// <summary>
        /// Individual entry in a formally defined framework such as a competency or an industry, instructional program, or occupation code.
        /// xsd:anyURI - could be a CTID in some scenarios
        /// </summary>
        public string TargetNode { get; set; }

        /// <summary>
        /// Name of an individual concept or competency in a formally defined framework.
        /// Note that ceasn:Competency does NOT have a Name property. This property would must like map to Competency.CompetencyText
        /// </summary>
        public string TargetNodeName { get; set; }

        /// <summary>
        /// Name of an individual concept or competency in a formally defined framework using languageMap
        /// </summary>
        public LanguageMap TargetNodeName_LangMap { get; set; } = null;

        /// <summary>
        /// Textual description of an individual concept or competency in a formally defined framework.
        /// </summary>
        public string TargetNodeDescription { get; set; }

        /// <summary>
        /// Alternately provide description using LanguageMap
        /// </summary>
        public LanguageMap TargetNodeDescription_LangMap { get; set; } = null;

		/// <summary>
		/// Measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		public decimal? Weight { get; set; }


    }
}
