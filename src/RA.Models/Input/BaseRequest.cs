using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class BaseRequest
	{
		/// <summary>
		/// quite likely will be passed in the header????
		/// </summary>
		public string APIKey { get; set; }

		/// <summary>
		/// Envelope Identifier
		/// Optional property, used where the publishing entity wishes to store the identifier.
		/// Contains registry envelope identifier for a document in the registy. It should be empty for a new document. 
		/// </summary>
		public string RegistryEnvelopeId { get; set; }

		/// <summary>
		/// Person/Organization making the request
		/// </summary>
		public string Submitter { get; set; }
	}
}
