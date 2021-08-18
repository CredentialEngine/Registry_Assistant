﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// CostProfile class
    /// 2018-09-02 Where LanguageMap alternates are available, only enter one (string or language map). The system will check the string version first. 
    /// </summary>
    public class CostProfile 
	{
        /// <summary>
        /// Inialize lists
        /// </summary>
        public CostProfile()
        {
            Jurisdiction = new List<Jurisdiction>();
            CostItems = new List<CostProfileItem>();
			Condition = new List<string>();
		}

		/// <summary>
		/// Cost Details - A URL
		/// Required
		/// </summary>
		public string CostDetails { get; set; }

		/// <summary>
		/// Description of cost profile
		/// Required
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Name for this cost profile
		/// Optional
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// A currency code, for ex USD
		/// Optional
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// List of cost items
		/// Provide a list of individual costs including
		/// - DirectCostType - using concept scheme
		/// - Price
		/// - ResidencyType
		/// - AudienceType
		/// - PaymentPattern
		/// Optional
		/// </summary>
		public List<CostProfileItem> CostItems { get; set; }

		/// <summary>
		/// Start date or effective date of this cost profile
		/// Optional
		/// </summary>
		public string StartDate { get; set; }
		/// <summary>
		/// End date or expiry date of this cost profile
		/// Optional
		/// </summary>
		public string EndDate { get; set; }

		/// <summary>
		/// List of condtions, containing:
		/// A single condition or aspect of experience that refines the conditions under which the resource being described is applicable.
		/// Optional
		/// </summary>
		public List<string> Condition { get; set; }
		/// <summary>
		/// Alternately use a LanguageMapList for the conditions
		/// </summary>
        public LanguageMapList Condition_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// List of jurisdictions
		/// Optional
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; }


    }

	/// <summary>
	/// Cost item class
	/// </summary>
	public class CostProfileItem
	{
		/// <summary>
		/// Initialize
		/// </summary>
		public CostProfileItem()
		{
			ResidencyType = new List<string>();
			AudienceType = new List<string>();
		}

		/// <summary>
		/// Must be a valid CTDL cost type. 
		/// costType:AggregateCost
		///	costType:Application
		///	costType:BackgroundCheck
		///	costType:EnrollmentFee
		///	costType:GraduationFee
		///	costType:LearningResource
		///	costType:Meals
		///	costType:MixedFees
		///	costType:OptionalCharges
		///	costType:ProcessingFee
		///	costType:ProgramSpecificFee
		///	costType:ReexaminationFee
		///	costType:RenewalFee
		///	costType:ReschedulingFee
		///	costType:RoomOrResidency
		///	costType:ScoringFee
		///	costType:StandaloneAssessment
		///	costType:StudentServicesFee
		///	costType:TechnologyFee
		///	costType:Tuition
		/// <see cref="https://credreg.net/ctdl/terms/CostType"/>
		/// </summary>
		public string DirectCostType { get; set; }
		/// <summary>
		/// List of Residency items
		/// </summary>
		public List<string> ResidencyType { get; set; }

		/// <summary>
		/// List of Audience Types
		/// audience:Citizen audience:CurrentMilitary audience:CurrentMilitaryDependent audience:CurrentMilitarySpouse audience:CurrentStudent audience:FormerMilitary audience:FormerMilitaryDependent audience:FormerMilitarySpouse audience:FormerStudent audience:FullTime audience:Member audience:NonCitizen audience:NonMember audience:NonResident audience:PartTime audience:PrivateEmployee audience:PublicEmployee audience:Resident
		/// <see cref="https://credreg.net/ctdl/terms/Audience"/>
		/// </summary>
		public List<string> AudienceType { get; set; }

		/// <summary>
		/// Payment Pattern
		/// </summary>
		public string PaymentPattern { get; set; }
		/// <summary>
		/// Alternately use a LanguageMap for PaymentPattern.
		/// </summary>
        public LanguageMap PaymentPattern_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// Price for this cost - optional
        /// </summary>
        public decimal Price { get; set; }

	}

}
