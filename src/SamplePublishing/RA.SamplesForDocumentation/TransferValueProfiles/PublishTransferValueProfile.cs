using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RA.Models.Input;

using InputEntity = RA.Models.Input.TransferValueProfile;

namespace RA.SamplesForDocumentation
{
	/// <summary>
	/// Sep. 14, 2020
	/// - Added use of dateEffective and expirtationDate for learningopportunity in an EntityReference
	/// </summary>
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
			myCTID = "ce-fd515001-6a9c-4f43-b401-3e65127fc807";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Name",
				Description = "This is some text that describes my transfer value profile.",
				Ctid = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/1234",
				StartDate = "2020-01-01",
				EndDate = "2021-12-21"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			myData.Identifier.Add( new IdentifierValue()
			{
				Name = "ACE Course Code",
				IdentifierType = "Internal Code",   //Formal name or acronym of the identifier type
				IdentifierValueCode = "0276"        //Alphanumeric string identifier of the entity
			} );
			//============== TransferValue ================================
			//Required. Provide a transfer value amount using Value and UnitText. ex. DegreeCredit This is a concept scheme and so has a strict vocabulary. 
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};

			//==============	transfer value from ===================================================
			//Resource that provides the transfer value described by this resource, according to the entity providing this resource.
			//A list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFrom.Add( new EntityReference()
			{
				CTID = "ce-476c1aca-6cd9-4dbe-ba91-16960bfb19ac"
			} );
			//If not provided as much information as is available
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var transferValueFrom = new EntityReference()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."
			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Organization that owns this LearningOpportunity",
				SubjectWebpage = "https://myOrganization.com",
				Description = "While optional, a description is helpful."
			};
			transferValueFrom.OwnedBy.Add( ownedBy );

			myData.TransferValueFrom.Add( transferValueFrom );

			//==============	transfer value For
			//Resource that accepts the transfer value described by this resource, according to the entity providing this resource.
			//Again, if applicable, a list of entity references. If the CTID is known, then just provide it.
			myData.TransferValueFor.Add( new EntityReference()
			{
				CTID = "ce-79946ece-72d2-49ce-9dd8-0b679b822768"
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
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//assign publish endpoint
			var assistantUrl = SampleServices.GetAppKeyValue( "registryAssistantApi" ) + "transfervalue/publish/";
			string jsonldPayload = "";
			List<string> messages = new List<string>();
			if ( usingSimplePost )
			{
				//use a simple method that returns a string
				result = new SampleServices().SimplePost( assistantUrl, payload, apiKey, ref jsonldPayload, ref messages );

				// Return the result
				return result;
			}
			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = "publish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			return req.FormattedPayload;
		}

		/// <summary>
		/// Use SameAs property to equate a LearningOpportunity in the credential registry with a learning opportunity referenced in a TransferValueFrom
		/// </summary>
		/// <param name="usingSimplePost"></param>
		/// <returns></returns>
		public string PublishTVPSameAsCredentialRecord( bool usingSimplePost = true )
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
			//myCTID = "ce-fd515001-6a9c-4f43-b401-3e65127fc807";

			// A simple transfer value profile object - required properties
			var myData = new TransferValueProfile()
			{
				Name = "My Transfer Value Profile Name",
				Description = "This is some text that describes my transfer value profile.",
				Ctid = myCTID,
				SubjectWebpage = "http://example.com/transferValueProfile/1234",
				StartDate = "2020-01-01",
				EndDate = "2021-12-21"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//============== TransferValue ====================================================
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//==============	transfer value from ===========================================
			//Resource that provides the transfer value described by this resource, according to the entity providing this resource.
			//A list of entity references. 
			//Where the CTID is not known for a document in the registry, provide as much information as is available
			//NOTE: see separate example that uses SameAs to equate a learning opportunity in the registry with a learning opportunity described in a transfer value from (or for)
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs
			//NOTE: you must provide owned by or offered by with TransferValueFrom or TransferValueFor
			var transferValueFrom = new EntityReference()
			{
				Type = "LearningOpportunityProfile",
				Name = "name of the learning opportunity",
				Description = "Description of the learning opportunity",
				SubjectWebpage = "https://example.com/anotherlOPP",
				LearningMethodDescription = "A useful description of the learning method",
				AssessmentMethodDescription = "How the learning opportunity is assessed."
			};
			var ownedBy = new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Organization that owns this LearningOpportunity",
				SubjectWebpage = "https://myOrganization.com",
				Description = "While optional, a description is helpful."
			};
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

			//assign publish endpoint
			var assistantUrl = SampleServices.GetAppKeyValue( "registryAssistantApi" ) + "transfervalue/publish/";
			string jsonldPayload = "";
			List<string> messages = new List<string>();
			if ( usingSimplePost )
			{
				//use a simple method that returns a string
				result = new SampleServices().SimplePost( assistantUrl, payload, apiKey, ref jsonldPayload, ref messages );

				// Return the result
				return result;
			}
			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = "publish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfile.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
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
			myRequest.TransferValueProfiles.Add( GetTVPOne( organizationIdentifierFromAccountsSite ) );
			myRequest.TransferValueProfiles.Add( GetTVPTwo( organizationIdentifierFromAccountsSite ) );
			myRequest.TransferValueProfiles.Add( GetTVPEnvironmentalChallenges( organizationIdentifierFromAccountsSite ) );
			// Serialize the request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );


			//assign publish endpoint
			var assistantUrl = SampleServices.GetAppKeyValue( "registryAssistantApi" ) + "transfervalue/bulkpublish/";
			string jsonldPayload = "";
			List<string> messages = new List<string>();
			if ( usingSimplePost )
			{
				//use a simple method that returns a string
				result = new SampleServices().SimplePost( assistantUrl, payload, apiKey, ref jsonldPayload, ref messages );

				// Return the result
				return result;
			}

			//otherwise use a method where return status can be inspected
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "transfervalue",
				RequestType = "bulkpublish",
				OrganizationApiKey = apiKey,
				CTID = myRequest.TransferValueProfiles[ 0 ].Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			return result;
		}



		/// <summary>
		/// uses transferValueFrom and TransferValueFor
		/// </summary>
		/// <param name="owningOrganizationCtid"></param>
		/// <returns></returns>
		public TransferValueProfile GetTVPOne( string owningOrganizationCtid )
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
			//============== TransferValue ====================================================
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//==============	transfer value from ===========================================
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
			//coded Notation could be replaced by Identifier in the near future
			myData.StartDate = "2015-01-01";
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

			//============== TransferValue ====================================================
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//==============	transfer value from ===========================================
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

		/// <summary>
		/// Environmental Challenges And Solutions
		/// </summary>
		/// <param name="owningOrganizationCtid"></param>
		/// <returns></returns>
		public TransferValueProfile GetTVPEnvironmentalChallenges( string owningOrganizationCtid )
		{

			//from previous test
			//
			var myData = new TransferValueProfile()
			{
				Name = "Environmental Challenges And Solutions",
				Description = "To provide knowledge of the scope and severity of environmental illnesses.",
				Ctid = "ce-489406de-1c64-40bd-af31-f7a502b8b850",
				SubjectWebpage = "https://stagingweb.acenet.edu/national-guide/Pages/Course.aspx?org=Huntington%20College%20of%20Health%20Sciences&cid=ffb1a50b-82c4-ea11-a812-000d3a33232a"
			};
			// OwnedBy is a list of OrganizationReferences. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID= owningOrganizationCtid
			} );
			myData.StartDate = "1994-09-01";
			myData.EndDate = "2001-06-30";

			//============== TransferValue ====================================================
			myData.TransferValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					Value=3,
					CreditUnitType = new List<string>() {"DegreeCredit"},
					CreditLevelType = new List<string>() {"LowerDivisionLevel"}
				}
			};
			//==============	transfer value from ===========================================
			//see: https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/RA.Models/Input/profiles/EntityReference.cs

			var learningOpportunity = new EntityReference()
			{
				Type = "LearningOpportunityProfile",
				Name = "Environmental Challenges And Solutions",
				Description = "To provide knowledge of the scope and severity of environmental illnesses.",
				SubjectWebpage = "https://stagingweb.acenet.edu/national-guide/Pages/Course.aspx?org=Huntington%20College%20of%20Health%20Sciences&cid=ffb1a50b-82c4-ea11-a812-000d3a33232a",
				DateEffective ="1994-09-01",
				ExpirationDate = "2001-06-30",
				EstimatedDuration = new List<DurationProfile>() 
				{ 
					new DurationProfile() { Description= "135 hours (self-paced)" } 
				}
			};
			learningOpportunity.OwnedBy = new List<OrganizationReference>() {  new OrganizationReference()
			{
				Type = "CredentialOrganization",
				Name = "Huntington College of Health Sciences",
				Description = "To provide knowledge of the scope and severity of environmental illnesses.",
				SubjectWebpage = "https://stagingweb.acenet.edu/national-guide/Pages/Course.aspx?org=Huntington%20College%20of%20Health%20Sciences&cid=ffb1a50b-82c4-ea11-a812-000d3a33232a"
			} };
			learningOpportunity.Teaches = new List<CredentialAlignmentObject>()
			{
				new CredentialAlignmentObject()
				{
					TargetNodeName="Upon successful completion of this course, the student will be able to recognize causes and effects of chemically induced illness"
				},
				new CredentialAlignmentObject()
				{
					TargetNodeName="And understand the role proper nutrition plays in avoiding and/or mitigating the damage these chemicals cause"
				},
				new CredentialAlignmentObject()
				{
					TargetNodeName="Know how to find alternative solutions to chemicals"
				}
			};
			myData.TransferValueFrom.Add( learningOpportunity );

			return myData;
		}

		private EntityReference AddTransferFromLearningOpportunity()
		{
			EntityReference er = new EntityReference()
			{
				Type = "LearningOpportunityProfile",
				Name = "Packet Switched Networks",
				Description = "Understanding of terminology and issues and their application in functioning packet switched networks.",
				LearningMethodDescription = "Methods of instruction include lecture, discussion, laboratory exercises, videotapes, and a final examination."
			};
			er.Teaches = new List<CredentialAlignmentObject>() { new CredentialAlignmentObject()
			{
				TargetNodeDescription="Upon successful completion of this course, the student will be able to describe packet technology; and discuss the history of public packet networks--their practical implementation and management issues related to implementation."
			} };

			er.OwnedBy = new List<OrganizationReference>() {  new OrganizationReference()
			{
				Type="CredentialOrganization",
				Name="Ameritech",
				SubjectWebpage="https://www.ameritech.edu/",
				Address = new List<Place>{ new Place()
				{
					City= "Waukesha",
					AddressRegion= "WI",
					Country="United States"
				} }

			} };

			return er;
		}
	}
}
