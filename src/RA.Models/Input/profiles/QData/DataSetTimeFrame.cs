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
    /// - StartDate (otherwise doesn't make sense)
    /// - DataAttributes (or doesn't make sense)
    /// https://credreg.net/qdata/terms/DataSetTimeFrame
    /// </summary>
    public class DataSetTimeFrame
	{

        /// <summary>
        /// Description of profile
        /// RECOMMENDED - or should it be Required?
        /// </summary>
        public string Description { get; set; }
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

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
        /// Attributes of the data set.
		/// HIGHLY RECOMMENDED or REQUIRED?
        /// qdata:DataProfile
        /// </summary>
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
		public List<string> DataSourceCoverageType { get; set; } = new List<string>();


        /// <summary>
        /// Name of profile
        /// OPTIONAL
        /// </summary>
        public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

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
