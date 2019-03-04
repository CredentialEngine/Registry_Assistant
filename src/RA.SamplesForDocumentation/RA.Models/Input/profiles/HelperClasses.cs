using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{

	public class IdentifierValue
	{
		public string Name { get; set; }

		public string Description { get; set; }
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

        public string IdentifierType { get; set; }

		public string IdentifierValueCode { get; set; }
	}

}
