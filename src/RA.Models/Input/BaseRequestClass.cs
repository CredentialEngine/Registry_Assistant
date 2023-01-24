using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Helper class for publishers that that are not using upper case CTID property
	/// </summary>
	public class BaseRequestClass
	{

		/// <summary>
		/// External Audience Type
		/// List of concepts that don't exist in the registry. Will be published as blank nodes
		/// OR should input be a list of Concepts?
		/// This would be replaced by ReferenceConcepts, no these are defined as CAO !!!!
		/// </summary>
		public List<CredentialAlignmentObject> ExternalAudienceType { get; set; } = new List<CredentialAlignmentObject>();


		/// <summary>
		/// External Audience Level Type
		/// List of concepts that don't exist in the registry. Will be published as blank nodes
		/// OR should input be a list of Concepts?
		/// </summary>
		public List<CredentialAlignmentObject> ExternalAudienceLevelType { get; set; } = new List<CredentialAlignmentObject>();

	}
}
