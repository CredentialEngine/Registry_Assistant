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
		/// <summary>
		/// Constructor
		/// </summary>
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
			TargetCompetency = new List<CredentialAlignmentObject>();

			ResidentOf = new List<Input.Jurisdiction>();
		}

        /// <summary>
        /// Name of this condition
        /// Optional
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
		///  Webpage that describes this condition
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// The type of credential seeker for whom the entity is applicable; select from an existing enumeration of such types.
		/// audience:Citizen audience:CurrentMilitary audience:CurrentMilitaryDependent audience:CurrentMilitarySpouse audience:CurrentStudent audience:FormerMilitary audience:FormerMilitaryDependent audience:FormerMilitarySpouse audience:FormerStudent audience:FullTime audience:Member audience:NonCitizen audience:NonMember audience:NonResident audience:PartTime audience:PrivateEmployee audience:PublicEmployee audience:Resident
		/// <see cref="https://credreg.net/ctdl/terms/Audience"/>
		/// </summary>
		public List<string> AudienceType { get; set; }
		/// <summary>
		/// Type of level indicating a point in a progression through an educational or training context, for which the credential is intended; select from an existing enumeration of such types.
		/// audLevel:AdvancedLevel audLevel:AssociatesDegreeLevel audLevel:BachelorsDegreeLevel audLevel:BeginnerLevel audLevel:DoctoralDegreeLevel audLevel:GraduateLevel audLevel:IntermediateLevel audLevel:LowerDivisionLevel audLevel:MastersDegreeLevel audLevel:PostSecondaryLevel audLevel:ProfessionalLevel audLevel:SecondaryLevel audLevel:UndergraduateLevel audLevel:UpperDivisionLevel
		/// <see cref="https://credreg.net/ctdl/terms/AudienceLevel"/>
		/// </summary>
		public List<string> AudienceLevelType { get; set; } = new List<string>();
		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// List of condtions, containing:
		/// A single condition or aspect of experience that refines the conditions under which the resource being described is applicable.
		/// </summary>
		public List<string> Condition { get; set; }
		/// <summary>
		/// Or use a LanguageMapList
		/// </summary>
        public LanguageMapList Condition_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Artifact to be submitted such as a transcript, portfolio, or an affidavit.
		/// Aug. 2019 - changed to be list of URIs. 
		/// Use SubmissionOfDescription for text values.
		/// </summary>
		public List<string> SubmissionOf { get; set; }

		/// <summary>
		/// Name, label, or description of an artifact to be submitted such as a transcript, portfolio, or an affidavit.
		/// </summary>
		public string SubmissionOfDescription { get; set; }
		/// <summary>
		/// Or use a LanguageMap
		/// </summary>
		public LanguageMap SubmissionOfDescription_Map { get; set; } = new LanguageMap();

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
		//20-09-30 being replaced by ValueProfile
		public ValueProfile CreditValue { get; set; } //= new ValueProfile();
		//
		public string CreditUnitTypeDescription { get; set; }
        public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();


		#region CreditHourType and CreditUnitType are obsolete
		//[Obsolete]
		//public string CreditHourType { get; set; }
		//public LanguageMap CreditHourType_Map { get; set; } = new LanguageMap();
		//[Obsolete]
		//public decimal CreditHourValue { get; set; }

		///// <summary>
		///// Only one credit unit type is allowed for input
		///// </summary>
		//[Obsolete]
		//public string CreditUnitType { get; set; }
		//[Obsolete]
		//public decimal CreditUnitValue { get; set; }
		#endregion

		//external classes =====================================
		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization
		/// </summary>
		public List<string> CommonCosts { get; set; }
		public List<CostProfile> EstimatedCost { get; set; }
		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();
		public List<Jurisdiction> ResidentOf { get; set; }

        public List<EntityReference> TargetAssessment { get; set; } 
        public List<EntityReference> TargetCredential { get; set; }
        public List<EntityReference> TargetLearningOpportunity { get; set; }

		//targetCompetency is typicall a competency required for the parent of this condition profile
		public List<CredentialAlignmentObject> TargetCompetency { get; set; }
		public List<ConditionProfile> AlternativeCondition { get; set; }
 
    } 

    /// <summary>
    /// The Connection profile is a subset of a conditon profile. 
    /// A separate profile is used by the API to clarify the subset of properties are applicable
    /// TODO: rename to ConnectionProfile to make the purpose clearer?
    /// </summary>
	public class Connections
	{
		public Connections()
		{
			//AssertedBy = new OrganizationReference();
		
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
		//20-09-30 being replaced by ValueProfile
		public ValueProfile CreditValue { get; set; } //= new ValueProfile();

		public string CreditUnitTypeDescription { get; set; }
        public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();

		//external classes =====================================

		public List<EntityReference> TargetAssessment { get; set; }
		public List<EntityReference> TargetCredential { get; set; }
		public List<EntityReference> TargetLearningOpportunity { get; set; }


	}
}
