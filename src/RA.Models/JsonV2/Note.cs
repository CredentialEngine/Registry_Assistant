// <copyright file="Note.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class Note
	{
		[JsonProperty( "@type" )]
		public string Type { get; set; } = "elm:Note";

		/// <summary>
		/// The textual content of further information about an object or topic supplied as free text.
		/// Required
		/// </summary>
		[JsonProperty( "elm:noteLiteral" )]
		public LanguageMap NoteLiteral { get; set; }

		/// <summary>
		/// The format of the note
		/// Concept
		/// Optional/unlikely to be used
		/// concept scheme: ceterms:Format
		/// </summary>
		[JsonProperty( "elm:noteFormat" )]
		public string NoteFormat { get; set; }

	}
}
