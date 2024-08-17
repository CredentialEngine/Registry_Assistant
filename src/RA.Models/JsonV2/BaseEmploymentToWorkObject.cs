using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class BaseEmploymentToWorkObject : BaseResourceDocument
	{
		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:inCatalog" )]
		public string InCatalog { get; set; }

		/// <summary>
		/// Type of official status of the resource; 
		/// URI to a concept
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:lifeCycleStatusType" )]
		public CredentialAlignmentObject LifeCycleStatusType { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetCompetency" )]
		public List<CredentialAlignmentObject> TargetCompetency { get; set; }

		/// <summary>
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// ceterms:versionIdentifier
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:versionIdentifier" )]
		public List<IdentifierValue> VersionIdentifier { get; set; }

	}
}
