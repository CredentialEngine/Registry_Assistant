using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// CostProfile class
    /// 2018-09-02 Where LanguageMap alternates are available, only enter one. The system will check the string version first. 
    /// </summary>
    //public class CostProfile : ICloneable
    public class CostProfile 
	{
        /// <summary>
        /// Inialize lists
        /// </summary>
        public CostProfile()
        {
            Jurisdiction = new List<Jurisdiction>();
            //Region = new List<GeoCoordinates>();
            CostItems = new List<CostProfileItem>();
            //ResidencyType = new List<string>();
            //AudienceType = new List<string>();
            //DirectCostType = new List<string>();
			Condition = new List<string>();

		}

        /// <summary>
        /// Details URL
        /// </summary>
        public string CostDetails { get; set; }
        /// <summary>
        /// A currency code, for ex USD
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Name of this cost profile
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Name_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// Description of cost profile
        /// Required
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// Start date or effective date of this cost profile
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// End date or expiry date of this cost profile
        /// </summary>
        public string EndDate { get; set; }

		/// <summary>
		/// List of condtions, containing:
		/// A single condition or aspect of experience that refines the conditions under which the resource being described is applicable.
		/// </summary>
		public List<string> Condition { get; set; }
        public LanguageMapList Condition_Map { get; set; } = new LanguageMapList();

        /// <summary>
        /// List of jurisdictions
        /// </summary>
        public List<Jurisdiction> Jurisdiction { get; set; }


		/// <summary>
		/// List of cost items
		/// This seems to be the logical approach at this time
		/// </summary>
		public List<CostProfileItem> CostItems { get; set; }

    }

	/// <summary>
	/// Cost item class
	/// </summary>
	public class CostProfileItem
	{
		/// <summary>
		/// Initialize
		/// </summary>
		public CostProfileItem()
		{
			ResidencyType = new List<string>();
			AudienceType = new List<string>();
		}

		/// <summary>
		/// Must be a valid CTDL cost type
		/// </summary>
		public string DirectCostType { get; set; }
		/// <summary>
		/// List of Residency items
		/// </summary>
		public List<string> ResidencyType { get; set; }

		/// <summary>
		/// List of Audience Types
		/// </summary>
		public List<string> AudienceType { get; set; }

		/// <summary>
		/// Payment Pattern
		/// </summary>
		public string PaymentPattern { get; set; }
        public LanguageMap PaymentPattern_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// Price for this cost - optional
        /// </summary>
        public decimal Price { get; set; }




	}
	/// <summary>
	/// Version of Cost Profile where all cost items are flattened into a single record.
	/// That is:
	/// - if there is a single cost profile with three costs
	/// - Output will be three cost profiles with the cost profile specific properties repeated for each cost profile item
	/// </summary>
	public class CostProfileFlattened : CostProfile
	{
		public CostProfileFlattened()
		{
			ResidencyType = new List<string>();
			AudienceType = new List<string>();
		}
		/// <summary>
		/// Must be a valid CTDL cost type
		/// </summary>
		public string DirectCostType { get; set; }
		/// <summary>
		/// List of Residency items
		/// </summary>
		public List<string> ResidencyType { get; set; }

		/// <summary>
		/// List of Audience Types
		/// </summary>
		public List<string> AudienceType { get; set; }

		/// <summary>
		/// Price for this cost - optional
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Payment Pattern
		/// </summary>
		public string PaymentPattern { get; set; }
	}


}
