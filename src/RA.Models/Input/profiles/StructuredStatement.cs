// <copyright file="StructuredStatement.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Structured Statement
	/// A statement provided verbatim or by reference, with additional context such as type, source, or citation.
	/// </summary>
	public class StructuredStatement
	{
		[JsonIgnore]
		public static string classType = "ceterms:StructuredStatement";

		public StructuredStatement()
		{
			SubjectWebpage = null;
		}

		#region Required Properties

		/// <summary>
		/// Name of this resource
		/// REQUIRED
		/// </summary>
		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; }

		#endregion

		#region Conditionally Required Properties

		/// <summary>
		/// Statement, characterization or account of the entity.
		/// 
		/// Conditional Required if no Statement Text
		/// </summary>
		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		/// <summary>
		/// Verbatim unformatted text of a statement, typically one made in a larger document.
		/// 
		/// Conditional Required if no Subject Webpage
		/// </summary>
		public string StatementText { get; set; }		
		
		[JsonProperty( "ceterms:statementText" )]
		public LanguageMap StatementTextLangMap { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// 
		/// Conditional Required if no Statement Text
		/// </summary>
		public string SubjectWebpage { get; set; }

		#endregion

		public string CodedNotation { get; set; }

		/// <summary>
		/// An alphanumeric string indicating the relative position of a resource in an ordered list of resources
		/// </summary>
		public string ListID { get; set; }

		/// <summary>
		/// Type of prior learning evidence accepted for evaluation under the policy; select from an existing enumeration of such types.
		/// Concept Scheme: ceterms:StatementCategory
		/// </summary>
		public List<string> StatementType { get; set; }

	}
}
