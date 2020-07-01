using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;
using InputEntity = RA.SamplesForDocumentation.SampleModels.Credential;
using OutputEntity = RA.Models.Input.Credential;
namespace RA.SamplesForDocumentation
{
	public class PublishCredential
	{


		/// <summary>
		/// Publish a credential using an input class
		/// An organization will have its data stored somewhere. The first step would be to have a process retrieve the information and send that data to a method to do the publishing. 		
		/// In this example: 
		/// -	InputEntity would be defined by the organization
		///-	A process would be developed to read data from the organization data source( s) and populate the class
		///		o   Important: a CTID must be created in the data source and populated
		///-	The Publish method would be called with the credential data.
		///Steps:
		///		o Establishes the apiKey, and the CTID for the owning organization
		///		o   Instantiates the credential class the is part of the request class used by the API
		///		o   Maps input data to output
		///		o   Adds the OwnedBy property
		///		o A sample for adding AccreditedBY using just the CTID for The Higher Learning Commission
		///		o	( Add all applicable properties as needed)
		///		o Create the request class used by the API, and assign the Credential, and other properties
		///		o Serialize the request class into JSON
		///			The sample using the “Newtonsoft.Json” library
		///			JsonConvert.SerializeObject( myRequest);
		///		o Formats the HttpClient with the header, content
		///		o   Gets the desired publishing endpoint
		///		o   Calls the endpoint, and hopefully gets a successful result
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
	public string Publish( InputEntity input )
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			//this is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

			OutputEntity output = new OutputEntity
			{
				Name = input.Name,
				Description = input.Description,
				InLanguage = new List<string>() { "en-US" },
				CodedNotation = input.CodedNotation,
				CredentialType = "BachelorDegree",//provide valid concept from schema 
				//*** the source data must assign a CTID and use for all transactions
				Ctid = input.Ctid,
				DateEffective = input.DateEffective,
				Image = input.ImageUrl,
				Subject = input.Subject,
				Keyword = input.Keyword
			};

			//typically the ownedBy is the same as the CTID for the data owner
			output.OwnedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//CTID for Higher learning commission.
			output.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );


			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new CredentialRequest()
			{
				Credential = output,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			var json = JsonConvert.SerializeObject( myRequest );
			//Use HttpClient to perform the publish
			using ( var client = new HttpClient() )
			{
				//Accept JSON
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
				//add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				//Format the json as content
				var content = new StringContent( json, Encoding.UTF8, "application/json" );
				//The endpoint to publish to
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/credential/publish/";
				//Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			//Return the result
			return result;
		}

		public string PublishSimpleRecord()
		{
			//Holds the result of the publish action
			var result = "";
			//assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			//this is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			//Assign a CTID for the entity being published and keep track of it
			var myCredCTID = "ce-" + Guid.NewGuid().ToString();
			DataService.SaveCredentialCTID( myCredCTID );

			//A simple credential object - see below for sample class definition
			var myCred = new Credential()
			{
				Name = "My Credential Name",
				Description = "This is some text that describes my credential.",
				Ctid = myCredCTID,
				SubjectWebpage = "http://example.com/credential/1234",
				CredentialType = "ceterms:Certificate",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				Naics = new List<string>() { "333922", "333923", "333924" }
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myCred.OwnedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//CTID for Higher learning commission.
			myCred.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			myCred.Requires = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Name = "My Requirements",
					Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
				}
			};

			//This holds the credential and the identifier (CTID) for the owning organization
			var myData = new CredentialRequest()
			{
				Credential = myCred,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			var jsonPayload = JsonConvert.SerializeObject( myData );
			//Use HttpClient to perform the publish
			using ( var client = new HttpClient() )
			{
				//Accept JSON
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
				//add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				//Format the json as content
				var content = new StringContent( jsonPayload, Encoding.UTF8, "application/json" );
				//The endpoint to publish to
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/credential/publish/";
				//Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			//Return the result
			return result;
		}
		public class DataService
		{
			internal static void SaveCredentialCTID( string myCredCTID )
			{
				throw new NotImplementedException();
			}
		}

	}
}
