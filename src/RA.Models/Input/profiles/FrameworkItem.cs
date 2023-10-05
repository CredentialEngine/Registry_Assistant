using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public LanguageMap FrameworkName_Map { get; set; } = new LanguageMap();

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
		/// Alternately provide name using LanguageMap
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Description of the framework item
		/// targetNodeDescription
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately provide description using LanguageMap
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

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
