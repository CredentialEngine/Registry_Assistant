using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class FinancialAssistanceProfile
	{

		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		public string SubjectWebpage { get; set; }
		public List<string> FinancialAssistanceType { get; set; } = new List<string>();

	}
}
