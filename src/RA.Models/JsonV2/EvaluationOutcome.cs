// <copyright file="EvaluationOutcome.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Evaluation Outcome
	/// Entity that describes the results of a valuation or appraisal of a resource.
	/// </summary>
	public class EvaluationOutcome : BaseResourceDocument
	{
		[JsonIgnore]
		public static string classType = "ceterms:EvaluationOutcome";

		public EvaluationOutcome()
		{
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

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		#endregion

		/// <summary>
		/// Description of a process by which a resource was created.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		/// Indicates contextualized data that reproduces or links to text such as part of a document or information about some aspect of a resource.
		/// Do not use this property if a simple text property such as ceterms:description is adequate
		/// Range: ceterms:StructuredStatement
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:hasStatement" )]
		public List<string> HasStatement { get; set; }

		/// <summary>
		/// Description of transfer value that is part of this resource.
		/// Range: ceterms:TransferValueProfile
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:isEvaluationOf" )]
		public List<string> IsEvaluationOf { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

	}
}
