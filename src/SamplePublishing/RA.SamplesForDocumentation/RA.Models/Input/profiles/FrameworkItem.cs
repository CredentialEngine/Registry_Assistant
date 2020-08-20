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
	/// SOC - occupations
	/// NAICS - industries
	/// CIP
	/// </summary>
	public class FrameworkItem
	{
		/// <summary>
		/// URL for framework
		/// </summary>
		public string Framework { get; set; }
		public string FrameworkName { get; set; }
        public LanguageMap FrameworkName_Map { get; set; } = new LanguageMap();

        public string CodedNotation { get; set; }
		//targetNodeName
		public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();
		//targetNodeDescription
		public string Description { get; set; }
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// URI for the FrameworkItem
		/// </summary>
		public string TargetNode { get; set; }

	}
}
