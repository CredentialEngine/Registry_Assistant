using System.Collections.Generic;

namespace RA.Models.Input
{
	public class AssessmentRequest : BaseRequest
	{
		public AssessmentRequest()
		{
			Assessment = new Assessment();
		}

		public Assessment Assessment { get; set; }


	}
	public class Assessment
    {
        public Assessment()
        {
            Subject = new List<string>();
            Keyword = new List<string>();
			
			AssessmentMethodType = new List<string>();
			CodedNotation = new List<string>();
			AssessmentUseType = new List<string>();

            AvailabilityListing = new List<string>();
            AvailableOnlineAt = new List<string>();
			Jurisdiction = new List<Input.Jurisdiction>();
			JurisdictionAssertions = new List<JurisdictionAssertedInProfile>();
			DeliveryType = new List<string>();

            InstructionalProgramType = new List<FrameworkItem>();
            EstimatedCost = new List<CostProfile>();
            EstimatedDuration = new List<DurationProfile>();
			//
            ScoringMethodType = new List<string>();
			
            AccreditedBy = new List<Input.OrganizationReference>();
            ApprovedBy = new List<Input.OrganizationReference>();
            OfferedBy = new List<Input.OrganizationReference>();
            RecognizedBy = new List<Input.OrganizationReference>();
            RegulatedBy = new List<Input.OrganizationReference>();

	        Corequisite = new List<ConditionProfile>();
            Recommends = new List<ConditionProfile>();
            Requires = new List<ConditionProfile>();
            EntryCondition = new List<ConditionProfile>();
			
			AdministrationProcess = new List<ProcessProfile>();
			DevelopmentProcess = new List<ProcessProfile>();
			MaintenanceProcess = new List<ProcessProfile>();

            Assesses = new List<CredentialAlignmentObject>();
			//RequiresCompetency = new List<CredentialAlignmentObject>();

			AvailableAt = new List<Place>();
			
			AdvancedStandingFrom = new List<Connections>();
			IsAdvancedStandingFor = new List<Connections>();
			PreparationFrom = new List<Connections>();
			IsPreparationFor = new List<Connections>();
			IsRecommendedFor = new List<Connections>();
			IsRequiredFor = new List<Connections>();

			ExternalResearch = new List<string>();
			InLanguage = new List<string>();
            CommonConditions = new List<string>();
            CommonCosts = new List<string>();
            FinancialAssistance = new List<Input.FinancialAlignmentObject>();
			VersionIdentifier = new List<IdentifierValue>();

		}



		#region *** Required Properties ***
		public string Name { get; set; }
		public string Description { get; set; }
		public string Ctid { get; set; }
		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// Organization that owns this resource
		/// </summary>
        public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();
        #endregion



        #region *** Required if available Properties ***

        #endregion

        #region *** Recommended Properties ***

        #endregion


        public List<string> Keyword { get; set; }
        public List<string> Subject { get; set; }
        
        public List<string> AssessmentMethodType { get; set; }
        public List<string> CodedNotation { get; set; }
        public string DateEffective { get; set; }
      
        public string AssessmentExample { get; set; }
        public string AssessmentExampleDescription { get; set; }
        public string AssessmentOutput { get; set; }
        public List<string> AssessmentUseType { get; set; }
        public List<string> AvailabilityListing { get; set; }
        public List<string> AvailableOnlineAt { get; set; }
       
		public List<string> InLanguage { get; set; }
		public List<Jurisdiction> Jurisdiction { get; set; }

	

        public string ProcessStandards { get; set; }

        public string ProcessStandardsDescription { get; set; }
        public List<string> DeliveryType { get; set; }
        public string DeliveryTypeDescription { get; set; }

		public List<FrameworkItem> InstructionalProgramType { get; set; }
		public bool? IsProctored { get; set; }
        public bool? HasGroupEvaluation { get; set; }
        public bool? HasGroupParticipation { get; set; }

        public List<DurationProfile> EstimatedDuration { get; set; }        
        //external classes
        public List<CostProfile> EstimatedCost { get; set; }
        public List<CostProfile> EstimatedCosts {
            get { return EstimatedCost; }
            set { EstimatedCost = value; }
        } 

        public string ScoringMethodDescription { get; set; }
        public string ScoringMethodExample { get; set; }
        public string ScoringMethodExampleDescription { get; set; }
        public List<string> ScoringMethodType { get; set; }
        public string CreditHourType { get; set; }
        public string CreditUnitType { get; set; }
        public decimal CreditHourValue { get; set; }
        public decimal CreditUnitValue { get; set; }
        public string CreditUnitTypeDescription { get; set; }

		public List<Connections> AdvancedStandingFrom { get; set; }
		public List<Connections> IsAdvancedStandingFor { get; set; }
		public List<Connections> PreparationFrom { get; set; }
		public List<Connections> IsPreparationFor { get; set; }
		public List<Connections> IsRecommendedFor { get; set; }
		public List<Connections> IsRequiredFor { get; set; }

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

        public List<ProcessProfile> AdministrationProcess { get; set; }
        public List<ProcessProfile> DevelopmentProcess { get; set; }
        public List<ProcessProfile> MaintenanceProcess { get; set; }
        public List<string> ExternalResearch { get; set; }

		public List<CredentialAlignmentObject> Assesses{ get; set; }
        public List<CredentialAlignmentObject> AssessesCompetency
        {
            get { return Assesses; }
            set { Assesses = value; }
        }
        //required competencies are handled with condition profiles
        //public List<CredentialAlignmentObject> RequiresCompetency { get; set; }

        public List<Place> AvailableAt { get; set; }
        public List<string> CommonCosts { get; set; }
        public List<string> CommonConditions { get; set; }
        public List<FinancialAlignmentObject> FinancialAssistance { get; set; }
		public List<IdentifierValue> VersionIdentifier { get; set; }
	}
}
