using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// A value and description of value awarded for, required by, or otherwise related to the resource.
	/// </summary>
	public class ValueProfile
	{
		/// <summary>
		/// The level of credit associated with the credit awarded or required.
		/// Concept
		/// Scheme?
		/// </summary>
		public List<string> CreditLevelType { get; set; } = new List<string>();

		/// <summary>
		/// Provide a valid concept from the CreditUnitType concept scheme, with or without the namespace. For example:
		/// creditUnit:DegreeCredit or ContinuingEducationUnit 
		/// If this object is a monetary purpose, the UnitText would typically be the related currency for the value (example: "USD")
		/// ConceptScheme: <see cref="https://credreg.net/ctdl/terms/CreditUnit"/>
		/// </summary>
		public List<string> CreditUnitType { get; set; } = new List<string>();

		/// <summary>
		/// Optional description of the value, using either a string value or as a language map
		/// </summary>
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();


		/// <summary>
		/// A percentage for this purpose. 
		/// Best practice is to treat the value of this property as a verbatim percentage; for example, a value of 1.5 should be interpreted as 1.5%
		/// Do not use if providing any of value, minimum and maximum value.
		/// qdata:percentage
		/// </summary>
		public decimal Percentage { get; set; }

		public List<string> Subject { get; set; } = new List<string>();
		public LanguageMapList Subject_Map { get; set; } = new LanguageMapList();

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
		/// Maximum value for this purpose.
		/// </summary>
		public decimal MaxValue { get; set; }
	}
}
