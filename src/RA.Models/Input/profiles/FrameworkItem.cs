// <copyright file="MetricManager.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Coded Framework
	/// Examples
	/// SOC/O*Net	- occupations
	/// NAICS		- industries
	/// CIP			- Classification of Instructional Programs
	/// </summary>
	public class FrameworkItem
	{
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
		[JsonProperty( PropertyName = "ceterms:frameworkName" )]
		public LanguageMap FrameworkNameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Name of the framework item, such as occupation or industry.
		/// targetNodeName
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///  LanguageMap for Name
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; } = null;

		/// <summary>
		/// Description of the framework item
		/// targetNodeDescription
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately provide description using LanguageMap
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// URI for the FrameworkItem
		/// </summary>
		public string TargetNode { get; set; }

		/// <summary>
		/// Measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		public decimal? Weight { get; set; }
	}
}
