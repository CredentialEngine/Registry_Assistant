using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class RubricRequest : BaseRequest
	{
		public RubricRequest()
		{
		}
		/// <summary>
		/// A structured and systematic evaluation tool used to assess performance, quality, and/or criteria.
		/// </summary>
		public Rubric Rubric { get; set; } = new Rubric();

		/// <summary>
		/// Resource providing explicit criteria for ensuring specific and measurable evaluation.
		/// </summary>
		public List<RubricCriterion> RubricCriterion { get; set; } = new List<RubricCriterion>();

		/// <summary>
		/// Level or quality indicator used with Rubric Criteria.
		/// </summary>
		public List<RubricLevel> RubricLevel { get; set; } = new List<RubricLevel>();

		///// <summary>
		///// Criterion Category for this Rubric
		///// Replaced by ceasn:hasCriterionCategory - points to a concept
		///// </summary>
		//[Obsolete]
		//public List<CriterionCategory> CriterionCategory { get; set; } = new List<CriterionCategory>();

		/// <summary>
		/// An individual component or specific element within a criterion that defines a particular aspect or standard for evaluation.
		/// </summary>
		public List<CriterionLevel> CriterionLevel { get; set; } = new List<CriterionLevel>();
	}

	/// <summary>
	/// A structured and systematic evaluation tool used to assess performance, quality, and/or criteria.
	/// A rubric is typically an evaluation tool or set of guidelines used to promote the consistent application of learning expectations, learning objectives, or learning standards in the classroom, or to measure their attainment against a consistent set of criteria. In instructional settings, rubrics clearly define academic expectations for students and help to ensure consistency in the evaluation of academic work from student to student, assignment to assignment, or course to course. Rubrics are also used as scoring instruments to determine grades or the degree to which learning standards have been demonstrated or attained by students.
	/// <see cref="http://standards.asn.desire2learn.com/rubric.html"/>
	/// </summary>
	public class Rubric
	{
		
		public Rubric()
		{
		}

		#region required
		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

        /// <summary>
        /// A name given to the resource.
		/// Required
        /// </summary>
        public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// An account of the resource.
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// The primary language used in or by this resource.
		/// Required
		/// </summary>
		public List<string> InLanguage { get; set; } = new List<string>();

		/// <summary>
		/// An agent responsible for making this entity available.
		/// Also referred to as the promulgating agency of the entity.
		/// List of URIs, for example to a ceterms:CredentialOrganization
		/// Or provide a list of CTIDs and the Assistant API will format the proper URL for the environment.
		/// Required
		/// </summary>
		public List<string> Publisher { get; set; } = new List<string>();

		/// <summary>
		/// Webpage that describes this entity.
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; }  //URI

		#endregion

        /// <summary>
        /// Alternative Coded Notation
        /// An alphanumeric notation or ID code identifying this competency in common use among end-users.
        /// ceasn:altCodedNotation
        /// </summary>
        public List<string> AltCodedNotation { get; set; } = new List<string>();

		/// <summary>
		/// The type of credential seeker for whom the entity is applicable; select from an existing enumeration of such types.
		/// <see href="https://credreg.net/ctdl/terms/Audience"></see>
		/// ceterms:Audience
		/// </summary>
		public List<string> AudienceType { get; set; }

        /// <summary>
        /// Type of level indicating a point in a progression through an educational or training context, for which the resource is intended; select from an existing enumeration of such types.
        /// <see href="https://credreg.net/ctdl/terms/AudienceLevel"></see>
		/// ceterms:audienceLevelType
        /// </summary>
        public List<string> AudienceLevelType { get; set; } = new List<string>();

		public List<string> DeliveryType { get; set; } = new List<string>();

		/// <summary>
		/// A general statement describing the education or training context. Alternatively, a more specific statement of the location of the audience in terms of its progression through an education or training context.
		/// ConceptScheme: ceterms:AudienceLevel
		/// </summary>
		public List<string> EducationLevelType { get; set; } = new List<string>();

		/// <summary>
		/// Type of evaluator; select from an existing enumeration of such types.
		/// ConceptScheme: ceasn:EvaluatorCategory
		/// </summary>
		public List<string> EvaluatorType { get; set; } = new List<string>();

		#region Classification
		/// <summary>
		/// Category or classification of this resource.
		/// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
		/// URI to a concept(based on the ONet work activities example) or to a blank node in RA.Models.Input.BaseRequest.ReferenceObjects
		/// ceterms:classification
		/// </summary>
		public List<string> Classification { get; set; } = new List<string>();


        #endregion

        /// <summary>
        /// Set of alpha-numeric symbols that uniquely identifies an item and supports its discovery and use.
        /// ceasn:codedNotation
        /// </summary>
        public string CodedNotation { get; set; }

        /// <summary>
        /// An entity primarily responsible for making this competency framework or competency.
        /// The creator property is used with non-canonical statements created by a third party.
        /// List of URIs (CTIDs recommended) to the creator
        /// ceasn:creator
        /// </summary>
        public List<string> Creator { get; set; } = new List<string>();

        /// <summary>
        /// Date Copyrighted
        /// Date of a statement of copyright for this competency framework, such as ©2017.
        /// ceasn:dateCopyrighted
        /// </summary>
        public string DateCopyrighted { get; set; }

        /// <summary>
        /// Only allow date (yyyy-mm-dd), no time
        /// xsd:date
        /// </summary>
        public string DateCreated { get; set; }

        /// <summary>
        ///  The date on which this resource was most recently modified in some way.
        /// xsd:dateTime
        /// </summary>
        public string DateModified { get; set; }

        /// <summary>
        /// xsd:dateTime
        /// </summary>
        public string DateValidFrom { get; set; }

		/// <summary>
		/// DateValidUntil
		/// xsd:dateTime
		/// </summary>
		public string DateValidUntil { get; set; }

        /// <summary>
        /// Derived From
        /// A third party version of the entity being reference that has been modified in meaning through editing, extension or refinement.
        /// List of URIs to frameworks
        /// ceasn:derivedFrom
        /// </summary>
        public List<string> DerivedFrom { get; set; }


        //????these are URIs/CTIDs? - could imply RubricCriterion is to be a top level class NO CTID yet
        /// <summary>
        /// RubricCriterian referenced defines a principle or standard to be met that demonstrates quality in performance of a task or obtaining an objective.
        /// List of CTIDs/URIs to a RubricCriterion
        /// </summary>
        public List<string> HasRubricCriterion { get; set; } = new List<string>();

		/// <summary>
		/// List of blank node identifiers that refer to an entry in request.RubricLevel
		/// </summary>
		public List<string> HasRubricLevel { get; set; } = new List<string>();

		///// <summary>
		///// Has Criterion Category
		///// Resource referenced by the Rubric that defines categories for clustering logical sets of RubricCriterion.
		///// ?List of URIs? or concepts? Oh, just CriterionCategory. So the latter have CTIDs.
		///// </summary>
		//public List<string> HasCriterionCategory { get; set; } = new List<string>();

		/// <summary>
		///  Indicates the Concept Scheme for clustering logical sets of Rubric Criteria.
		///  URI to concept scheme or blank node
		/// </summary>
		public string HasCriterionCategorySet { get; set; } 

		/// <summary>
		/// Reference to a progression model used.
		/// TBD - string, or list
		/// </summary>
		public string HasProgressionModel { get; set; }

        /// <summary>
        /// Reference to a progression level used.
        /// TBD - string, or list
        /// </summary>
        public string HasProgressionLevel { get; set; }

		/// <summary>
		/// Description of what the rubric's creator intended to assess or evaluate.
		/// asn:hasScope
		/// </summary>
		public string HasScope { get; set; }
		public LanguageMap HasScope_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see href="https://credreg.net/ctdl/terms/identifier">Identifier</see>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// </summary>
		public string InCatalog { get; set; }


		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> ConceptKeyword { get; set; }
		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		public LanguageMapList ConceptKeyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Latest version of the resource.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string LatestVersion { get; set; }

        /// <summary>
        /// A legal document giving official permission to do something with this resource.
        /// </summary>
        public string License { get; set; }

		/// <summary>
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Recommended
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// Version of the resource that immediately follows this version.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string NextVersion { get; set; }

        /// <summary>
        /// Previous version of the resource.
        /// full URL OR CTID (recommended)
        /// </summary>
        public string PreviousVersion { get; set; }

        /// <summary>
        /// List of Organizations that offer this credential in a specific Jurisdiction. 
        /// </summary>
        public List<JurisdictionAssertion> OfferedIn { get; set; } = new List<JurisdictionAssertion>();

        /// <summary>
        /// The publication status of the of this competency framework.
        /// </summary>
        public string PublicationStatusType { get; set; }

        /// <summary>
        /// Name of an agent responsible for making this entity available.
        /// </summary>
        public List<string> PublisherName { get; set; } = new List<string>();
        /// <summary>
        /// Language map for publisher name
        /// </summary>
        public LanguageMapList PublisherName_Map { get; set; } = new LanguageMapList();

        /// <summary>
        /// Information about rights held in and over this resource.
        /// ceasn:rights
        /// </summary>
        public string Rights { get; set; }
        public LanguageMap Rights_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
        /// </summary>
        public List<string> Subject { get; set; } = new List<string>();
        /// <summary>
        /// Language map list for Subject
        /// </summary>
        public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

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
        /// Language map list for AlternativeOccupationType
        /// </summary>
        public LanguageMapList AlternativeOccupationType_Map { get; set; } = new LanguageMapList();
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
        /// Language map list for AlternativeIndustryType
        /// </summary>
        public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();
		/// <summary>
		/// List of valid NAICS codes. See:
		/// https://www.naics.com/search/
		/// </summary>
		public List<string> Naics { get; set; }

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

		/// <summary>
		/// Occupation that is the focus or target of this resource.
		/// List of CTIDs, URIs or blank nodes
		/// </summary>
		public List<string> TargetOccupation { get; set; } = new List<string>();

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();

        #region -- Process Profiles --
        /// <summary>
        /// Description of a process by which a resource is administered.
        /// ceterms:administrationProcess
        /// </summary>
        public List<ProcessProfile> AdministrationProcess { get; set; }

        /// <summary>
        /// Description of a process by which a resource was created.
        /// </summary>
        public List<ProcessProfile> DevelopmentProcess { get; set; }

        /// <summary>
        ///  Description of a process by which a resource is maintained, including review and updating.
        /// </summary>
        public List<ProcessProfile> MaintenanceProcess { get; set; }

        /// <summary>
        /// Description of a process by which a resource is reviewed.
        /// </summary>
        public List<ProcessProfile> ReviewProcess { get; set; }

        #endregion

    }
    public class RubricLevel
	{

		public RubricLevel()
		{
		}

		/// <summary>
		/// The identifier for a Rubric level.
		/// Must be a valid blank node identifier: _:UUID
		/// example:		_:9c09016c-907e-46ab-8d63-c1cda5474836
		/// Classes with the property hasRubricLevel would a list of strings (this bnode identifier)
		/// </summary>
		public string Id { get; set; }

		#region base properties

		/// <summary>
		/// A name given to the resource.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Language map for Name
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();


		#endregion
		public string CodedNotation { get; set; }
		/// <summary>
		/// Criterion Level for this resource.
		/// List of CriterionLevel
		/// </summary>	
		public List<string> HasCriterionLevel { get; set; } = new List<string>();

		/// <summary>
		/// Reference to a progression level used.
		/// TBD - string, or list
		/// </summary>
		public List<string> HasProgressionLevel { get; set; } = new List<string>();

		public string ListID { get; set; }
	}

	/// <summary>
	/// RubricCriterian defines a principle or standard to be met that demonstrates quality in performance of a task or obtaining an objective.
	/// </summary>
	public class RubricCriterion
	{

		public RubricCriterion()
		{
		}

		#region Base properties
		public string CTID { get; set; }

		/// <summary>
		/// A name given to the resource.
		/// </summary>
		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// An account of the resource.
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		//public List<string> Language { get; set; } = new List<string>();

		public string CodedNotation { get; set; }

		/// <summary>
		/// Concept in a ProgressionModel concept scheme
		/// URI
		/// </summary>
		public List<string> HasProgressionLevel { get; set; } = new List<string>();

		/// <summary>
		/// An alphanumeric string indicating the relative position of a resource in an ordered list of resources such as "A", "B", or "a", "b", or "I", "II", or "1", "2".
		/// </summary>
		public string ListID { get; set; }

		public decimal? Weight { get; set; }
		#endregion

		#region relationship properties

		/// <summary>
		/// Indicates a Concept for clustering logical sets of Rubric Criteria.
		/// No specific concept scheme
		/// </summary>
		public List<string> HasCriterionCategory { get; set; } = new List<string>();

		/// <summary>
		/// Resource description of a level of performance based on a RubricCriterion.
		/// List of CriterionLevel
		/// </summary>
		public List<string> HasCriterionLevel { get; set; } = new List<string>();

		/// <summary>
		/// Task that is the focus or target of this resource.
		/// CTID for an existing Task
		/// </summary>
		public List<string> TargetTask { get; set; } = new List<string>();

		/// <summary>
		/// A competency relevant to the condition being described.
		/// targetCompetency is typically a competency required for the parent of this condition profile
		/// TODO - the range for targetCompetency is a credentialAlignmentObject or Compentency. Need to handle the latter.
		/// Does that mean CAO should be a blank node?
		/// </summary>
		public List<CredentialAlignmentObject> TargetCompetency { get; set; }
		#endregion


	}
	
    /// <summary>
    /// Resource description of a level of performance based on a RubricCriterion.
    /// </summary>
    public class CriterionLevel
	{

		public CriterionLevel()
		{
		}
		/// <summary>
		/// Identifier for this CriterionLevel.
		/// Use a unique identifier typically using the blank node format of "_:" + UUID.
		/// Where a class has the property: HasCriterionLevel, it will have a list of these type of identifiers.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Label for the level achieved as defined by the Rubric Criterion.
		/// At least one of BenchmarkLabel or BenchmarkText is required
		/// </summary>
		public string BenchmarkLabel { get; set; }
		public LanguageMap BenchmarkLabel_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Description of the level achieved as defined by the Rubric Criterion.
		/// At least one of BenchmarkLabel or BenchmarkText is required
		/// </summary>
		public string BenchmarkText { get; set; }
		public LanguageMap BenchmarkText_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// An alphanumeric notation or ID code as defined by the promulgating body to identify this resource.
		/// </summary>
		public string CodedNotation { get; set; }
		public string ListID { get; set; }
		/// <summary>
		/// Predefined feedback text for the benefit of the subject being evaluated.
		/// </summary>
		public string Feedback { get; set; }
		public LanguageMap Feedback_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Indicates whether the criterion level is evaluated as having been met or not.
		/// </summary>
		public bool? IsBinaryEvaluation { get; set; }

		/// <summary>
		/// Points to be awarded for achieving this level for a RubricCriterion.
		/// </summary>
		public decimal? Value { get; set; }
		public decimal? MinValue { get; set; }
		public decimal? MaxValue { get; set; }

		public decimal? Percentage { get; set; }
		public decimal? MinPercentage { get; set; }
		public decimal? MaxPercentage { get; set; }


		#region relationship properties

		/// <summary>
		/// Reference to the RubricCriterion to which the CriterionLevel being described belongs.
		/// IS THIS REALLY AN INVERSE PROPERTY??
		/// </summary>
		public List<string> HasCriterionLevel { get; set; } = new List<string>();

		/// <summary>
		/// Reference to a progression model used.
		/// </summary>
		//[JsonProperty( "asn:hasProgressionLevel" )]
		public List<string> HasProgressionLevel { get; set; } = new List<string>();

		#endregion
	}
}

