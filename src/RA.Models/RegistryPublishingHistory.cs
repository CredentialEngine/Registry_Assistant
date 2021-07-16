using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	public class RegistryPublishingHistory
	{
		//public int Id { get; set; }
		public string Password { get; set; }
		public string Environment { get; set; }
		public System.DateTime Created { get; set; }
		//CTID of org which owns the data
		public string DataOwnerCTID { get; set; }
		//CTID of org which published the data
		public string PublisherIdentifier { get; set; }
		public string PublishMethodURI { get; set; }
		public string PublishingEntityType { get; set; }
		public string CtdlType { get; set; }
		public string EntityCtid { get; set; }
		public string EntityName { get; set; }
		public bool isAddTransaction { get; set; }
		public bool WasChanged { get; set; }
		public string PublishInput { get; set; }
		public string PublishPayload { get; set; }
		
		public string EnvelopeId { get; set; }
	}
}
