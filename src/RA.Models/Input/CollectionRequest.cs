using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class CollectionRequest : BaseRequest
	{

		public Collection Collection { get; set; }

		public List<Competency> Competencies { get; set; } = new List<Competency>();
	}
	public class CollectionGraphRequest : BaseRequest
	{
		public CollectionGraphRequest()
		{
			CollectionGraph = new JsonV2.GraphContainer();
		}

		public JsonV2.GraphContainer CollectionGraph { get; set; }

	}
	public class Collection
	{
		public string Type { get; set; } = "ceasn:Collection";


		public string CTID { get; set; }

		public string Author { get; set; }
		//????????????
		public List<string> Classification { get; set; }

		/// <summary>
		/// A word or phrase used by the promulgating agency to refine and differentiate individual resources contextually.
		/// </summary>
		public List<string> ConceptKeyword { get; set; } = new List<string>();
		public LanguageMapList ConceptKeyword_Map { get; set; }
		/// <summary>
		/// An entity primarily responsible for making this resource.
		/// </summary>
		public List<string> Creator { get; set; }


		/// <summary>
		/// Only allow date (yyyy-mm-dd), no time
		/// </summary>
		public string DateCreated { get; set; }

		public string DateModified { get; set; }

		/// <summary>
		/// A short description of this resource.
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; }

		/// <summary>
		/// Resource in a Collection.
		/// Current Range
		/// ceasn:Competency ceterms:Task ceterms:WorkRole ceterms:Job ceterms:Course ceterms:LearningOpportunityProfile ceterms:LearningProgram
		/// </summary>
		public List<string> HasMember { get; set; }

		/// <summary>
		/// An alternative URI by which this competency framework or competency is identified.
		/// </summary>
		public List<string> Identifier { get; set; }

		//The primary language used in or by this resource.
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// URI
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// The name or title of this resource.
		/// </summary>
		public string Name { get; set; } 

		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// An agent responsible for making this resource available.
		/// List of URIs
		/// </summary>
		public List<string> Publisher { get; set; }

		/// <summary>
		/// Name of an agent responsible for making this resource available.
		/// </summary>
		public List<string> PublisherName { get; set; } = new List<string>();
		public LanguageMapList PublisherName_Map { get; set; }

		#region Occupations, Industries, and instructional programs
		//=====================================================================
		//List of occupations from a published framework, that is with a web URL
		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net. 
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		/// </summary>
		public List<FrameworkItem> OccupationType { get; set; } = new List<FrameworkItem>();
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
		public List<FrameworkItem> IndustryType { get; set; } = new List<FrameworkItem>();

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


	}
}
