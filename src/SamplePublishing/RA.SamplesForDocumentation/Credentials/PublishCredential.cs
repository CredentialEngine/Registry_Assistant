using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;
using YourCredential = RA.SamplesForDocumentation.SampleModels.Credential;
using APIRequestCredential = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class PublishCredential
	{
		/// <summary>
		/// Publish a credential using an input class
		/// An organization will have its data stored somewhere. The first step would be to have a process retrieve the information and send that data to a method to do the publishing. 		
		/// In this example: 
		/// -	YourCredential would be defined by the organization
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
		public string PublishFromInput( YourCredential input )
		{
			//Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();

			var myData = new APIRequestCredential
			{
				Name = input.Name,
				Description = input.Description,
				InLanguage = new List<string>() { "en-US" },
				CodedNotation = input.CodedNotation,
				//provide valid concept from schema 
				CredentialType = "BachelorDegree",
				//*** the source data must assign a CTID and use for all transactions
				Ctid = input.Ctid,
				DateEffective = input.DateEffective,
				Image = input.ImageUrl,
				Subject = input.Subject,
				Keyword = input.Keyword
			};

			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//CTID for Higher learning commission.
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//Add organization that is not in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Council on Social Work Education (CSWE)",
				SubjectWebpage = "https://www.cswe.org/",
				Description = "Founded in 1952, the Council on Social Work Education (CSWE) is the national association representing social work education in the United States."
			} );

			//add occupations
			PopulateOccupations( myData );
			//industries
			PopulateIndustries( myData );
			//Programs
			PopulatePrograms( myData );
			//Financial Assistance
			FinancialAssistanceProfiles.PopulateSimpleFinancialAssistanceProfile( myData );

			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "credential", "publish", payload, apiKey );

			return result;
		}

		public string PublishSimpleRecord( string requestType = "publish" )
		{
			//Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString();
			DataService.SaveCredentialCTID( myCTID );

			//A simple credential object - see below for sample class definition
			var myData = new Credential()
			{
				Name = "My Credential Name",
				Description = "This is some text that describes my credential.",
				Ctid = myCTID,
				SubjectWebpage = "http://example.com/credential/1234",
				CredentialType = "ceterms:Certificate",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "Credentials", "Technical Information", "Credential Registry" },
				Naics = new List<string>() { "333922", "333923", "333924" }
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//CTID for Higher learning commission.
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			myData.Requires = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Name = "My Requirements",
					Condition = new List<string>() { "Condition One", "Condition Two", "Condition Three" }
				}
			};
			//add occupations
			PopulateOccupations( myData );
			//industries
			PopulateIndustries( myData );
			//Programs
			PopulatePrograms( myData );

			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "credential", requestType, payload, apiKey );
			//Return the result
			return result;
		}

		public string PublishQACredentialWithETPL( string requestType = "publish" )
		{
			//Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetAppKeyValue( "myOrgApiKey" );
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetAppKeyValue( "myOrgCTID" );
			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString();


			//A simple credential object - see below for sample class definition
			var myData = new Credential()
			{
				Name = "My Quality Assurance Credential Name",
				Description = "This is some text that describes my quality assurance credential.",
				Ctid = myCTID,
				SubjectWebpage = "http://example.com/credential/1234",
				CredentialType = "ceterms:QualityAssuranceCredential",
				InLanguage = new List<string>() { "en-US" },
				Keyword = new List<string>() { "ETPL", "QualityAssurance" }
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );


			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//Serialize the credential request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "credential", requestType, payload, apiKey );
			//Return the result
			return result;
		}

		/// <summary>
		/// Possible Input Types
		/// - List of frameworks
		/// - list of occupation names
		/// - List of SOC codes
		/// 
		/// </summary>
		/// <param name="request"></param>
		public static void PopulateOccupations( Credential request )
		{
			request.OccupationType = new List<FrameworkItem>();

			//occupations from a framework like ONet - where the information is stored locally and can be included in publishing
			request.OccupationType.Add( new FrameworkItem()
			{
				Framework = "https://www.onetonline.org/",
				FrameworkName = "O*NET OnLine",
				Name = "Information Security Analysts",
				TargetNode = "https://www.onetonline.org/link/summary/15-1122.00",
				CodedNotation = "15-1122.00",
				Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
			} );
			request.OccupationType.Add( new FrameworkItem()
			{
				Framework = "https://www.onetonline.org/",
				FrameworkName = "O*NET OnLine",
				Name = "Computer Network Support Specialists",
				TargetNode = "https://www.onetonline.org/link/summary/15-1152.00",
				CodedNotation = "15-1152.00",
				Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
			} );


			//Occupations not in a known framework, list of strings
			request.AlternativeOccupationType = new List<string>() { "Cybersecurity", "Forensic Scientist", "Forensic Anthropologist" };

			//O*Net helper - ALternately provided a list of O*Net codes. The Assistant API will validate the codes and format the output including the framework name and URL, the occupation, description, and code
			request.ONET_Codes = new List<string>() { "13-2099.01", "13-2052.00", "13-2061.00", "13-2051.00" };
		}

		/// <summary>
		/// Possible Input Types
		/// - List of frameworks
		/// - list of industry names
		/// - List of NAICS codes
		/// </summary>
		/// <param name="request"></param>
		public static void PopulateIndustries( Credential request )
		{
			request.IndustryType = new List<FrameworkItem>
			{

				//occupations from a framework like NAICS - where the information is stored locally and can be included in publishing
				new FrameworkItem()
				{
					Framework = "https://www.naics.com/",
					FrameworkName = "NAICS - North American Industry Classification System",
					Name = "National Security",
					TargetNode = "https://www.naics.com/naics-code-description/?code=928110",
					CodedNotation = "928110",
					Description = "This industry comprises government establishments of the Armed Forces, including the National Guard, primarily engaged in national security and related activities."
				},
				new FrameworkItem()
				{
					Framework = "https://www.naics.com/",
					FrameworkName = "NAICS - North American Industry Classification System",
					Name = "Regulation and Administration of Transportation Programs",
					TargetNode = "https://www.naics.com/naics-code-description/?code=926120",
					CodedNotation = "926120",
					Description = "This industry comprises government establishments primarily engaged in the administration, regulation, licensing, planning, inspection, and investigation of transportation services and facilities. Included in this industry are government establishments responsible for motor vehicle and operator licensing, the Coast Guard (except the Coast Guard Academy), and parking authorities."
				}
			};


			//Industries not in a known framework, list of strings
			request.AlternativeIndustryType = new List<string>() { "Cybersecurity", "Forensic Science", "Forensic Anthropology" };

			//NAICS helper - ALternately provided a list of NAICS codes. The Assistant API will validate the codes and format the output including the framework name and URL, the name, description, and code
			request.Naics = new List<string>() { "9271", "927110", "9281", "928110" };
		}

		/// <summary>
		/// Possible Input Types
		/// - List of frameworks
		/// - list of program names
		/// - List of CIP codes
		/// </summary>
		/// <param name="request"></param>
		public static void PopulatePrograms( Credential request )
		{
			request.InstructionalProgramType = new List<FrameworkItem>
			{

				//programs from a framework like Classification of Instructional Program - where the information is stored locally and can be included in publishing
				new FrameworkItem()
				{
					Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					FrameworkName = "Classification of Instructional Program",
					Name = "Medieval and Renaissance Studies",
					TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.1301",
					CodedNotation = "30.1301",
					Description = "A program that focuses on the  study of the Medieval and/or Renaissance periods in European and circum-Mediterranean history from the perspective of various disciplines in the humanities and social sciences, including history and archeology, as well as studies of period art and music."
				},
				new FrameworkItem()
				{
					Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					FrameworkName = "Classification of Instructional Program",
					Name = "Classical, Ancient Mediterranean and Near Eastern Studies and Archaeology",
					TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.2202",
					CodedNotation = "30.2202",
					Description = "A program that focuses on the cultures, environment, and history of the ancient Near East, Europe, and the Mediterranean basin from the perspective of the humanities and social sciences, including archaeology."
				}
			};


			//programs not in a known framework, list of strings
			request.AlternativeInstructionalProgramType = new List<string>() { "Cybersecurity 101", "Forensic Science 120", "Forensic Anthropology 400" };

			//CIP code helper - ALternately provided a list of CIP codes. The Assistant API will validate the codes and format the output including the framework name and URL, the name, description, and code
			request.CIP_Codes = new List<string>() { "31.0504", "31.0505", "31.0599", "31.9999" };
		}
		public class DataService
		{
			internal static void SaveCredentialCTID( string myCTID )
			{
				throw new NotImplementedException();
			}
		}

	}
}
