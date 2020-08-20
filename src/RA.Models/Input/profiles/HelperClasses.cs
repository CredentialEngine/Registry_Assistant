using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{

	/// <summary>
	/// Alphanumeric Identifier value.
	/// </summary>
	public class IdentifierValue
	{
		public string Name { get; set; }

		public string Description { get; set; }
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Formal name or acronym of the identifier type such as ISBN and ISSN.
		/// </summary>
		public string IdentifierType { get; set; }

		/// <summary>
		/// Alphanumeric string identifier of the entity.
		/// Where a formal identification system exists for the identifier, recommended best practice is to use a string conforming to that system.
		/// </summary>
		public string IdentifierValueCode { get; set; }
	}

}
