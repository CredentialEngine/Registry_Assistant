using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{

	/// <summary>
	/// Single TransferValueProfileRequest requests
	/// </summary>
	public class TransferValueProfileRequest : BaseRequest
	{
		public TransferValueProfileRequest()
		{
		}

		/// <summary>
		/// List of TransferValueProfiles to publish
		/// </summary>
		public TransferValueProfile TransferValueProfile { get; set; } = new TransferValueProfile();
		/// <summary>
		/// Proposed: use actual learning opportunity classes rather than entityReference
		/// </summary>
		public List<LearningOpportunity> TargetLearningOpportunities { get; set; } = new List<LearningOpportunity>();
	}

	/// <summary>
	/// List of TransferValueProfileRequest requests
	/// </summary>
	public class TransferValueProfileBulkRequest : BaseRequest
	{
		public TransferValueProfileBulkRequest()
		{
		}

		/// <summary>
		/// List of TransferValueProfiles to publish
		/// </summary>
		public List<TransferValueProfile> TransferValueProfiles { get; set; } = new List<TransferValueProfile>();
	}

	/// <summary>
	/// Description of transfer value for a resource.
	/// </summary>
	public class TransferValueProfile
	{
		/*
		 * 
			ceterms:name
			ceterms:description
			ceterms:ctid
			ceterms:subjectWebpage
			ceterms:startDate
			ceterms:endDate
			ceterms:statusType
			ceterms:identifier
			ceterms:ownedBy
			ceterms:derivedFrom
			ceterms:transferValue
			ceterms:transferValueFrom
			ceterms:transferValueFor
			--------------
			ceterms:learningMethodDescription
			ceterms:assessmentMethodDescription
			ceterms:recognizes
			ceterms:owns
		 * 
		 */

		#region Required 
		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// Required
		/// </summary>
		public string Ctid { get; set; }

		/// <summary>
		/// Name or title of the resource.
		/// Required
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Transfer Value Profile Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// A third party version of the entity being referenced that has been modified in meaning through editing, extension or refinement.
		/// List of CTIDs or should it be entityReferences?
		/// ceasn:derivedFrom
		/// </summary>
		public List<EntityReference> DerivedFrom { get; set; } = new List<EntityReference>();


		/// <summary>
		/// Organization(s) that owns this resource
		/// Required
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Webpage that describes this entity.
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; } //URL


		/// <summary>
		/// A suggested or articulated credit- or point-related transfer value.
		/// Required
		/// </summary>
		public List<ValueProfile> TransferValue { get; set; } = new List<ValueProfile>();
		#endregion


		/// <summary>
		/// Identifier
		/// Definition:	en-US: An alternative URI by which this profile is identified.
		/// List of URIs 
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();


		/// <summary>
		/// Date the validity or usefulness of the information in this resource begins.
		/// </summary>
		public string StartDate { get; set; }

		/// <summary>
		/// Date this assertion ends.
		/// Required
		/// </summary>
		public string EndDate { get; set; }

		/// <summary>
		/// FUTURE
		/// Type of official status of the TransferProfile; select from an enumeration of such types.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// </summary>
		//public string LifecycleStatusType { get; set; }


		/// <summary>
		///  Resource that provides the transfer value described by this resource, according to the entity providing this resource.
		///  2020-12-07 The data type was changed to object. The handled CTDL classes are: 
		///  - AssessmentProfile
		///  - LearningOpportunityProfile
		///  - All credential classes (pending)
		/// </summary>
		public List<object> TransferValueFrom { get; set; } = new List<object>();

		/// <summary>
		///  Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
		///  2020-12-07 The data type was changed to object. The handled CTDL classes are: 
		///  - AssessmentProfile
		///  - LearningOpportunityProfile
		///  - All credential classes (pending)
		/// </summary>
		public List<object> TransferValueFor { get; set; } = new List<object>();

	}
}
