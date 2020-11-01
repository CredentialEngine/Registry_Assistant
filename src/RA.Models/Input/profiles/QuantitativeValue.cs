using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Class for providing values for a property like FinancialAssistance.FinancialAssistanceValue or  LearningOpportunity.CreditValue.
	/// Recommended: Provide a single value OR a Min and Max value and provide a valid concept in UnitText
	/// If UnitText is provided, then a value is required.	
	/// Alternatively just a description can be provided if value is more complicated than can be expressed using either the Value or MinValue/MaxValue
	/// </summary>
	public class QuantitativeValue
	{
		/// <summary>
		/// Provide a valid concept from the CreditUnitType concept scheme, with or without the namespace. For example:
		/// creditUnit:DegreeCredit or ContinuingEducationUnit
		/// <see cref="https://credreg.net/ctdl/terms/creditUnitType"/> 
		/// If this object is a monetary purpose, the UnitText would typically be the related currency for the value (example: "USD")
		/// </summary>
		public string UnitText { get; set; }
		/// <summary>
		/// A single value for this purpose. 
		/// Do not use if providing a minimum and maximum value.
		/// </summary>
		public decimal Value { get; set; }
		/// <summary>
		/// Minimum value for this purpose. If provided, a maximum value must also be provided
		/// </summary>
		public decimal MinValue { get; set; }
		/// <summary>
		/// Minimum value for this purpose.
		/// </summary>
		public decimal MaxValue { get; set; }

		/// <summary>
		/// A percentage for this purpose. 
		/// Do not use if providing any of value, minimum and maximum value.
		/// qdata:percentage
		/// </summary>
		public decimal Percentage { get; set; }
		/// <summary>
		/// Optional description of the value, using either a string value or as a language map
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
	}
}
