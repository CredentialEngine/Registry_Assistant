using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using ApiRequest = RA.Implementations.Models.phase1.CredentialRequest;
using ApiCredential = RA.Implementations.Models.phase1.Credential;
using RA.Models;
using RAI = RA.Models.Input;
using RAResponse = RA.Models.RegistryAssistantResponse;


namespace RA.Implementations.phase1
{
	public class CredentialPublishing
	{
		public void Publish( int recordIdentifier, bool doingFormatOnly = true )
		{

			//organization will create process to retrieve organization data to publish
			YourCredential input = GetCredentialDataForPublish( recordIdentifier );

			//instantiate the data input class for the api
			ApiRequest request = new ApiRequest();
			//call custom method to map organization data to the assistant input class
			MapToAssistant( input, request.Credential );

			//The response object will contain any messages, the RegistryEnvelopeIdentifier, and the formatted payload document that would have been sent to the registry
			RAResponse response = new RAResponse();
			//serialize the data in Json format
			string postBody = JsonConvert.SerializeObject( request, Services.GetJsonSettings() );

			if ( doingFormatOnly )
			{
				if ( Services.FormatRequest( postBody, "credential", ref response ) )
				{

				}
				else
				{
					//error handling
				}
			}
			else
			{
				if ( Services.PublishRequest( postBody, "credential", ref response ) )
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
		/// <param name="recordIdentifier"></param>
		/// <returns></returns>
		public YourCredential GetCredentialDataForPublish( int recordIdentifier )
		{
			YourCredential org = new YourCredential();


			return org;
		}


		public static void MapToAssistant( YourCredential input, ApiCredential output )
		{
			//specify credential type
			output.CredentialType = "Certification";
			output.Name = input.Name;
			output.Description = input.Description;
			output.SubjectWebpage = input.SubjectWebpage;
			output.Ctid = input.Ctid;

			output.CredentialStatusType = "credentialStat:Active";
			//organization entities like ownedBy and offeredBy are represented by OrganizationReference. Either a CTID is provided - where the entity has a Ctid and will ultimately reside in the registry, or 
			output.OwnedBy = MapToOrgReferenceList( input.OwningOrganization );
		}
		public static List<RAI.OrganizationReference> MapToOrgReferenceList( YourOrganization org )
		{
			List<RAI.OrganizationReference> list = new List<RAI.OrganizationReference>();
			RAI.OrganizationReference or = new RAI.OrganizationReference();
			if ( org == null || org.Id == 0 )
				return list;

			//??these should just be set to @Id???
			if ( string.IsNullOrWhiteSpace( org.Ctid ) )
			{
				or.Name = org.Name;
				or.Description = org.Description;
				or.SubjectWebpage = org.SubjectWebpage;
				//set the type. ?????
				//if ( org.ISQAOrganization )
				//	or.Type = "QACredentialOrganization";
				//else
					or.Type = "CredentialOrganization";

				if ( org.SocialMedia != null && org.SocialMedia.Count > 0 )
				{
					or.SocialMedia = new List<string>();
				}
			}
			else
			{
				or.Id = Services.idUrl + org.Ctid;
			}

			list.Add( or );
			return list;
		}
		
	}
	public class YourCredential
	{
		public YourCredential()
		{

		}



		#region *** Required Properties ***
		/*
		 *	ceterms:Badge
			ceterms:Certification
			ceterms:License
			ceterms:MicroCredential
			ceterms:Certificate
			ceterms:AssociateDegree
			ceterms:BachelorDegree
			ceterms:MasterDegree
			ceterms:DoctoralDegree
			ceterms:SecondarySchoolDiploma
			ceterms:QualityAssuranceCredential
			ceterms:ApprenticeshipCertificate
			ceterms:DigitalBadge
			ceterms:OpenBadge
			ceterms:ProfessionalDoctorate
			ceterms:ResearchDoctorate
			ceterms:GeneralEducationDevelopment
			ceterms:JourneymanCertificate
			ceterms:MasterCertificate
		 */
		public string Type { get; set; }

		/// <summary>
		/// Name 
		/// Required
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Credential description 
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
		/// Credential subject web page
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; }


		#endregion

		#region *** Required if available Properties ***
		public YourOrganization OwningOrganization { get; set; }

		#endregion
		#region *** Recommended Properties ***

		#endregion




	}
}
