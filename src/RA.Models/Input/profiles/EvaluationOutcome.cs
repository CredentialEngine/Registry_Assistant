// <copyright file="EvaluationOutcome.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// Evaluation Outcome
	/// Entity that describes the results of a valuation or appraisal of a resource.
	/// </summary>
	public class EvaluationOutcome
	{

		public EvaluationOutcome()
		{
			Type = "ceterms:EvaluationOutcome";
			SubjectWebpage = null;
		}

		#region Required Properties
		/// <summary>
		/// Need a custom mapping to @type based on input value
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// Name of this resource
		/// REQUIRED
		/// </summary>
		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; }

		/// <summary>
		/// Statement, characterization or account of the entity.
		/// REQUIRED
		/// </summary>
		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		#endregion

		/// <summary>
		/// Description of a process by which a resource was created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		/// Indicates contextualized data that reproduces or links to text such as part of a document or information about some aspect of a resource.
		/// Do not use this property if a simple text property such as ceterms:description is adequate
		/// Range: ceterms:StructuredStatement
		/// </summary>
		public List<StructuredStatement> HasStructuredStatement { get; set; }

		/// <summary>
		/// Description of transfer value that is part of this resource.
		/// Range: ceterms:TransferValueProfile
		/// </summary>
		public List<string> IsEvaluationOf { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		public string SubjectWebpage { get; set; }


	}

}
