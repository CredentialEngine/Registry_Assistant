﻿using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.Input
{
    /// <summary>
    /// Progression Model Publishing Request
    /// 22-09-27 Noticed that progression model and progression level are diverging rapidly from concept scheme and concept. Creating custom classess
    /// </summary>
    public class ProgressionModelRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public ProgressionModelRequest()
		{
			ProgressionModel = new ProgressionModel();
		}

		/// <summary>
		/// ProgressionModel
		/// Required
		/// </summary>
		public ProgressionModel ProgressionModel { get; set; } = new ProgressionModel();

		/// <summary>
		/// ProgressionLevels for ProgressionModel
		/// Required
		/// </summary>
		public List<ProgressionLevel> ProgressionLevels { get; set; } = new List<ProgressionLevel>();

		/// <summary>
		/// Generate HasTopChild
		/// if true, the HasTopChild property is not included in the input document. The HasTopChild property in the JSON document will be generated from the Concept list.
		/// Should only be used where the structure is flat. That is there are no concepts that have child concepts. 
		///			SO that is: all concepts are top childs.
		/// Note: in some cases IsTopChildOf was provided and not HasTopChild. In this case: GenerateHasTopChild=true, and GenerateIsTopChild=false
		/// </summary>
		public bool GenerateHasTopChild { get; set; } = false;
		/// <summary>
		/// Generate HasTopChild where the concept has property of: TopConceptOf
		/// </summary>
		public bool GenerateHasTopChildFromIsTopChild { get; set; } = false;
		/// <summary>
		/// Generate IsTopChild
		/// if true, the IsTopChild property must not be included in the input document and the IsTopChild property in the JSON document will be generated for each concept in the list.
		/// Must only be used where the structure is flat. That is there are no concepts have child concepts.
		/// </summary>
		public bool GenerateIsTopChild { get; set; } = false;
	}

	/// <summary>
	/// Progression Model
	/// </summary>
	public class ProgressionModel
    {

        /// <summary>
        /// constructor
        /// </summary>
        public ProgressionModel()
        {
            Type = "ProgressionModel";
            ConceptTerm = null;
        }

		/// <summary>
		/// Helper property for use with blank nodes
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// CTID - identifier for Progression Model. 
		/// REQUIRED
		/// </summary>
		public string CTID { get; set; }

        /// <summary>
        /// Name of the Progression Model
        /// REQUIRED
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Name_LangMap { get; set; } = new LanguageMap();

        /// <summary>
        /// Progression Model description
        /// REQUIRED and must be a minimum of 15 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_LangMap { get; set; } = new LanguageMap();

        /// <summary>
        /// Top Concepts
        /// Concept of the scheme at the top of a hierarchy of narrower concepts.
        /// List of CTIDs (recommended) or actual registry URIs
        /// REQUIRED
        /// </summary>
        public List<string> HasTopConcept { get; set; } = new List<string>();

        /// <summary>
        /// Language. 
        /// Required unless defaultLanguage is provided
        /// </summary>
        public List<string> InLanguage { get; set; } = new List<string>();

        /// <summary>
        /// An agent responsible for making this resource available.
        /// This was originally defined as a single, and continuing to match CaSS.
        ///  REQUIRED
        /// </summary>
        public List<OrganizationReference> Publisher { get; set; } = new List<OrganizationReference>();

        /// <summary>
        /// ChangeNotes via LanguageMapList
        /// </summary>
        public LanguageMapList ChangeNote_LangMap { get; set; } = new LanguageMapList();

        /// <summary>
        /// Concept Keyword
        /// A word or phrase used by the promulgating agency to refine and differentiate individual resources contextually.
        /// </summary>
        public List<string> ConceptKeyword { get; set; } = new List<string>();
		/// <summary>
		/// Concept Keywords via LanguageMapList
		/// </summary>
		public LanguageMapList ConceptKeyword_LangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// Concept Term
		/// A term drawn from a controlled vocabulary used by the promulgating agency to refine and differentiate individual resources contextually.
		/// skos:Concept
		/// </summary>
		public List<string> ConceptTerm { get; set; } 

		/// <summary>
		/// Creator
		/// An entity primarily responsible for making this resource.
		/// </summary>
		public List<OrganizationReference> Creator { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Date Copyrighted
		/// Date of a statement of copyright for this resource. Typically the year only, but a full date is allowed.
		/// xsd:string
		/// </summary>
		public string DateCopyrighted { get; set; }

		/// <summary>
		/// Date Created
		/// Date of creation of this resource.
		/// xsd:date
		/// </summary>
		public string DateCreated { get; set; }

		/// <summary>
		/// The date on which this resource was most recently modified in some way.
		/// xsd:dateTime
		/// </summary>
		public string DateModified { get; set; }

		/// <summary>
		/// A legal document giving official permission to do something with this resource.
		/// xsd:anyURI
		/// </summary>
		public string License { get; set; }

		/// <summary>
		/// The publication status of the of this resource.
		/// <seealso cref="https://credreg.net/ctdlasn/terms/PublicationStatus"/>
		/// </summary>
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Name of an agent responsible for making this resource available.
		/// </summary>
		public List<string> PublisherName { get; set; } = new List<string>();
		public LanguageMapList PublisherName_LangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// Information about rights held in and over this resource.
		/// rdf:langString
		/// </summary>
		public string Rights { get; set; }
		public LanguageMap Rights_LangMap { get; set; } = new LanguageMap();

		/// <summary>
		///  An agent owning or managing rights over this resource.
		///  Use OrganizationReference for flexibility
		/// </summary>
		public OrganizationReference RightsHolder { get; set; } = new OrganizationReference();

		/// <summary>
		/// Original Source of Progression Model
		/// xsd:anyURI
		/// </summary>
		public List<string> Source { get; set; } = new List<string>();

	}

	/// <summary>
	/// ProgressionLevel currently the same as the Concept class
	/// </summary>
	public class ProgressionLevel : BasePrimaryResource
    {

		/// <summary>
		/// CTID - identifier for concept. 
		/// Format: ce-UUID (lowercase)
		/// example: ce-a044dbd5-12ec-4747-97bd-a8311eb0a042
		/// </summary>
		public string CTID { get; set; }

        /// <summary>
        /// PrefLabel 
        /// Required
        /// </summary>
        public string PrefLabel { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap PrefLabel_LangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Progression Model to which this Progression Level belongs.
		/// </summary>
		public string InProgressionModel { get; set; }

		/// <summary>
		/// Concept that is broader in some way than this concept.
		/// Concept URL (CTID)
		/// </summary>
		public string Broader { get; set; }

        /// <summary>
        /// Supplies a complete explanation of the intended meaning of a concept.
        /// </summary>
        public string Definition { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Definition_LangMap { get; set; } = new LanguageMap();

        /// <summary>
        /// Description of a specific aspect or characteristic of this resource.
        /// List of Competencies(CTIDs)
        /// </summary>
        public List<string> FacetedDescription { get; set; } = new List<string>();

        /// <summary>
        /// Concept that is narrower in some way than this concept.
        /// List of Concept URLs(CTIDs)
        /// </summary>
        public List<string> Narrower { get; set; } = new List<string>();

		/// <summary>
		/// Alphanumeric notation or ID code as defined by the promulgating body to identify this resource.
		/// </summary>
		public string Notation { get; set; }

		/// <summary>
		/// Annotations to the concept for purposes of general documentation.
		/// rdf:langString
		/// </summary>
		public List<string> Note { get; set; } = new List<string>();

        [JsonProperty( PropertyName = "skos:note" )]
        public LanguageMapList Note_LangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// Progression Level that logically comes after this level.
		/// Provide the CTID or the full URI for the target pathway component. 
		/// </summary>
		public string Precedes { get; set; }

        /// <summary>
        /// Progression Level is preceded by the referenced components
        /// </summary>
        public string PrecededBy { get; set; }

		/// <summary>
		/// URI to the Progression Model
		/// </summary>
		public string TopConceptOf { get; set; }
	}
}
