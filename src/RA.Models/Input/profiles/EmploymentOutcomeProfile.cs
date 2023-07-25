using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input.profiles.QData;

namespace RA.Models.Input
{
	/// <summary>
	/// Employment Outcome Profile
	/// Entity that describes employment outcomes and related statistical information for a given credential.
	/// ceterms:EmploymentOutcomeProfile
	/// <see cref="https://purl.org/ctdl/terms/EmploymentOutcomeProfile"/>
	/// </summary>
	public class EmploymentOutcomeProfile
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
		///  Number of jobs obtained in the region during a given timeframe.
		///  21-02-19 - UPDATE added JobsObtainedList, which uses QuantitativeValue. This allows for providing percentage data - typically with a description. 
		///  ceterms:jobsObtained
		/// </summary>
		public List<QuantitativeValue> JobsObtainedList { get; set; }
		/// <summary>
		/// Where JobsObtained is a simple integer, this property can be used. 
		/// </summary>
		public int JobsObtained { get; set; }
		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// ceterms:jurisdiction
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; } = new List<JurisdictionProfile>();

		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

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
		public List<DataSetProfile> RelevantDataSet { get; set; }
		public List<string> RelevantDataSetList { get; set; }
	}
}
