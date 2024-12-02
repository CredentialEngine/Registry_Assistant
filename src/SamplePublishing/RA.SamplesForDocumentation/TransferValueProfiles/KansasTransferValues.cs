using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation.TransferValueProfiles
{
	public class KansasTransferValues
	{
		#region IntroductorySociology
		/// <summary>
		/// Sample publish method
		/// </summary>
		/// <returns>If successful, returns the formatted graph from the registry.</returns>
		public string KBORInfantrymanLevel10TVP( bool doingPublish = false )
		{
			// Holds the result of the publish action
			var result = string.Empty;
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}
			// This is the CTID of the organization that owns the data being published
			//var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			var kborCTID = "ce-e4c8e9bd-88bd-4704-9e3a-78faebdca59d";
			var organizationIdentifierFromAccountsSite = kborCTID;

			// Assign a CTID for the entity being published and keep track of it
			//NOTE: afer being generated, this value be saved and used for successive tests or duplicates will occur.
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
			//from github
			//myCTID = "";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "Kansas Credit for Prior Learning Task Force",
				Description = "Desc TBD for Kansas Credit for Prior Learning Task Force",
				CTID = myCTID,
				LifeCycleStatusType = "Active",
				SubjectWebpage = "https://military.kansasregents.org/MAP/course_info/001902/25",
				//StartDate = "2018-12-01",
				//EndDate = "2023-11-30"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//myData.Identifier.Add( new IdentifierValue()
			//{
			//	IdentifierTypeName = "ACE Course Code",
			//	IdentifierValueCode = "CLEP-0026"        //Alphanumeric string identifier of the entity
			//} );
			//============== TransferValue ================================
			//Required. Provide a transfer value amount using Value and UnitText. ex. DegreeCredit This is a concept scheme and so has a strict vocabulary. 
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=1,
					CreditUnitType = new List<string>() {"DegreeCredit"}
				}
			};

			//==============	transfer value from ===================================================
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var transferValueFrom = new LearningOpportunity()
			{
				Type = "LearningOpportunity",
				Name = "Introductory Sociology",
				Description = "The Introductory Sociology examination is designed to assess an individual's knowledge of the material typically presented in a one-semester introductory-level sociology course at most colleges and universities. The examination emphasizes basic facts and concepts as well as general theoretical approaches used by sociologists on the topics of institutions, social patterns, social processes, social stratifications, and the sociological perspective. Highly specialized knowledge of the subject and the methodology of the discipline is not required or measured by the test content.The exam contains approximately 100 questions to be answered in 90 minutes. Some of these are pretest questions that will not be scored. Any time test takers spend on tutorials and providing personal information is in addition to the actual testing time.",
				SubjectWebpage = "https://clep.collegeboard.org/?t=introductorySocialogy",
				DeliveryType = new List<string>() { "deliveryType:InPerson" }
			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "College Board's College-Level Examination Program (CLEP)",
				SubjectWebpage = "https://clep.collegeboard.org/"
			};
			transferValueFrom.OwnedBy = new List<OrganizationReference>();
			transferValueFrom.OwnedBy.Add( ownedBy );

			myData.TransferValueFrom.Add( transferValueFrom );

			//===================================================================================


			// This holds the transfer value profile and the identifier (CTID) for the owning organization
			var myRequest = new TransferValueProfileRequest()
			{
				TransferValueProfile = myData,
				DefaultLanguage = "en-US",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			// Serialize the request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			List<string> messages = new List<string>();
			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = doingPublish ? "publish" : "format",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}
		public List<CredentialAlignmentObject> AssignIntroductorySociologyCompetencies()
		{
			var frameworkName = "ACE - Introductory Sociology Skills Measured";
			var frameworkCTID = "ce-85ea2bf0-b0a3-422c-9074-14f2efe51480";
			var targetUrl = "https://sandbox.credentialengineregistry.org/resources/";

			var output = new List<CredentialAlignmentObject>()
			{
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-f53c9247-9775-4186-a1f1-edd0a965edeb",
					TargetNodeName="Identification of specific names, facts, and concepts from sociological literature"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-7351578b-f8d3-4ca6-8ff5-1818dd4092db",
					TargetNodeName="Understanding of relationships between concepts, empirical generalizations, and theoretical propositions of sociology"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-2b3b27a9-e42c-4021-bb7c-178766320134",
					TargetNodeName="Understanding of the methods by which sociological relationships are established; "
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-6c4d0738-cb85-4263-badd-78ed1437b465",
					TargetNodeName="application of concepts, propositions, and methods to hypothetical situations"
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-ec073a6d-ce14-4e21-a548-bcb9a5d5bb24",
					TargetNodeName="Interpretation of tables and charts"
				},

				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName, Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-1cfa68c0-5ad5-483b-ad8a-e622f33d4a1a",
					TargetNodeName="institutions", Weight=.2M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-30c840c0-2978-49b9-9044-d74721c827c1",
					TargetNodeName="social patterns", Weight=.1M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-5d0cbcee-ef29-46fc-a8e4-b260cb737d3e",
					TargetNodeName="social process", Weight=.25M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-f7b31a05-4b6d-47f6-b001-1c7ad32ad6d1",
					TargetNodeName="social stratification (process and structure)", Weight=.25M
				},
				new CredentialAlignmentObject()
				{
					FrameworkName=frameworkName,
					Framework= targetUrl + frameworkCTID,
					TargetNode = targetUrl + "ce-90311565-e23b-40ec-8927-e1d9d04b599c",
					TargetNodeName="the socialogical perspective", Weight=.2M
				}
			};

			return output;
		}

		#endregion


	}
}
