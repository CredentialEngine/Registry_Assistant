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
		/// Geographic or political region in which the credential is formally applicable or an organization has authority to act.
		/// ceterms:JurisdictionProfile
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();

		/// <summary>
		/// Outcome produced in the action.
		/// ceterms:CredentialAlignmentObject
		/// </summary>
		public List<CredentialAlignmentObject> Result { get; set; } = new List<CredentialAlignmentObject>();

		//public List<string> Subject { get; set; }

	}
}
