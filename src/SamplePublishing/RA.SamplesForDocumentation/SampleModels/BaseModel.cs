
using System.Collections.Generic;

using RA.Models.Input;

namespace RA.SamplesForDocumentation.SampleModels
{
	public class BaseModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string SubjectWebpage { get; set; }
		public string CTID { get; set; }
		public string DateEffective { get; set; }

		public List<Place> Address { get; set; } = new List<Place>();
	}
	
}
