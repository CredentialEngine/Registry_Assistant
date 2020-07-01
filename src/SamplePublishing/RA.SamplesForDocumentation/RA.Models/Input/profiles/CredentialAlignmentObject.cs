using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// 2018-09-02 Where LanguageMap alternates are available, only enter one. The system will check the string version first. 
    /// </summary>
    public class CredentialAlignmentObject
	{
		//public string AlignmentType { get; set; }
		public string CodedNotation { get; set; }
		/// <summary>
		/// URL
		/// </summary>
		public string Framework { get; set; }
		public string FrameworkName { get; set; }
        public LanguageMap FrameworkName_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// URL
        /// </summary>
        public string TargetNode { get; set; }
		public string TargetNodeDescription { get; set; }
        public LanguageMap TargetNodeDescription_Map { get; set; } = new LanguageMap();
        public string TargetNodeName { get; set; }
        public LanguageMap TargetNodeName_Map { get; set; } = new LanguageMap();

        public decimal Weight { get; set; }
	}
}
