using System.Collections.Generic;
namespace RA.Models.Input
{

	/// <summary>
	/// Purge Request
	/// For Administration use only in the sandbox. 
	/// Use to purge data for either a publishing organization (that is a third party publisher) or the owning organization
	/// </summary>
	public class PurgeRequest
	{
		/// <summary>
		/// EntityType to be deleted. 
		/// Example: credential (not Certificate, License, etc.)
		/// </summary>
		public string EntityType { get; set; }

		/// <summary>
		/// Date range to be deleted from this starting date
		/// </summary>
		public string FromDate { get; set; }
		/// <summary>
		/// Date range to be deleted up to this ending date.
		/// </summary>
		public string UntilDate { get; set; }

		/// <summary>
		/// Identifier for Organization which published the data to be purged
		/// </summary>
		public string PublishByOrganizationIdentifier { get; set; }

		/// <summary>
		/// Identifier for Organization which owned the data to be purged
		/// </summary>
		public string OwnedByOrganizationIdentifier { get; set; }
		/// <summary>
		/// Leave blank for default
		/// </summary>
		public string Community { get; set; }
	}
}
