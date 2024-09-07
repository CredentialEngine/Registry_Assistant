using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles.QData
{

    /// <summary>
    /// Data within a dataset defined by specific values for all relevant dimensions and associated with measured values.
    /// </summary>
    public class Observation
    {
        /*
qdata:atPoint
qdata:dataWithholdingType
qdata:isObservationOf
qdata:sizeOfNoData
qdata:sizeOfData
qdata:sizeOfPopulation
qdata:standardDeviation



         */


        public Observation()
        {
            Type = "qdata:Observation";
        }
        public string Type { get; set; }

        /// <summary>
        /// Currency abbreviation (e.g., USD).
        /// </summary>
        public string Currency { get; set; }


        /// <summary>
        /// Mean value.
        /// </summary>
        public decimal? Mean { get; set; }

        /// <summary>
        /// Median salary value.
        /// </summary>
        public decimal? Median { get; set; }

        /// <summary>
        /// The upper percentage of some characteristic or property.
        /// </summary>
        public decimal? maxPercentage { get; set; }

        /// <summary>
        /// The lower percentage of some characteristic or property.
        /// </summary>
        public decimal? minPercentage { get; set; }

        /// <summary>
        /// Quotient of two values of the data set, expressed as a percentage.
        /// </summary>
        public decimal? percentage { get; set; }


        /// <summary>
        /// 10th percentile salary value.
        /// </summary>
        public decimal? Percentile10 { get; set; }

        /// <summary>
        /// 25th percentile salary value.
        /// </summary>
        public decimal? Percentile25 { get; set; }

        /// <summary>
        /// 75th percentile salary value.
        /// </summary>
        public decimal? Percentile75 { get; set; }

        /// <summary>
        /// 90th percentile salary value.
        /// </summary>
        public decimal? Percentile90 { get; set; }


        /// <summary>
        /// Value of a monetary amount or a quantitative value.
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// Lower value of some characteristic or property.
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Upper value of some characteristic or property.
        /// </summary>
        public decimal? MaxValue { get; set; }
    }

}
