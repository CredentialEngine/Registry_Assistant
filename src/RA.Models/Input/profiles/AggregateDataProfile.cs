using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input.profiles.QData;

namespace RA.Models.Input
{
	public class AggregateDataProfile
	{
	
		/// <summary>
		/// Effective date of this profile
		/// </summary>
		public string DateEffective { get; set; }

		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// A currency code, for ex USD
		/// Optional
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// DemographicInformation
		/// Aggregate data or summaries of statistical data relating to the population of credential holders including data about gender, geopolitical regions, age, education levels, and other categories of interest.
		/// </summary>
		public string DemographicInformation { get; set; }
		/// <summary>
		/// DemographicInformation - language map
		/// </summary>
		public LanguageMap DemographicInformation_Map { get; set; } = new LanguageMap();

		/// <summary>
		///  Upper interquartile earnings.
		/// </summary>
		public int HighEarnings { get; set; }

		/// <summary>
		///  Number of jobs obtained in the region during a given timeframe.
		///  ceterms:jobsObtained
		/// </summary>
		public List<QuantitativeValue> JobsObtained { get; set; }

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
		///  Number of credentials awarded.
		///  ceterms:numberAwarded
		/// </summary>
		public int NumberAwarded { get; set; }

		/// <summary>
		/// Number of months after earning a credential when employment and earnings data is collected.
		/// Number of months usually range between 3 months (one quarter) to ten years.
		/// </summary>
		public int PostReceiptMonths { get; set; }

		/// <summary>
		/// Authoritative source of an entity's information.
		/// URL
		/// ceterms:source
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Relevant Data Set
		/// Data Set on which earnings or employment data is based.
		/// qdata:relevantDataSet
		/// </summary>
		public List<DataSetProfile> RelevantDataSet { get; set; } = new List<DataSetProfile>();

		/// <summary>
		/// Typically the DataSetProfile information will be published with the credential. 
		/// In the future there could be use cases where a reference to an existing dataSetProfile will be published with the credential.
		/// </summary>
		public List<string> RelevantDataSetList { get; set; }
	}
}
