// <copyright file="Note.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using Newtonsoft.Json;

namespace RA.Models.Input
{
	public class Note
	{
		[JsonProperty( "@type" )]
		public string Type { get; set; } = "elm:Note";

		/// <summary>
		/// The textual content of further information about an object or topic supplied as free text.
		/// Required
		/// </summary>
		public string NoteLiteral { get; set; }

		[JsonProperty( "elm:noteLiteral" )]
		public LanguageMap NoteLiteralLangMap { get; set; }

		/// <summary>
		/// Concept
		/// Optional/unlikely to be used
		/// concept scheme: ceterms:Format
		/// </summary>
		public string NoteFormat { get; set; }

	}
}
