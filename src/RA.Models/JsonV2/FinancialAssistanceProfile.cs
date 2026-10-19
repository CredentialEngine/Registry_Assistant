using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class FinancialAssistanceProfile
	{
		public FinancialAssistanceProfile()
		{
			Type = "ceterms:FinancialAssistanceProfile";
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		/// <summary>
		/// Only present for a resource with a CTID
		/// </summary>
		[JsonProperty( "@id" )]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// Required for standalone publishing, not used for embedded resources
		/// <see cref="https://credreg.net/ctdl/terms/ctid"/>
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Agent that offers the resource.
		/// Required for standalone publishing, optional for embedded resources
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:offeredBy" )]
		public List<string> OfferedBy { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		[JsonProperty( PropertyName = "ceterms:financialAssistanceType" )]
		public List<CredentialAlignmentObject> FinancialAssistanceType { get; set; }

		[JsonProperty( PropertyName = "ceterms:financialAssistanceForType" )]
		public List<CredentialAlignmentObject> FinancialAssistanceForType { get; set; }

		[JsonProperty( PropertyName = "ceterms:financialAssistanceValue" )]
		public List<QuantitativeValue> FinancialAssistanceValue { get; set; } = null;

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		[JsonProperty( PropertyName = "ceterms:requires" )]
		public List<ConditionProfile> Requires { get; set; }

	}
}
