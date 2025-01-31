﻿namespace RA.Models.Input
{
	/// <summary>
	/// 2018-09-02 Where LanguageMap alternates are available, only enter one. The system will check the string version first. 
	/// </summary>
	public class CredentialAlignmentObject
	{
		public string Type { get; set; } = "ceterms:CredentialAlignmentObject";

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Could be a registry URL or external, typically expect a framework URL.
		/// URL
		/// </summary>
		public string Framework { get; set; }
		/// <summary>
		/// Formal name of the framework.
		/// </summary>
		public string FrameworkName { get; set; }
		/// <summary>
		/// Name of the framework - using LanguageMap
		/// </summary>
		public LanguageMap FrameworkName_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Individual entry in a formally defined framework such as a competency or an industry, instructional program, or occupation code.
		/// xsd:anyURI
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
		public LanguageMap TargetNodeName_Map { get; set; } = null;

		/// <summary>
		/// Textual description of an individual concept or competency in a formally defined framework.
		/// </summary>
		public string TargetNodeDescription { get; set; }
		/// <summary>
		/// Alternately provide description using LanguageMap
		/// </summary>
		public LanguageMap TargetNodeDescription_Map { get; set; } = null;

		/// <summary>
		/// Measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		public decimal? Weight { get; set; }
	}


}
