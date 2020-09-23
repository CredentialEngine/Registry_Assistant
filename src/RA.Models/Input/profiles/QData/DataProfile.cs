using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles.QData
{
	/// <summary>
	/// Data Profile
	/// Entity describing the attributes of the data set, its subjects and their values.
	/// qdata:DataProfile
	/// </summary>
	public class DataProfile
	{
		/// <summary>
		/// Describes whether and how the provided earnings have been adjusted for factors such as inflation, participant demographics and economic conditions.
		/// qdata:adjustment
		/// </summary>
		public string Adjustment { get; set; }
		public LanguageMap Adjustment_Map { get; set; }

		/// <summary>
		/// Type of administrative record used; e.g. W2, 1099, and Unemployment Insurance Wage Record.
		/// qdata:administrativeRecordType
		/// skos:Concept
		/// <see cref="https://credreg.net/qdata/terms/administrativeRecordType#AdministrativeRecordCategory"/>
		/// adminRecord:Tax1099
		/// adminRecord:TaxW2
		/// adminRecord:UnemploymentInsurance
		/// </summary>
		public string AdministrativeRecordType { get; set; }

		/// <summary>
		/// Number of credential holders in the reporting group for which employment and earnings data is included in the data set.
		/// qdata:dataAvailable
		/// </summary>
		public List<QuantitativeValue> DataAvailable { get; set; } = new List<QuantitativeValue>();

		/// <summary>
		/// Number of credential holders in the reporting group for which employment and earnings data has not been included in the data set.
		/// qdata:dataNotAvailable
		/// </summary>
		public List<QuantitativeValue> DataNotAvailable { get; set; } = new List<QuantitativeValue>();


		/// <summary>
		/// Earnings rate for a demographic category.
		/// qdata:demographicEarningsRate
		/// </summary>
		public List<QuantitativeValue> DemographicEarningsRate { get; set; } = new List<QuantitativeValue>();

		/// <summary>
		/// Employment rate for a demographic category.
		/// qdata:demographicEmploymentRate
		/// </summary>
		public List<QuantitativeValue> DemographicEmploymentRate { get; set; } = new List<QuantitativeValue>();

		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Reference to an entity describing aggregate earnings.
		/// qdata:earningsAmount
		/// </summary>
		public MonetaryAmount EarningsAmount { get; set; } = new MonetaryAmount();

		/// <summary>
		/// Definition of "earnings" used by the data source in the context of the reporting group.
		/// qdata:earningsDefinition
		/// </summary>
		public string EarningsDefinition { get; set; }
		public LanguageMap EarningsDefinition_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Reference to an entity describing median earnings as well as earnings at various percentiles.
		/// qdata:earningsDistribution
		/// schema:MonetaryAmountDistribution
		/// </summary>
		public MonetaryAmountDistribution EarningsDistribution { get; set; } = new MonetaryAmountDistribution();

		/// <summary>
		/// Statement of any work time or earnings threshold used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the time period of the data set.
		/// qdata:earningsThreshold
		/// </summary>
		public string EarningsThreshold { get; set; }
		public LanguageMap EarningsThreshold_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Statement of criteria used to determine whether sufficient levels of work time and/or earnings have been met to be considered employed during the earning time period.
		/// qdata:employmentDefinition
		/// </summary>
		public string EmploymentDefinition { get; set; }
		public LanguageMap EmploymentDefinition_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Rate computed by dividing the number of holders or subjects meeting the data set's criteria of employment (meetEmploymentCriteria) by the number of holders or subjects for which data was available (dataAvailable).
		/// qdata:employmentRate
		/// </summary>
		public QuantitativeValue EmploymentRate { get; set; } = new QuantitativeValue();

		/// <summary>
		///  Number of credential holders in the final data collection and reporting.
		/// qdata:holdersInSet
		/// </summary>
		public QuantitativeValue HoldersInSet { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Mechanism by which income is determined; i.e., actual or annualized earnings.
		/// qdata:incomeDeterminationType
		/// skos:Concept
		/// <see cref="https://credreg.net/qdata/terms/IncomeDeterminationMethod#IncomeDeterminationMethod"/>
		/// incomeDetermination:ActualEarnings 
		/// incomeDetermination:AnnualizedEarnings
		/// </summary>
		public string IncomeDeterminationType { get; set; }

		/// <summary>
		/// Employment rate for an industry category.
		/// qdata:industryRate
		/// </summary>
		public QuantitativeValue IndustryRate { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Number of holders that do not meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set (employmentDefinition).
		/// qdata:insufficientEmploymentCriteria
		/// </summary>
		public QuantitativeValue InsufficientEmploymentCriteria { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Number of holders that meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set (employmentDefinition).
		/// qdata:meetEmploymentCriteria
		/// </summary>
		public QuantitativeValue MeetEmploymentCriteria { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Non-holders who departed or are likely to depart higher education prematurely.
		/// qdata:nonCompleters
		/// </summary>
		public QuantitativeValue NonCompleters { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Non-holder subject actively pursuing the credential through a program or assessment.
		/// qdata:nonHoldersInSet
		/// </summary>
		public QuantitativeValue NonHoldersInSet { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Employment rate for an occupation category.
		/// qdata:occupationRate
		/// </summary>
		public QuantitativeValue OccupationRate { get; set; } = new QuantitativeValue();

		/// <summary>
		///  Reference to an entity describing median earnings as well as earnings at various percentiles for holders or subjects in the region.
		/// qdata:regionalEarningsDistribution
		/// </summary>
		public QuantitativeValue RegionalEarningsDistribution { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Rate computed by dividing the number of holders or subjects in the region meeting the data set's criteria of employment (meetEmploymentCriteria) by the number of holders or subjects in the region for which data was available (dataAvailable).
		/// qdata:regionalEmploymentRate
		/// </summary>
		public QuantitativeValue RegionalEmploymentRate { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Number of people employed in the area of work (e.g., industry, occupation) in which the credential provided preparation.
		/// qdata:relatedEmployment
		/// </summary>
		public QuantitativeValue RelatedEmployment { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Category of subject excluded from the data.
		/// qdata:subjectExcluded
		/// </summary>
		public SubjectProfile SubjectExcluded { get; set; } = new SubjectProfile();

		/// <summary>
		/// Category of subject included in the data.
		/// qdata:subjectIncluded
		/// </summary>
		public SubjectProfile SubjectIncluded { get; set; } = new SubjectProfile();

		/// <summary>
		/// Total credential holders and non-holders in the final data collection and reporting.
		/// qdata:subjectsInSet
		/// </summary>
		public QuantitativeValue SubjectsInSet { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Number of holders that meet the prescribed employment threshold in terms of earnings or time engaged in work as defined for the data set (employmentDefinition).
		/// qdata:sufficientEmploymentCriteria
		/// </summary>
		public QuantitativeValue SufficientEmploymentCriteria { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Number of people employed outside the area of work (e.g., industry, occupation) in which the credential provided preparation.
		/// qdata:unrelatedEmployment
		/// </summary>
		public QuantitativeValue UnrelatedEmployment { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Statement of earnings thresholds used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the chosen employment and earnings time period.
		/// qdata:workTimeThreshold
		/// </summary>
		public string WorkTimeThreshold { get; set; }
		public LanguageMap WorkTimeThreshold_Map { get; set; } = new LanguageMap();
	}
}
