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
		public string PublishSimpleRecord()
		{
			// Holds the result of the publish action
			var result = "";
			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			// This is the Ctid of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = "ce-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
			// Assign a Ctid for the entity being published and keep track of it
			var myCTID = "ce-" + Guid.NewGuid().ToString().ToLower();

			// A simple pathway object - see below for sample class definition
			var myPathway = new Pathway()
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
			myPathway.OwnedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );

			//list of pathway components to publish
			List<PathwayComponent> pathwayComponents = new List<PathwayComponent>();
			//add the destination component (uses the same Ctid as for HasDestinationComponent
			var destinationComponent = new PathwayComponent()
			{
				PathwayComponentType = "CredentialComponent",
				Ctid= "ce-5e7fcaaf-74e2-47be-a4a9-2bed98f282d7",
				Name = "Associate Degree: Biotechnology",
				Description = "This badge is earned in Canvas for completing BIO 193 and BIO 202.",
				CredentialType= "DigitalBadge"
			};
			
			//add to input component list
			pathwayComponents.Add( destinationComponent );
			//add some more components
			pathwayComponents.Add( new PathwayComponent()
			{
				PathwayComponentType= "CourseComponent",
				Ctid= "ce-1f8d3d06-3953-4bd8-8750-7dc5e9a062eb",
				Name = "Programming Concepts and Methology I", 
				Description="Description of the course",
				ProgramTerm= "1st Term", 
				CodedNotation= "COMP B11"
			} );

			// The input request class holds the pathway and the identifier (Ctid) for the owning organization
			var myRequest = new PathwayRequest()
			{
				Pathway = myPathway,
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			//add pathway components to the request
			myRequest.PathwayComponents.AddRange( pathwayComponents );

			// Serialize the request object
			var json = JsonConvert.SerializeObject( myRequest );
			// Use HttpClient to perform the publish
			using ( var client = new HttpClient() )
			{
				// Accept JSON
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
				// Add API Key (for a publish request)
				client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
				// Format the json as content
				var content = new StringContent( json, Encoding.UTF8, "application/json" );
				// The endpoint to publish to
				var publishEndpoint = "https://sandbox.credentialengine.org/assistant/pathway/publish/";
				// Perform the actual publish action and store the result
				result = client.PostAsync( publishEndpoint, content ).Result.Content.ReadAsStringAsync().Result;
			}
			// Return the result
			return result;
		}

		//public class PathwayRequest
		//{
		//	public Pathway Pathway { get; set; }
		//	public string PublishForOrganizationIdentifier { get; set; }
		//	public List<PathwayComponent> PathwayComponents { get; set; }
		//}

		//public class Pathway
		//{
		//	public string Ctid { get; set; }
		//	public string Name { get; set; }
		//	public string Description { get; set; }
		//	public string SubjectWebpage { get; set; }
		//	public List<string> HasDestinationComponent { get; set; } = new List<string>();
		//	public string HasProgressionModel { get; set; }

		//	// Enter at least one of 
		//	public List<OrganizationReference> OwnedBy { get; set; }
		//	public List<string> HasChild { get; set; }
		//	// Other properties
		//	public List<string> Keyword { get; set; }

		//	public List<FrameworkItem> OccupationType { get; set; } = new List<FrameworkItem>();
		//	public List<string> AlternativeOccupationType { get; set; } = new List<string>();
		//	//
		//	public List<FrameworkItem> IndustryType { get; set; } = new List<FrameworkItem>();
		//	public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		//}

		//public class PathwayComponent
		//{
		//	// PathwayComponentType - Type of PathwayComponent
		//	public string PathwayComponentType { get; set; }
		//	public string Ctid { get; set; }
		//	public string Name { get; set; }
		//	public string Description { get; set; }
		//	// Must include at least one of subject webpage or source data.
		//	public string SubjectWebpage { get; set; }
		//	public string SourceData { get; set; }

		//	public string CodedNotation { get; set; }
		//	public List<ComponentCondition> HasCondition { get; set; } = new List<ComponentCondition>();
		//	public List<string> HasChild { get; set; }
		//	// Concept in a ProgressionModel concept scheme
		//	public string HasProgressionLevel { get; set; }
		//	public List<string> IsChildOf { get; set; }
		//	public List<string> IsDestinationComponentOf { get; set; }
		//	// IsPartOf - the pathway that this component is a part of. This will be generated if missing.
		//	public List<string> IsPartOf { get; set; }
		//	// Other properties
		//	public string CredentialType { get; set; }

		//	/// <summary>
		//	/// ProgramTerm
		//	/// Categorization of a term sequence based on the normative time between entry into a program of study and its completion such as "1st quarter", "2nd quarter"..."5th quarter".
		//	/// Used by: 
		//	/// ceterms:CourseComponent only 
		//	/// </summary>
		//	public string ProgramTerm { get; set; }
		//}
	}
}
