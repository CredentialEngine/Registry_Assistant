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
	/// History
	/// 21-01-13 Added DevelopementProcess
	/// </summary>
	public class TransferValueProfile
	{

		#region Required 
		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// Required
		/// </summary>
		public string CTID { get; set; }

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
		/// List of CTIDs - must exist in the registry
		/// ceasn:derivedFrom
		/// </summary>
		public List<string> DerivedFrom { get; set; } = new List<string>();

		/// <summary>
		/// Entity describing the process by which the transfer value profile, or aspects of it, were created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; } = new List<ProcessProfile>();

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
		/// Definition:	en-US: Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();


		/// <summary>
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Required
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; }

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
		///  Resource that provides the transfer value described by this resource, according to the entity providing this resource.
		///  2020-12-07 The data type was changed to object. The handled CTDL classes are: 
		///  - AssessmentProfile
		///  - LearningOpportunityProfile
		///  - All credential classes
		///  - Competency - TBD
		///  - Job  - TBD
		///  - Occupation  - TBD
		///  2021-05-10 An additional type will be a EntityReference. Actually any of the latter with a type and CTID
		/// </summary>
		public List<object> TransferValueFrom { get; set; } = new List<object>();

		/// <summary>
		///  Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
		///  2020-12-07 The data type was changed to object. The handled CTDL classes are: 
		///  - AssessmentProfile
		///  - LearningOpportunityProfile
		///  - All credential classes
		///  - Competency - TBD
		///  - Job  - TBD
		///  - Occupation  - TBD
		///  2021-05-10 An additional type will be a EntityReference. Actually any of the latter with a type and CTID
		/// </summary>
		public List<object> TransferValueFor { get; set; } = new List<object>();

		/// <summary>
		///  Resource that replaces this resource.
		///  full URL OR CTID (recommended)
		/// </summary>
		public string SupersededBy { get; set; }
		/// <summary>
		/// Resource that this resource replaces.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string Supersedes { get; set; }

	}

	
}
