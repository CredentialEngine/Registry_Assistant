using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class CostProfiles
	{
		/// <summary>
		/// Sample Cost Profile
		/// Required: <see cref="https://credreg.net/registry/policy#costprofile_required"/>
		/// - Description
		/// - CostDetails URL
		/// Recommended
		/// - Currency
		///   - For list of valid ISO 4217 currency codes, see: https://en.wikipedia.org/wiki/ISO_4217#List_of_ISO_4217_currency_codes
		/// - CostProfileItem 
		///   - helper for Direct Cost Type 
		///   Must be a valid CTDL cost type.<see cref="https://credreg.net/ctdl/terms#CostType"/>
		///		 Example: Tuition, Application, AggregateCost, RoomOrResidency
		///   - price, AudienceType, ResidencyType
		/// </summary>
		/// <returns></returns>
		public static CostProfile PopulateCostProfile()
		{
			var output = new CostProfile()
			{
				Description = "A required description of the cost profile",
				CostDetails = "https://example.com/t=loppCostProfile",
				Currency = "USD",
				CostItems = new List<CostProfileItem>()
				 {
					 new CostProfileItem()
					 {
						 DirectCostType="Application",
						 Price=100,
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="Tuition",
						 Price=12999,
						 PaymentPattern="Full amount due at time of registration"
					 }
				 }
			};
			output.Jurisdiction = new List<JurisdictionProfile>()
			{
				//example of a jurisdiction is for the United States, except Oregon
				new JurisdictionProfile()
				{
					GlobalJurisdiction=false,
					 Description="Description of the Jurisdiction. This could be used if the finer details are not available.",
					  MainJurisdiction = new Place()
					  {
						   Country="United States"
					  },
					  JurisdictionException = new List<Place>()
					  {
						  new Place()
						  {
							  AddressRegion="Oregon"
						  }
					  }
				}
			};
			return output;
		}

		public static List<CostProfile> PopulateFullCostProfile()
		{
			var output = new List<CostProfile>();
			//instate cost profile
			var profile = new CostProfile()
			{
				Name="Costs for an in-state student.",
				Description = "A required description of the cost profile",
				CostDetails = "https://example.com/t=loppCostProfile",
				Currency = "USD", 
				Condition = new List<string>()
				{
					"Must have been a resident of the state for at least one year.", 
				}
			};
			//instate
			profile.CostItems = new List<CostProfileItem>()
			{
				new CostProfileItem()
					 {
						 DirectCostType="Application",
						 Price=100,
						 AudienceType = new List<string>()
						 {
							 "audience:Resident"
						 }
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="Tuition",
						 Price=12999,
						 PaymentPattern="Full amount due at time of registration",
						 AudienceType = new List<string>()
						 {
							 "audience:Resident"
						 }
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="RoomOrResidency",
						 Price=5999,
						 PaymentPattern="Payable before the start of each term.",
						 AudienceType = new List<string>()
						 {
							 "audience:Resident"
						 }
					 }
			};

			output.Add( profile );

			//out of state
			profile = new CostProfile()
			{
				Name = "Costs for an out of state student.",
				Description = "A required description of the cost profile",
				CostDetails = "https://example.com/t=loppCostProfile",
				Currency = "USD",
				Condition = new List<string>()
				{
					"Must a resident of the United States.",
				}
			};
			profile.CostItems = new List<CostProfileItem>()
			{
				new CostProfileItem()
					 {
						 DirectCostType="Application",
						 Price=150,
						 AudienceType = new List<string>()
						 {
							 "audience:NonResident"
						 }
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="Tuition",
						 Price=14999,
						 PaymentPattern="Full amount due at time of registration",
						 AudienceType = new List<string>()
						 {
							 "audience:NonResident"
						 }
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="RoomOrResidency",
						 Price=7999,
						 PaymentPattern="Payable before the start of each term.",
						 AudienceType = new List<string>()
						 {
							 "audience:NonResident"
						 }
					 }
			};

			output.Add( profile );

			return output;
		}
	}
}
