using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Json
{
	/// <summary>
	/// Base class to use for references to organizations that are not in the registry
	/// </summary>
	public class OrganizationBase : EntityBase
	{
		public OrganizationBase()
		{
			SubjectWebpage = new List<IdProperty>();
			SocialMedia = new List<IdProperty>();
		}
		
		/// <summary>
		/// The type of organization is one of :
		/// - CredentialOrganization
		/// - QACredentialOrganization
		/// </summary>

		[JsonProperty( "@type" )]
		public new string Type { get; set; }
		
		[JsonProperty( PropertyName = "ceterms:socialMedia" )]
		public List<IdProperty> SocialMedia { get; set; }

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

	/// <summary>
	/// Base class to use for references to entities that are not in the registry
	/// </summary>
	public class EntityBase
	{
		public EntityBase()
		{
			SubjectWebpage = new List<IdProperty>();
		}

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }


		/// <summary>
		/// The type of the referenced entity
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:name" )]
		public string Name { get; set; }

		[JsonProperty( "ceterms:description" )]
		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string Ctid { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public List<IdProperty> SubjectWebpage { get; set; } //URL


		public virtual void NegateNonIdProperties()
		{
			Type = null;
			Name = null;
			Description = null;
			SubjectWebpage = null;
			Ctid = null;
		}
	}
}
