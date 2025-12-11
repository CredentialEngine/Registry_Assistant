using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Any value in inheriting from CredentialAlignmentObject?
	/// </summary>
	public class AlignmentObject
	{
		public AlignmentObject()
		{
			Type = "ceterms:AlignmentObject";
			AlignmentDate = null;
		}

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
		/// Statement, characterization or account of the entity. 
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Individual entry in a formally defined framework such as a competency or an industry, instructional program, or occupation code that is the source of an alignment. 
		/// xsd:anyURI
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sourceNode" )]
		public string SourceNode { get; set; }

		/// <summary>
		/// Textual description of an individual concept or competency in a formally defined framework  that is the source of an alignment.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sourceNodeDescription" )]
		public LanguageMap SourceNodeDescription { get; set; }

		/// <summary>
		/// Name of an individual concept or competency in a formally defined framework  that is the source of an alignment. 
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:sourceNodeName" )]
		public LanguageMap SourceNodeName { get; set; }

		/// <summary>
		/// Target Node
		/// The node of a framework targeted by the alignment. Must be a valid URL.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:targetNode" )]
		public string TargetNode { get; set; }

		/// <summary>
		/// Target Description
		/// The description of a node in an established educational framework.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:targetNodeDescription" )]
		public LanguageMap TargetNodeDescription { get; set; } = new LanguageMap();

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
	}
}
