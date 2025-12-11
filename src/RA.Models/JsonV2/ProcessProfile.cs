using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Common JSON-LD class for all process profiles
	/// </summary>
	public class ProcessProfile
	{
		public ProcessProfile()
		{
			Type = "ceterms:ProcessProfile";

			ProcessingAgent = null;
			ProcessMethod = null;
			ProcessStandards = null;
			ScoringMethodExample = null;
			SubjectWebpage = null;

			TargetAssessment = null;
			TargetCredential = null;
			TargetLearningOpportunity = null;
			TargetCompetencyFramework = null;

			Jurisdiction = null;
			// Region = new List<GeoCoordinates>();
			ExternalInputType = null;
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name{ get; set; } = new LanguageMap();

		/// <summary>
		/// Effective date of the content of this profile
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		[JsonProperty( PropertyName = "ceterms:externalInputType" )]
		public List<CredentialAlignmentObject> ExternalInputType { get; set; }

		[JsonProperty( PropertyName = "qdata:dataCollectionMethodType" )]
		public List<CredentialAlignmentObject> DataCollectionMethodType { get; set; }

		/// <summary>
		/// Instrument
		/// Object that helped the agent perform the action. e.g. John wrote a book with a pen.
		/// A credential or other instrument whose criteria was applied in executing the action.
		/// Provide the CTID for a credential in the Credential Registry or provide minimum data for a credential not in the registry.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:instrument" )]
		public List<string> Instrument { get; set; }

		[JsonProperty( PropertyName = "ceterms:processFrequency" )]
		public LanguageMap ProcessFrequency { get; set; }

		[JsonProperty( PropertyName = "ceterms:processingAgent" )]
		public List<string> ProcessingAgent { get; set; }

		[JsonProperty( PropertyName = "ceterms:processMethod" )]
		public string ProcessMethod { get; set; }

		[JsonProperty( PropertyName = "ceterms:processMethodDescription" )]
		public LanguageMap ProcessMethodDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandards", NullValueHandling = NullValueHandling.Ignore )]
		public string ProcessStandards { get; set; }

		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:scoringMethodDescription" )]
		public LanguageMap ScoringMethodDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:scoringMethodExample" )]
		public string ScoringMethodExample { get; set; }

		[JsonProperty( PropertyName = "ceterms:scoringMethodExampleDescription" )]
		public LanguageMap ScoringMethodExampleDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		[JsonProperty( PropertyName = "ceterms:verificationMethodDescription" )]
		public LanguageMap VerificationMethodDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		// [JsonProperty( PropertyName = "ceterms:region" )]
		// public List<GeoCoordinates> Region { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetCredential" )]
		public List<string> TargetCredential { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetAssessment" )]
		public List<string> TargetAssessment { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetLearningOpportunity" )]
		public List<string> TargetLearningOpportunity { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetCompetencyFramework" )]
		public List<string> TargetCompetencyFramework { get; set; }
	}
}



