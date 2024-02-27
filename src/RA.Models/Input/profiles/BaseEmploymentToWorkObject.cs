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
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Required
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		public string InCatalog { get; set; }

		/// <summary>
		/// A competency relevant to the resource being described.
		/// Range: ceasn:Competency
		/// </summary>
		//public List<string> TargetCompetencyOld { get; set; } = new List<string>();

		/// <summary>
		/// A competency relevant to the resource being described.
		/// Sep/2023 - Decision was made to use CredentialAlignmentObject to be compatible with datatype used by condition profile
		/// Range: CredentialAlignmentObject
		/// </summary>
		public List<CredentialAlignmentObject> TargetCompetency { get; set; }
	}
}
