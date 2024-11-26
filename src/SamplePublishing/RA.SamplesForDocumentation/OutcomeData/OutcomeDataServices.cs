using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;
using RA.Models.Input.profiles;
using RA.Models.Input.profiles.QData;

namespace RA.SamplesForDocumentation.OutcomeData
{
    public class OutcomeDataServices
    {
        public DataSetTimeFrame FormatPassRatesTimeFrame( DataSetTimeFrameDTO period )
        {
            int value = 0;
            decimal percent = 0M;
            var passRate = new QuantitativeValue()
            {
                Description = period.PassRateDescription
            };
            var subjectsInSet = new QuantitativeValue();

            if ( period != null && !string.IsNullOrWhiteSpace( period.StartDate ) )
            {
                if ( DateTime.TryParse( period.StartDate, out DateTime dateOut ) )
                {
                    period.StartDate = dateOut.ToString( "yyyy-MM-dd" );
                }
                if ( DateTime.TryParse( period.EndDate, out DateTime dateOut2 ) )
                {
                    period.EndDate = dateOut2.ToString( "yyyy-MM-dd" );
                }

                //
                if ( int.TryParse( period.ValueInput, out value ) )
                {
                    passRate.Value = value;
                }
                if ( decimal.TryParse( period.PercentageInput, out percent ) )
                {
                    passRate.Percentage = percent;
                }
                if ( int.TryParse( period.SubjectsInSetInput, out value ) )
                {
                    subjectsInSet.Value = value;
                }
                if ( passRate.Value > 0 && ( passRate.Percentage ?? 0 ) > 0 && subjectsInSet.Value > 0 )
                {
                    var timeFrame = new DataSetTimeFrame()
                    {
                        Description = period.Description,
                        StartDate = period.StartDate,
                        EndDate = period.EndDate
                    };
                    var dataProfile = new DataProfile()
                    {
                        PassRate = new List<QuantitativeValue>() { passRate },
                        SubjectsInSet = new List<QuantitativeValue>() { subjectsInSet }
                    };
                    timeFrame.DataAttributes.Add( dataProfile );
                    return timeFrame;
                }


            }//
            return null;
        }

    }

    public class OutcomeDataDTO
    {
        #region Required
        /// <summary>
        /// DataSetProfile CTID - Required 
        /// </summary>
        public string CTID { get; set; }


        /// <summary>
        /// Description
        /// Required
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// DataSetProfile name
        /// Required
        ///		- could generate a template like Outcome Data for ...
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Credentialing organization or a third party providing the data.
        /// </summary>
        public OrganizationReference DataProvider { get; set; } = new OrganizationReference();

        /// <summary>
        /// optional. Typically present for easy reference
        /// </summary>
        public string DataProviderName { get; set; }


        /// <summary>
        /// Credential Organization
		/// don't need the organization for publishing
        /// or where can this be used for reference
        /// </summary>
        public OrganizationReference Organization { get; set; } = new OrganizationReference();
        //optional
        public string OrganizationName { get; set; }

        /// <summary>
        /// use for xref, would be used for publishing
		/// Or, actually it would be the state org (same as TPP)
        /// </summary>
        public string CredentialOrganizationCTID { get; set; }
        #endregion



        /// <summary>
        /// NEW
        /// Means to point to a credential where data is published by a third party.
        /// schema:about
		/// This should be required for direct publishing. Could there ever be general outcome data for say a whole dept? 
        /// </summary>
        public List<EntityReference> About { get; set; } = new List<EntityReference>();

        /// <summary>
        /// Authoritative source of an entity's information.
        /// URL 
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Data Suppression Policy
        /// Description of a data suppression policy for earnings and employment data when cell size is below a certain threshold to ensure an individual's privacy and security.
        /// </summary>
        public string DataSuppressionPolicy { get; set; }
        //
        public EntityReference Credential { get; set; } = new EntityReference();
        public string CredentialName { get; set; }
        public string CredentialCTID { get; set; }

        //
        //public string DataSetTimePeriodDescription { get; set; }
        //public string DataSetTimePeriod1StartDate { get; set; }
        //public string DataSetTimePeriod1EndDate { get; set; }
        //public string DataSetTimePeriod1Percent { get; set; }
        //public string DataSetTimePeriod1Value { get; set; }
        //public string DataSetTimePeriod1Subjects { get; set; }

        /// <summary>
        /// Data Set Time Period
        /// Short- and long-term post-award reporting intervals including start and end dates.
        /// </summary>
        //public List<DataSetTimeFrameDTO> DataSetTimePeriod { get; set; } = new List<DataSetTimeFrameDTO>();


    }
    public class DataSetTimeFrameDTO
    {
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string PassRateDescription { get; set; }
        public decimal Percentage { get; set; }
        public int Value { get; set; }
        public int SubjectsInSet { get; set; }

        public string PercentageInput { get; set; }
        public string ValueInput { get; set; }
        public string SubjectsInSetInput { get; set; }
    }
    public class DataProfileDTO
    {
        public string Description { get; set; }

        public QuantitativeValue HoldersInSet { get; set; } = new QuantitativeValue();

        public decimal Percentage { get; set; }
        public int Value { get; set; }
        public int SubjectsInSet { get; set; }

        public string PercentageInput { get; set; }
        public string ValueInput { get; set; }
        public string SubjectsInSetInput { get; set; }
    }

    //public class MonetaryAmount
    //{
    //    public string Description { get; set; }
    //    /// <summary>
    //    /// Currency abbreviation (e.g., USD).
    //    /// </summary>
    //    public string Currency { get; set; }

    //    /// <summary>
    //    /// Value of a monetary amount or a quantitative value.
    //    /// </summary>
    //    public decimal Value { get; set; }
    //}
}
