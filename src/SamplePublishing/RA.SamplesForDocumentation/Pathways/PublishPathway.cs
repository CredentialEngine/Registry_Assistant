using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class PublishPathway
	{
		public string PublishSimpleRecord( string requestType = "publish" )
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			// Assign a Ctid for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();

			// A simple pathway object - see below for sample class definition
			var myData = new Pathway()
			{
				Name = "Associate Degree: Biotechnology Pathway",
				Description = "This is some text that describes my pathway.",
				Ctid = myCTID,
				SubjectWebpage = "https://example.org/pathway/1234",
				//add Ctid for Destination component
				HasDestinationComponent = new List<string>() { "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7" },
				HasChild = new List<string>() { "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7" },
				Keyword = new List<string>() { "High School", "Chemistry" }
			};
			// OwnedBy, etc. are organization references. As a convenience just the Ctid is necessary.
			// The ownedBY Ctid is typically the same as the Ctid for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );

			//list of pathway components to publish
			List<PathwayComponent> pathwayComponents = new List<PathwayComponent>();
			//add the destination component (uses the same Ctid as for HasDestinationComponent
			var destinationComponent = new PathwayComponent()
			{
				PathwayComponentType = "CredentialComponent",
				CTID = "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7",
				Name = "Associate Degree: Biotechnology",
				Description = "This badge is earned in Canvas for completing BIO 193 and BIO 202.",
				CredentialType = "DigitalBadge"
			};

			//add to input component list
			pathwayComponents.Add( destinationComponent );
			//add some more components
			pathwayComponents.Add( new PathwayComponent()
			{
				PathwayComponentType = "CourseComponent",
				CTID = "ce-1f8d3d06-3953-4bd8-8750-7dc5e9a062eb",
				Name = "Programming Concepts and Methology I",
				Description = "Description of the course",
				ProgramTerm = "1st Term",
				CodedNotation = "COMP B11"
			} );
			//add a selection component
			pathwayComponents.Add( AddSelectionComponent() );

			// The input request class holds the pathway and the identifier (Ctid) for the owning organization
			var myRequest = new PathwayRequest()
			{
				Pathway = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add pathway components to the request
			myRequest.PathwayComponents.AddRange( pathwayComponents );


			// Serialize the request object
			var payload = JsonConvert.SerializeObject( myRequest );
			//call the Assistant API
			result = new SampleServices().SimplePost( "pathway", requestType, payload, apiKey );
			// Return the result
			return result;
		}
		public PathwayComponent AddSelectionComponent()
		{
			var output = new PathwayComponent()
			{
				Name = "Selection Component",
				CTID = "ce-44cfeece-214a-47f0-94ce-f49b972bbecd",
				Description = "Description of this component",
				SubjectWebpage = "https://example.com?t=selectionComponent",
				ComponentCategory = "Selection",
				HasChild = new List<string>() { "ce-e1d14d25-f9cf-45e9-b625-ef79ed003f6b", "ce-39da55fa-140b-4c0a-92e2-c8e38e5f07f0", "ce-88d5a63d-4ca5-4689-bec1-55c88b6a5529" }
			};

			var conditions = new ComponentCondition()
			{
				Name = "Conditions for this SelectionComponent",
				Description = "Require two of the target components.",
				RequiredNumber = 3,
				TargetComponent = new List<string>() { "ce-e1d14d25-f9cf-45e9-b625-ef79ed003f6b", "ce-39da55fa-140b-4c0a-92e2-c8e38e5f07f0", "ce-88d5a63d-4ca5-4689-bec1-55c88b6a5529" }
			};
			output.HasCondition.Add( conditions );

			return output;
		}
	}
}
