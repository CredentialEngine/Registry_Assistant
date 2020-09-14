using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;

namespace RA.SamplesForDocumentation.SampleModels
{
	public class BaseModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string SubjectWebpage { get; set; }
		public string Ctid { get; set; }
		public string DateEffective { get; set; }

		public Place Address { get; set; } = new Place();
	}
	
}
