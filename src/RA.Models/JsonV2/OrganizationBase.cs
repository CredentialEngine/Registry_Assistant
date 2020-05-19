using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Base class to use for references to organizations that are not in the registry
	/// </summary>
	public class OrganizationBase : EntityBase
	{
		public OrganizationBase()
		{
			//SubjectWebpage = new List<string>();
			SocialMedia = new List<string>();
		}
		
		/// <summary>
		/// The type of organization is one of :
		/// - CredentialOrganization
		/// - QACredentialOrganization
		/// </summary>

		[JsonProperty( "@type" )]
		public new string Type { get; set; }
		
		[JsonProperty( PropertyName = "ceterms:socialMedia" )]
		public List<string> SocialMedia { get; set; }

		public override void NegateNonIdProperties()
		{
			Type = null;
			Name = null;
			Description = null;
			SubjectWebpage = null;
			Ctid = null;
			SocialMedia = null;
		}
	}


}
