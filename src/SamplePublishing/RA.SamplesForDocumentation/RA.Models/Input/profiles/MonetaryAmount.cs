using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class MonetaryAmount
	{
		/// <summary>
		/// Currency abbreviation (e.g., USD).
		/// </summary>
		public string Currency { get; set; } 
		public decimal Value { get; set; }
		public decimal MinValue { get; set; }
		public decimal MaxValue { get; set; }

		/// <summary>
		/// Provide a valid concept from the CreditUnitType concept scheme, with or without the namespace. For example:
		/// creditUnit:ContinuingEducationUnit or ContinuingEducationUnit
		/// <see cref="https://credreg.net/ctdl/terms/creditUnitType"/> 
		/// </summary>
		public string UnitText { get; set; }
	}

	/// <summary>
	/// Statistical distribution of monetary amounts.
	/// https://credreg.net/qdata/terms/MonetaryAmountDistribution#MonetaryAmountDistribution
	/// </summary>
	public class MonetaryAmountDistribution
	{
		/// <summary>
		/// Currency abbreviation (e.g., USD).
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// Median salary value.
		/// </summary>
		public decimal Median { get; set; }

		/// <summary>
		/// 10th percentile salary value.
		/// </summary>
		public decimal Percentile10 { get; set; }

		/// <summary>
		/// 25th percentile salary value.
		/// </summary>
		public decimal Percentile25 { get; set; }

		/// <summary>
		/// 75th percentile salary value.
		/// </summary>
		public decimal Percentile75 { get; set; }

		/// <summary>
		/// 90th percentile salary value.
		/// </summary>
		public decimal Percentile90 { get; set; }

	}
}
