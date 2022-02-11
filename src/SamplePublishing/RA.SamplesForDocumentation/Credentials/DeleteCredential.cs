using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using APIRequest = RA.Models.Input.DeleteRequest;
using RA.Models.Input;

namespace RA.SamplesForDocumentation.Credentials
{
    public class DeleteCredential
    {
		public void SingleDelete( string owningOrganization, string ctidToDelete, string community = "" )
		{

			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}

			// This is the CTID of the organization that owns the data being deleted
			if ( string.IsNullOrWhiteSpace( owningOrganization ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//

			//====================	DELETE REQUEST ====================
			//
			DeleteRequest dr = new DeleteRequest()
			{
				CTID = ctidToDelete.ToLower(),
				PublishForOrganizationIdentifier = owningOrganization.ToLower(),
				Community = community
			};
			string message = "";
			new SampleServices().DeleteRequest( dr, apiKey, "credential", ref message, community );
		}

	}
}
