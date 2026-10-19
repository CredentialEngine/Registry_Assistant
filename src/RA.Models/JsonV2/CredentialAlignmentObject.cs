using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Credential Alignment Object
	/// Modifications
	/// 2017-10-17 TargetNodeName is now required!
	/// </summary>
	public class CredentialAlignmentObject
	{
		/// <summary>
		/// constructor
		/// </summary>
		public CredentialAlignmentObject()
		{
			Type = "ceterms:CredentialAlignmentObject";
			BNodeId = null;
			CodedNotation = null;
			AlignmentDate = null;
			AlignmentType = null;
		}

		/// <summary>
		/// Need a custom mapping to @type based on input value
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// An identifier for use with blank nodes, to minimize duplicates
		/// </summary>
		[JsonProperty( "@id" )]
		public string BNodeId { get; set; }

		/// <summary>
		/// Alignment Date
		/// The date the alignment was made.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alignmentDate" )]
		public string AlignmentDate { get; set; }

		/// <summary>
		/// Alignment Type
		/// A category of alignment between the learning resource and the framework node.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alignmentType" )]
		public string AlignmentType { get; set; }

		/// <summary>
		/// Coded Notation
		/// A short set of alpha-numeric symbols that uniquely identifies a resource and supports its discovery.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:codedNotation" )]
		public string CodedNotation { get; set; }

		/// <summary>
		/// Framework URL
		/// The framework to which the resource being described is aligned.Must be a valid URL.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:framework" )]
		public string Framework { get; set; }

		/// <summary>
		/// Framework Name
		/// Formal name of the framework, or progression model, etc.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:frameworkName" )]
		public LanguageMap FrameworkName { get; set; }

		/// <summary>
		/// Target Description
		/// The description of a node in an established educational framework.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:targetNodeDescription" )]
		public LanguageMap TargetNodeDescription { get; set; } = new LanguageMap();

		/// <summary>
		/// Target Node
		/// The node of a framework targeted by the alignment. Must be a valid URL or short URI.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:targetNode" )]
		public string TargetNode { get; set; }

		/// <summary>
		/// Target Node Name
		/// The name of a node in an established educational framework.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:targetNodeName" )]
		public LanguageMap TargetNodeName { get; set; } = new LanguageMap();

		/// <summary>
		/// Weight
		/// An asserted measurement of the weight, degree, percent, or strength of a recommendation, requirement, or comparison.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:weight" )]
		public decimal? Weight { get; set; }

		/// <summary>
		/// A helper property for use where the range is a concept URL
		/// </summary>
		[JsonIgnore]
		public string PurlUrl { get; set; }

	}
}
