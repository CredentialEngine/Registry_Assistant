using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Class for providing data about a propery like LearningOpportunity.CreditValue.
	/// Required?: Provide a single value or a Min and Max value.
	///			   May not actually be required, if description used. If UnitText is provided, then a value is required.	
	/// Required: Provide a valid concept in UnitText or description
	/// </summary>
	public class QuantitativeValue
	{
		/// <summary>
		/// Provide a valid concept from the CreditUnitType concept scheme, with or without the namespace. For example:
		/// creditUnit:ContinuingEducationUnit or ContinuingEducationUnit
		/// <see cref="https://credreg.net/ctdl/terms/creditUnitType"/> 
		/// </summary>
		public string UnitText { get; set; }
		//public LanguageMap UnitText_Map { get; set; } = new LanguageMap();
		public decimal Value { get; set; }
		public decimal MinValue { get; set; }
		public decimal MaxValue { get; set; }
		/// <summary>
		/// Optional description of the credit, using either a string value or as a language map
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
	}
}
