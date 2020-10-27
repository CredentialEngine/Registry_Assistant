using System;
using System.Collections.Generic;

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
			//Region = new List<GeoCoordinates>();

			AudienceType = new List<string>();
			AvailabilityListing = new List<string>();
			AvailableOnlineAt = new List<string>();
			//CodedNotation = new List<string>();


			AccreditedBy = new List<Input.OrganizationReference>();
			ApprovedBy = new List<Input.OrganizationReference>();
			OfferedBy = new List<Input.OrganizationReference>();
			RecognizedBy = new List<Input.OrganizationReference>();
			RegulatedBy = new List<Input.OrganizationReference>();

			//JurisdictionAssertions = new List<JurisdictionAssertedInProfile>();

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

		#region *** Recommended Benchmark ***
		/// <summary>
		/// A credit-related value.
		/// </summary>
		public QuantitativeValue CreditValue { get; set; } = new QuantitativeValue();

		/// <summary>
		/// Competency that the learning opportunity is intended to teach.
		/// </summary>
		public List<CredentialAlignmentObject> Teaches { get; set; }

		/// <summary>
		/// Types of methods used to conduct the learning opportunity; select from an existing enumeration of such types.
		/// </summary>
		public List<string> LearningMethodType { get; set; }


		/// <summary>
		/// Learning Method Description 
		///  Description of the learning methods for a resource.
		/// 
		/// </summary>
		public string LearningMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap LearningMethodDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Type of means by which a learning opportunity or assessment is delivered to credential seekers and by which they interact; select from an existing enumeration of such types.
		/// </summary>
		public List<string> DeliveryType { get; set; }

		/// <summary>
		/// Detailed description of the delivery type of an assessment or learning opportunity.
		/// </summary>
		public string DeliveryTypeDescription { get; set; }
		public LanguageMap DeliveryTypeDescription_Map { get; set; } = new LanguageMap();
		#endregion

		//=========== optional ================================

		/// <summary>
		/// Assessment Method Description 
		/// Description of the assessment methods for a resource.
		/// </summary>
		public string AssessmentMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap AssessmentMethodDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Type of method used to conduct an assessment; select from an existing enumeration of such types.
		/// </summary>
		public List<string> AssessmentMethodType { get; set; } = new List<string>();

		public List<string> AudienceType { get; set; }
		public List<string> AudienceLevelType { get; set; } = new List<string>();


		public string CodedNotation { get; set; }

		public List<string> CommonCosts { get; set; }
		public List<string> CommonConditions { get; set; }

		/// <summary>
		/// Detailed description of credit unit. 
		/// Recommendation is to use CreditValue rather than this property.
		/// </summary>
		public string CreditUnitTypeDescription { get; set; }
		public LanguageMap CreditUnitTypeDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Start Date of the Learning opportunity
		/// </summary>
		public string DateEffective { get; set; }

		public List<CostProfile> EstimatedCost { get; set; }

		/// <summary>
		/// Estimated time it will take to complete a credential, learning opportunity or assessment.
		/// </summary>
		public List<DurationProfile> EstimatedDuration { get; set; }

		/// <summary>
		/// End date of the learning opportunity if applicable
		/// </summary>
		public string ExpirationDate { get; set; }

		public List<FinancialAssistanceProfile> FinancialAssistance { get; set; } = new List<FinancialAssistanceProfile>();

		/// <summary>
		/// List of 'child' learning opps
		/// </summary>
		public List<EntityReference> HasPart { get; set; }

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see cref="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		//List of language codes. ex: en, es
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// Not sure of best use. Should be initially limited to lopps?
		/// </summary>
		public List<EntityReference> IsPartOfLearningOpportunity { get; set; }

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();

		public List<string> Keyword { get; set; }
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();


		public List<string> Subject { get; set; }
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

		public List<string> TargetLearningResource { get; set; } = new List<string>();

		public List<IdentifierValue> VersionIdentifier { get; set; }


		#region Occupations, Industries, and instructional programs
		//=====================================================================
		//List of occupations from a published framework, that is with a web URL
		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net. 
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		/// </summary>
		public List<FrameworkItem> OccupationType { get; set; }
		/// <summary>
		/// AlternativeOccupationType
		/// Occupations that are not found in a formal framework can be still added using AlternativeOccupationType. 
		/// Any occupations added using this property will be added to or appended to the OccupationType output.
		/// </summary>
		public List<string> AlternativeOccupationType { get; set; } = new List<string>();

		/// <summary>
		/// List of valid O*Net codes. See:
		/// https://www.onetonline.org/find/
		/// The API will validate and format the ONet codes as Occupations
		/// </summary>
		public List<string> ONET_Codes { get; set; } = new List<string>();

		//=============================================================================
		/// <summary>
		/// IndustryType
		/// Type of industry; select from an existing enumeration of such types such as the SIC, NAICS, and ISIC classifications.
		/// Best practice in identifying industries for U.S. credentials is to provide the NAICS code using the ceterms:naics property. 
		/// Other credentials may use the ceterms:industrytype property and any framework of the class ceterms:IndustryClassification.
		/// </summary>
		public List<FrameworkItem> IndustryType { get; set; }

		/// <summary>
		/// AlternativeIndustryType
		/// Industries that are not found in a formal framework can be still added using AlternativeIndustryType. 
		/// Any industries added using this property will be added to or appended to the IndustryType output.
		/// </summary>
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		/// <summary>
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> NaicsList { get; set; } = new List<string>();

		//=============================================================================
		/// <summary>
		/// InstructionalProgramType
		/// Type of instructional program; select from an existing enumeration of such types.
		/// </summary>
		public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();

		/// <summary>
		/// AlternativeInstructionalProgramType
		/// Programs that are not found in a formal framework can be still added using AlternativeInstructionalProgramType. 
		/// Any programs added using this property will be added to or appended to the InstructionalProgramType output.
		/// </summary>
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();

		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion

		//


		#region  Conditions and connections
		public List<ConditionProfile> Corequisite { get; set; }
		public List<ConditionProfile> Recommends { get; set; }
		public List<ConditionProfile> Requires { get; set; }
		public List<ConditionProfile> EntryCondition { get; set; }

		public List<Connections> AdvancedStandingFrom { get; set; }
		public List<Connections> IsAdvancedStandingFor { get; set; }
		public List<Connections> PreparationFrom { get; set; }
		public List<Connections> IsPreparationFor { get; set; }
		public List<Connections> IsRecommendedFor { get; set; }
		public List<Connections> IsRequiredFor { get; set; }
		#endregion



		#region -- Quality Assurance BY --
		public List<OrganizationReference> AccreditedBy { get; set; }
		public List<OrganizationReference> ApprovedBy { get; set; }

		public List<OrganizationReference> RecognizedBy { get; set; }
		public List<OrganizationReference> RegulatedBy { get; set; }
		#endregion

		#region Quality Assurance IN - Jurisdiction based Quality Assurance  (INs)
		//There are currently two separate approaches to publishing properties like assertedIn
		//- Publish all 'IN' properties using JurisdictionAssertions
		//- Publish using ehe separate specific properties like AccreditedIn, ApprovedIn, etc
		// 2010-01-06 The property JurisdictionAssertions may become obsolete soon. We recomend to NOT use this property.

		/// <summary>
		/// Handling assertions in jurisdictions
		/// The property JurisdictionAssertions is a simple approach, using one record per asserting organization - where that organization will have multiple assertion types. 
		/// The JurisdictionAssertedInProfile has a list of boolean properties where the assertion(s) can be selected.
		/// This approach simplifies the input where the same organization asserts more than action.
		/// 2010-01-06 TBD - this property will LIKELY be made obsolete once any partner who has been using it has been informed.
		/// </summary>
		//[Obsolete]
		//public List<JurisdictionAssertedInProfile> JurisdictionAssertions { get; set; } = new List<JurisdictionAssertedInProfile>();

		//JurisdictionAssertion
		//Each 'IN' property must include one or more organizations and a Main jurisdiction. Only one main jusrisdiction (and multiple exceptions) can be entered with each property.
		//Only use this property where the organization only makes the assertion for a specific jurisdiction. 
		//Use the 'BY' equivalent (ex. accreditedBy) where the organization makes a general assertion

		/// <summary>
		/// List of Organizations that accredit this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> AccreditedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that approve this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> ApprovedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that offer this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> OfferedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that recognize this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RecognizedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that regulate this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RegulatedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that revoke this learning opportunity in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RevokedIn { get; set; } = new List<JurisdictionAssertion>();

		#endregion



		/// <summary>
		/// PLANNED NOT IMPLEMENTED
		/// Type of official status of the TransferProfile; select from an enumeration of such types.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// </summary>
		public string LifecycleStatusType { get; set; }




	}
}
