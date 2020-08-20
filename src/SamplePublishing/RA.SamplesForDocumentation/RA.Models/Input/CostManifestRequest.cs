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

		//public List<BlankNode> BlankNodes = new List<BlankNode>();
	}

	public class CostManifest
	{
		public CostManifest()
		{
			//OwningOrganization = new OrganizationReference();
			EstimatedCost = new List<CostProfile>();
		}

        public string Name { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Name_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// Description 
        /// Required
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

        public string StartDate { get; set; }
		public string EndDate { get; set; }

		public string Ctid { get; set; }
		/// <summary>
		/// URL for cost details
		/// </summary>
		public string CostDetails { get; set; }

		/// <summary>
		/// Organization that owns this CostManifest
		/// </summary>
		public OrganizationReference CostManifestOf { get; set; } = new OrganizationReference();
        //public OrganizationReference OwningOrganization
        //{
        //    get { return CostManifestOf; }
        //    set { CostManifestOf = value; }
        //}
        public List<CostProfile> EstimatedCost { get; set; }
	}
}
