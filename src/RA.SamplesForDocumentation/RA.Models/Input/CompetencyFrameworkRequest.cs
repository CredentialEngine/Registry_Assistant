using System.Collections.Generic;

namespace RA.Models.Input
{
	public class CompetencyFrameworkRequest : BaseRequest
    {
        public CompetencyFrameworkRequest()
        {
            CompetencyFramework = new CompetencyFramework();
        }
        public string CTID { get; set; }
        public CompetencyFramework CompetencyFramework { get; set; } = new CompetencyFramework();

		public List<Competency> Competencies { get; set; } = new List<Competency>();
	}
    public class CASSCompetencyFrameworkRequest : BaseRequest
    {
        public CASSCompetencyFrameworkRequest()
        {
            CompetencyFrameworkGraph = new CompetencyFrameworksGraph();
        }
        public string CTID { get; set; }

        public CompetencyFrameworksGraph CompetencyFrameworkGraph { get; set; } 

    }
    public class CompetencyFramework 
    {
        
        public CompetencyFramework()
        {
        }
        //won't be entered, only one type
        //public string Type { get; set; }

        //required??
        public string CtdlId { get; set; }

        //required
        public string Ctid { get; set; }

        public List<string> alignFrom { get; set; } = new List<string>();

        public List<string> alignTo { get; set; } = new List<string>();

        public List<string> author { get; set; } = new List<string>();

		public List<string> conceptKeyword { get; set; } = new List<string>();
		public LanguageMapList conceptKeyword_map { get; set; } = new LanguageMapList();

        public List<string> conceptTerm { get; set; } = new List<string>();

        public List<string> creator { get; set; } = new List<string>();

        public string dateCopyrighted { get; set; }

        public string dateCreated { get; set; }


        public string dateValidFrom { get; set; }

        public string dateValidUntil { get; set; }

        public string derivedFrom { get; set; }

		public string description { get; set; } 
		//???language map??
		public LanguageMap description_map { get; set; } = new LanguageMap();

        public List<string> educationLevelType { get; set; } = new List<string>();

		/// <summary>
		/// List of the top competencies, those that are directly connection to the framework. Provide either a CTID for a competency that is include in the Competencies property, or the full URI formatted like the following:
		/// "https://credentialengineregistry.org/resources/ce-b1e0eca2-7a19-49e9-8841-fa16ddf8396d"
		/// </summary>
		public List<string> hasTopChild { get; set; } = new List<string>();

        public List<string> identifier { get; set; } = new List<string>();

        public List<string> inLanguage { get; set; } = new List<string>();

        public string license { get; set; }

        public LanguageMapList localSubject { get; set; } = new LanguageMapList();

		public string name { get; set; } 
		public LanguageMap name_map { get; set; } = new LanguageMap();

        public string publicationStatusType { get; set; }

		/// <summary>
		/// List of URIs
		/// </summary>
        public List<string> publisher { get; set; } = new List<string>();

		public List<string> publisherName { get; set; } = new List<string>();
		public LanguageMapList publisherName_map { get; set; } = new LanguageMapList();
        //

        public string repositoryDate { get; set; }

		/// <summary>
		/// Formerly a URI, 
		/// 19-01-18 Now a language string!!
		/// </summary>
		public string rights { get; set; }
		public LanguageMap rights_map { get; set; } = new LanguageMap();
		//public List<string> rights { get; set; } = new List<string>();

		/// <summary>
		/// URL
		/// </summary>
		public string rightsHolder { get; set; }

        public List<string> source { get; set; } = new List<string>();

		//
		public string tableOfContents { get; set; }
		public LanguageMap tableOfContents_map { get; set; } = new LanguageMap();

		/// <summary>
		/// List of all competencies for this framework
		/// </summary>
        public List<Competency> Competencies { get; set; } = new List<Competency>();
    }

    public class Competency 
    {
        //required": [ "@type", "@id", "ceasn:competencyText", "ceasn:inLanguage", "ceasn:isPartOf", "ceterms:ctid" ]
        
        public Competency()
        {
        }

        //public string Type { get; set; }

        //public string CtdlId { get; set; }

        public string Ctid { get; set; }
		public string competencyText { get; set; } 
		public LanguageMap competencyText_map { get; set; } = new LanguageMap();

        public List<string> alignFrom { get; set; } = new List<string>();

        public List<string> alignTo { get; set; } = new List<string>();

        public List<string> altCodedNotation { get; set; } = new List<string>();

        public List<string> author { get; set; } = new List<string>();

        public List<string> broadAlignment { get; set; } = new List<string>();

        public string codedNotation { get; set; }

		public List<string> comment { get; set; } = new List<string>();
		public LanguageMapList comment_map { get; set; } = new LanguageMapList();

		public List<string> competencyCategory { get; set; } = new List<string>();
		public LanguageMapList competencyCategory_maplist { get; set; } = new LanguageMapList();

        ///ProficiencyScale??
        public List<string> complexityLevel { get; set; } = new List<string>();

        public List<string> comprisedOf { get; set; } = new List<string>();

		public List<string> conceptKeyword { get; set; } = new List<string>();
		public LanguageMapList conceptKeyword_maplist { get; set; } = new LanguageMapList();

        public List<string> conceptTerm { get; set; } = new List<string>();

        public List<string> creator { get; set; } = new List<string>();

        public List<string> crossSubjectReference { get; set; } = new List<string>();

        public string dateCreated { get; set; }

        public string derivedFrom { get; set; } 

        public List<string> educationLevelType { get; set; } = new List<string>();

        public List<string> exactAlignment { get; set; } = new List<string>();

		/// <summary>
		/// List of URIs for child competencies under this competency
		/// Provide either a CTID for a competency that is include in the Competencies property, or the full URI formatted like the following:
		/// "https://credentialengineregistry.org/resources/ce-b1e0eca2-7a19-49e9-8841-fa16ddf8396d"
		/// </summary>
		public List<string> hasChild { get; set; } = new List<string>();

        public List<string> identifier { get; set; } = new List<string>();

        public List<string> inLanguage { get; set; } = new List<string>();

        public List<string> isChildOf { get; set; } = new List<string>();
		public string isTopChildOf { get; set; }

		public string isPartOf { get; set; }
		//public List<string> isPartOf { get; set; } = new List<string>();
		public string isVersionOf { get; set; }

        public string listID { get; set; }

		public List<string> localSubject { get; set; } = new List<string>();
		public LanguageMapList localSubject_maplist { get; set; } = new LanguageMapList();

        public List<string> majorAlignment { get; set; } = new List<string>();

        public List<string> minorAlignment { get; set; } = new List<string>();

        public List<string> narrowAlignment { get; set; } = new List<string>();

        public List<string> prerequisiteAlignment { get; set; } = new List<string>();

        public List<string> skillEmbodied { get; set; } = new List<string>();


        public string weight { get; set; }

    }

        
}
