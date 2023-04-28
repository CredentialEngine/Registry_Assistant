using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Pathway Publishing Request class
	/// </summary>
	public class PathwayRequest : BaseRequest
	{
		/// <summary>
		/// constuctor
		/// </summary>
		public PathwayRequest()
		{
		}

		public Pathway Pathway { get; set; } = new Pathway();

		/// <summary>
		/// GenerateIsHasParts
		/// If true, generate the hasPart and/or isPartOf property where not provided for pathways
		/// </summary>
		public bool GenerateIsHasParts { get; set; } = true;

		/// <summary>
		/// Pathway Components
		/// </summary>
		public List<PathwayComponent> PathwayComponents { get; set; } = new List<PathwayComponent>();

	}//
	/// <summary>
	/// Proposed option to publish a document already formatted as CTDL JSON-LD.
	/// </summary>
	public class PathwayGraphRequest : BaseRequest
	{
		public PathwayGraphRequest()
		{
			PathwayGraph = new GraphInput();
		}

		public GraphInput PathwayGraph { get; set; }

	}
	/// <summary>
	/// Resource composed of a structured set of PathwayComponents defining points along a route to fulfillment of a goal or objective.
	/// </summary>
	public class Pathway
	{
		public Pathway(){}

		#region *** Required Properties ***
		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Pathway Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Pathway Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// The webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// This property identifies a child pathway(s) or pathwayComponent(s) in the downward path.
		/// Provide the CTID or the full URI for the target environment. 
		/// It would be a burden to have a user provide a blank node Id.
		/// However, we could recommend that a CTID is provided, and just convert.
		/// </summary>
		public List<string> HasChild { get; set; } = new List<string>();

		/// <summary>
		/// Goal or destination node of the pathway. 
		/// Provide the CTID or the full URI for the target environment. 
		/// URI for a ceterms:PathwayComponent
		/// Multipicity: Many
		/// </summary>
		public List<string> HasDestinationComponent { get; set; } = new List<string>();

		/// <summary>
		/// This property identifies all the PathwayComponents in a Pathway
		/// Provide the CTID or the full URI for the target environment. 
		/// However, we recommend that a CTID be provided, and the API will format accordingly.
		/// As a helper, this could be generated from all of the provided components
		/// 
		/// 
		/// Per meeting on July 16, 2020 Stuart agreed that ceterms:Pathway will NOT have the property ceterms:hasPart.
		/// </summary>
		public List<string> HasPart { get; set; } = new List<string>();

		/// <summary>
		/// CTID/URL to a Progression Model
		/// </summary>
		public string HasProgressionModel { get; set; }

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


		#endregion
		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; } = new List<string>();
        /// <summary>
        /// Language map list for Keyword
        /// </summary>
        public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// </summary>
		public List<string> Subject { get; set; } = new List<string>();
		/// <summary>
		/// Language map list for Subject
		/// </summary>
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

		#region Occupations, and Industries
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
		//public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid NAICS codes. These will be mapped to industry type
		/// See:
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
		/// Language map list for AlternativeInstructionalProgramType
		/// </summary>
		public LanguageMapList AlternativeInstructionalProgramType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion
	}

    /// <summary>
    /// History
    /// 21-01-06 remove CodedNotation
	/// 23-04-30 Add CodedNotation for competencyComponent only
    /// </summary>
    public class PathwayComponent 
	{
		/// <summary>
		/// Type of PathwayComponent. 
		/// Valid values (with or without ceterms:) :
		/// ceterms:AssessmentComponent	
		/// ceterms:BasicComponent	
		/// ceterms:CocurricularComponent	
		/// ceterms:CompetencyComponent	
		/// ceterms:CourseComponent 	
		/// ceterms:CredentialComponent 	
		/// ceterms:ExtracurricularComponent 	
		/// ceterms:JobComponent 	
		/// ceterms:selectioncomponent  2022-01-31 - OBSOLETE
		/// ceterms:WorkExperienceComponent
		/// </summary>
		public string PathwayComponentType { get; set; }



		/// <summary>
		/// PathwayComponent Name
		/// Required
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		#region Common Properties
		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Label identifying the category to further distinguish one component from another as designated by the promulgating body.
		/// Examples may include "Required", "Core", "General Education", "Elective", etc.
		/// </summary>
		public List<string> ComponentDesignation { get; set; } = new List<string>();


		/// <summary>
		/// PathwayComponent Description 
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Resource(s) that describes what must be done to complete a PathwayComponent, or part thereof, as determined by the issuer of the Pathway.
		/// ceterms:ComponentCondition
		/// </summary>
		public List<ComponentCondition> HasCondition { get; set; } = new List<ComponentCondition>();

		/// <summary>
		/// This property identifies a child pathway(s) or pathwayComponent(s) in the downward path.
		/// Provide the CTID or the full URI for the target environment. 
		/// NOTE: This would include all target components from related component conditions. The API will populate HasChild with the latter componentCondition.Target Components if not already present.
		/// ceterms:PathwayComponent
		/// </summary>
		public List<string> HasChild { get; set; } = new List<string>();

		/// <summary>
		/// Concept in a ProgressionModel concept scheme
		/// URI
		/// </summary>
		public List<string> HasProgressionLevel { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see href="https://purl.org/ctdl/terms/identifier"/>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();


		/// <summary>
		/// The referenced resource is higher in some arbitrary hierarchy than this resource.
		/// Provide the CTID or the full URI for the target environment. 
		/// This is an inverse property to HasChild. It is not necessary to provide both properties. 
		/// ceterms:PathwayComponent
		/// </summary>
		public List<string> IsChildOf { get; set; } = new List<string>();


		/// <summary>
		/// Pathway for which this resource is the goal or destination.
		/// Like IsTopChildOf
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:Pathway
		/// </summary>
		public List<string> IsDestinationComponentOf { get; set; } = new List<string>();

		/// <summary>
		/// This property identifies the Pathways of which it is a part. 		/// 
		/// Provide the CTID or the full URI for the target environment. 
		/// Note if this is left empty (recommended) the API will assign the current Pathway
		/// </summary>
		public List<string> IsPartOf { get; set; } = new List<string>();


		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Points associated with this resource, or points possible.
		/// </summary>
		public QuantitativeValue PointValue { get; set; } = new QuantitativeValue();


		/// <summary>
		/// Resource that logically comes after this resource.
		/// This property indicates a simple or suggested ordering of resources; if a required ordering is intended, use ceterms:prerequisite instead.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:ComponentCondition
		/// </summary>
		public List<string> Precedes { get; set; } = new List<string>();

		/// <summary>
		/// Component is preceded by the referenced components
		/// </summary>
		public List<string> PrecededBy { get; set; } = new List<string>();

        ///// <summary>
        ///// Resource(s) required as a prior condition to this resource.
        ///// Provide the CTID or the full URI for the target environment. 
        ///// ceterms:ComponentCondition
        ///// </summary>
        //[Obsolete]		//June 30, 2022
        //public List<string> Prerequisite { get; set; } = new List<string>();

        /// <summary>
        /// Indicates the resource for which a pathway component or similar proxy resource is a stand-in.
        /// This property is slated to completely replace SourceData in late 2022
        /// URL
        /// </summary>
        public string ProxyFor { get; set; }

        /// <summary>
        /// URL to structured data representing the resource.
        /// The preferred data serialization is JSON-LD or some other serialization of RDF.
        /// If this is a registry URI, the user can just provide a CTID.
		/// NOTE THIS WILL LIKELY BE COMPLETELY REPLACED BY ProxyFor
        /// URL
        /// </summary>
        [Obsolete] //early warning
		public string SourceData { get; set; }
		/// <summary>
		/// Where the source data is not in the registry, a 'blank node' can be provided. 
		/// The blank node would most likely be of a type closely associated with the the type of pathway component. 
		/// Examples: learningOpportunity/Course for a CourseComponent, AssessmentProfile for an AssessmentComponent, etc. 
		/// </summary>
		public List<EntityReference> ProxyForBNode { get; set; } = new List<EntityReference>();

		/// <summary>
		/// The webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }
		#endregion

		#region BasicComponent,	CocurricularComponent, ExtracurricularComponent 
		/// <summary>
		/// Component Category
		/// Identifies the type of PathwayComponent subclass not explicitly covered in the current array of PathwayComponent subclasses.
		/// Used by: 
		/// ceterms:BasicComponent,	ceterms:CocurricularComponent, ceterms:ExtracurricularComponent 
		/// </summary>
		public string ComponentCategory { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap ComponentCategory_Map { get; set; } = new LanguageMap();
        #endregion


        #region CompetencyComponent
        /// <summary>
        ///  Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
        /// Used by: 
        /// ceterms:CompetencyComponent only
        /// </summary>
        public string CodedNotation { get; set; }

        #endregion


        #region CourseComponent
        /// <summary>
        /// CreditValue
        /// A credit-related value.
        /// Used by: 
        /// ceterms:CourseComponent only 
        /// </summary>
        public List<ValueProfile> CreditValue { get; set; } = new List<ValueProfile>();

		/// <summary>
		/// ProgramTerm
		/// Categorization of a term sequence based on the normative time between entry into a program of study and its completion such as "1st quarter", "2nd quarter"..."5th quarter".
		/// Used by: 
		/// ceterms:CourseComponent only 
		/// </summary>
		public string ProgramTerm { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap ProgramTerm_Map { get; set; } = new LanguageMap();

		#endregion

		#region CredentialComponent
		/// <summary>
		/// Type of credential such as badge, certification, bachelor degree.
		/// The credential type as defined in CTDL. 
		/// Used by: 
		/// ceterms:CredentialComponent only
		/// </summary>
		public string CredentialType { get; set; }

		#endregion


		#region JobComponent only - OccupationType and Industry type
		/// <summary>
		/// OccupationType
		/// Type of occupation; select from an existing enumeration of such types.
		///  For U.S. credentials, best practice is to identify an occupation using a framework such as the O*Net. 
		///  Other credentials may use any framework of the class ceterms:OccupationClassification, such as the EU's ESCO, ISCO-08, and SOC 2010.
		///  ceterms:occupationType
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

		// Industry type and helpers
		/// <summary>
		/// IndustryType
		/// Type of industry; select from an existing enumeration of such types such as the SIC, NAICS, and ISIC classifications.
		/// Best practice in identifying industries for U.S. credentials is to provide the NAICS code using the ceterms:naics property. 
		/// Other credentials may use the ceterms:industrytype property and any framework of the class ceterms:IndustryClassification.
		/// ceterms:industryType
		/// </summary>
		public List<FrameworkItem> IndustryType { get; set; } = new List<FrameworkItem>();
		/// <summary>
		/// AlternativeIndustryType
		/// Industries that are not found in a formal framework can be still added using AlternativeIndustryType. 
		/// Any industries added using this property will be added to or appended to the IndustryType output.
		/// </summary>
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		/// <summary>
		/// List of valid NAICS codes. These will be mapped to industry type
		/// See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> NaicsList { get; set; } = new List<string>();
		#endregion
	}

	/// <summary>
	/// Component Condition
	/// </summary>
	public class ComponentCondition
	{

		/// <summary>
		/// ComponentCondition Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();


		/// <summary>
		/// ComponentCondition Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Number of targetComponent resources that must be fulfilled in order to satisfy the ComponentCondition.
		/// Integer
		/// </summary>
		public int RequiredNumber { get; set; }

		/// <summary>
		/// Candidate PathwayComponent for the ComponentCondition as determined by applying the RuleSet.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:PathwayComponent
		/// LIst of CTIDs (recommended) or fully qualified registry URL
		/// </summary>
		public List<string> TargetComponent { get; set; } = new List<string>();

		/// <summary>
		/// Resource(s) that describes what must be done to complete a PathwayComponent, or part thereof, as determined by the issuer of the Pathway.
		/// ceterms:ComponentCondition
		/// </summary>
		public List<ComponentCondition> HasCondition { get; set; } = new List<ComponentCondition>();

		/// <summary>
		/// Referenced resource defines a single constraint.
		/// URI or CTID??
		/// ceterms:hasConstraint
		///  Range: ceterms:Constraint
		/// </summary>
		public List<Constraint> HasConstraint { get; set; } = new List<Constraint>();

		/// <summary>
		/// Type that denotes a logical operation such as "AND", "OR", "NOT"; select from an existing enumeration of such types.
		/// ceterms:logicalOperator
		/// Range: ceterms:Concept (Select from a controlled vocabulary: ceterms:logicalOperator)
		/// </summary>
		public string LogicalOperator { get; set; }

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

	}

	/// <summary>
	/// Resource that identifies the parameters defining a limitation or restriction applicable to candidate pathway components.
	/// </summary>
	public class Constraint
	{
		/// <summary>
		/// Constraint Name
		/// Optional
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Constraint Description 
		/// Optional
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Type of symbol that denotes an operator in a constraint expression such as "gteq" (greater than or equal to), "eq" (equal to), "lt" (less than), "isAllOf" (is all of), "isAnyOf" (is any of); 
		/// select from an existing enumeration of such types.
		/// ceterms:Concept (Select from a controlled vocabulary-ceterms:Comparator)
		/// </summary>
		public string Comparator { get; set; }

		/// <summary>
		/// Left hand parameter of a constraint.
		/// Range: rdf:Property, skos:Concept (Select from a controlled vocabulary)
		/// </summary>
		public List<string> LeftSource { get; set; } = new List<string>();

        /// <summary>
        /// Action performed on the left constraint; 
        /// Required if LeftSource has multiple values.
        /// 
        /// Range: ceterms:Concept (Select from a controlled vocabulary-ceterms:ArrayOperation)
        /// </summary>
        public string LeftAction { get; set; }


		/// <summary>
		/// Right hand parameter of a constraint.
		/// Range: rdf:Property, skos:Concept (Select from a controlled vocabulary)
		/// </summary>
		public List<string> RightSource { get; set; } = new List<string>();

        /// <summary>
        /// Action performed on the right constraint; 
        /// Required if RightSource has multiple values.
        /// 
        /// Range: ceterms:Concept (Select from a controlled vocabulary-ceterms:ArrayOperation)
        /// </summary>
        public string RightAction{ get; set; }


		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

	}


}
