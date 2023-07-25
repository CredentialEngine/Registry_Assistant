using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input.profiles.QData;

namespace RA.Models.Input
{
	/// <summary>
	/// Entity describing the count and related statistical information of holders of a given credential.
	/// </summary>
	public class HoldersProfile
	{
		public string CTID { get; set; }

		/// <summary>
		/// Effective date of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Description of this profile
		/// ceterms:description
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// DemographicInformation
		/// Aggregate data or summaries of statistical data relating to the population of credential holders including data about gender, geopolitical regions, age, education levels, and other categories of interest.
		/// </summary>
		public string DemographicInformation { get; set; }
		/// <summary>
		/// DemographicInformation - language map
		/// </summary>
		public LanguageMap DemographicInformation_Map { get; set; } = new LanguageMap();

		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		///  Number of credentials awarded.
		///  ceterms:numberAwarded
		/// </summary>
		public int NumberAwarded { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// ceterms:jurisdiction
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; } = new List<JurisdictionProfile>();

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
		//21-02-19 mparsons - change to external reference by URI/CTID
		public List<string> RelevantDataSetList { get; set; } = new List<string>();
	}
}
