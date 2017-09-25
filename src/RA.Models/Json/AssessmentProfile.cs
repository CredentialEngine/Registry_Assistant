using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Json
{
    public class AssessmentProfile : JsonLDDocument
    {
		[JsonIgnore]
		public static string classType = "ceterms:AssessmentProfile";
		public AssessmentProfile()
        {
            Keyword = new List<string>();
            SubjectWebpage = new List<IdProperty>();
            AvailabilityListing = new List<IdProperty>();
            AvailableOnlineAt = new List<IdProperty>();
            DeliveryType = new List<CredentialAlignmentObject>();
            AssessmentMethodType = new List<CredentialAlignmentObject>();
            EstimatedCost = new List<CostProfile>();
            EstimatedDuration = new List<DurationProfile>();
            ScoringMethodType = new List<CredentialAlignmentObject>();
            Type = "ceterms:AssessmentProfile";
            Requires = new List<ConditionProfile>();
            Corequisite = new List<ConditionProfile>();
            Recommends = new List<ConditionProfile>();
            EntryCondition = new List<ConditionProfile>();
			CreditUnitType = new List<CredentialAlignmentObject>();
			Assesses = new List<CredentialAlignmentObject>();
			OwnedBy = null;
			AccreditedBy = null;
            ApprovedBy = null;
            OfferedBy = null;
            RegulatedBy = null;
            RecognizedBy = null;

			AccreditedIn = null;
			ApprovedIn = null;
			OfferedIn = null;
			RecognizedIn = null;
			RegulatedIn = null;
		
			InLanguage = new List<string>();
			AdministrationProcess = new List<ProcessProfile>();
			DevelopmentProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();

			AdvancedStandingFrom = new List<ConditionProfile>();
			IsAdvancedStandingFor = new List<ConditionProfile>();
			IsPreparationFor = new List<ConditionProfile>();
			IsRecommendedFor = new List<ConditionProfile>();
			IsRequiredFor = new List<ConditionProfile>();
			PreparationFrom = new List<ConditionProfile>();
			Jurisdiction = new List<JurisdictionProfile>();
            ExternalResearch = new List<IdProperty>();
			AvailableAt = new List<Json.AvailableAt>();
            CommonConditions = new List<EntityBase>();
            CommonCosts = new List<EntityBase>();
            FinancialAssistance = new List<FinancialAlignmentObject>();
        }

        [JsonProperty( PropertyName = "ceterms:name" )]
        public string Name { get; set; }

        [JsonProperty( PropertyName = "ceterms:description" )]
        public string Description { get; set; }

        [JsonProperty( PropertyName = "ceterms:inLanguage" )]
        public List<string> InLanguage { get; set; }

        [JsonProperty( PropertyName = "ceterms:keyword" )]
        public List<string> Keyword { get; set; }

        [JsonProperty( PropertyName = "ceterms:subject" )]
        public List<CredentialAlignmentObject> Subject { get; set; } 

        [JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
        public List<IdProperty> SubjectWebpage { get; set; } //URL

        [JsonProperty( PropertyName = "ceterms:assessmentMethodType" )]
        public List<CredentialAlignmentObject> AssessmentMethodType { get; set; }

		[JsonProperty( PropertyName = "ceterms:assesses" )]
		public List<CredentialAlignmentObject> Assesses { get; set; }


		[JsonProperty( PropertyName = "ceterms:codedNotation" )]
        public List<string> CodedNotation { get; set; }

        [JsonProperty( PropertyName = "ceterms:dateEffective" )]
        public string DateEffective { get; set; }

        /// <summary>
        /// Need a custom mapping to @type based on input value
        /// ceterms:CredentialOrganization, oR
        /// ceterms:QACredentialOrganization
        /// </summary>
        [JsonProperty( "@type" )]
        public string Type { get; set; }

        [JsonProperty( PropertyName = "ceterms:ctid" )]
        public string Ctid { get; set; }

        [JsonProperty( PropertyName = "ceterms:assessmentExample" )] //URL
        public IdProperty AssessmentExample { get; set; }

        [JsonProperty( PropertyName = "ceterms:assessmentExampleDescription" )]
        public string AssessmentExampleDescription { get; set; }

        [JsonProperty( PropertyName = "ceterms:assessmentOutput" )]
        public string AssessmentOutput { get; set; }

        [JsonProperty( PropertyName = "ceterms:assessmentUseType" )]
        public List<CredentialAlignmentObject> AssessmentUseType { get; set; }

        [JsonProperty( PropertyName = "ceterms:availabilityListing" )]
        public List<IdProperty> AvailabilityListing { get; set; } //URL

        [JsonProperty( PropertyName = "ceterms:processStandards" )]
        public IdProperty ProcessStandards { get; set; }

        [JsonProperty( "@id" )]
        public string CtdlId { get; set; }


        [JsonProperty( PropertyName = "ceterms:availableOnlineAt" )] //URL
        public List<IdProperty> AvailableOnlineAt { get; set; }

        [JsonProperty( PropertyName = "ceterms:deliveryType" )]
        public List<CredentialAlignmentObject> DeliveryType { get; set; }


        [JsonProperty( PropertyName = "ceterms:isProctored", DefaultValueHandling = DefaultValueHandling.Include )]
        public bool? IsProctored { get; set; }
        [JsonProperty( PropertyName = "ceterms:hasGroupEvaluation", DefaultValueHandling = DefaultValueHandling.Include )]
        public bool? HasGroupEvaluation { get; set; }
        [JsonProperty( PropertyName = "ceterms:hasGroupParticipation", DefaultValueHandling = DefaultValueHandling.Include )]
        public bool? HasGroupParticipation { get; set; }
        [JsonProperty( PropertyName = "ceterms:deliveryTypeDescription" )]
        public string DeliveryTypeDescription { get; set; }
        [JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
        public string ProcessStandardsDescription { get; set; }

        [JsonProperty( PropertyName = "ceterms:estimatedDuration" )]
        public List<DurationProfile> EstimatedDuration { get; set; }

        [JsonProperty( PropertyName = "ceterms:estimatedCost" )]
        public List<CostProfile> EstimatedCost { get; set; }

        [JsonProperty( PropertyName = "ceterms:scoringMethodDescription" )]
        public string ScoringMethodDescription { get; set; }
        [JsonProperty( PropertyName = "ceterms:scoringMethodExample" )]
        public IdProperty ScoringMethodExample { get; set; }
        [JsonProperty( PropertyName = "ceterms:scoringMethodExampleDescription" )]
        public string ScoringMethodExampleDescription { get; set; }
        [JsonProperty( PropertyName = "ceterms:scoringMethodType" )]
        public List<CredentialAlignmentObject> ScoringMethodType { get; set; }


		[JsonProperty( PropertyName = "ceterms:instructionalProgramType" )]
		public List<CredentialAlignmentObject> InstructionalProgramType { get; set; }

		[JsonProperty( PropertyName = "ceterms:creditHourType" )]
        public string CreditHourType { get; set; }

        [JsonProperty( PropertyName = "ceterms:creditUnitType" )]
        public List<CredentialAlignmentObject> CreditUnitType { get; set; }

        [JsonProperty( PropertyName = "ceterms:creditHourValue" )]
        public decimal CreditHourValue { get; set; }

        [JsonProperty( PropertyName = "ceterms:creditUnitValue" )]
        public decimal CreditUnitValue { get; set; }

        [JsonProperty( PropertyName = "ceterms:creditUnitTypeDescription" )]
        public string CreditUnitTypeDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<OrganizationBase> OwnedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:accreditedBy" )]
        public List<OrganizationBase> AccreditedBy { get; set; }

        [JsonProperty( PropertyName = "ceterms:approvedBy" )]
        public List<OrganizationBase> ApprovedBy { get; set; }

        [JsonProperty( PropertyName = "ceterms:offeredBy" )]
        public List<OrganizationBase> OfferedBy { get; set; }

        [JsonProperty( PropertyName = "ceterms:recognizedBy" )]
        public List<OrganizationBase> RecognizedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:regulatedBy" )]
		public List<OrganizationBase> RegulatedBy { get; set; }

		#region INs

		[JsonProperty( PropertyName = "ceterms:accreditedIn" )]
		public List<JurisdictionProfile> AccreditedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:approvedIn" )]
		public List<JurisdictionProfile> ApprovedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:offeredIn" )]
		public List<JurisdictionProfile> OfferedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:recognizedIn" )]
		public List<JurisdictionProfile> RecognizedIn { get; set; }

		[JsonProperty( PropertyName = "ceterms:regulatedIn" )]
		public List<JurisdictionProfile> RegulatedIn { get; set; }

		#endregion

		[JsonProperty( PropertyName = "ceterms:requires" )]
        public List<ConditionProfile> Requires { get; set; }

        [JsonProperty( PropertyName = "ceterms:corequisite" )]
        public List<ConditionProfile> Corequisite { get; set; }

        [JsonProperty( PropertyName = "ceterms:recommends" )]
        public List<ConditionProfile> Recommends { get; set; }

        [JsonProperty( PropertyName = "ceterms:entryCondition" )]
        public List<ConditionProfile> EntryCondition { get; set; }

		[JsonProperty( PropertyName = "ceterms:commonConditions" )]
		public List<EntityBase> CommonConditions { get; set; }

		[JsonProperty( PropertyName = "ceterms:commonCosts" )]
		public List<EntityBase> CommonCosts { get; set; }

        [JsonProperty( PropertyName = "ceterms:administrationProcess" )]
        public List<ProcessProfile> AdministrationProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:developmentProcess" )]
        public List<ProcessProfile> DevelopmentProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
        public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

        [JsonProperty( PropertyName = "ceterms:externalResearch" )]
        public List<IdProperty> ExternalResearch { get; set; }

		[JsonProperty( PropertyName = "ceterms:advancedStandingFrom" )]
		public List<ConditionProfile> AdvancedStandingFrom { get; set; }

		[JsonProperty( PropertyName = "ceterms:isAdvancedStandingFor" )]
		public List<ConditionProfile> IsAdvancedStandingFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:preparationFrom" )]
		public List<ConditionProfile> PreparationFrom { get; set; }

		[JsonProperty( PropertyName = "ceterms:isPreparationFor" )]
		public List<ConditionProfile> IsPreparationFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:isRecommendedFor" )]
		public List<ConditionProfile> IsRecommendedFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:isRequiredFor" )]
		public List<ConditionProfile> IsRequiredFor { get; set; }

		[JsonProperty( PropertyName = "ceterms:availableAt" )]
		public List<AvailableAt> AvailableAt { get; set; }
        
        [JsonProperty( PropertyName = "ceterms:financialAssistance" )]
        public List<FinancialAlignmentObject> FinancialAssistance { get; set; }
    }
}

