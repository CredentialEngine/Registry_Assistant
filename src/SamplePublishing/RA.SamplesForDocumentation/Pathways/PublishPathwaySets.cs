using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RA.Models.Input;

namespace RA.SamplesForDocumentation.Pathways
{
	public class PublishPathwaySets
	{
		public string PublishSimpleRecord( string requestType = "publish" )
		{
			// Holds the result of the publish action
			var result = string.Empty;
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			// Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();

			// A simple pathwaySet object - see below for sample class definition
			var myData = new PathwaySet()
			{
				Name = "Information Technology Pathways set.",
				Description = "This is some text that describes this pathway set.",
				CTID = myCTID,
				SubjectWebpage = "https://example.org/pathwayset/1234"
			};
			// OwnedBy, etc. are organization references. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//provide the CTIDs for the pathways in HasPathway. There must be a minimum of two pathways. 
			//HasPathway can also refer to a pathway already published, that is not in the current list of Pathways
			var pathway1CTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
			var pathway2CTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();
			var pathway3CTID = "ce-" + Guid.NewGuid().ToString().ToLowerInvariant();

			myData.HasPathway.AddRange( new List<string>() { pathway1CTID, pathway2CTID, pathway3CTID } );

			// The input request class holds the pathwaySet and the identifier (CTID) for the owning organization
			var myRequest = new PathwaySetRequest()
			{
				PathwaySet = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add pathway components to the request
			//Note for this example, the third pathway is not provided, as it has already been published. The API will validate that the pathway has been published. 
			myRequest.Pathways.Add( AddPathway1( pathway1CTID, organizationIdentifierFromAccountsSite));
			myRequest.Pathways.Add( AddPathway2( pathway2CTID, organizationIdentifierFromAccountsSite ) );


			// Serialize the request object
			//var payload = JsonConvert.SerializeObject( myRequest );
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );
			//call the Assistant API
			result = new SampleServices().SimplePost( "pathwaySet", requestType, payload, apiKey );
			// Return the result
			return result;
		}
		public PathwayRequest AddPathway1( string myCTID, string organizationIdentifierFromAccountsSite )
		{

			// A simple pathway object - see below for sample class definition
			var myData = new Pathway()
			{
				Name = "Associate Degree: Information Technology - Cybersecurity Emphasis",
				Description = "This is some text that describes my pathway.",
				CTID = myCTID,
				SubjectWebpage = "https://example.org/pathway/1234",
				//add CTID for Destination component
				HasDestinationComponent = new List<string>() { "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7" },
				HasChild = new List<string>() { "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7" }
			};
			// OwnedBy, etc. are organization references. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
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
				ProgramTerm = "1st Term"
			} );


			// The input request class holds the pathway and the identifier (CTID) for the owning organization
			var myRequest = new PathwayRequest()
			{
				Pathway = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add pathway components to the request
			myRequest.PathwayComponents.AddRange( pathwayComponents );

			return myRequest;
		}

		public PathwayRequest AddPathway2( string myCTID, string organizationIdentifierFromAccountsSite )
		{

			// A simple pathway object - see below for sample class definition
			var myData = new Pathway()
			{
				Name = "Associate Degree: Information Technology - Information Systems Emphasis",
				Description = "This is some text that describes my pathway.",
				CTID = myCTID,
				SubjectWebpage = "https://example.org/pathway/1236",
				//add CTID for Destination component
				HasDestinationComponent = new List<string>() { "ce-5e7fcaaf-8500-47be-a4a9-2bed98f282d7" },
				HasChild = new List<string>() { "ce-5e7fcaaf-8500-47be-a4a9-2bed98f282d7" }
			};
			// OwnedBy, etc. are organization references. As a convenience just the CTID is necessary.
			// The ownedBY CTID is typically the same as the CTID for the data owner.
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );

			//list of pathway components to publish
			List<PathwayComponent> pathwayComponents = new List<PathwayComponent>();
			//add the destination component (uses the same CTID as for HasDestinationComponent
			var destinationComponent = new PathwayComponent()
			{
				PathwayComponentType = "CredentialComponent",
				CTID = "ce-5e7fcaaf-8500-47be-a4a9-2bed98f282d7",
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
				CTID = "ce-1f8d3d06-8500-4bd8-8750-7dc5e9a062eb",
				Name = "Programming Concepts and Methology I",
				Description = "Description of the course",
				ProgramTerm = "1st Term"
			} );


			// The input request class holds the pathway and the identifier (CTID) for the owning organization
			var myRequest = new PathwayRequest()
			{
				Pathway = myData,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add pathway components to the request
			myRequest.PathwayComponents.AddRange( pathwayComponents );

			return myRequest;
		}
	}
}
