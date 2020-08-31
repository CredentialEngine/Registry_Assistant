using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	public class TransferOwnerRequest
	{
		public string DataOwnerCTID { get; set; }
		public string NewOwnerCTID { get; set; }
		public string PublishingEntityType { get; set; }
		public string EntityCtid { get; set; }
		public string Community { get; set; }
	}
	public class TransferOwnerResponse
	{
		public TransferOwnerResponse()
		{
			Messages = new List<string>();
			ResultContent = "";
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
