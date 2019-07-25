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
		public System.DateTime Created { get; set; }
		public string DataOwnerCTID { get; set; }
		public string PublishIdentifier { get; set; }
		public string PublishMethodURI { get; set; }
		public string PublishingEntityType { get; set; }
		public string CtdlType { get; set; }
		public string EntityCtid { get; set; }
		public string EntityName { get; set; }
		public string PublishInput { get; set; }
		public string PublishPayload { get; set; }
		public string Environment { get; set; }
		public string EnvelopeId { get; set; }
	}
}
