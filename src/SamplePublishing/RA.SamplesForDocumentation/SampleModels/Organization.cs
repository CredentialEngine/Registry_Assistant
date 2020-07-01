using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.SamplesForDocumentation.SampleModels
{
	public class Organization : BaseModel
	{
		public string OrganizationType { get; set; } = "CredentialOrganization";

		public string ImageUrl { get; set; }
		/// <summary>
		/// The type of the described agent.
		/// Must provide valid organization types.
		/// May provide with or without the orgType namespace
		/// orgType:CertificationBody
		/// </summary>
		public List<string> AgentType { get; set; }

		public List<string> Keywords { get; set; }
	}
}
