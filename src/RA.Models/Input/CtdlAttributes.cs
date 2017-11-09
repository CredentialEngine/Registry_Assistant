using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RA.Models.input
{
	
	public class CtdlAttributes : Attribute
	{
		public CtdlAttributes() { }
		public bool IsRequired{ get; set; }
		public bool IsMulti { get; set; }
		public string Display { get; set; }
		public string Description { get; set; }
	}

}
