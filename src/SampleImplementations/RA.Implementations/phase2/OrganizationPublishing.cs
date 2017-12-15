using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using ApiRequest = RA.Implementations.Models.phase1.OrganizationRequest;
using ApiOrganization = RA.Implementations.Models.phase1.Organization;
using RA.Models;
using RAI = RA.Models.Input;
using RAResponse = RA.Models.RegistryAssistantResponse;

namespace RA.Implementations.phase1
{
    public class OrganizationPublishing
    {

		public void Publish( int organizationIdentifier, bool doingFormatOnly = true )
		{

			//organization will create process to retrieve organization data to publish
			YourOrganization input = GetOrganizationDataForPublish( organizationIdentifier );

			//instantiate the data input class for the api
			ApiRequest request = new ApiRequest();
			//call custom method to map organization data to the assistant input class
			MapToAssistant( input, request.Organization );

			//The response object will contain any messages, the RegistryEnvelopeIdentifier, and the formatted payload document that would have been sent to the registry
			RAResponse response = new RAResponse();
			//serialize the data in Json format
			string postBody = JsonConvert.SerializeObject( request, Services.GetJsonSettings() );
			
			if ( doingFormatOnly )
			{
				if ( Services.FormatRequest( postBody, "organization", ref response ) )
				{

				} else
				{
					//error handling
				}
			} else
			{
				if ( Services.PublishRequest( postBody, "organization", ref response ) )
				{

				}
				else
				{
					//error handling
				}
			}

		}

		/// <summary>
		/// Add method to retrieve data to be published
		/// </summary>
		/// <param name="organizationIdentifier"></param>
		/// <returns></returns>
		public YourOrganization GetOrganizationDataForPublish(int organizationIdentifier )
		{
			YourOrganization org = new YourOrganization();


			return org;
		}


		public static void MapToAssistant( YourOrganization input, ApiOrganization output )
		{

			output.Name = input.Name;
			output.Description = input.Description;
			output.SubjectWebpage = input.SubjectWebpage;
			output.Ctid = input.Ctid;
			//specify ctdl type" CredentialOrganization, or QACredentialOrganization
			output.Type = "CredentialOrganization";

			//required AgentSectorType
			output.AgentSectorType = "PrivateNonProfit";

			//must have at least one means of contacting the org
			output.Email = input.Email;

			//with or without concept prefix
			output.AgentType = input.AgentType;

			output.Image = input.Image;
			output.AgentPurpose = input.AgentPurpose;
			output.AgentPurposeDescription = input.AgentPurposeDescription;


		}

		
	}
	public class YourOrganization
	{
		public YourOrganization()
		{
			Type = "CredentialOrganization";
			AgentType = AgentType = new List<string>() { "orgType:Business", "BusinessAssociation", "Vendor" };
			Email = new List<string>() { "info@myOrg.com", "support@myOrg.com" };
			;
		}

		public int Id { get; set; }

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
		public List<string> AgentType { get; set; }

		/// <summary>
		/// The types of sociological, economic, or political subdivision of society served by an agent. Enter one of:
		/// <value>
		/// agentSector:PrivateForProfit agentSector:PrivateNonProfit agentSector:Public
		/// </value>
		/// </summary>
		public string AgentSectorType { get; set; }

		#endregion

		#region *** Required if available Properties ***
		public string AgentPurpose { get; set; }
		public string AgentPurposeDescription { get; set; }

		#endregion
		#region *** Recommended Properties ***

		public List<string> SocialMedia { get; set; }
		public List<string> Keyword { get; set; }
		/// <summary>
		/// Url for Organization image
		/// </summary>
		public string Image { get; set; }
		public List<string> ServiceType { get; set; }
		#endregion


		public List<string> Email { get; set; }


	}
}
