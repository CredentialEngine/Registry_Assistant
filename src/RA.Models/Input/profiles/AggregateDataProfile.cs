
using System.Collections.Generic;

using RA.Models.Input.profiles.QData;

namespace RA.Models.Input
{
    public class AggregateDataProfile
    {

        /// <summary>
        /// Effective date of this profile
        /// </summary>
        public string DateEffective { get; set; }
        public string ExpirationDate { get; set; }

        /// <summary>
        /// Profile description 
        /// Optional
        /// Minimum of 15 characters when present, but should be clear.
        /// </summary>
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
		public decimal? HighEarnings { get; set; }

        /// <summary>
        ///  Number of jobs obtained in the region during a given timeframe.
        ///  ceterms:jobsObtained
        /// </summary>
        public List<QuantitativeValue> JobsObtained { get; set; }

        /// <summary>
        /// Jurisdiction Profile
        /// Geo-political information about applicable geographic areas and their exceptions.
        /// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
        /// </summary>
        public List<JurisdictionProfile> Jurisdiction { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		///  Lower interquartile earnings.
		/// </summary>
		public decimal? LowEarnings { get; set; }

		/// <summary>
		///  Median earnings.
		/// </summary>
		public decimal? MedianEarnings { get; set; }

        /// <summary>
        /// Optional name for this profile
        /// </summary>
        public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();


        /// <summary>
        /// List of Alternate Names for this resource
        /// </summary>
        public List<string> AlternateName { get; set; } = new List<string>();
        /// <summary>
        /// LanguageMap for AlternateName
        /// </summary>
        public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

        /// <summary>
        /// Faculty-to-Student Ratio
        /// Ratio of the number of teaching faculty to the number of students.
        /// The expression of the ratio should feature the number of faculty first, followed by the number of students, e.g., "1:10" to mean "one faculty per ten students".
        /// qdata:facultyToStudentRatio
        /// </summary>
        public string FacultyToStudentRatio { get; set; }

        /// <summary>
        ///  Number of credentials awarded.
        ///  ceterms:numberAwarded
        /// </summary>
        public int? NumberAwarded { get; set; }

        /// <summary>
        /// Number of months after earning a credential when employment and earnings data is collected.
        /// Number of months usually range between 3 months (one quarter) to ten years.
        /// </summary>
        public int? PostReceiptMonths { get; set; }

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
        /// Typically the DataSetProfile information will be published with the credential/learning opp, etc.
        /// In the future there could be use cases where a reference to an existing dataSetProfile will be published with the credential.
        /// </summary>
        public List<string> RelevantDataSetList { get; set; }
    }
}
