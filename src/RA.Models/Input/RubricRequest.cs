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

		/// <summary>
		/// Criterion Category for this Rubric
		/// Replaced by ceasn:hasCriterionCategory - points to a concept
		/// </summary>
		[Obsolete]
		public List<CriterionCategory> CriterionCategory { get; set; } = new List<CriterionCategory>();

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
		/*
		

	ceterms:administrationProcess	x
	ceasn:altCodedNotation		x
	ceterms:alternateName		x			??
	ceterms:audienceLevelType	x	
	ceterms:audienceType		x
	ceterms:classification      x
	ceasn:codedNotation			x
	ceasn:conceptKeyword
	ceasn:creator               x
	ceterms:ctid				x
	ceasn:dateCopyrighted       x
	ceasn:dateCreated           x
	ceterms:dateEffective       x		remove
	ceasn:dateModified          x
	ceasn:dateValidFrom         x
	ceasn:dateValidUntil        x
	ceterms:deliveryType
	ceterms:description			x
	ceasn:derivedFrom			x
	ceasn:educationLevelType		Use audienceLevel
	ceterms:expirationDate      x	remove
	ceasn:evaluatorType

	ceasn:hasCriterionCategorySe
	ceasn:hasRubricCriterion
	ceasn:hasRubricLevel

	asn:hasProgressionLevel     x   TBD on multiplicity
	asn:hasProgressionModel     x   TBD on multiplicity
	ceasn:hasScope              x
	ceterms:identifier			x

	ceterms:inLanguage			x

	ceterms:industryType		x
	ceterms:instructionalProgramType	x
	ceterms:keyword				x
	ceterms:latestVersion		x
	ceasn:license               x
	ceterms:lifecycleStatusType

	ceterms:name				x
	ceterms:nextVersion			x
	ceterms:occupationType		x
	ceterms:offeredIn			x
	ceterms:previousVersion		x
	ceasn:publicationStatusType x
	ceasn:publisher             x
	ceasn:publisherName         x
	ceasn:rights                x
	ceterms:subject             x
	ceterms:subjectWebpage		x
	ceterms:targetOccupation
	ceterms:versionIdentifier   x


		*/
		public Rubric()
		{
		}
		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

        /// <summary>
        /// A name given to the resource.
        /// </summary>
        public string Name { get; set; }
        public LanguageMap Name_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// An account of the resource.
        /// </summary>
        //[JsonProperty( "dcterms:description" )]
        public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// Entity describing the process by which a credential, assessment, organization, or aspects of it, are administered.
        /// ceterms:administrationProcess
        /// </summary>
        public List<ProcessProfile> AdministrationProcess { get; set; }

        /// <summary>
        /// Alternative Coded Notation
        /// An alphanumeric notation or ID code identifying this competency in common use among end-users.
        /// ceasn:altCodedNotation
        /// </summary>
        public List<string> AltCodedNotation { get; set; } = new List<string>();

        /// <summary>
        /// List of Alternate Names for this learning opportunity
        /// </summary>
        public List<string> AlternateName { get; set; } = new List<string>();
        /// <summary>
        /// LanguageMap for AlternateName
        /// </summary>
        public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

        /// <summary>
        /// The type of credential seeker for whom the entity is applicable; select from an existing enumeration of such types.
        /// <see href="https://credreg.net/ctdl/terms/Audience"></see>
		/// ceterms:audienceType
        /// </summary>
        public List<string> AudienceType { get; set; }

        /// <summary>
        /// Type of level indicating a point in a progression through an educational or training context, for which the resource is intended; select from an existing enumeration of such types.
        /// <see href="https://credreg.net/ctdl/terms/AudienceLevel"></see>
		/// ceterms:audienceLevelType
        /// </summary>
        public List<string> AudienceLevelType { get; set; } = new List<string>();

        #region Classification
        /// <summary>
        /// Category or classification of this resource.
        /// Where a more specific property exists, such as ceterms:naics, ceterms:isicV4, ceterms:credentialType, etc., use that property instead of this one.
        /// URI to a concept(based on the O*Net work activities example). 
        /// OR can use blank nodes where the blank node Id would be in this list
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
        /// Effective date of this resource's content.
        /// Only allowing date (yyyy-mm-dd), no time. 
        /// xsd:date
        /// </summary>
        public string DateEffective { get; set; }

        /// <summary>
        /// xsd:dateTime
        /// </summary>
        public string DateValidFrom { get; set; }

        /// <summary>
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

        /// <summary>
        /// Date beyond which the resource is no longer offered or available.
        /// Only allowing date (yyyy-mm-dd), no time. 
        /// xsd:date
        /// ceterms:expirationDate
        /// </summary>
        public string ExpirationDate { get; set; }

        //????these are URIs/CTIDs? - could imply RubricCriterion is to be a top level class NO CTID yet
        /// <summary>
        /// RubricCriterian referenced defines a principle or standard to be met that demonstrates quality in performance of a task or obtaining an objective.
        /// List of CTIDs/URIs to a RubricCriterion
        /// </summary>
        public List<string> HasCriterionList { get; set; } = new List<string>();

        public List<RubricCriterion> HasRubricCriterion { get; set; } = new List<RubricCriterion>();

        /// <summary>
        /// Has Criterion Category
        /// Resource referenced by the Rubric that defines categories for clustering logical sets of RubricCriterion.
        /// ?LIst of URIs? or concepts? Oh, just CriterionCategory. So the latter have CTIDs.
        /// </summary>
        public List<string> HasCriterionCategory { get; set; } = new List<string>();

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

        public List<string> InLanguage { get; set; } = new List<string>();

        /// <summary>
        /// Keyword or key phrase describing relevant aspects of an entity.
        /// </summary>
        public List<string> Keyword { get; set; }
        /// <summary>
        /// Language map list for Keyword
        /// </summary>
        public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

        //
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
        /// An agent responsible for making this entity available.
        /// Also referred to as the promulgating agency of the entity.
        /// List of URIs, for example to a ceterms:CredentialOrganization
        /// Or provide a list of CTIDs and the Assistant API will format the proper URL for the environment.
        /// Required
        /// </summary>
        public List<string> Publisher { get; set; } = new List<string>();

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
        /// Original resource on which this resource is based or derived from.
        /// </summary>
        public string Source { get; set; }  //URI

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
        /// VersionIdentifier
        /// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
        /// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
        /// </summary>
        public List<IdentifierValue> VersionIdentifier { get; set; } = new List<IdentifierValue>();
    }
	public class RubricLevel
	{
		/*
ceasn:codedNotation
ceasn:description
asn:hasProgressionLevel
ceasn:listID
ceasn:name


		*/
		public RubricLevel()
		{
		}

		#region base properties
		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

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
		/// Resource description of a level of performance based on a RubricCriterion.
		/// List of CriterionLevel
		/// </summary>
		public List<CriterionLevel> HasCriterionItem { get; set; } = new List<CriterionLevel>();
		public string ListID { get; set; }
	}

	/// <summary>
	/// RubricCriterian defines a principle or standard to be met that demonstrates quality in performance of a task or obtaining an objective.
	/// </summary>
	public class RubricCriterion
	{
		/*
		 * ceasn:codedNotation	
		 * ceterms:ctid
		 * ceasn:description
		 * asn:hasProgressionLevel
		 * ceasn:listID
		 * ceasn:name
			ceterms:targetCompetency
			ceterms:targetTask
			ceasn:weight
			ceasn:hasCriterionLevel


		 * 
		 * 
		 */
		public RubricCriterion()
		{
		}

		#region Base properties

		//public string CTID { get; set; }

		/// <summary>
		/// An account of the resource.
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		public List<string> Language { get; set; } = new List<string>();

		/// <summary>
		/// Numeric value representing the resource's position in a list (array) of resources.
		/// </summary>
		public int Sequence { get; set; }

		/// <summary>
		/// A name given to the resource.
		/// </summary>
		public string Title { get; set; }
		public LanguageMap Title_Map { get; set; } = new LanguageMap();
		#endregion

		#region relationship properties

		/// <summary>
		/// Reference to the Rubric to which the RubricCriteria being described belongs.
		/// /// List of Rubric URIs
		/// </summary>
		public List<string> CriterionFor { get; set; } = new List<string>();

		/// <summary>
		/// Resource description of a level of performance based on a RubricCriterion.
		/// List of CriterionLevel
		/// </summary>
		public List<CriterionLevel> HasCriterionItem { get; set; } = new List<CriterionLevel>();
		#endregion`


	}

	/// <summary>
	/// Resource that defines categories for clustering logical sets of RubricCriterion.
	/// </summary>
	[Obsolete]
	public class CriterionCategory
	{
		public CriterionCategory()
		{
		}

		//[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		/// <summary>
		/// An account of the resource.
		/// </summary>
		//[JsonProperty( "dcterms:description" )]
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		//[JsonProperty( "dcterms:Language" )]
		public List<string> Language { get; set; } = new List<string>();

		/// <summary>
		/// A name given to the resource.
		/// </summary>
		//[JsonProperty( "dcterms:title" )]
		public string Title { get; set; }
		public LanguageMap Title_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Resource referenced is a Rubric to which this CriterionCategory belongs.
		/// List of Rubric URIs
		/// </summary>
		//[JsonProperty( "asn:criterionCategoryOf" )]
		public List<string> CriterionCategoryOf { get; set; } = new List<string>();

		/// <summary>
		/// RubricCriterian referenced defines a principle or standard to be met that demonstrates quality in performance of a task or obtaining an objective.
		/// List of RubricCriterian
		/// LIKELY REQUIRED??
		/// </summary>
		//[JsonProperty( "asn:hasCriterion" )]
		public List<string> HasCriterion { get; set; } = new List<string>();

	}

    /// <summary>
    /// Resource description of a level of performance based on a RubricCriterion.
    /// </summary>
    public class CriterionLevel
	{
		/*
			ceasn:codedNotation
			ceasn:benchmarkLabel
			ceasn:benchmarkText
			ceasn:feedback
			ceasn:hasCriterionLevel
			asn:hasProgressionLevel
			ceasn:isBinaryEvaluation
			ceasn:listID
			qdata:maxPercentage
			schema:maxValue
			qdata:minPercentage
			schema:minValue
			qdata:percentage
			schema:value


		 */
		public CriterionLevel()
		{
		}

		#region base properties

		/// <summary>
		/// Description of a level of achievement in performance of a task defined by the RubricCriterion.
		/// </summary>
		public string Benchmark { get; set; }
		public LanguageMap Benchmark_Map { get; set; } = new LanguageMap();


		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		public List<string> Language { get; set; } = new List<string>();

		/// <summary>
		/// Qualitative description of the degree of achievement used for a column header in a tabular rubric.
		/// </summary>
		//[JsonProperty( "asn:qualityLabel" )]
		public string QualityLabel { get; set; }
		public LanguageMap QualityLabel_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Points to be awarded for achieving this level for a RubricCriterion.
		/// </summary>
		//[JsonProperty( "asn:score" )]
		public decimal? Score { get; set; }

		/// <summary>
		/// Numeric value representing the resource's position in a list (array) of resources.
		/// </summary>
		//[JsonProperty( "asn:sequence" )]
		public int Sequence { get; set; }

		#endregion

		#region relationship properties

		/// <summary>
		/// Reference to the RubricCriterion to which the CriterionLevel being described belongs.
		/// </summary>
		//[JsonProperty( "asn:levelFor" )]
		public List<string> LevelFor { get; set; } = new List<string>();

		/// <summary>
		/// Reference to a progression model used.
		/// </summary>
		//[JsonProperty( "asn:hasProgressionLevel" )]
		public List<string> HasProgressionLevel { get; set; } = new List<string>();

		#endregion
	}
}

