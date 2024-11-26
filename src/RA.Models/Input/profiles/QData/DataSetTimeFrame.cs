using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public DataSetTimeFrame()
        {
            Type = "qdata:DataSetTimeFrame";
        }
        [JsonProperty( "@type" )]
        public string Type { get; set; }

        /// <summary>
        /// An identifier for use with blank nodes. 
        /// It will be ignored if included with a primary resource
        /// </summary>
        [JsonProperty( "@id" )]
        public string BlankNodeId { get; set; }

        #region For Data Set Temporal Coverage, Require at least one of Start date, end date or time interval
        /// <summary>
        /// Length of the interval between two events. 
        /// The input is in ISO_8601 format. 
        /// <seealso href="https://en.wikipedia.org/wiki/ISO_8601#Durations">ISO_8601 Durations</seealso>
        /// Examples:
        ///     5 years - P5Y
        ///     4.5 years - P4.5Y
        ///     10 months - P10M
        ///     26 weeks - P26W
        /// schema:Duration
        /// </summary>
        public string TimeInterval { get; set; }

        /// <summary>
        /// Start date for the profile
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// End date for the profile
        /// </summary>
        public string EndDate { get; set; }

        #endregion

        /// <summary>
        /// Description of profile
        /// RECOMMENDED 
        /// </summary>
        public string Description { get; set; }

        [JsonProperty( PropertyName = "ceterms:description" )]
        public LanguageMap Description_Map { get; set; }

        /// <summary>
        /// Description of profile
        /// RECOMMENDED 
        /// </summary>
        public string Description { get; set; }
        public LanguageMap Description_Map { get; set; }

        /// <summary>
        /// Attributes of the data set.
        /// HIGHLY RECOMMENDED
        /// 2024-07-31 This profile is now (MOSTLY) obsolete.
        /// qdata:DataProfile
        /// </summary>
        [Obsolete( "2024-10-01 DataAttributes is deprecated, but need to handle import of older data." )]
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
        [Obsolete( "2024-07-30 DataSourceCoverageType is deprecated" )]
        public List<string> DataSourceCoverageType { get; set; } = new List<string>();


        /// <summary>
        /// Name or title of the resource.
        /// OPTIONAL
        /// </summary>
        public string Name { get; set; }

        [JsonProperty( PropertyName = "ceterms:name" )]
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
