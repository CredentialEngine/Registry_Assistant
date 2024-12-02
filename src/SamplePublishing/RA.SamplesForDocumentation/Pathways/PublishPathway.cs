using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RA.Models.Input;

//using ThisEntity = Models.Common.Pathway;

namespace RA.SamplesForDocumentation
{
	public class PublishPathway
	{
		public string PublishSimpleRecord( string requestType = "publish" )
		{
			// Holds the result of the publish action
			var result = string.Empty;
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}

			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//
	
			// Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();

			// A simple pathway object - see below for sample class definition
			var myData = new Pathway()
			{
				Name = "Associate Degree: Biotechnology Pathway",
				Description = "This is some text that describes my pathway.",
				CTID = myCTID,
				LifeCycleStatusType = "Active",
				SubjectWebpage = "https://example.org/pathway/1234",
				//add CTID for Destination component
				HasDestinationComponent = new List<string>() { "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7" },
				HasChild = new List<string>() { "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7" },
				Keyword = new List<string>() { "High School", "Chemistry" },
				PreviousVersion = "https://sandbox.credentialengineregistry.org/resources/ce-276a16e8-69d6-447f-9904-71c393d7a88d",
				LatestVersion = "https://sandbox.credentialengineregistry.org/resources/ce-a7e72ad1-4220-4ca1-8028-cdb2fec245a0",
				NextVersion = "https://sandbox.credentialengineregistry.org/resources/ce-330c41fb-f8d6-49ca-a97f-18e1bf6c1eb0",
			};
			// OwnedBy, etc. are organization references. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = "ce-a588a18b-2616-4c74-bdcd-6507c0693d0e"	//sandbox org
			} );
			myData.VersionIdentifier.Add( new IdentifierValue()
			{
				IdentifierTypeName = "MyVersion",
				IdentifierValueCode = "2023-09-01"        //Alphanumeric string identifier of the entity
			} );
			//list of pathway components to publish
			List<PathwayComponent> pathwayComponents = new List<PathwayComponent>();
			//add the destination component (uses the same CTID as for HasDestinationComponent
			var destinationComponent = new PathwayComponent()
			{
				PathwayComponentType = "CredentialComponent",
				CTID = "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7",
				Name = "Associate Degree: Biotechnology",
				Description = "This badge is earned in Canvas for completing BIO 193 and BIO 202.",
				CredentialType = "DigitalBadge",
				SourceData =""	//if provided: URI or CTID of an existing credential
			};

			//add to input component list
			pathwayComponents.Add( destinationComponent );
			//add some more components
			pathwayComponents.Add( new PathwayComponent()
			{
				PathwayComponentType = "AssessmentComponent",
				CTID = "ce-1f8d3d06-3953-4bd8-8750-7dc5e9a062eb",
				Name = "Programming Concepts and Methology Assessment",
				Description = "Description of the course",
				ProgramTerm = "1st Term"
			} );
			//add a selection component
			AddSelectionComponent( ref pathwayComponents );

			// The input request class holds the pathway and the identifier (CTID) for the owning organization
			var myRequest = new PathwayRequest()
			{
				Pathway = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add pathway components to the request
			myRequest.PathwayComponents.AddRange( pathwayComponents );


			// Serialize the request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			//result = new SampleServices().SimplePost( "pathway", requestType, payload, apiKey );
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "pathway",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Pathway.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;

		}
		public void AddSelectionComponent( ref List<PathwayComponent> pathwayComponents )
		{
			//create a selection component that references three components 
			var output = new PathwayComponent()
			{
				Name = "Selection Component",
				CTID = "ce-44cfeece-214a-47f0-94ce-f49b972bbecd",
				Description = "Description of this component",
				SubjectWebpage = "https://example.com?t=selectionComponent",
				ComponentCategory = "Selection",
				HasChild = new List<string>() { "ce-e1d14d25-f9cf-45e9-b625-ef79ed003f6b", "ce-39da55fa-140b-4c0a-92e2-c8e38e5f07f0", "ce-88d5a63d-4ca5-4689-bec1-55c88b6a5529" }
			};
			//include a condition indicating that 2 of the 3 are required. 
			var conditions = new ComponentCondition()
			{
				Name = "Conditions for this SelectionComponent",
				Description = "Require two of the target components.",
				RequiredNumber = 2,
				TargetComponent = new List<string>() { "ce-e1d14d25-f9cf-45e9-b625-ef79ed003f6b", "ce-39da55fa-140b-4c0a-92e2-c8e38e5f07f0", "ce-88d5a63d-4ca5-4689-bec1-55c88b6a5529" }
			};
			output.HasCondition.Add( conditions );

			pathwayComponents.Add( output );
			//now add the three components
			pathwayComponents.Add( new PathwayComponent()
			{
				PathwayComponentType = "CourseComponent",
				CTID = "ce-e1d14d25-f9cf-45e9-b625-ef79ed003f6b",
				Name = "Programming Concepts and Methology 102",
				Description = "Description of the course",
				ProgramTerm = "1st Term"
			} );
			pathwayComponents.Add( new PathwayComponent()
			{
				PathwayComponentType = "CourseComponent",
				CTID = "ce-39da55fa-140b-4c0a-92e2-c8e38e5f07f0",
				Name = "Programming Concepts and Methology 103",
				Description = "Description of the course",
				ProgramTerm = "1st Term"
			} );
			pathwayComponents.Add( new PathwayComponent()
			{
				PathwayComponentType = "CourseComponent",
				CTID = "ce-88d5a63d-4ca5-4689-bec1-55c88b6a5529",
				Name = "Programming Concepts and Methology 104",
				Description = "Description of the course",
				ProgramTerm = "1st Term"
			} );
		}

		/*
		public static void MapToAssistant( ThisEntity input, ThisRequest request, ref List<string> messages )
		{

			globalMonitor = new AssistantMonitor() { RequestType = "Pathway" };
			ThisRequestEntity output = request.Pathway;

			request.DefaultLanguage = "en-US";

			output.Name = input.Name;
			output.Description = input.Description;
			output.CTID = input.CTID.ToLowerInvariant();
			output.SubjectWebpage = input.SubjectWebpage;

			output.OwnedBy = MapToOrgReferences( input.OwningOrganization );
			output.OfferedBy = MapOrganizationsToOrgRef( input.OfferedByOrganization );

			if ( input.HasProgressionModel != null
			&& !string.IsNullOrWhiteSpace( input.HasProgressionModel.CTID ) )
			{
				//if present must be published
				if ( input.HasProgressionModel.CredentialRegistryId == null || input.HasProgressionModel.CredentialRegistryId.Length != 36 )
				{
					messages.Add( string.Format( "The progression model has not been published. First ensure that the progression model: '{0}' has been published, then return and publish this pathway.", input.HasProgressionModel.Name ?? "missing name" ) );
				}
				output.HasProgressionModel = input.HasProgressionModel.CTID;
			}
			else
			{
				//is progression model is not required
			}

			//must have at least one destination component
			//IF THERE ARE ANY COMPONENTS, REQUIRE A DESTINATION COMPONENT
			if ( input.HasDestinationComponent == null || input.HasDestinationComponent.Count == 0 )
			{
				if ( input.HasPart != null && input.HasPart.Count > 0 )
				{
					messages.Add( "Error - This pathway has pathway components but there is no Destination Component. When publishing any components, at least one destination component must be identified." );
				}
				//messages.Add( "Error - A pathway must have a Destination Component in order to be published to the Credential Registry." );
			}
			else
			{
				foreach ( var item in input.HasDestinationComponent )
				{
					if ( item != null && item.Id > 0 )
						output.HasDestinationComponent.Add( item.CTID );
				}
			}

			//must have at least one child
			if ( input.HasChild == null || input.HasChild.Count == 0 )
			{
				//messages.Add( "Error - A pathway must have at least one Child component in order to be published to the Credential Registry." );
			}
			else
			{
				//map using ctid
				foreach ( var item in input.HasChild )
				{
					if ( item != null && item.Id > 0 )
						output.HasChild.Add( item.CTID );
				}
			}
			//
			foreach ( var item in input.HasPart )
			{
				//???

				output.HasPart.Add( item.CTID );
			}
			//
			output.Subject = MapToStringList( input.Subject );
			output.Keyword = MapToStringList( input.Keyword );

			//frameworks =========================================
			output.IndustryType = MapEnumermationToFrameworkItem( input.IndustryType, "NAICS", "https://www.census.gov/eos/www/naics/" );

			//handle others
			if ( input.AlternativeIndustries != null && input.AlternativeIndustries.Count > 0 )
			{
				//output.IndustryType.AddRange( MapTextValueProfileToFrameworkItem( input.AlternativeIndustries ) );
				output.AlternativeIndustryType.AddRange( MapTextValueProfileToStringList( input.AlternativeIndustries ) );
			}

			output.OccupationType = MapEnumermationToFrameworkItem( input.OccupationType, "Standard Occupational Classification", "https://www.bls.gov/soc/" );
			//handle others
			if ( input.AlternativeOccupations != null && input.AlternativeOccupations.Count > 0 )
			{
				//output.OccupationType.AddRange( MapTextValueProfileToFrameworkItem( input.AlternativeOccupations ) );
				output.AlternativeOccupationType.AddRange( MapTextValueProfileToStringList( input.AlternativeOccupations ) );
			}

			//=======================
			foreach ( var item in input.HasPart )
			{
				RequestPathwayComponent component = new RequestPathwayComponent();
				MapPathwayComponent( item, component, ref messages );
				request.PathwayComponents.Add( component );
			}

			if ( messages.Count > 0 )
				globalMonitor.Messages.AddRange( messages );

		}

		public static void MapPathwayComponent( MC.PathwayComponent input, RequestPathwayComponent output, ref List<string> messages )
		{

			output.PathwayComponentType = input.PathwayComponentType;
			output.Name = input.Name;
			output.Description = input.Description;
			//
			//if ( isComponentDescriptionRequired && string.IsNullOrWhiteSpace( output.Description ) )
			//	messages.Add( string.Format("Error: a description must be provided for component type: {0}, Name: {1}", input.PathwayComponentType, input.Name) );

			output.CTID = input.CTID.ToLowerInvariant();
			output.SubjectWebpage = input.SubjectWebpage;
			output.SourceData = input.SourceData;

			//if ( isComponentSubjectWebpageOrSourceRequired && string.IsNullOrWhiteSpace( output.SubjectWebpage ) && string.IsNullOrWhiteSpace( output.SourceData) )
			//	messages.Add( string.Format( "Error: either a subject webpage or sourceData must be provided for component type: {0}, Name: {1}", input.PathwayComponentType, input.Name ) );

			output.CodedNotation = input.CodedNotation;
			output.ComponentCategory = input.ComponentCategory;
			output.ComponentDesignation = input.ComponentDesignationList;
			//NOTE can be literal, API will format as URI
			output.CredentialType = input.CredentialType;
			output.CreditValue = MapQuantitativeValueToValueProfile( input.CreditValue );
			output.ProgramTerm = input.ProgramTerm;

			//??
			//if (!string.IsNullOrWhiteSpace(input.HasProgressionLevel))
			//{
			//	//needs to be a CTID - currently have text
			//	output.HasProgressionLevel = input.HasProgressionLevel;
			//}
			//
			foreach ( var item in input.ProgressionLevels )
			{
				if ( !string.IsNullOrWhiteSpace( item.CTID ) )
					output.HasProgressionLevel.Add( item.CTID );
			}

			output.PointValue = MapQuantitativeValue( input.PointValue );
			foreach ( var item in input.HasChild )
			{
				output.HasChild.Add( item.CTID );
			}
			//
			foreach ( var item in input.Preceeds )
			{
				output.Preceeds.Add( item.CTID );
			}
			//
			foreach ( var item in input.Prerequisite )
			{
				output.Prerequisite.Add( item.CTID );
			}
			//
			foreach ( var item in input.HasCondition )
			{
				var cc = new RMI.ComponentCondition()
				{
					Name = item.Name,
					Description = item.Description,
					RequiredNumber = item.RequiredNumber
				};
				foreach ( var tc in item.TargetComponent )
				{
					cc.TargetComponent.Add( tc.CTID );
				}
				output.HasCondition.Add( cc );
			}
		}



		*/
	}
}
