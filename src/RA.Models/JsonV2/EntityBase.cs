using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Base class to use for references to entities that are not in the registry
	/// </summary>
	public class EntityBase
	{
		public EntityBase()
		{
			//SubjectWebpage = new List<string>();
			SubjectWebpage = null;
		}

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }


		/// <summary>
		/// The type of the referenced entity
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "ceterms:name" )]
		public string Name { get; set; }

		[JsonProperty( "ceterms:description" )]
		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string Ctid { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		#region Additional properties for assessments and lopps
		//
		[JsonProperty( PropertyName = "ceterms:assesses" )]
		public List<CredentialAlignmentObject> Assesses { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:assessmentMethodDescription" )]
		public LanguageMap AssessmentMethodDescription { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:availableAt" )]
		public List<Place> AvailableAt { get; set; }
		//

		[JsonProperty( PropertyName = "ceterms:codedNotation" )]
		public string CodedNotation { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:creditValue" )]
		public List<QuantitativeValue> CreditValue { get; set; } = null;
		[JsonProperty( PropertyName = "ceterms:estimatedDuration" )]
		public List<DurationProfile> EstimatedDuration { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:learningMethodDescription" )]
		public LanguageMap LearningMethodDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:learningMethodType" )]
		public List<CredentialAlignmentObject> LearningMethodType { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:offeredBy" )]
		public List<string> OfferedBy { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:subject" )]
		public List<CredentialAlignmentObject> Subject { get; set; }
		//
		[JsonProperty( PropertyName = "ceterms:teaches" )]
		public List<CredentialAlignmentObject> Teaches { get; set; }

		#endregion
		//
		public virtual void NegateNonIdProperties()
		{
			Type = null;
			Name = null;
			Description = null;
			SubjectWebpage = null;
			Ctid = null;
		}
	}
}
