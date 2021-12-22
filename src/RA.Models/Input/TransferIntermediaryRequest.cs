using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing a TransferIntermediary with TransferProfiles
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
		/// Name or title of the resource.
		/// Not Required
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Resource(s) for which this resource is an intermediary.
		/// Required
		/// Range: ceterms:TransferValueProfile
		/// TBD: Provide the CTID for a transfer value that is already published to the registry. Or may be publishing all at once?
		/// Probably handle like PathwaySet, where could refer to a TVP in the request.TransferValueProfiles or a published one. 
		/// Required
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
