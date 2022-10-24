using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing a TransferIntermediary with just IntermediaryFor.
	/// </summary>
	public class TransferIntermediaryRequest : BaseRequest
	{
		public TransferIntermediaryRequest()
		{
		}

		/// <summary>
		/// TransferIntermediary
		/// </summary>
		public TransferIntermediary TransferIntermediary { get; set; } = new TransferIntermediary();
	}

	/// <summary>
	/// A request only used by the Transfer Intermediary bulk publish request. 
	/// </summary>
	public class TransferIntermediaryBulkRequest : BaseRequest
	{
		public TransferIntermediaryBulkRequest()
		{
		}

		/// <summary>
		/// TransferIntermediary
		/// </summary>
		public TransferIntermediary TransferIntermediary { get; set; } = new TransferIntermediary();

		/// <summary>
		/// Do not publish the transfer intermediary if any transfer value profiles fail.
		/// The partner can provide the prefered action. If only one transfer value fails, the preference may be to rerun using TransferIntermediary.IntermediaryFor to hold the CTIDs for all published transver values, and then just include the one transfer value that failed
		/// </summary>
		public bool SkipPublishIfAnyTransferProfilesFail { get; set; }

		/// <summary>
		/// List of TransferValueProfiles to published with the TransferIntermediary
		/// </summary>
		public List<TransferValueProfile> TransferValueProfiles { get; set; } = new List<TransferValueProfile>();
	}

	/// <summary>
	/// Transfer Intermediary
	/// Surrogate resource to which other resources are mapped in order to indicate their common transferability.
	/// Usage Note: Used when multiple resources such as courses are grouped together to indicate they have mutually agreed upon transfer value.
	/// ceterms:TransferIntermediary
	/// Required:
	/// - name, intermediaryFor, CTID, and ownedBy
	/// </summary>
	public class TransferIntermediary
	{
		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use. 
		/// Not Required
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		///  A credit-related value.
		///  https://credreg.net/ctdl/terms/creditValue
		/// </summary>
		public List<ValueProfile> CreditValue { get; set; } = new List<ValueProfile>();

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
		/// Not Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
        /// <summary>
        /// List of Alternate Names for this credential
        /// </summary>
        public List<string> AlternateName { get; set; } = new List<string>();
        /// <summary>
        /// LanguageMap for AlternateName
        /// </summary>
        public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();


		/// <summary>
		/// Resource(s) for which this resource is an intermediary.
		/// Required. If a list the complete list of transfer value profiles is included with this transaction, then this property does not need to duplicate the information. 
		/// Range: ceterms:TransferValueProfile
		/// Provide the CTID for a transfer value that is already published to the registry or is included the list of transfer value profiles with this request. 
		/// NOTE: API will always take all transfer value profiles in the request list, 
		/// </summary>
		public List<string> IntermediaryFor { get; set; } = new List<string>();

		/// <summary>
		/// Organization that owns this resource
		/// Required
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Requirement or set of requirements for this resource
		/// Not Required
		/// </summary>
		public List<ConditionProfile> Requires { get; set; } = new List<ConditionProfile>();

        /// <summary>
        /// Name or title of the resource.
        /// Not Required
        /// </summary>
        public string SubjectWebpage { get; set; }

        /// <summary>
        /// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
        /// Not Required
        /// https://credreg.net/ctdl/terms/subject
        /// </summary>
        public List<string> Subject { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();
		
	}
}
