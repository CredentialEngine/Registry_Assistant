using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles.QData
{
    /// <summary>
    /// What is being measured and the method of measurement used for observations within a data set. 
    /// </summary>
    public class Metric : BasePrimaryResource
    {

        /*
            qdata:adjustment
            qdata:recordType
            qdata:dataSourceCoverageType
            ceasn:derivedFrom
            qdata:earningsDefinition
            qdata:earningsThreshold
            qdata:employmentDefinition
            qdata:incomeDeterminationType
            qdata:metricType
            owl:sameAs


         */
        /// <summary>
        /// Globally unique Credential Transparency Identifier (CTID)
        /// required
        /// <see cref="https://credreg.net/ctdl/terms/ctid"/>
        /// </summary>
        public string CTID { get; set; }

        /// <summary>
		/// Name of this Job
		/// Optional
		/// ceterms:name
		/// </summary>
		public string Name { get; set; }
        public LanguageMap Name_Map { get; set; }

        /// <summary>
        /// Describes whether and how the Observations have been adjusted for relevant factors.
        /// </summary>
        public string Adjustment { get; set; }

        /// <summary>
        /// Type of record used to source the data. 
        /// Examples include various official records and self-reporting via a survey.
        /// concept in conceptScheme: ???
        /// </summary>
        public string RecordType { get; set; }

        /// <summary>
        /// The entity being described has been modified, extended or refined from the referenced resource.
        /// Range:
        /// ceasn:Competency 
        /// ceasn:CompetencyFramework 
        /// ceasn:Rubric 
        /// ceterms:TransferValueProfile  
        /// qdata:Metric
        /// </summary>
        public string DerivedFrom { get; set; }

        /// <summary>
        /// Definition of "earnings" used by the data source.
        /// </summary>
        public string EarningsDefinition { get; set; }

        /// <summary>
        /// Statement of any work time or earnings threshold used in determining whether a sufficient level of workforce attachment has been achieved to qualify as employed during the time period of the data set.
        /// </summary>
        public string EarningsThreshold { get; set; }

        /// <summary>
        /// Definition of employment used by the data source.
        /// </summary>
        public string EmploymentDefinition { get; set; }

        /// <summary>
        /// Mechanism by which income is determined; i.e., actual or annualized earnings; select from an enumeration of such types.
        /// Concept in conceptScheme: qdata:IncomeDeterminationMethod
        /// </summary>
        public string IncomeDeterminationType { get; set; }

        /// <summary>
        /// Type of phenomenon being measured, select from an existing enumeration of such types.
        /// Concept in ConceptScheme: qdata:MetricCategory
        /// </summary>
        public List<string> MetricType { get; set; }

        /// <summary>
        /// Links an individual to an individual; such an owl:sameAs statement indicates that two URI references actually refer to the same thing: the individuals have the same "identity".
        /// </summary>
        public List<string> SameAs { get; set; }
    }
}
