using System.Collections.Generic;

namespace RA.Models.Input
{
	public class CostManifestRequest : BaseRequest
	{
		public CostManifestRequest()
		{
			CostManifest = new CostManifest();
		}
		public CostManifest CostManifest { get; set; }
	}

	public class CostManifest
	{
		public CostManifest()
		{
			EstimatedCost = new List<CostProfile>();
		}
		/// <summary>
		/// Required
		/// </summary>
		public string CTID { get; set; }

        /// <summary>
        /// Description 
        /// Required
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// URL for cost details
		/// Required
		/// </summary>
		public string CostDetails { get; set; }

		/// <summary>
		/// Cost Manifest Name
		/// Recommended
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		public string StartDate { get; set; }
		public string EndDate { get; set; }

		/// <summary>
		/// Organization that owns this CostManifest
		/// </summary>
		public OrganizationReference CostManifestOf { get; set; } = new OrganizationReference();
		/// <summary>
		/// List of cost profiles
		/// </summary>
        public List<CostProfile> EstimatedCost { get; set; }
	}
}
