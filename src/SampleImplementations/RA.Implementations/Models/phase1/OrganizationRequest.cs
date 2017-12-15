using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Implementations.Models.phase1
{
	public class OrganizationRequest
	{
		public OrganizationRequest()
		{
			Organization = new Organization();
		}

		/// <summary>
		/// Organization Input Class
		/// </summary>
		public Organization Organization { get; set; }

	}

	public class Organization
	{
		public Organization()
		{
			AgentType = new List<string>();
			Email = new List<string>();
			Keyword = new List<string>();
			ServiceType = new List<string>();
			SocialMedia = new List<string>();
		}



		#region *** Required Properties ***
		/// <summary>
		/// The type of organization is one of :
		/// - CredentialOrganization
		/// - QACredentialOrganization
		/// Required
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Name 
		/// Required
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Organization description 
		/// Required
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Credential Identifier
		/// format: 
		/// ce-UUID (guid)
		/// Required
		/// </summary>
		public string Ctid { get; set; }

		/// <summary>
		/// Organization subject web page
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// The type of the described agent.
		/// Must provide valid organization types
		/// </summary>
		public List<string> AgentType { get; set; }/// <summary>
												   /// The types of sociological, economic, or political subdivision of society served by an agent. Enter one of:
												   /// <value>
												   /// agentSector:PrivateForProfit agentSector:PrivateNonProfit agentSector:Public
												   /// </value>
												   /// </summary>
		public string AgentSectorType { get; set; }
		#endregion

		#region *** At least one of  ***
		//(more in next phase) 
		public List<string> Email { get; set; }
		#endregion

		#region *** Required if available Properties ***
		

		#endregion
		#region *** Recommended Properties ***
		//more in next phase
		

		public List<string> SocialMedia { get; set; }
		public List<string> Keyword { get; set; }
		/// <summary>
		/// Url for Organization image
		/// </summary>
		public string Image { get; set; }
		public List<string> ServiceType { get; set; }
		public string AgentPurpose { get; set; }
		public string AgentPurposeDescription { get; set; }

		#endregion





	}
}
