using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.SamplesForDocumentation.SampleModels
{
	public class Credential : BaseModel
	{
		public string AvailabilityListing { get; set; }
		public string AvailableOnlineAt { get; set; }
		public string CodedNotation { get; set; }
		public string ImageUrl { get; set; }
		public string LatestVersionUrl { get; set; }
		public string PreviousVersion { get; set; }
		public List<string> Subject { get; set; }
		public List<string> Keyword { get; set; }
	}
}
