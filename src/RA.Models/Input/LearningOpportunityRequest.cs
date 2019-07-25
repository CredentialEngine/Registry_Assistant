﻿using System.Collections.Generic;

namespace RA.Models.Input
{
	public class LearningOpportunityRequest : BaseRequest
	{
		public LearningOpportunityRequest()
		{
			LearningOpportunity = new LearningOpportunity();
		}

		public LearningOpportunity LearningOpportunity { get; set; }
		//public List<BlankNode> BlankNodes = new List<BlankNode>();
	}

	public class LearningOpportunity
	{
		public LearningOpportunity()
		{

			Subject = new List<string>();
			Keyword = new List<string>();
			DeliveryType = new List<string>();
			LearningMethodType = new List<string>();
			EstimatedCost = new List<CostProfile>();
			Jurisdiction = new List<Jurisdiction>();
			//Region = new List<GeoCoordinates>();

			AudienceType = new List<string>();
			AudienceLevelType = new List<string>();
			AvailabilityListing = new List<string>();
			AvailableOnlineAt = new List<string>();
			//CodedNotation = new List<string>();


			AccreditedBy = new List<Input.OrganizationReference>();
			ApprovedBy = new List<Input.OrganizationReference>();
			OfferedBy = new List<Input.OrganizationReference>();
			RecognizedBy = new List<Input.OrganizationReference>();
			RegulatedBy = new List<Input.OrganizationReference>();

			JurisdictionAssertions = new List<JurisdictionAssertedInProfile>();

			Corequisite = new List<ConditionProfile>();
			Recommends = new List<ConditionProfile>();
			Requires = new List<ConditionProfile>();
			EntryCondition = new List<ConditionProfile>();

			Teaches = new List<CredentialAlignmentObject>();

			AdvancedStandingFrom = new List<Connections>();
			IsAdvancedStandingFor = new List<Connections>();
			PreparationFrom = new List<Connections>();
			IsPreparationFor = new List<Connections>();
			IsRecommendedFor = new List<Connections>();
			IsRequiredFor = new List<Connections>();
			InLanguage = new List<string>();
			AvailableAt = new List<Place>();
			CommonConditions = new List<string>();
			CommonCosts = new List<string>();
			//FinancialAssistanceOLD = new List<Input.FinancialAlignmentObject>();

			HasPart = new List<EntityReference>();
			IsPartOfLearningOpportunity = new List<EntityReference>();
			VersionIdentifier = new List<IdentifierValue>();
		}



		#region *** Required Properties ***
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		public string SubjectWebpage { get; set; } //URL
		public string Ctid { get; set; }


		#region at least one of

		/// <summary>
		/// Organization that owns this resource
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();
		//OR
		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }
		#endregion

		#region at least one of the following
		public List<string> AvailableOnlineAt { get; set; } //URL
		public List<string> AvailabilityListing { get; set; } //URL
		public List<Place> AvailableAt { get; set; }
		#endregion

		#endregion

		#region *** Required if available Properties ***

		public List<CredentialAlignmentObject> Teaches { get; set; }

		public List<string> LearningMethodType { get; set; }
		public List<string> DeliveryType { get; set; }
		public string DeliveryTypeDescription { get; set; }
		public LanguageMap DeliveryTypeDescription_Map { get; set; } = new LanguageMap();
		#endregion

		#region *** Recommended Properties ***
		public string DateEffective { get; set; }
		//List of language codes. ex: en, es
		public List<string> InLanguage { get; set; }
		public List<DurationProfile> EstimatedDuration { get; set; }
		public List<ConditionProfile> Requires { get; set; }

		//Credit Information
		//
		public QuantitativeValue CreditValue { get; set; } = new QuantitativeValue();
		//
		public string CreditHourType { get; set; }
		public LanguageMap CreditHourType_Map { get; set; } = new LanguageMap();
		public string CreditUnitType { get; set; }
		public decimal CreditHourValue { get; set; }
		public decimal CreditUnitValue { get; set; }
		public string CreditUnitTypeDescription { get; set; }
		public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();

		public List<string> Keyword { get; set; }
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();
		public List<string> Subject { get; set; }
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();
		#endregion

		public List<string> AudienceType { get; set; }
        public List<string> AudienceLevelType { get; set; }= new List<string>();
		public string CodedNotation { get; set; }

		public string VerificationMethodDescription { get; set; }
		public LanguageMap VerificationMethodDescription_Map { get; set; } = new LanguageMap();

		//
		public List<FrameworkItem> OccupationType { get; set; }
		public List<string> AlternativeOccupationType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeOccupationType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// </summary>
		public List<string> ONET_Codes { get; set; } = new List<string>();

		public List<FrameworkItem> IndustryType { get; set; }
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		//public List<string> Naics { get; set; }

		public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeInstructionalProgramType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		//

		//external classes
		public List<CostProfile> EstimatedCost { get; set; }

		public List<Jurisdiction> Jurisdiction { get; set; }
		//public List<GeoCoordinates> Region { get; set; }
		//quality assurance
		public List<OrganizationReference> AccreditedBy { get; set; }
		public List<OrganizationReference> ApprovedBy { get; set; }
		public List<OrganizationReference> RecognizedBy { get; set; }
		public List<OrganizationReference> RegulatedBy { get; set; }


		public List<JurisdictionAssertedInProfile> JurisdictionAssertions { get; set; }

		//conditions
		public List<ConditionProfile> Corequisite { get; set; }
		public List<ConditionProfile> Recommends { get; set; }
		public List<ConditionProfile> EntryCondition { get; set; }


		//required competencies are input with condition profiles
		//public List<CredentialAlignmentObject> RequiresCompetency { get; set; }

		public List<Connections> AdvancedStandingFrom { get; set; }
		public List<Connections> IsAdvancedStandingFor { get; set; }
		public List<Connections> PreparationFrom { get; set; }
		public List<Connections> IsPreparationFor { get; set; }
		public List<Connections> IsRecommendedFor { get; set; }
		public List<Connections> IsRequiredFor { get; set; }

		/// <summary>
		/// List of 'child' learning opps
		/// </summary>
		public List<EntityReference> HasPart { get; set; }

		/// <summary>
		/// Not sure of best use. Should be initially limited to lopps?
		/// </summary>
		public List<EntityReference> IsPartOfLearningOpportunity { get; set; }

		public List<string> CommonCosts { get; set; }
		public List<string> CommonConditions { get; set; }
		//public List<FinancialAlignmentObject> FinancialAssistanceOLD { get; set; }
		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();
		public List<IdentifierValue> VersionIdentifier { get; set; }
	}
}
