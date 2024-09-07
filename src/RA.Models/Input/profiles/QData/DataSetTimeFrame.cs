using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles.QData
{
    /// <summary>
    /// DataSet Time Frame
    /// Time frame including earnings and employment start and end dates of the data set.
    /// Required:
    /// Requires at least one of: timeInterval, startDate, endDate
    /// https://credreg.net/qdata/terms/DataSetTimeFrame
    /// </summary>
    public class DataSetTimeFrame
	{
        /// <summary>
        /// Length of the interval between two events. 
        /// </summary>
        public DurationProfile TimeInterval { get; set; }

        /// <summary>
        /// Start date for the profile
        /// REQUIRED
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// End date for the profile
        /// REQUIRED
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Description of profile
        /// RECOMMENDED 
        /// </summary>
        public string Description { get; set; }
        public LanguageMap Description_Map { get; set; }

        /// <summary>
        /// Attributes of the data set.
		/// HIGHLY RECOMMENDED
        /// 2024-07-31 This profile is now obsolete.
        /// qdata:DataProfile
        /// </summary>
        [Obsolete]
        public List<DataProfile> DataAttributes { get; set; } = new List<DataProfile>();

        /// <summary>
        /// Data Source Coverage Type
        /// Type of geographic coverage of the subjects.
        /// <see cref="https://credreg.net/qdata/terms/dataSourceCoverageType"/>
        /// skos:Concept
        /// <see cref="https://credreg.net/qdata/terms/DataSourceCoverage"/>
        /// sourceCoverage:Country
        ///	sourceCoverage:Global
        ///	sourceCoverage:Region
        ///	sourceCoverage:StateOrProvince
        ///	sourceCoverage:UrbanArea
        /// </summary>
        [Obsolete]
        public List<string> DataSourceCoverageType { get; set; } = new List<string>();


        /// <summary>
        /// Name or title of the resource.
        /// OPTIONAL
        /// </summary>
        public string Name { get; set; }
		public LanguageMap Name_Map { get; set; }

        /// <summary>
        /// List of Alternate Names for this resource
        /// OPTIONAL
        /// </summary>
        public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();


	}
}
