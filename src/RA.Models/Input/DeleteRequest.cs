using System;
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
		/// A maximum of 50 CTIDs may be requested at a time.
		/// </summary>
		public List<string> CTIDList { get; set; } = new List<string>();
		/// <summary>
		/// CTID for the Organization which Owns the data being deleted
		/// </summary>
		public string PublishForOrganizationIdentifier { get; set; }

		/// <summary>
		/// Leave blank for default
		/// Formerly known as Community
		/// </summary>
		public string Registry { get; set; }
	}

	/// <summary>
	/// Request class for a Deactive/Cease request
	/// </summary>
	public class CeaseRequest
	{
		/// <summary>
		/// CTID of document to be deleted
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// CTID for the Organization which Owns the data being deleted
		/// </summary>
		public string PublishForOrganizationIdentifier { get; set; }

		/// <summary>
		/// New status for the resource.
		/// If blank, the resources will be set to deactivated/ceased
		/// </summary>
		public string NewStatus { get; set; }

		/// <summary>
		/// Leave blank for default
		/// Formerly known as Community
		/// </summary>
		public string Registry { get; set; }
	}
}
