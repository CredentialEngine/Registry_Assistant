using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Common input class for all process profiles 
    /// 2018-09-02 Where LanguageMap alternates are available, only enter one. The system will check the string version first. 
    /// </summary>
    public class ProcessProfile
    {
        public ProcessProfile()
        {
            ExternalInputType = new List<string>();
			Jurisdiction = new List<Jurisdiction>();
			
			TargetCredential = new List<EntityReference>();
			TargetAssessment = new List<EntityReference>();
			TargetLearningOpportunity = new List<EntityReference>();
			TargetCompetencyFramework = new List<EntityReference>();
			//Region = new List<GeoCoordinates>();
		}
        public string Description { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();
        public string SubjectWebpage { get; set; }
		public string DateEffective { get; set; }
		//public List<CredentialAlignmentObject> ExternalInputType { get; set; }
		public List<string> ExternalInputType { get; set; }
		public string ProcessFrequency { get; set; }
        public LanguageMap ProcessFrequency_Map { get; set; } = new LanguageMap();
        public List<OrganizationReference> ProcessingAgent { get; set; } = new List<OrganizationReference>();
        public string ProcessMethod { get; set; }
        public string ProcessMethodDescription { get; set; }
        public LanguageMap ProcessMethodDescription_Map { get; set; } = new LanguageMap();
        public string ProcessStandards { get; set; }
        public string ProcessStandardsDescription { get; set; }
        public LanguageMap ProcessStandardsDescription_Map { get; set; } = new LanguageMap();
        public string ScoringMethodDescription { get; set; }
        public LanguageMap ScoringMethodDescription_Map { get; set; } = new LanguageMap();
        public string ScoringMethodExample { get; set; }
        public string ScoringMethodExampleDescription { get; set; }
        public LanguageMap ScoringMethodExampleDescription_Map { get; set; } = new LanguageMap();

        public List<EntityReference> TargetCredential { get; set; }
        public List<EntityReference> TargetAssessment { get; set; }
        public List<EntityReference> TargetLearningOpportunity { get; set; }
        public List<EntityReference> TargetCompetencyFramework{ get; set; }
        public string VerificationMethodDescription { get; set; }
        public LanguageMap VerificationMethodDescription_Map { get; set; } = new LanguageMap();
        public List<Jurisdiction> Jurisdiction { get; set; }
        //public List<GeoCoordinates> Region { get; set; }

    }
}



