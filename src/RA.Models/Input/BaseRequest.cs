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
		/// API Key
		/// Used to verify publishing org has rights to publish data for the 
		/// </summary>
		public string APIKey { get; set; }


		/// <summary>
		/// Publisher/Organization making the request
		/// This is the API Key for the publishing organization
		/// </summary>
		public string PublishByOrganizationIdentifier { get; set; }

		/// <summary>
		/// Identifier for Organization which Owns the data being published
		/// 2017-12-13 - this will be the CTID for the owning org.
		/// </summary>
		public string PublishForOrganizationIdentifier { get; set; }

		//public string ApiPublisher { get; set; }

		/// <summary>
		/// Envelope Identifier
		/// Optional property, used where the publishing entity wishes to store the identifier.
		/// Contains registry envelope identifier for a document in the registy. It should be empty for a new document. 
		/// </summary>
		public string RegistryEnvelopeId { get; set; }
	}
}
