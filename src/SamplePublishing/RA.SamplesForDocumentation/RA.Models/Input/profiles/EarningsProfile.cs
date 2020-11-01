using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input.profiles.QData;

namespace RA.Models.Input
{
	/// <summary>
	/// Earnings Profile
	/// Entity that describes earning and related statistical information for a given credential.
	/// </summary>
	public class EarningsProfile
	{

		/// <summary>
		/// Effective date of this profile
		/// </summary>
		public string DateEffective { get; set; }

		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		///  Upper interquartile earnings.
		/// </summary>
		public int HighEarnings { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();

		/// <summary>
		///  Lower interquartile earnings.
		/// </summary>
		public int LowEarnings { get; set; }

		/// <summary>
		///  Median earnings.
		/// </summary>
		public int MedianEarnings { get; set; }

		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Number of months after earning a credential when employment and earnings data is collected.
		/// Number of months usually range between 3 months (one quarter) to ten years.
		/// </summary>
		public int PostReceiptMonths { get; set; }
		//public decimal Region { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// URL
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Relevant Data Set
		/// Data Set on which earnings or employment data is based.
		/// qdata:relevantDataSet
		/// </summary>
		public List<DataSetProfile> RelevantDataSet { get; set; } = new List<DataSetProfile>();

	}
}
