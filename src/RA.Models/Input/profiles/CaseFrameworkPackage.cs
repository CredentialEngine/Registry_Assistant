using System.Collections.Generic;

namespace RA.Models.Input
{
	//CaseFrameworkPackage
	public class CFPackage
	{
		public CFDocument CFDocument { get; set; } = new CFDocument();

		public List<CFItem> CFItems { get; set; } = new List<CFItem>();

		public List<CFAssociation> CFAssociations { get; set; } = new List<CFAssociation>();

		/// <summary>
		/// Class for framework helpers like CFItemTypes and CFLicenses
		/// </summary>
		public CFDefinition CFDefinitions { get; set; } = new CFDefinition();
	}

	/// <summary>
	/// Framework
	/// Missing:
	/// - InLanguage - can default
	/// - Description may not always be present
	/// - Publisher - this is to be a URI to an organization, not the name as below
	///     - this could be required to be the publisher/data owner
	/// CFDocument
	///     CFItemType (blank) Content standards
	///         CFItemType (standard)
	///             CFItemType (topic)
	///                 CFItemType (Competency)
	///                     CFItemType (Benchmark)
	///     CFItemType (blank) ex. Course frameworks
	///         CFItemType (Course)
	///             CFItemType (standard)
	///                 CFItemType (topic)
	///                     CFItemType (Competency)
	///                         CFItemType (Benchmark)
	/// </summary>
	public class CFDocument
	{
		// prefix with ce- to get the CTID
		public string identifier { get; set; }

		// source
		public string uri { get; set; }

		// author
		public string creator { get; set; }

		// framework name
		public string title { get; set; }

		public string description { get; set; }

		// publisherName
		public string publisher { get; set; }

		// dateModified
		public string lastChangeDateTime { get; set; }

		/// <summary>
		/// adoptionStatus
		/// Draft, Adopted, ???
		/// </summary>
		public string adoptionStatus { get; set; }

		/// <summary>
		/// Is this the date of the latter status? If so then could be used for created
		/// </summary>
		public string statusStartDate { get; set; }

		/// <summary>
		/// convert to version code
		/// </summary>
		public string caseVersion { get; set; }
		public string frameworkType { get; set; }

		/// <summary>
		/// These would have to be translated
		/// Current known values: ["AS", "BA"], or ["09","10","11","12",]
		/// </summary>
		public List<string> educationLevel { get; set; } = new List<string>();

		public CFItemTypeURI license { get; set; }

	}

	/// <summary>
	/// Framework item, used for competency
	/// </summary>
	public class CFItem
	{
		public string uri { get; set; }

		public string identifier { get; set; }

		public string lastChangeDateTime { get; set; }

		public string language { get; set; }

		public string fullStatement { get; set; }

		public string abbreviatedStatement { get; set; }

		/// <summary>
		/// humanCodingScheme
		/// ex: Standard 1
		/// </summary>
		public string humanCodingScheme { get; set; }

		/// <summary>
		/// These would have to be translated
		/// Current known values: ["AS", "BA"], or ["09","10","11","12",]
		/// </summary>
		public List<string> educationLevel { get; set; } = new List<string>();

		/// <summary>
		/// CFItemType
		/// Seems to related to CFItemTypeURI. Seems redundant?
		/// Types: Standard, Topic, Competency, Benchmark, Course, Element(competency as well?)
		/// </summary>
		public string CFItemType { get; set; }

		public CFItemTypeURI CFItemTypeURI { get; set; }
	}


	/// <summary>
	/// Association
	/// A node that describes the relationship between two data nodes.
	/// For example isChildOf
	/// </summary>
	public class CFAssociation
	{
		/// <summary>
		/// URI of the node
		/// </summary>
		public string uri { get; set; }

		/// <summary>
		/// Identifier of the node
		/// This will be the last part of the uri
		/// </summary>
		public string identifier { get; set; }

		public string lastChangeDateTime { get; set; }

		public CFItemTypeURI originNodeURI { get; set; }

		/// <summary>
		/// Example: isChildOf
		/// </summary>
		public string associationType { get; set; }

		/// <summary>
		/// could just be IsPartOf
		/// NO, or could depend upon: associationType
		/// </summary>
		public CFItemTypeURI destinationNodeURI { get; set; }
		public string sequenceNumber { get; set; }

	}

	public class CFDefinition
	{
		public List<CFItemTypeDetail> CFItemTypes { get; set; } = new List<CFItemTypeDetail>();
		public List<CFLicense> CFLicenses { get; set; } = new List<CFLicense>();

		/* Also
		 * CFConcepts
		 * CFSubjects
		 * CFAssociationGroupings
		 */ 
	}

	public class CFItemTypeDetail
	{

		/// <summary>
		/// Uri to the current CFItemType directly. Ex
		/// https://case.georgiastandards.org/ims/case/v1p1/CFItemTypes/(identifier)
		/// </summary>
		public string uri { get; set; }

		public string identifier { get; set; }

		public string title { get; set; }

		public string description { get; set; }
		public string typeCode { get; set; }

		public string hierarchyCode { get; set; }

		public string lastChangeDateTime { get; set; }
	}

	/// <summary>
	/// Not sure how to use these nodes
	/// Seems to be the equivalent of CredentialType, except an object instead of just an URI
	/// </summary>
	public class CFItemTypeURI
	{
		public string title { get; set; }

		public string identifier { get; set; }

		public string uri { get; set; }
	}

	public class CFLicense
	{
		public string uri { get; set; }

		public string identifier { get; set; }

		public string title { get; set; }

		public string licenseText { get; set; }

		public string lastChangeDateTime { get; set; }
	}

}
