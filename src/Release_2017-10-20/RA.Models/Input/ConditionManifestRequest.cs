using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace RA.Models.Input
{
	public class ConditionManifestRequest : BaseRequest
	{
		public ConditionManifestRequest()
		{
			//ConditionManifests = new List<ConditionManifest>();
			ConditionManifest = new ConditionManifest();
		}

		/// <summary>
		/// consider allowing multiple?
		/// Probably too many complications
		/// </summary>
		//public List<ConditionManifest> ConditionManifests { get; set; }

		public ConditionManifest ConditionManifest { get; set; }


	}

	public class ConditionManifest
	{
		public ConditionManifest()
		{
            EntryConditions = new List<ConditionProfile>();
			RequiredConditions = new List<ConditionProfile>();
			RecommendedConditions = new List<ConditionProfile>();
			CorequisiteConditions = new List<ConditionProfile>();
			OwningOrganization = new OrganizationReference();
		}

		public string Name { get; set; }
		public string Description { get; set; }
		public string SubjectWebpage { get; set; } //URL
		public string Ctid { get; set; }

		/// <summary>
		/// Organization that owns this credential
		/// </summary>
		public OrganizationReference OwningOrganization { get; set; }

		/// <summary>
		/// Required condition profiles
		/// </summary>
		public List<ConditionProfile> RequiredConditions { get; set; }
		/// <summary>
		/// Recommended condition profiles
		/// </summary>
		public List<ConditionProfile> RecommendedConditions { get; set; }

		/// <summary>
		/// Entry Conditions condition profiles
		/// </summary>
		public List<ConditionProfile> EntryConditions { get; set; }

		/// <summary>
		/// Corequisite Conditions
		/// ceterms:corequisite
		/// The resource being referenced must be pursued concurrently with the resource being described.
		/// </summary>
		/// <see cref="http://purl.org/ctdl/terms/corequisite"/>
		/// <remarks>Includes dual (double) degrees that cannot be earned independently of each other.</remarks>
		public List<ConditionProfile> CorequisiteConditions { get; set; }
	}
}
