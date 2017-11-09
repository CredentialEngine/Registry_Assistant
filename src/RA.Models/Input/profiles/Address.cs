using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class PostalAddress 
	{
		public PostalAddress()
		{
			ContactPoint = new List<ContactPoint>();
		}
		public string Name { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string PostOfficeBoxNumber { get; set; }
		public string City { get; set; }
		public string AddressRegion { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }

		public List<ContactPoint> ContactPoint { get; set; }
	}



}
