using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// FinancialAssistanceProfile is a class that describes financial assistance that is offered or available
	/// The Name is always required. 
	/// At least one of the following must be provided:
	/// Description, Subject Webpage, or Financial Assistance Type
	/// </summary>
	public class FinancialAssistanceProfile
	{
		/// <summary>
		/// Name for this profile
		/// Required
		/// </summary>
		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Description is not required.
		/// There is no minimum length, but should be reasonable.
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		public string SubjectWebpage { get; set; }
		/// <summary>
		/// The financial assistance type is a list of one or more concepts from the ceterms:FinancialAssistance concept scheme. 
		/// <see cref="https://credreg.net/ctdl/terms/financialAssistanceType"/>
		/// </summary>
		public List<string> FinancialAssistanceType { get; set; } = new List<string>();
		/// <summary>
		/// The Financial Assistance Value(s) available for this profile
		/// The QuantitativeValue includes the UnitText property. Financial Assistance Value, the UnitText if present, is expected to be a currency. It is not required if a description is available. 
		/// Recommended
		/// </summary>
		public List<QuantitativeValue> FinancialAssistanceValue { get; set; } = new List<QuantitativeValue>();
	}
}
