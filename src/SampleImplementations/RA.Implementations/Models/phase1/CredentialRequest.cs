using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;

namespace RA.Implementations.Models.phase1
{
	/// <summary>
	/// Class used with a Credential format or publish request
	/// </summary>
	public class CredentialRequest 
	{
		public CredentialRequest()
		{
			Credential = new Credential();
		}
		/// <summary>
		/// Credential Input Class
		/// </summary>
		public Credential Credential { get; set; }


	}

	public class Credential
	{
		public Credential()
		{
			AudienceLevelType = new List<string>();
			Keyword = new List<string>();

			OwnedBy = new List<OrganizationReference>();
			OfferedBy = new List<OrganizationReference>();
		}


		#region *** Required Properties ***
		
		/// <summary>
		/// Name of this credential
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Credential description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// CTID - unique identifier
		/// If not provided, will be set to ce-UUID
		/// ex: ce-F22CA1DC-2D2E-49C0-9E72-0457AD348873
		/// It will be the primary key for retrieving this entity from the registry. 
		/// Also it must be provided 
		/// </summary>
		public string Ctid { get; set; }
		/// <summary>
		/// The credential type as defined in CTDL
		/// </summary>
		public string CredentialType { get; set; }

		/// <summary>
		/// SubjectWebpage URL
		/// </summary>
		public string SubjectWebpage { get; set; } //URL

		public string CredentialStatusType { get; set; }
		#endregion

		#region *** At least one of  ***
		//(more in next phase) 
		/// <summary>
		/// Organization(s) that own this credential
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		/// <summary>
		/// Organization(s) that offers this credential
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }
		#endregion

		#region *** Required if available Properties ***

		#endregion

		#region *** Recommended Properties ***
		//more in next phase
		public string DateEffective { get; set; }

		public List<string> Keyword { get; set; }
		public List<string> AudienceLevelType { get; set; }


		//the BYs

		//Conditions

		#endregion

		
		
		
	}

}
