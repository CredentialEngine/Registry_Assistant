using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class FinancialAssistanceProfiles
	{
		/// <summary>
		/// Example for populating Financial Assistance Profiles for a Credential Request. 
		/// The same approach would be used for other classes that support Financial Assistance Profiles such as Assessments and LearningOpportunities. 
		/// </summary>
		/// <param name="request"></param>
		public static void PopulateSimpleFinancialAssistanceProfile( Credential request )
		{
			request.FinancialAssistance = new List<FinancialAssistanceProfile>();
			//WIOA
			request.FinancialAssistance.Add( new FinancialAssistanceProfile()
			{
				Name = "WIOA Related Funding",
				Description = "Description of funding",
				FinancialAssistanceType = new List<string>() { "WIOA" }
			} );
			request.FinancialAssistance.Add( new FinancialAssistanceProfile()
			{
				Name = "Funding for veterans",
				Description = "description of funding",
				SubjectWebpage = "https://example.com/optional",
				FinancialAssistanceType = new List<string>() { "Military", "Veteran" }
			} );
			request.FinancialAssistance.Add( new FinancialAssistanceProfile()
			{
				Name = "GI Bill Funding",
				Description = "description of funding",
				SubjectWebpage = "http://example.com/gibill",
				FinancialAssistanceType = new List<string>() { "Post911GIBill " }
			} );
			request.FinancialAssistance.Add( new FinancialAssistanceProfile()
			{
				Name = "Additional Funding",
				Description = "Additional Funding is available from the following sources.",
				FinancialAssistanceType = new List<string>() { "Federal Scholarship", "Federal Subsidized Loan", "Federal Unsubsidized Loan", "Federal Work Study" }
			} );

		}

		/// <summary>
		/// Add financial assistance profiles that include financial assistance values
		/// </summary>
		/// <param name="request"></param>
		public static void PopulateFinancialAssistanceProfile( Credential request )
		{
			request.FinancialAssistance = new List<FinancialAssistanceProfile>();

			//financial assistance from Scholarships and Grants
			var profile1 = new FinancialAssistanceProfile()
			{
				Name = "Scholarships and Grants",
				Description = "description of funding",
				SubjectWebpage = "http://example.com",
				//two types of financial assistance
				FinancialAssistanceType = new List<string>() { "Scholarship", "Grant" }
			} ;
			//add a FinancialAssistanceValue
			//Note: for Financial Assistance Value, the UnitText is expected to be a currency. It is not required if a description is available. 
			profile1.FinancialAssistanceValue = new List<QuantitativeValue>()
			{
				new QuantitativeValue()
				{
					MinValue=1000, MaxValue = 2500, UnitText="USD"
				}
			};
			request.FinancialAssistance.Add( profile1 );

			//financial assistance from Loans
			var profile2 = new FinancialAssistanceProfile()
			{
				Name = "Assistance available via loans",
				Description = "description of funding",
				SubjectWebpage = "http://example.com",
				//One type of financial assistance
				FinancialAssistanceType = new List<string>() { "Loan" }
			};
			//add a FinancialAssistanceValue for a specific value
			profile2.FinancialAssistanceValue = new List<QuantitativeValue>()
			{
				new QuantitativeValue()
				{
					Value=5768, UnitText="USD"
				}
			};
			request.FinancialAssistance.Add( profile2 );

		}
	}
}
