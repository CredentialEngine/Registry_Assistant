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
		public static Jurisdiction SampleJurisdiction()
		{
			Jurisdiction entity = new Jurisdiction()
			{
				Description = "Description of Jurisdiction",
				GlobalJurisdiction = false
			};
			entity.MainJurisdiction = new Place()
			{
				AddressRegion = "United States"
			};
			entity.JurisdictionException = new List<Place>()
			{
				new Place()
				{
					AddressRegion = "Oregon"
				}
			};

			return entity;

		}
	}
}