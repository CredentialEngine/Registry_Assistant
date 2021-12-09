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

		public string CTID { get; set; }

		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();

		public List<string> Result { get; set; }

		public List<string> Subject { get; set; }

	}
}
