using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2.QData
{
	/// <summary>
	/// Aspect or characteristic along which observations in a dataset can be organized and identified.
	/// Requires at least one of: name, description
	/// </summary>
	public class Dimension
	{
		public Dimension()
		{
			Type = "qdata:Dimension";
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// An identifier for use with blank nodes. 
		/// It will be ignored if included with a primary resource
		/// </summary>
		[JsonProperty( "@id" )]
		public string Id { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Type of resource that may be a point in the dimension.
		/// Items in a dimension will typically be instances of CTDL classes or concepts drawn from a CTDL concept scheme.
		/// Concept in ConceptScheme: qdata:DemographicCategory 
		/// </summary>
		[JsonProperty( PropertyName = "qdata:dimensionType" )]
		public string DimensionType { get; set; }

		/// <summary>
		/// Identifies a specific entity in a  Dimension. 
		/// The value of this property will be a resource with any type, e.g. skos:Concept, ceterms:Credential (or any subclass thereof), ceterms:Industry, ceterms:Place, etc.
		/// rdfs:Resource - a URI
		/// </summary>
		[JsonProperty( PropertyName = "qdata:hasPoint" )]
		public List<string> HasPoint { get; set; }
	}
}
