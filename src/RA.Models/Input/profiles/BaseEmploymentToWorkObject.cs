using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class BaseEmploymentToWorkObject : BasePrimaryResource
	{
		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		public string InCatalog { get; set; }

		/// <summary>
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Required
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// A competency relevant to the resource being described.
		/// Sep/2023 - Decision was made to use CredentialAlignmentObject to be compatible with datatype used by condition profile
		/// Range: CredentialAlignmentObject
		/// </summary>
		public List<CredentialAlignmentObject> TargetCompetency { get; set; }

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner.
		/// The resource version captured here is any local identifier used by the resource owner to identify the version of the resource in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();

	}
}
