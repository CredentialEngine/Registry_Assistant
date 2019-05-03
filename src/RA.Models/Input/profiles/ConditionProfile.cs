using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Input class for a condition profile
    /// 2018-09-02 Where LanguageMap alternates are available, only enter one. The system will check the string version first. 
    /// </summary>
    public class ConditionProfile
    {
        public ConditionProfile()
        {
            EstimatedCost = new List<CostProfile>();

            //SubjectWebpage = new List<string>();
            AudienceLevelType = new List<string>();
            AudienceType = new List<string>();
            Condition = new List<string>();
			SubmissionOf = new List<string>();

			AlternativeCondition = new List<ConditionProfile>();

			//ApplicableAudienceType = new List<string>();
			TargetAssessment = new List<EntityReference>();
			TargetCredential = new List<EntityReference>();
			TargetLearningOpportunity = new List<EntityReference>();
			RequiresCompetency = new List<CredentialAlignmentObject>();


			Jurisdiction = new List<Input.Jurisdiction>();
			ResidentOf = new List<Input.Jurisdiction>();
		}

        /// <summary>
        /// Name of this condition
        /// Required
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Name_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// Condition description 
        /// Required
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();
        public string SubjectWebpage { get; set; } //URL

        public List<string> AudienceLevelType { get; set; }
        public List<string> AudienceType { get; set; }
        public string DateEffective { get; set; }
		/// <summary>
		/// List of condtions, containing:
		/// A single condition or aspect of experience that refines the conditions under which the resource being described is applicable.
		/// </summary>
		public List<string> Condition { get; set; }
        public LanguageMapList Condition_Map { get; set; } = new LanguageMapList();


        public List<string> SubmissionOf { get; set; }
        public LanguageMapList SubmissionOf_Map { get; set; } = new LanguageMapList();

        /// <summary>
        /// Organization that asserts this condition
        /// This should be single, but as CTDL defines as multi-value, need to handle a List
        /// </summary>
        public object AssertedBy { get; set; } = new object();
        //public List<OrganizationReference> AssertedBys { get; set; } = new List<OrganizationReference>();

        public string Experience { get; set; }
        public int MinimumAge { get; set; }
        public decimal YearsOfExperience { get; set; }
        public decimal Weight { get; set; }
		//Credit Information
		//
		public QuantitativeValue CreditValue { get; set; } = new QuantitativeValue();
		//
		public string CreditHourType { get; set; }
        public decimal CreditHourValue { get; set; }
        //public int CreditUnitTypeId { get; set; }

        /// <summary>
		/// Only one credit unit type is allowed for input
		/// </summary>
        public string CreditUnitType { get; set; }
        public LanguageMap CreditHourType_Map { get; set; } = new LanguageMap();
        public string CreditUnitTypeDescription { get; set; }
        public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();
        public decimal CreditUnitValue { get; set; }

        //external classes =====================================
        public List<CostProfile> EstimatedCost { get; set; }
        public List<Jurisdiction> Jurisdiction { get; set; }
        public List<Jurisdiction> ResidentOf { get; set; }

        public List<EntityReference> TargetAssessment { get; set; } 
        public List<EntityReference> TargetCredential { get; set; }
        public List<EntityReference> TargetLearningOpportunity { get; set; }

		public List<CredentialAlignmentObject> RequiresCompetency { get; set; }
		public List<ConditionProfile> AlternativeCondition { get; set; }
 
    } 

	public class Connections
	{
		public Connections()
		{
			AssertedBy = new OrganizationReference();
		
			TargetAssessment = new List<EntityReference>();
			TargetCredential = new List<EntityReference>();
			TargetLearningOpportunity = new List<EntityReference>();
		}
        public string Type { get; set; }
        /// <summary>
        /// Name of this condition
        /// Required
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Name_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// Condition description 
        /// Required
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// Organization that owns this credential
        /// </summary>
        public OrganizationReference AssertedBy { get; set; }

		public decimal Weight { get; set; }

		//Credit Information
		public QuantitativeValue CreditValue { get; set; } = new QuantitativeValue();
		//
		public string CreditHourType { get; set; }
        public LanguageMap CreditHourType_Map { get; set; } = new LanguageMap();
        public decimal CreditHourValue { get; set; }
	

		/// <summary>
		/// Only one credit unit type is allowed for input
		/// </summary>
		public string CreditUnitType { get; set; }
		public string CreditUnitTypeDescription { get; set; }
        public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();
        public decimal CreditUnitValue { get; set; }

		//external classes =====================================

		public List<EntityReference> TargetAssessment { get; set; }
		public List<EntityReference> TargetCredential { get; set; }
		public List<EntityReference> TargetLearningOpportunity { get; set; }


	}
}
