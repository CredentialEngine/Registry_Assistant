using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class CostManifestRequest
	{
		public CostManifestRequest()
		{
			//CostManifests = new List<CostManifest>();
		}
		//TODO - only allow single or multiple???
		public CostManifest CostManifest { get; set; }
		//public List<CostManifest> CostManifests { get; set; }

		/// <summary>
		/// API key for the requesting partner - required for publishing
		/// May ultimately be passed in the header
		/// </summary>
		public string APIKey { get; set; }

		/// <summary>
		/// Envelope Identifier
		/// Currently required to update an existing document
		/// </summary>
		public string RegistryEnvelopeId { get; set; }
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
