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
		public string Ctid { get; set; }

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
		///  ceterms:jobsObtained
		/// </summary>
		public int JobsObtained { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// ceterms:jurisdiction
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();

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
		public List<DataSetProfile> RelevantDataSet { get; set; } = new List<DataSetProfile>();
	}
}
