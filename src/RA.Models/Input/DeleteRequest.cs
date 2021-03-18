using System.Collections.Generic;

namespace RA.Models.Input
{
	/// <summary>
	/// Class for delete by CTID
	/// </summary>
	public class DeleteRequest 
	{
        /// <summary>
        /// CTID of document to be deleted
        /// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Optionally provide a list of CTIDs where many records are to be deleted
		/// </summary>
		public List<string> CTIDList { get; set; } = new List<string>();
		/// <summary>
		/// Identifier for Organization which Owns the data being deleted
		/// 2017-12-13 - this will be the CTID for the owning org.
		/// </summary>
		public string PublishForOrganizationIdentifier { get; set; }

		/// <summary>
		/// Leave blank for default
		/// </summary>
		public string Community { get; set; }
	}

	/// <summary>
	/// Class for delete by EnvelopeId
	/// </summary>
    public class EnvelopeDelete
    {
        /// <summary>
        /// Envelope Identifier
        /// Optional property, used where the publishing entity wishes to store the identifier.
        /// Contains registry envelope identifier for a document in the registy. It should be empty for a new document. 
        /// </summary>
        public string RegistryEnvelopeId { get; set; }

        /// <summary>
        /// CTID of document to be deleted
        /// </summary>
		public string CTID { get; set; }

        /// <summary>
        /// Identifier for Organization which Owns the data being published
        /// 2017-12-13 - this will be the CTID for the owning org.
        /// </summary>
        public string PublishForOrganizationIdentifier { get; set; }
		/// <summary>
		/// Leave blank for default
		/// </summary>
		public string Community { get; set; }
	}
}
