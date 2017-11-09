using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class CostManifestRequest : BaseRequest
	{
		public CostManifestRequest()
		{
			//CostManifests = new List<CostManifest>();
			CostManifest = new CostManifest();
		}
		//TODO - only allow single or multiple???
		public CostManifest CostManifest { get; set; }
		//public List<CostManifest> CostManifests { get; set; }


	}

	public class CostManifest
	{
		public CostManifest()
		{
			OwningOrganization = new OrganizationReference();
			EstimatedCosts = new List<CostProfile>();
		}

		public string Name { get; set; }
		public string Description { get; set; }

		public string StartDate { get; set; }
		public string EndDate { get; set; }

		public string Ctid { get; set; }
		/// <summary>
		/// URL for cost details
		/// </summary>
		public string CostDetails { get; set; }

		/// <summary>
		/// Organization that owns this credential
		/// </summary>
		public OrganizationReference OwningOrganization { get; set; }

		public List<CostProfile> EstimatedCosts { get; set; }
	}
}
