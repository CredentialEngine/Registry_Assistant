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
		/// </summary>
		public string Ctid { get; set; }

		/// <summary>
		/// Name or title of the resource.
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
		/// List of CTIDs for competencies or frameworks
		/// ceasn:derivedFrom
		/// </summary>
		public List<string> DerivedFrom { get; set; } = new List<string>();

		public string SubjectWebpage { get; set; } //URL

		/// <summary>
		/// Organization(s) that owns this resource
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();

		#endregion

		/// <summary>
		/// May be replaced by Identifier
		/// </summary>
		//public string CodedNotation { get; set; }

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
		/// </summary>
		public string EndDate { get; set; }

		/// <summary>
		/// Type of official status of the TransferProfile; select from an enumeration of such types.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// </summary>
		public string LifecycleStatusType { get; set; }


		/// <summary>
		/// A suggested or articulated credit- or point-related transfer value.
		/// Required
		/// </summary>
		public List<ValueProfile> TransferValue { get; set; } = new List<ValueProfile>();

		/// <summary>
		///  Resource that provides the transfer value described by this resource, according to the entity providing this resource.
		/// <see cref="https://credreg.net/registry/assistant#EntityReference"/>
		/// </summary>
		public List<EntityReference> TransferValueFrom { get; set; } = new List<EntityReference>();

		/// <summary>
		///  Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
		/// <see cref="https://credreg.net/registry/assistant#EntityReference"/>
		/// </summary>
		public List<EntityReference> TransferValueFor { get; set; } = new List<EntityReference>();


	}

}
