using System.Collections.Generic;

using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using RA.Models.Input.profiles.QData;

using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class Jurisdictions
	{
		public static List<JurisdictionProfile> SampleJurisdictions()
		{
			var output = new List<JurisdictionProfile>();
			output.Add( SampleJurisdiction());
			return output;
		}
		
		public static JurisdictionProfile SampleJurisdiction()
		{
			var entity = new JurisdictionProfile()
			{
				Description = "Description of Jurisdiction",
				GlobalJurisdiction = false
			};
			//A main jurisdiction is defined using the Place class. 
			//The Place GEO properties such as Country, AddressRegion or City
			//Or the Name property could be used for say, a continent
			entity.MainJurisdiction = new Place()
			{
				Country = "United States"
			};
			//Include any exceptions to the MainJurisdiction.
			//Example: If a credential is valid in all states, except Oregon, add the latter as an exception.
			entity.JurisdictionException = new List<Place>()
			{
				new Place()
				{
					AddressRegion = "Oregon",
					Country = "United States"
				}
			};

			return entity;
		}

		public static JurisdictionAssertion SampleJurisdictionAssertion()
		{
			JurisdictionAssertion entity = new JurisdictionAssertion()
			{
				Description = "Description of Jurisdiction Assertion",
				GlobalJurisdiction = false
			};
			entity.AssertedBy = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="QACredentialOrganization",
					Name="A QA Organization",
					SubjectWebpage="https://example.com?t=myqasite",
					Description="An optional but useful description of this QA organization."
				}
			};
			//A main jurisdiction is defined using the Place class. 
			//The Place GEO properties such as Country, AddressRegion or City
			//Or the Name property could be used for say, a continent
			entity.MainJurisdiction = new Place()
			{
				Country = "United States"
			};
			//Include any exceptions to the MainJurisdiction.
			//Example: If a credential is valid in all states, except Oregon, add the latter as an exception.
			entity.JurisdictionException = new List<Place>()
			{
				new Place()
				{
					AddressRegion = "Oregon",
					Country = "United States"
				}
			};

			return entity;
		}
	}
}
