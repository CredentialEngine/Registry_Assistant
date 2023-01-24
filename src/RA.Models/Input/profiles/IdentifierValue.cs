using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{

	/// <summary>
	///  Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
	/// <see cref="https://credreg.net/ctdl/terms/IdentifierValue"/>
	/// </summary>
	public class IdentifierValue
	{
		/// <summary>
		/// Framework, scheme, type, or other organizing principle of this identifier.
		/// URI
		/// NOTE - this doesn't have to be resolvable!
		/// Optional
		/// </summary>
		public string IdentifierType { get; set; }

		/// <summary>
		/// Formal name or acronym of the framework, scheme, type, or other organizing principle of this identifier, such as ISBN or ISSN.
		/// </summary>
		public string IdentifierTypeName { get; set; }

		/// <summary>
		/// Language map for IdentifierTypeName
		/// </summary>
		public LanguageMap IdentifierTypeName_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Alphanumeric string identifier of the entity.
		/// Where a formal identification system exists for the identifier, recommended best practice is to use a string conforming to that system.
		/// </summary>
		public string IdentifierValueCode { get; set; }
	}

}
