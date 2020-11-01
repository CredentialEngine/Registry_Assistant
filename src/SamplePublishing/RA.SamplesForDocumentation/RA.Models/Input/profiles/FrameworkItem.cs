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
		/// URL for framework
		/// </summary>
		public string Framework { get; set; }
		/// <summary>
		/// Formal name of the framework
		/// </summary>
		public string FrameworkName { get; set; }
        public LanguageMap FrameworkName_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Experimental
		/// Allow a of codes for a known framework and API will validate and format as a CredentialAlignmentObject
		/// Current supported frameworks: NAICS and O*Net
		/// How to designate the framework
		/// </summary>
		public List<string> FrameworkCodeNotationList { get; set; } = new List<string>();

		public string CodedNotation { get; set; }

		/// <summary>
		/// Name of the framework item, such as occupation or industry.
		/// targetNodeName
		/// </summary>
		public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Description of the framework item
		/// targetNodeDescription
		/// </summary>
		public string Description { get; set; }
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// URI for the FrameworkItem
		/// </summary>
		public string TargetNode { get; set; }

		/// <summary>
		/// Weight
		/// An asserted measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		public decimal Weight { get; set; }
	}
}
