using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class TransferIntermediaryRequest : BaseRequest
	{
		public TransferIntermediaryRequest()
		{
		}

		/// <summary>
		/// List of TransferValueProfiles to publish
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
	/// ceterms:TransferIntermediary
	/// </summary>
	public class TransferIntermediary
	{
		/*
		 * URI: ceterms:TransferIntermediary
Label: Transfer Intermediary
Definition: Surrogate resource to which other resources are mapped in order to indicate their common transferability.
Usage Note: Used when multiple resources such as courses are grouped together to indicate they have mutually agreed upon transfer value.
Properties: ceterms:name, ceterms:description, ceterms:intermediaryFor, ceterms:codedNotation, ceterms:requires, ceterms:creditValue**
		 */

		public string CTID { get; set; }
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
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Name or title of the resource.
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Resource(s) for which this resource is an intermediary.
		/// Required
		/// Range: ceterms:TransferValueProfile
		/// TBD: Provide the CTID for a transfer value that is already published to the registry. Or may be publishing all at once?
		/// Probably handle like PathwaySet, where could refer to a TVP in the request.TransferValueProfiles or a published one. 
		/// </summary>
		public List<string> IntermediaryFor { get; set; } = new List<string>();

		/// <summary>
		/// Requirement or set of requirements for this resource
		/// </summary>
		public List<ConditionProfile> Requires { get; set; } = new List<ConditionProfile>();
		/* TBD*/
		/// <summary>
		/// Words or brief phrases describing the topicality of the entity; select subject terms from an existing enumeration of such terms.
		/// https://credreg.net/ctdl/terms/subject
		/// </summary>
		public List<string> Subject { get; set; }
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();
		
	}
}
