using System.Collections.Generic;

namespace RA.Models.Input
{
	public class CompetencyFrameworkRequest : BaseRequest
    {
        public CompetencyFrameworkRequest()
        {
            CompetencyFramework = new CompetencyFramework();
        }
        public CompetencyFramework CompetencyFramework { get; set; } = new CompetencyFramework();

		public List<Competency> Competencies { get; set; } = new List<Competency>();
	}
    public class CompetencyFrameworkGraphRequest : BaseRequest
    {
        public CompetencyFrameworkGraphRequest()
        {
            CompetencyFrameworkGraph = new CompetencyFrameworkGraph();
        }
        public string CTID { get; set; }

        public CompetencyFrameworkGraph CompetencyFrameworkGraph { get; set; } 

    }
    public class CompetencyFramework 
    {
        
        public CompetencyFramework()
        {
        }

        /// <summary>
		/// A unique URI for this framework. 
		/// If not entered, a credential registry URI will be generated using the CTID.
		/// </summary>
        public string CtdlId { get; set; }


		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID) by which the creator, owner or provider of a credential, learning opportunity competency, or assessment recognizes the entity in transactions with the external environment (e.g., in verifiable claims involving a credential).
		/// required
		/// </summary>
		public string Ctid { get; set; }

		/// <summary>
		/// A competency framework or competency from which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// List of URIs to frameworks
		/// </summary>
		public List<string> alignFrom { get; set; } = new List<string>();

		/// <summary>
		/// A competency framework or competency to which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// List of URIs to frameworks
		/// </summary>
		public List<string> alignTo { get; set; } = new List<string>();

		/// <summary>
		/// A person or organization chiefly responsible for the intellectual or artistic content of this competency framework or competency.
		/// List of Names
		/// </summary>
		public List<string> author { get; set; } = new List<string>();

		/// <summary>
		/// A word or phrase used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// The conceptKeyword property is used in ASN-conforming data solely to denote the significant topicality of the competency using free-text keywords and phrases derived and assigned by the indexer, e.g., "George Washington", "Ayers Rock", etc.
		/// </summary>
		public List<string> conceptKeyword { get; set; } = new List<string>();
		public LanguageMapList conceptKeyword_map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Concept Term
		/// Definition:	en-US: A term drawn from a controlled vocabulary used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// List of URIs to concepts
		/// </summary>
		public List<string> conceptTerm { get; set; } = new List<string>();

		/// <summary>
		/// An entity primarily responsible for making this competency framework or competency.
		/// The creator property is used with non-canonical statements created by a third party.
		/// List of URIs to the creator
		/// </summary>
		public List<string> creator { get; set; } = new List<string>();

		/// <summary>
		/// Date Copyrighted
		/// Date of a statement of copyright for this competency framework, such as ©2017.
		/// </summary>
		public string dateCopyrighted { get; set; }

		/// <summary>
		/// Date of creation of this competency framework or competency.
		/// </summary>
		public string dateCreated { get; set; }

		/// <summary>
		/// The date on which this framework or competency was most recently modified in some way.
		/// </summary>
		public string dateModified { get; set; }

		/// <summary>
		/// Beginning date of validity of this competency framework.
		/// Where the competency document is valid for a given period of time, use both the dateValidFrom and dateValidUntil properties.
		/// </summary>
		public string dateValidFrom { get; set; }

		/// <summary>
		/// End date of validity of this competency framework.
		/// Where the standard document is valid for a given period of time, use both the dateValidFrom and dateValidUntil properties.
		/// </summary>
		public string dateValidUntil { get; set; }

		/// <summary>
		/// Derived From
		/// Definition:	en-US: A third party version of the entity being reference that has been modified in meaning through editing, extension or refinement.
		/// List of URIs to frameworks
		/// </summary>
		public string derivedFrom { get; set; }

		/// <summary>
		/// A short description of this competency framework.
		/// </summary>
		public string description { get; set; } 
		//language map version of description
		public LanguageMap description_map { get; set; } = new LanguageMap();

		/// <summary>
		/// Education Level Type
		/// Definition:	en-US:A general statement describing the education or training context. Alternatively, a more specific statement of the location of the audience in terms of its progression through an education or training context.
		/// Best practice is to use terms from the http://purl.org/ctdl/terms/AudienceLevel concept scheme.
		/// List of URIs to concepts
		/// </summary>
		public List<string> educationLevelType { get; set; } = new List<string>();

		/// <summary>
		/// List of the top competencies, those that are directly connected to the framework. Provide either a CTID for a competency that is include in the Competencies property, or the full URI formatted like the following:
		/// "https://credentialengineregistry.org/resources/ce-b1e0eca2-7a19-49e9-8841-fa16ddf8396d"
		/// List of URIs to competencies.
		/// NOTE: or just provide the CTIDs, and the system will format the proper URI for the current environment.
		/// </summary>
		public List<string> hasTopChild { get; set; } = new List<string>();

		/// <summary>
		/// Identifier
		/// Definition:	en-US: An alternative URI by which this competency framework or competency is identified.
		/// List of URIs 
		/// </summary>
		public List<string> identifier { get; set; } = new List<string>();

		public List<string> altIdentifier { get; set; } = new List<string>();
		/// <summary>
		/// In Language
		/// Definition:	en-US: The primary language used in or by this competency framework or competency.The primary language used in or by this competency framework or competency.
		/// This is the language the text is primarily written in, even if it makes use of other languages. For example, a competency for teaching spanish to english-speaking students would primarily be in english, because that is the language used to convey the material.
		/// </summary>
		public List<string> inLanguage { get; set; } = new List<string>();

		/// <summary>
		/// A legal document giving official permission to do something with this competency framework.
		/// Value must be the URI to a license document (e.g., Creative Commons license or bespoke license).
		/// </summary>
		public string license { get; set; }

		/// <summary>
		/// The text string denoting the subject of the competency framework or competency as designated by the promulgating agency.
		/// The localSubject property and subject property may or may not point to the same subject. If so, enter the text string for the subject as the value of this property and the URI for that subject in the subject property.
		/// </summary>
		public LanguageMapList localSubject { get; set; } = new LanguageMapList();

		/// <summary>
		/// The name or title of this competency framework.
		/// </summary>
		public string name { get; set; } 
		public LanguageMap name_map { get; set; } = new LanguageMap();

		/// <summary>
		/// The publication status of the of this competency framework.
		/// </summary>
		public string publicationStatusType { get; set; }

		/// <summary>
		/// An agent responsible for making this entity available.
		/// Also referred to as the promulgating agency of the entity.
		/// List of URIs, for example to a ceterms:CredentialOrganization
		/// </summary>
		public List<string> publisher { get; set; } = new List<string>();

		/// <summary>
		/// Name of an agent responsible for making this entity available.
		/// </summary>
		public List<string> publisherName { get; set; } = new List<string>();
		public LanguageMapList publisherName_map { get; set; } = new LanguageMapList();
		//

		/// <summary>
		/// The date this competency framework was added to the repository.
		/// </summary>
		public string repositoryDate { get; set; }

		/// <summary>
		/// Formerly a URI, 
		/// 19-01-18 Now a language string!!
		/// </summary>
		public string rights { get; set; }
		public LanguageMap rights_map { get; set; } = new LanguageMap();
		//public List<string> rights { get; set; } = new List<string>();

		/// <summary>
		/// An agent owning or managing rights over this competency framework.
		/// URL
		/// </summary>
		public string rightsHolder { get; set; }

		/// <summary>
		///  The original competency framework which this competency framework is based on or derived from.
		///  URI
		/// </summary>
		public List<string> source { get; set; } = new List<string>();

		/// <summary>
		/// A list of sub-units of this competency framework.
		/// The table of contents is a "manifest", or a hierarchic, ordered, syntactic representation of the competencies that are part of this competency framework.
		/// </summary>
		public string tableOfContents { get; set; }
		public LanguageMap tableOfContents_map { get; set; } = new LanguageMap();
		public List<FrameworkItem> OccupationType { get; set; } = new List<FrameworkItem>();

		public List<FrameworkItem> IndustryType { get; set; } = new List<FrameworkItem>();


	}

    public class Competency 
    {
        //required": [ "@type", "@id", "ceasn:competencyText", "ceasn:inLanguage", "ceasn:isPartOf", "ceterms:ctid" ]
        
        public Competency()
        {
        }

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID) by which the creator, owner or provider of a credential, learning opportunity competency, or assessment recognizes the entity in transactions with the external environment (e.g., in verifiable claims involving a credential).
		/// required
		/// </summary>
		public string Ctid { get; set; }

		/// <summary>
		/// The text of the competency.
		/// This property should be used to provide the actual text of the competency statement. To provide information about the competency other than the text itself, use the comment property.
		/// </summary>
		public string competencyText { get; set; } 
		public LanguageMap competencyText_map { get; set; } = new LanguageMap();

		/// <summary>
		/// A competency framework or competency from which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// </summary>
		public List<string> alignFrom { get; set; } = new List<string>();

		/// <summary>
		/// A competency framework or competency to which this competency framework or competency is aligned.
		/// An alignment is an assertion of some degree of equivalency between the subject and the object of the assertion.
		/// </summary>
		public List<string> alignTo { get; set; } = new List<string>();

		/// <summary>
		/// Alternative Coded Notation
		/// An alphanumeric notation or ID code identifying this competency in common use among end-users.
		/// </summary>
		public List<string> altCodedNotation { get; set; } = new List<string>();

		/// <summary>
		/// A person or organization chiefly responsible for the intellectual or artistic content of this competency framework or competency.
		/// List of Names
		/// </summary>
		public List<string> author { get; set; } = new List<string>();

		/// <summary>
		/// Broad Alignment
		/// The referenced competency covers all of the relevant concepts in this competency as well as relevant concepts not found in this competency.
		/// </summary>
		public List<string> broadAlignment { get; set; } = new List<string>();

		/// <summary>
		/// Coded Notation
		/// Definition:	en-US: An alphanumeric notation or ID code as defined by the promulgating body to identify this competency.
		/// </summary>
		public string codedNotation { get; set; }

		/// <summary>
		/// Comment
		/// Definition:	en-US: Supplemental text provided by the promulgating body that clarifies the nature, scope or use of this competency.
		/// </summary>
		public List<string> comment { get; set; } = new List<string>();
		public LanguageMapList comment_map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Competency Category
		/// Definition:	en-US: The textual label identifying the category of the competency as designated by the promulgating body.
		/// </summary>
		public string competencyCategory { get; set; } 
		public LanguageMap competencyCategory_map { get; set; } = new LanguageMap();

		/// <summary>
		/// Complexity Level
		/// Definition:	en-US: The expected performance level of a learner or professional as defined by a competency.
		/// </summary>
		public List<string> complexityLevel { get; set; } = new List<string>();

		/// <summary>
		/// Comprised Of
		/// Definition:	en-US: This competency includes, comprehends or encompasses, in whole or in part, the meaning, nature or importance of the referenced competency.
		/// List of URIs
		/// </summary>
		public List<string> comprisedOf { get; set; } = new List<string>();

		/// <summary>
		/// Concept Keyword
		/// Definition:	en-US: A word or phrase used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// </summary>
		public List<string> conceptKeyword { get; set; } = new List<string>();
		public LanguageMapList conceptKeyword_maplist { get; set; } = new LanguageMapList();

		/// <summary>
		/// Concept Term
		/// Definition:	en-US: A term drawn from a controlled vocabulary used by the promulgating agency to refine and differentiate individual competencies contextually.
		/// List of URIs to a concept
		/// </summary>
		public List<string> conceptTerm { get; set; } = new List<string>();

		/// <summary>
		/// An entity primarily responsible for making this competency framework or competency.
		/// The creator property is used with non-canonical statements created by a third party.
		/// List of URIs to the creator
		/// </summary>
		public List<string> creator { get; set; } = new List<string>();

		/// <summary>
		/// Cross-Subject Reference
		/// Definition:	en-US: A relationship between this competency and a competency in a separate competency framework.
		/// List of URIs to competencies
		/// </summary>
		public List<string> crossSubjectReference { get; set; } = new List<string>();

		/// <summary>
		/// Date of creation of this competency framework or competency.
		/// </summary>
		public string dateCreated { get; set; }

		/// <summary>
		/// The date on which this framework or competency was most recently modified in some way.
		/// </summary>
		public string dateModified { get; set; }

		/// <summary>
		/// Derived From
		/// Definition:	en-US: A third party version of the entity being reference that has been modified in meaning through editing, extension or refinement.
		/// List of URIs to competencies
		/// </summary>
		public string derivedFrom { get; set; }

		/// <summary>
		/// Education Level Type
		/// Definition:	en-US:A general statement describing the education or training context. Alternatively, a more specific statement of the location of the audience in terms of its progression through an education or training context.
		/// Best practice is to use terms from the http://purl.org/ctdl/terms/AudienceLevel concept scheme.
		/// List of URIs to concepts
		/// </summary>
		public List<string> educationLevelType { get; set; } = new List<string>();
		/// <summary>
		/// Resource being described includes, comprehends or encompass, in whole or in part, the meaning, nature or importance of the resource being referenced.
		/// Range Includes: ceasn:Competency, ceasn:Concept
		/// </summary>
		public List<string> encompasses { get; set; } = new List<string>();
		/// <summary>
		/// Exact Alignment
		/// Definition:	en-US: The relevant concepts in this competency and the referenced competency are coextensive.
		/// List of URIs to competencies
		/// </summary>
		public List<string> exactAlignment { get; set; } = new List<string>();

		/// <summary>
		/// Has Child
		/// Definition:	en-US: The referenced competency is lower in some arbitrary hierarchy than this competency.List of URIs for child competencies under this competency
		/// Provide either a CTID for a competency that is include in the Competencies property, or the full URI formatted like the following:
		/// "https://credentialengineregistry.org/resources/ce-b1e0eca2-7a19-49e9-8841-fa16ddf8396d"
		/// List of URIs to competencies.
		/// NOTE: or just provide the CTIDs, and the system will format the proper URI for the current environment.
		/// </summary>
		public List<string> hasChild { get; set; } = new List<string>();

		/// <summary>
		/// Identifier
		/// Definition:	en-US: An alternative URI by which this competency framework or competency is identified.
		/// List of URIs 
		/// </summary>
		public List<string> identifier { get; set; } = new List<string>();

		/// <summary>
		/// Is Child Of
		/// Definition:	en-US: The referenced competency is higher in some arbitrary hierarchy than this competency.
		/// List of URIs to competencies.
		/// NOTE: or just provide the CTIDs, and the system will format the proper URI for the current environment.
		/// </summary>
		public List<string> isChildOf { get; set; } = new List<string>();
		/// <summary>
		/// Indicates that this competency is at the top of the framework.
		/// </summary>
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

		public List<string> knowledgeEmbodied { get; set; } = new List<string>();
		public List<string> taskEmbodied { get; set; } = new List<string>();
		public List<string> hasSourceIdentifier { get; set; }
		public string weight { get; set; }

    }

        
}
