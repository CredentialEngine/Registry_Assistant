﻿using System;
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
		public LanguageMap Name_Map { get; set; } = null;
		/// <summary>
		/// Condition description 
		/// Required. Minimum of 10 characters, but should be clear.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = null;

		/// <summary>
		/// Organization that asserts this condition
		/// This should be single, but as CTDL defines as multi-value, need to handle a List
		/// </summary>
		public List<OrganizationReference> AssertedBy { get; set; } = null;

		/// <summary>
		///  Webpage that describes this condition
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// Constraints, prerequisites, entry conditions, or requirements in a context where more than one alternative condition or path has been defined and from which any one path fulfills the parent condition.
		/// </summary>
		public List<ConditionProfile> AlternativeCondition { get; set; }

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
		public List<string> AudienceLevelType { get; set; } = null;


		/// <summary>
		/// List of condtions, containing:
		/// A single condition or aspect of experience that refines the conditions under which the resource being described is applicable.
		/// </summary>
		public List<string> Condition { get; set; }
		/// <summary>
		/// Or use a LanguageMapList
		/// </summary>
		public LanguageMapList Condition_Map { get; set; } = null;

		//

		/// <summary>
		/// Credit Information
		/// 20-09-30 being replaced by ValueProfile
		/// 21-04-04 had started with singles now allowing a List 
		/// </summary>
		public List<ValueProfile> CreditValue { get; set; } = null;

		/// <summary>
		/// Detailed description of credit unit type.
		/// </summary>
		public string CreditUnitTypeDescription { get; set; }
		/// <summary>
		/// Language map for Detailed description of credit unit type.
		/// </summary>
		public LanguageMap CreditUnitTypeDescription_Map { get; set; } = null;

		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Amount and nature of required work, experiential learning or other relevant experience.
		/// </summary>
		public string Experience { get; set; }
		/// <summary>
		/// Language Map for Experience.
		/// </summary>
		public LanguageMap Experience_Map { get; set; }

		/// <summary>
		/// Minimum allowed age at which a person is eligible for the related resource.
		/// </summary>
		public int MinimumAge { get; set; }

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
		public LanguageMap SubmissionOfDescription_Map { get; set; } = null;

		/// <summary>
		/// Years of relevant experience.
		/// </summary>
		public decimal YearsOfExperience { get; set; }
		/// <summary>
		/// Measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		public decimal Weight { get; set; }

		//external classes =====================================
		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization
		/// </summary>
		public List<string> CommonCosts { get; set; }
		/// <summary>
		/// Estimated cost of for the related resource.
		/// </summary>
		public List<CostProfile> EstimatedCost { get; set; }
		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();
		/// <summary>
		/// Geographic or political region of which a person must be a legal resident or citizen in order to be eligible for the resource.
		/// </summary>
		public List<Jurisdiction> ResidentOf { get; set; }

		/// <summary>
		/// Assessment that provides direct, indirect, formative or summative evaluation or estimation of the nature, ability, or quality for an entity.
		/// </summary>
		public List<EntityReference> TargetAssessment { get; set; }
		/// <summary>
		/// Credential that is a focus or target of the condition, process or verification service.
		/// </summary>
		public List<EntityReference> TargetCredential { get; set; }
		/// <summary>
		/// Learning opportunity that is the focus of a condition, process or another learning opportunity.
		/// </summary>
		public List<EntityReference> TargetLearningOpportunity { get; set; }

		//
		/// <summary>
		/// A competency relevant to the condition being described.
		/// targetCompetency is typically a competency required for the parent of this condition profile
		/// </summary>
		public List<CredentialAlignmentObject> TargetCompetency { get; set; }

        /// <summary>
        /// List of Alternate Names for this resource
        /// </summary>
        public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = null;

		#region Helpers
		/// <summary>
		/// Target Competency Framework - Helper property
		/// A list of CTIDs (TBD if better to leave as single). 
		/// API will look up framework, get all competencies and add as TargetCompetencies for this condition. 
		/// NOTE: will likely only allow for a "Requires" condition profile
		/// </summary>
		public List<string> TargetCompetencyFramework { get; set; }


        #endregion
    }

    /// <summary>
    /// The Connection profile is a subset of a condition profile. 
    /// A separate profile is used by the API to clarify the subset of properties are applicable
    /// 2023-03-28 Renamed to ConnectionProfile to make the purpose clearer? The old class is retained below ConnectionProfile for legacy purposes.
    /// </summary>
    public class ConnectionProfile
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConnectionProfile()
		{
			//AssertedBy = new OrganizationReference();

			TargetAssessment = new List<EntityReference>();
			TargetCredential = new List<EntityReference>();
			TargetLearningOpportunity = new List<EntityReference>();
		}
		/// <summary>
		/// Name of this condition
		/// Required
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = null;
		/// <summary>
		/// Condition description 
		/// Required. Minimum of 10 characters, but should be clear.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = null;

		/// <summary>
		/// Organization that owns this credential
		/// </summary>
		public OrganizationReference AssertedBy { get; set; }

		/// <summary>
		/// Measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		public decimal Weight { get; set; }

		/// <summary>
		/// Credit Information
		/// 20-09-30 being replaced by ValueProfile
		/// 21-04-04 had started with singles now allowing a List 
		/// </summary>
		public List<ValueProfile> CreditValue { get; set; } = null;

		/// <summary>
		/// Detailed description of credit unit type.
		/// </summary>
		public string CreditUnitTypeDescription { get; set; }
		/// <summary>
		/// Language map for Detailed description of credit unit type.
		/// </summary>
		public LanguageMap CreditUnitTypeDescription_Map { get; set; } = null;

		//external classes =====================================

		/// <summary>
		/// Assessment that provides direct, indirect, formative or summative evaluation or estimation of the nature, ability, or quality for an entity.
		/// </summary>
		public List<EntityReference> TargetAssessment { get; set; }
		/// <summary>
		/// Credential that is a focus or target of the condition, process or verification service.
		/// </summary>
		public List<EntityReference> TargetCredential { get; set; }
		/// <summary>
		/// Learning opportunity that is the focus of a condition, process or another learning opportunity.
		/// </summary>
		public List<EntityReference> TargetLearningOpportunity { get; set; }

	}
	//retain in order to not mess up previous use
    public class Connections : ConnectionProfile
    {

    }
}
