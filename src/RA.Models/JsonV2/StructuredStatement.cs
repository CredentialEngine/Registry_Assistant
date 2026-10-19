// <copyright file="StructuredStatement.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Structured Statement
	/// A statement provided verbatim or by reference, with additional context such as type, source, or citation.
	/// </summary>
	public class StructuredStatement : BaseResourceDocument
	{

		public StructuredStatement()
		{
			Type = "ceterms:StructuredStatement";
			SubjectWebpage = null;
		}

		#region Required Properties
		/// <summary>
		/// Need a custom mapping to @type based on input value
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:name" )]
		public LanguageMap Name { get; set; }

		#endregion

		#region Conditionally Required Properties

		/// <summary>
		/// Statement, characterization or account of the entity.
		///
		/// Conditional Required if no Statement Text
		/// </summary>
		[JsonProperty( "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Verbatim unformatted text of a statement, typically one made in a larger document.
		///
		/// Conditional Required if no Subject Webpage
		/// </summary>
		[JsonProperty( "ceterms:statementText" )]
		public LanguageMap StatementText { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		///
		/// Conditional Required if no Statement Text
		/// </summary>
		[JsonProperty( "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		#endregion

		/// <summary>
		/// Statement of a date made as part of a StructuredStatement, the meaning of which is indicated by the statementType property.
		/// xsd:date
		/// </summary>
		[JsonProperty( "ceterms:associatedDate" )]
		public string AssociatedDate { get; set; }

		[JsonProperty( "ceterms:codedNotation" )]
		public string CodedNotation { get; set; }

		/// <summary>
		/// An alphanumeric string indicating the relative position of a resource in an ordered list of resources
		/// </summary>
		[JsonProperty( "ceasn:listID" )]
		public string ListID { get; set; }

		/// <summary>
		/// Type of prior learning evidence accepted for evaluation under the policy.
		/// Concept Scheme: ceterms:StatementCategory
		/// Range: list of URIs
		/// </summary>
		[JsonProperty( "ceterms:statementType" )]
		public List<string> StatementType { get; set; }

	}

}
