using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RA.Models.Input;

using InputEntity = RA.Models.Input.TransferValueProfile;

namespace RA.SamplesForDocumentation
{
	public class PublishTransferValueProfile
	{
		/* Usage
		 * - update App.config with your ApiKey and CTID of the owning org
		 * - note the apiKey is not required in the sandbox
		 * Example of this record published on sandbox
		 * https://sandbox.credentialengineregistry.org/graph/ce-fd515001-6a9c-4f43-b401-3e65127fc807
		 */
		/// <summary>
		/// Sample publish method
		/// </summary>
		/// <returns>If successful, returns the formatted graph from the registry.</returns>
		public string PublishSimpleRecord( bool usingSimplePost = true )
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from previous test
			myCTID= "ce-fd515001-6a9c-4f43-b401-3e65127fc807";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Name",
				Description = "This is some text that describes my transfer value profile.",
				Ctid = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/1234"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//==============	transfer value from
			//Resource that provides the transfer value described by this resource, according to the entity providing this resource.
			//A list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFrom.Add( new EntityReference()
			{
				CTID = "ce-476c1aca-6cd9-4dbe-ba91-16960bfb19ac"
			} );
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			myData.TransferValueFrom.Add( new EntityReference()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."

			} );

			//==============	transfer value For
			//Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
			//Again, if applicable, a list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFor.Add( new EntityReference()
			{
				CTID = "ce-79946ece-72d2-49ce-9dd8-0b679b822768"
			} );


			//						optional
			//coded Notation will likely be replaced by Identifier in the near future
			myData.CodedNotation = "So-me-coded:notation";
			myData.StartDate = "2020-01-01";
			myData.EndDate = "2021-12-21";

			//===================================================================================
			//				additions in pending ( in near future)
			myData.LifecycleStatusType = "Active";  //this will be the default once activated
			//identifier will likely replace codedNotation for more flexibility. Although the name may change
			// A third party version of the entity being referenced that has been modified in meaning through editing, extension or refinement.
			myData.Identifier.Add( new IdentifierValue()
			{
				Name = "ACE Course Code",
				IdentifierType = "Internal Code",   //Formal name or acronym of the identifier type
				IdentifierValueCode = "0276"        //Alphanumeric string identifier of the entity
			} );

			//===================================================================================


			// This holds the transfer value profile and the identifier (CTID) for the owning organization
			var myRequest = new TransferValueProfileRequest()
			{
				TransferValueProfile = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			// Serialize the request object
			var payload = JsonConvert.SerializeObject( myRequest );


			//assign publish endpoint
			var assistantUrl = SampleServices.GetAppKeyValue( "registryAssistantApi" ) + "transfervalue/publish/";
			if ( usingSimplePost )
			{
				//use a simple method that returns a string
				result = new SampleServices().SimplePost( assistantUrl, payload, apiKey );

				// Return the result
				return result;
			}
			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = "publish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.Ctid.ToLower(),	//added here for logging
				Identifier = "testing",		//useful for logging, might use the ctid
				InputPayload = payload
			};
			
			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}


			
			
		public string PublishList( bool usingSimplePost = true )
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			// Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();

			//===================================================================================


			// This holds the transfer value profile and the identifier (CTID) for the owning organization
			var myRequest = new TransferValueProfileBulkRequest()
			{
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add some transfer value profiles
			myRequest.TransferValueProfile.Add( GetTVPOne( organizationIdentifierFromAccountsSite ) );
			myRequest.TransferValueProfile.Add( GetTVPTwo( organizationIdentifierFromAccountsSite ) );
			// Serialize the request object
			var payload = JsonConvert.SerializeObject( myRequest ); 


			//assign publish endpoint
			var assistantUrl = SampleServices.GetAppKeyValue( "registryAssistantApi" ) + "transfervalue/bulkpublish/";
			if ( usingSimplePost )
			{
				//use a simple method that returns a string
				//note this method will return a list of responses, so likely the simple method should not be used. 
				result = new SampleServices().SimplePost( assistantUrl, payload, apiKey );

				// Return the result
				return result;
			}

			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = "bulkpublish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile[0].Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			return result;
		}




		public TransferValueProfile GetTVPOne( string owningOrganizationCtid)
		{
			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from previous test
			myCTID = "ce-fd515001-6a9c-4f43-b401-3e65127fc807";
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Name",
				Description = "This is some text that describes my transfer value profile.",
				Ctid = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/tvp1"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = owningOrganizationCtid
			} );

			//==============	transfer value from
			//Resource that provides the transfer value described by this resource, according to the entity providing this resource.
			//A list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFrom.Add( new EntityReference()
			{
				CTID = "ce-476c1aca-6cd9-4dbe-ba91-16960bfb19ac"
			} );
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			myData.TransferValueFrom.Add( new EntityReference()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."

			} );

			//==============	transfer value For
			//Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
			//Again, if applicable, a list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFor.Add( new EntityReference()
			{
				CTID = "ce-79946ece-72d2-49ce-9dd8-0b679b822768"
			} );


			//						optional
			//coded Notation will likely be replaced by Identifier in the near future
			myData.CodedNotation = "So-me-coded:notation";
			myData.StartDate = "2020-01-01";
			myData.EndDate = "2021-12-21";

			return myData;
		}
		public TransferValueProfile GetTVPTwo( string owningOrganizationCtid )
		{
			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();
			//from previous test
			//
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Number Two ",
				Description = "This is some text that describes my transfer value profile number 2.",
				Ctid = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/tvp2"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = owningOrganizationCtid
			} );

			//==============	transfer value from
			//Resource that provides the transfer value described by this resource, according to the entity providing this resource.
			//A list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFrom.Add( new EntityReference()
			{
				CTID = "ce-476c1aca-6cd9-4dbe-ba91-16960bfb19ac"
			} );
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			myData.TransferValueFrom.Add( new EntityReference()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."

			} );

			//==============	transfer value For
			//Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
			//Again, if applicable, a list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFor.Add( new EntityReference()
			{
				CTID = "ce-79946ece-72d2-49ce-9dd8-0b679b822768"
			} );


			//						optional
			//coded Notation will likely be replaced by Identifier in the near future
			myData.CodedNotation = "So-me-coded:notation";
			myData.StartDate = "2020-01-01";
			myData.EndDate = "2021-12-21";

			//===================================================================================
			//				additions in pending ( in near future)
			//identifier will likely replace codedNotation for more flexibility. Although the name may change
			// A third party version of the entity being referenced that has been modified in meaning through editing, extension or refinement.
			myData.Identifier.Add( new IdentifierValue()
			{
				Name = "ACE Course Code",
				IdentifierType = "Internal Code",   //Formal name or acronym of the identifier type
				IdentifierValueCode = "0276"        //Alphanumeric string identifier of the entity
			} );

			return myData;
		}

	}
}
