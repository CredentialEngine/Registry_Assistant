using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Collection Request class
	/// </summary>
	public class CollectionRequest : BaseRequest
	{
		/// <summary>
		/// Collection to publish
		/// </summary>
		public Collection Collection { get; set; }
		/// <summary>
		/// Members can be any of:
		/// "ceterms:AssessmentProfile",
		/// "ceterms:CollectionMember",
		/// "ceterms:Credential", //any of the valid credential subclasses
		/// "ceasn:Competency",
		/// "ceterms:Course",
		/// "ceterms:Job",
		/// "ceterms:LearningOpportunityProfile",
		/// "ceterms:LearningProgram",
		/// "ceterms:Occupation",
		/// "ceterms:Task",
		/// "ceterms:WorkRole",
		/// </summary>
		public List<object> Members { get; set; } = new List<object>();

		/// <summary>
		/// CollectionMember
		/// Collection members will be published in the graph like Members, but have a separate input propery for better organization
		/// </summary>
		public List<CollectionMember> CollectionMembers { get; set; } = new List<CollectionMember>();
	}

	/// <summary>
	/// Proposed option to publish a document already formatted as CTDL JSON-LD.
	/// </summary>
	public class CollectionGraphRequest : BaseRequest
	{
		public CollectionGraphRequest()
		{
			CollectionGraph = new GraphInput();
		}

		public GraphInput CollectionGraph { get; set; }

	}
	
	/// <summary>
	/// Collection
	/// </summary>
	public class Collection
	{
		/// <summary>
		/// Type for this class
		/// </summary>
		public string Type { get; set; } = "ceterms:Collection";

		/// <summary>
		/// CTDL unique identifier
		/// </summary>
		public string CTID { get; set; }

		#region Classification
		/// <summary>
		/// Category or classification of this resource.
		/// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
		/// URI to a concept(based on the ONet work activities example)
		/// Recommend using CTIDs
		/// ceterms:classification
		/// </summary>
		public List<string> Classification { get; set; } = new List<string>();

		/// <summary>
		/// Additional Classification
		/// List of concepts that don't exist in the registry. Will be published as blank nodes
		/// OR should input be a list of Concepts?
		/// </summary>
		public List<CredentialAlignmentObject> AdditionalClassification { get; set; } = new List<CredentialAlignmentObject>();
		#endregion

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies an item and supports its discovery and use.
		/// ceterms:codedNotation
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		public string DateEffective { get; set; }
		/// <summary>
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// A short description of this resource.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Language map for Description
		/// </summary>
		public LanguageMap Description_Map { get; set; }

		/// <summary>
		/// Resource in a Collection.
		/// REQUIRED
		/// NEW - using object, as could be: CTID, URI, or CollectionMember, or credential classes, assessment, learning classes, 
		/// Current Range
		/// ceasn:Competency ceterms:Task ceterms:WorkRole ceterms:Job ceterms:Course ceterms:LearningOpportunityProfile ceterms:LearningProgram
		/// List of CTIDs (recommended) or URIs
		/// </summary>
		public List<string> HasMember { get; set; } = new List<string>();

		///// <summary>
		///// An alternative URI by which this competency framework or competency is identified.
		///// xsd:anyURI
		///// 21-10-31 - removed Identifier
		///// </summary>
		//public List<string> Identifier { get; set; }

		/// <summary>
		/// The primary language used in or by this resource.
		/// </summary>
		public List<string> InLanguage { get; set; }

		/// <summary>
		/// A word or phrase used by the promulgating agency to refine and differentiate individual resources contextually.
		/// </summary>
		public List<string> Keyword { get; set; }
		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// URI
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// Type of official status of the TransferProfile; select from an enumeration of such types.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// Type of collection, list, set, or other grouping of resources; select from an existing enumeration of such types.
		/// ConceptScheme: CollectionCategory 
		/// Current valid values:
		/// collectionCategory:ETPL, collectionCategory:GIBill, collectionCategory:IndustryRecognized, collectionCategory:Quality, collectionCategory:Perkins
		/// </summary>
		public List<string> CollectionType { get; set; }

		/// <summary>
		/// The name or title of this resource.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Language map for Name
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Conditions for collection membership
		/// </summary>
		public List<ConditionProfile> MembershipCondition { get; set; } = new List<ConditionProfile>();

		/// <summary>
		/// Organization that owns this resource
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Subjects
		/// </summary>
		public List<string> Subject { get; set; }
		/// <summary>
		/// Language map list for Subject
		/// </summary>
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		public List<string> SubjectWebpage { get; set; } = new List<string>();

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

	/// <summary>
	/// Collection Member (proposed)
	/// The collection member will use a blank node format for the id.
	/// Tip: we may use the CTID from the is proxy for in the blank node id
	/// </summary>
	public class CollectionMember
	{
		/// <summary>
		/// Type for this class
		/// </summary>
		public string Type { get; set; } = "ceterms:CollectionMember";

		/// <summary>
		/// A short description of this resource.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Language map for Description
		/// </summary>
		public LanguageMap Description_Map { get; set; }

		/// <summary>
		/// The name or title of this resource.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Language map for Name
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Indicates the resource for which this resource is a stand-in.
		/// CTID/URI
		/// OR could may use an object to support BNodes
		/// </summary>
		public string ProxyFor { get; set; }

		/// <summary>
		/// Start date of this resource
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		public string StartDate { get; set; }
		/// <summary>
		/// Expiration date of this resource
		/// Only allow date (yyyy-mm-dd), no time
		/// xsd:date
		/// </summary>
		public string EndDate { get; set; }

	}
}
