using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class LearningOpportunityRequest
	{
		public LearningOpportunityRequest()
		{
			LearningOpportunity = new LearningOpportunity();
		}

		public LearningOpportunity LearningOpportunity { get; set; }

		/// <summary>
		/// API key for the requesting partner - required for publishing
		/// May ultimately be passed in the header
		/// </summary>
		public string APIKey { get; set; }

		/// <summary>
		/// Envelope Identifier
		/// Currently required to update an existing document
		/// </summary>
		public string RegistryEnvelopeId { get; set; }
	}

	public class LearningOpportunity
	{
		public LearningOpportunity()
		{
			Subject = new List<string>();
			Keyword = new List<string>();
            DeliveryType = new List<string>();
            InstructionalProgramType = new List<string>();
			LearningMethodType = new List<string>();
			EstimatedCosts = new List<CostProfile>();
			Jurisdiction = new List<Jurisdiction>();
			//Region = new List<GeoCoordinates>();
            //SubjectWebpage = new List<string>();
            AvailabilityListing = new List<string>();
            AvailableOnlineAt = new List<string>();
			CodedNotation = new List<string>();

			//OwnedBy = new List<string>();
			OwnedBy = new OrganizationReference();
			
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

			TeachesCompetency = new List<CredentialAlignmentObject>();
			//RequiresCompetency = new List<CredentialAlignmentObject>();
			
			AdvancedStandingFrom = new List<Connections>();
			IsAdvancedStandingFor = new List<Connections>();
			PreparationFrom = new List<Connections>();
			IsPreparationFor = new List<Connections>();
			IsRecommendedFor = new List<Connections>();
			IsRequiredFor = new List<Connections>();
			InLanguage = new List<string>();
			AvailableAt = new List<PostalAddress>();
            CommonConditions = new List<EntityReference>();
            CommonCosts = new List<EntityReference>();
            FinancialAssistance = new List<Input.FinancialAlignmentObject>();
        }

		public string Name { get; set; }
		public string Description { get; set; }
		public List<string> InLanguage { get; set; }
		public List<string> Keyword { get; set; }
        public List<string> Subject { get; set; }
        public string SubjectWebpage { get; set; } //URL
      
		public List<string> CodedNotation { get; set; }
		public string DateEffective { get; set; }
        //public string Type { get; set; }
        public string Ctid { get; set; }
		
		/// <summary>
		/// Organization that owns this credential
		/// </summary>
		public OrganizationReference OwnedBy { get; set; }

		public string VerificationMethodDescription { get; set; }
        public List<string> AvailableOnlineAt { get; set; } //URL
		public List<string> AvailabilityListing { get; set; } //URL
		public List<string> LearningMethodType { get; set; }
		public List<string> DeliveryType { get; set; }
        public string DeliveryTypeDescription { get; set; }
        public List<string> InstructionalProgramType { get; set; }
        public List<DurationProfile> EstimatedDuration { get; set; }

        public string CreditHourType { get; set; }
        public string CreditUnitType { get; set; }

        public decimal CreditHourValue { get; set; }
        public decimal CreditUnitValue { get; set; }
        public string CreditUnitTypeDescription { get; set; }
        //external classes
        public List<CostProfile> EstimatedCosts { get; set; }

		public List<Jurisdiction> Jurisdiction { get; set; }
		//public List<GeoCoordinates> Region { get; set; }
		
        public List<OrganizationReference> AccreditedBy { get; set; }
        public List<OrganizationReference> ApprovedBy { get; set; }
        public List<OrganizationReference> OfferedBy { get; set; }
        public List<OrganizationReference> RecognizedBy { get; set; }
		public List<OrganizationReference> RegulatedBy { get; set; }


		public List<JurisdictionAssertedInProfile> JurisdictionAssertions { get; set; }
      
		public List<ConditionProfile> Requires { get; set; }
        public List<ConditionProfile> Corequisite { get; set; }
        public List<ConditionProfile> Recommends { get; set; }
        public List<ConditionProfile> EntryCondition { get; set; }

		public List<CredentialAlignmentObject> TeachesCompetency { get; set; }
		//required competencies are input with condition profiles
		//public List<CredentialAlignmentObject> RequiresCompetency { get; set; }

		public List<Connections> AdvancedStandingFrom { get; set; }
		public List<Connections> IsAdvancedStandingFor { get; set; }
		public List<Connections> PreparationFrom { get; set; }
		public List<Connections> IsPreparationFor { get; set; }
		public List<Connections> IsRecommendedFor { get; set; }
		public List<Connections> IsRequiredFor { get; set; }

		public List<PostalAddress> AvailableAt { get; set; }
        public List<EntityReference> CommonCosts { get; set; }
        public List<EntityReference> CommonConditions { get; set; }
        public List<FinancialAlignmentObject> FinancialAssistance { get; set; }
    }
}
