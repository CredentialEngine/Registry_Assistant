using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles
{
	/// <summary>
	/// Action taken by an agent asserting that the resource being described has a workforce demand level worthy of note.
	/// </summary>
	public class WorkforceDemandAction : CredentialingAction
	{

		/// <summary>
		/// Globally unique Credential Transparency Identifier (CTID)
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Outcome produced in the action.
		/// ceterms:CredentialAlignmentObject
		/// </summary>
		public List<CredentialAlignmentObject> Result { get; set; } = new List<CredentialAlignmentObject>();

		//public List<string> Subject { get; set; }

	}
}
