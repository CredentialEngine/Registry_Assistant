using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	/// <summary>
	/// Request class for a transfer of owner request
	/// </summary>
	public class TransferOwnerRequest
	{
		/// <summary>
		/// CTID of the current owning organization.
		/// </summary>
		public string DataOwnerCTID { get; set; }

		/// <summary>
		/// CTID for the organization to which ownership is being transferred.
		/// </summary>
		public string NewOwnerCTID { get; set; }

		/// <summary>
		/// CTDL type of the resource being transfer.
		/// This is used a check to ensure accuracy. 
		/// </summary>
		public string PublishingEntityType { get; set; }

		/// <summary>
		/// List of CTIDs for the resource(s) being transferred. 
		/// </summary>
		public List<string> EntityCTIDList { get; set; } = new List<string>();

		/// <summary>
		/// Community/registry for the request. 
		/// If blank, defaults to main registry for the environment. 
		/// </summary>
		public string Community { get; set; }
	}

	public class TransferOwnerResponse
	{
		public TransferOwnerResponse()
		{
			Messages = new List<string>();
			ResultContent = string.Empty;
		}

		/// True if action was successfull, otherwise false
		public bool Successful { get; set; }

		/// <summary>
		/// List of error or warning messages
		/// </summary>
		public List<string> Messages { get; set; }

		/// <summary>
		/// Payload of request to registry, containing properties formatted as CTDL - JSON-LD
		/// </summary>
		public string ResultContent { get; set; }
	}
}
