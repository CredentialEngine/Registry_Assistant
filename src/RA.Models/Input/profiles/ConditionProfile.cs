using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Common input class for all condition profiles
	/// </summary>
	//public class ConditionProfiles
	//{
	//	public ConditionProfiles()
	//	{
	//		Requires = new List<ConditionProfile>();
	//		Recommends = new List<ConditionProfile>();
	//		EntryConditions = new List<ConditionProfile>();
	//		Corequisite = new List<ConditionProfile>();
	//		Renewal = new List<ConditionProfile>();
	//		AdvancedStandingFor = new List<ConditionProfile>();
	//		AdvancedStandingFrom = new List<ConditionProfile>();
	//		IsPreparationFor = new List<ConditionProfile>();
	//		PreparationFrom = new List<ConditionProfile>();

	//		IsRequiredFor = new List<ConditionProfile>();
	//		IsRecommendedFor = new List<ConditionProfile>();
 
	//	}
	//	public List<ConditionProfile> Requires { get; set; }
	//	public List<ConditionProfile> Recommends { get; set; }
	//	public List<ConditionProfile> EntryConditions { get; set; }

	//	public List<ConditionProfile> Corequisite { get; set; }

	//	public List<ConditionProfile> Renewal { get; set; }

	//	public List<ConditionProfile> AdvancedStandingFor { get; set; }
	//	public List<ConditionProfile> AdvancedStandingFrom { get; set; }

	//	public List<ConditionProfile> IsPreparationFor { get; set; }
	//	public List<ConditionProfile> PreparationFrom { get; set; }

	//	public List<ConditionProfile> IsRequiredFor { get; set; }
	//	public List<ConditionProfile> IsRecommendedFor { get; set; }
      
 //   }


	/// <summary>
	/// Input class for a condition profile
	/// </summary>
	public class ConditionProfile
    {
        public ConditionProfile()
        {
            AssertedBy = new OrganizationReference();
            EstimatedCosts = new List<CostProfile>();

            SubjectWebpage = new List<string>();
            AudienceLevelType = new List<string>();
            AudienceType = new List<string>();
            Condition = new List<string>();
			SubmissionOf = new List<string>();

			AlternativeCondition = new List<ConditionProfile>();
			SubjectWebpage = new List<string>();

			//ApplicableAudienceType = new List<string>();
			TargetAssessment = new List<EntityReference>();
			TargetCredential = new List<EntityReference>();
			TargetLearningOpportunity = new List<EntityReference>();
			RequiresCompetency = new List<CredentialAlignmentObject>();

			AlternativeCondition = new List<ConditionProfile>();
			Jurisdiction = new List<Input.Jurisdiction>();
			ResidentOf = new List<Input.Jurisdiction>();
		}

        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> SubjectWebpage { get; set; } //URL

        //TODO - alter from enumeration
        public List<string> AudienceLevelType { get; set; }
        public List<string> AudienceType { get; set; }
        public string DateEffective { get; set; }
		/// <summary>
		/// List of condtions, containing:
		/// A single condition or aspect of experience that refines the conditions under which the resource being described is applicable.
		/// </summary>
		public List<string> Condition { get; set; }

        public List<string> SubmissionOf { get; set; }
        /// <summary>
        /// Organization that asserts this condition
        /// </summary>
        public OrganizationReference AssertedBy { get; set; }

        public string Experience { get; set; }
        public int MinimumAge { get; set; }
        public decimal YearsOfExperience { get; set; }
        public decimal Weight { get; set; }

        public string CreditHourType { get; set; }
        public decimal CreditHourValue { get; set; }
        //public int CreditUnitTypeId { get; set; }

        /// <summary>
		/// Only one credit unit type is allowed for input
		/// </summary>
        public string CreditUnitType { get; set; } 
        public string CreditUnitTypeDescription { get; set; }
        public decimal CreditUnitValue { get; set; }

        //external classes =====================================
        public List<CostProfile> EstimatedCosts { get; set; }
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
        public string Description { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Organization that owns this credential
        /// </summary>
        public OrganizationReference AssertedBy { get; set; }

		public decimal Weight { get; set; }

		public string CreditHourType { get; set; }
		public decimal CreditHourValue { get; set; }
	

		/// <summary>
		/// Only one credit unit type is allowed for input
		/// </summary>
		public string CreditUnitType { get; set; }
		public string CreditUnitTypeDescription { get; set; }
		public decimal CreditUnitValue { get; set; }

		//external classes =====================================

		public List<EntityReference> TargetAssessment { get; set; }
		public List<EntityReference> TargetCredential { get; set; }
		public List<EntityReference> TargetLearningOpportunity { get; set; }


	}
}
