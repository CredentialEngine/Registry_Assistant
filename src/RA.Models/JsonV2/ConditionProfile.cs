using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class ConditionProfile
	{
		public ConditionProfile()
		{
			Type = "ceterms:ConditionProfile";

			AssertedBy = null;
			SubjectWebpage = null;
			AlternativeCondition = new List<ConditionProfile>();
			Jurisdiction = new List<JurisdictionProfile>();
			ResidentOf = new List<JurisdictionProfile>();

			TargetAssessment = new List<string>();
			TargetCredential = new List<string>();
			TargetLearningOpportunity = new List<string>();
			TargetCompetency = new List<CredentialAlignmentObject>();
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Organization to which a person is formally related through work, sudy, or social engagement.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:assertedBy" )]
		public List<string> AssertedBy { get; set; }

		/// <summary>
		/// Organization that asserts this condition
		/// NOTE: It must be serialized to a List
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:affiliation" )]
		public List<string> Affiliation { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		[JsonProperty( PropertyName = "ceterms:audienceLevelType" )]
		public List<CredentialAlignmentObject> AudienceLevelType { get; set; }

		[JsonProperty( PropertyName = "ceterms:audienceType" )]
		public List<CredentialAlignmentObject> AudienceType { get; set; }

		[JsonProperty( PropertyName = "ceterms:condition" )]
		public LanguageMapList Condition { get; set; }

		// 20-10-31 CreditValue is now of type ValueProfile
		[JsonProperty( PropertyName = "ceterms:creditValue" )]
		public List<ValueProfile> CreditValue { get; set; } = null;

		[JsonProperty( PropertyName = "ceterms:creditUnitTypeDescription" )]
		public LanguageMap CreditUnitTypeDescription { get; set; }

		// external classes =====================================
		[JsonProperty( PropertyName = "ceterms:commonCosts" )]
		public List<string> CommonCosts { get; set; }

		[JsonProperty( PropertyName = "ceterms:estimatedCost" )]
		public List<CostProfile> EstimatedCost { get; set; }

		[JsonProperty( PropertyName = "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		/// <summary>
		/// Amount and nature of required work, experiential learning or other relevant experience.
		/// Setting to an object to handle old records with experience as a string instead of a language string
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:experience" )]
		// public LanguageMap Experience { get; set; }
		public object Experience { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:minimumAge" )]
		public int MinimumAge { get; set; }

		[JsonProperty( PropertyName = "ceterms:residentOf" )]
		public List<JurisdictionProfile> ResidentOf { get; set; }

		/// <summary>
		/// Artifact to be submitted such as a transcript, portfolio, or an affidavit.
		/// A datatype if object is used to handle legacy data that was published as a single string, vs more recent data published as a list.
		/// Aug. 2019 - changed to be list of URIs. Property is defined as an object for backwards compatibility.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:submissionOf" )]
		// public List<string> SubmissionOf { get; set; }
		public object SubmissionOf { get; set; }

		[JsonProperty( PropertyName = "ceterms:submissionOfDescription" )]
		public LanguageMap SubmissionOfDescription { get; set; }

		#region Targets
		[JsonProperty( PropertyName = "ceterms:targetAssessment" )]
		public List<string> TargetAssessment { get; set; }

		/// <summary>
		/// A competency relevant to the resource being described.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:targetCompetency" )]
		public List<CredentialAlignmentObject> TargetCompetency { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetCredential" )]
		public List<string> TargetCredential { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetLearningOpportunity" )]
		public List<string> TargetLearningOpportunity { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetJob" )]
		public List<string> TargetJob { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetOccupation" )]
		public List<string> TargetOccupation { get; set; }

		[JsonProperty( PropertyName = "ceterms:targetTask" )]
		public List<string> TargetTask { get; set; }
		#endregion

		[JsonProperty( PropertyName = "ceterms:weight" )]
		public decimal? Weight { get; set; }

		[JsonProperty( PropertyName = "ceterms:yearsOfExperience" )]
		public decimal? YearsOfExperience { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternativeCondition" )]
		public List<ConditionProfile> AlternativeCondition { get; set; }
	}
}
